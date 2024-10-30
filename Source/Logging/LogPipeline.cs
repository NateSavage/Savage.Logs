
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

using Savage.Logs.Collections;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Savage.Logs.LogDecorations;

namespace Savage.Logs {

    /// <summary>  A series of transformations that takes in a log entry and attaches data to it before sending it to output sinks. </summary>
    /// <remarks> This class can be called from off the main thread and needs to remain thread safe. </remarks>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    public class LogPipeline : PipelineNode, ILogger {

        public LogPipelineSettings Settings;
        public readonly Theme Theme;

        /// <summary> Logs are queued in this buffer, and passed down the pipeline later. </summary>
        readonly DoubleBuffer<LogEntry> _messageBuffer = new DoubleBuffer<LogEntry>();
        Task _bufferDaemon = Task.Run(DoNothing);

        
        #region Construction
            
        public LogPipeline(LogPipelineSettings? configuration = null, Theme? theme = null) {
            Settings = configuration ?? LogPipelineSettings.Default();
            Theme = theme ?? Theme.DefaultDark();
            
            // TODO: add unit test for assertion listener
            var assertionListener = new AssertionListener(Settings.AssertionVerbosity, Settings.IncludeCallerFileNameForTrace);
            System.Diagnostics.Trace.Listeners.Add(assertionListener);
          
            CallingFileAttachment.InternalLocation = Settings.CallerFileNameDisplayLocation;

            if (Settings.LogUnhandledExceptions) // this covers all threads, not just the main one
                AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            
            if (Savage.Logs.Log.GlobalLogPipeline is null)
                Savage.Logs.Log.GlobalLogPipeline = this;
        }

        public LogPipeline DropWhenVerbosityIsBelow(Verbosity maxDisplayedVerbosity) {
            Settings.MinimumVerbosity = maxDisplayedVerbosity;
            return this;
        }
        
        public LogPipeline AttachCallerFileName() {
            Settings.IncludeCallerFileName = true;
            return this;
        }
        
        public static LogPipeline Create(LogPipelineSettings? configuration = null, Theme? theme = null) => new LogPipeline(configuration, theme);
        #endregion Construction

        #region Public Logging Methods
            
            // add method for exceptions?
        /// <inheritdoc cref="Verbosity.Trace"/>
        public void LogTrace(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Trace, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Debug"/>
        public void LogDebug(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Debug, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Info"/>
        public void LogInfo(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Info, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Warning"/>
        public void LogWarning(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Warning, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Error"/>
        public void LogError(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Error, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Fatal"/>
        public void LogFatal(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Fatal, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Audit"/>
        public void LogAudit(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => QueueForBroadcast(Verbosity.Audit, message, attachments, callerPath);
        #endregion Public Logging Methods

        void LogUnhandledException(object sender, UnhandledExceptionEventArgs arguments) => UnhandledException(sender, (Exception)arguments.ExceptionObject);

        /// <summary> Logs an unhandled exception. </summary>
        void UnhandledException(object caller, Exception exception) {
            // do not replace == with is because == can be overridden and is cannot
            string callerPath = caller == null ? "UnknownCaller" : caller.GetType().Name;
            var decorations = new MessageAttachment[] {
                new ThreadIdAttachment(),
                new StackTraceAttachment(exception)
            };
            QueueForBroadcast(Verbosity.Fatal, exception.Message, decorations, callerPath);
        }
        
        internal void QueueForBroadcast(Verbosity verbosity, string message, MessageAttachment[] decorations, string callerPath) {
            if (verbosity > Settings.MinimumVerbosity)
                return;
            
            var logEntry = new LogEntry(message, verbosity, decorations);
            
            if (ShouldDrop(logEntry))
                return;
            
            if(Settings.IncludeCallerFileName)
                logEntry.Attachments.Add(new CallingFileAttachment(callerPath));
            
            // we need to do the initial round of metadata on the main thread because delaying and batching write time is bad
            // and recording the thread id from a different thread from the caller is bad
            AttachMetaDataTo(ref logEntry);
            
            _messageBuffer.Front.Add(logEntry);
            
            // if the buffer daemon is still running, it will automatically check if there's more data for it to keep churning through
            // after it finishes it's last job
            if (_bufferDaemon.IsCompleted)
                _bufferDaemon = Task.Run(BroadcastLogs);
        }
        
        /// <summary> Pushes queued messages through the pipeline. </summary>
        void BroadcastLogs() {
            do {
                _messageBuffer.Swap();
                lock(_messageBuffer.Back) {
                    foreach (PipelineNode child in Children) {
                        foreach (var message in _messageBuffer.Back) 
                            child.ProcessAndPushToChildrenRecursive(message);
                    }

                    // if we want to go even faster we could start treating this like a rolling buffer and not bother to erase anything in it
                    _messageBuffer.Back.Clear();
                }
                
                // if more messages were written to the front buffer while we were working on the back buffer
                // we can swap the buffers again and keep churning
            } while (_messageBuffer.Front.Count > 0); 
        }
        
        /// <returns> A description of the pipeline in dot graph language.</returns>
        public string DebugString() {
            throw new NotImplementedException();
        }

    #region Microsoft.Extensions.Logging.ILogger Implementation

        public bool IsEnabled(LogLevel logLevel) => Settings.MinimumVerbosity <= logLevel.ToSavageLogsVerbosity();
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            QueueForBroadcast(logLevel.ToSavageLogsVerbosity(), formatter(state, exception), new MessageAttachment[] { new MicrosoftEventIdAttachment(eventId) }, callerPath: null);
        }
        
        // TState is promised to not be null, we can't use the language feature that tells the compiler that pre dotnet 8
        public IDisposable BeginScope<TState>(TState state) {
            throw new NotImplementedException();
        }
    #endregion Microsoft.Extensions.Logging.ILogger Implementation
        
        static void DoNothing() { }
    }
}
