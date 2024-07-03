
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

using Savage.Logs.Collections;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Threading;
using System.Linq;
using Savage.Logs.LogDecorations;

[assembly: InternalsVisibleTo("Savage.Logs.Benchmarks")]
[assembly: InternalsVisibleTo("Loggy.Tests")]

namespace Savage.Logs {

    /// <summary>  A series of transformations that takes in a log entry and attaches data to it before sending it to output sinks. </summary>
    /// <remarks> This class can be called from off the main thread and needs to remain thread safe. </remarks>
    public class LogPipeline {

        /// <summary>
        /// Invoked after the <see cref="LogEntry"/> has been passed to every output sink, output sinks *should* have finished attaching all decorations before this is called. <br/>
        /// sinks *may* not have finished recording the entry, some sinks may take a significant amount of time to write to a file or transfer data over the wire.
        /// </summary>
        public event Action<LogEntry> MessageLogged;

        public LogPipelineSettings Settings;

        public Theme Theme;

        private List<DecorationGenerator> decorationGenerators;

        private List<Predicate<LogEntry>> conditionalFilters;
        private List<Func<LogEntry, LogDecoration>> conditionalGenerators;

        /// <summary> Functions that this sink will use to ignore or include messages. </summary>
        public List<Predicate<LogEntry>> Filters = new List<Predicate<LogEntry>>();

        private HashSet<ILogSink> outputSinks = new HashSet<ILogSink>();
        private AssertionListener assertionListener;
        private bool initialized;

        /// <summary> Logs are stored in a buffer, and passed down the pipeline after </summary>
        private DoubleBuffer<LogEntry> messageBuffer = new DoubleBuffer<LogEntry>();
        private int logBufferKey;

        /// <summary> Lock this object whenever making modifications to settings. </summary>
        private object configurationLock = new object();

        #region Construction

        public LogPipeline(LogPipelineSettings configuration, Theme theme) {
            lock (configurationLock) {
                if (initialized) { // reset state that configuration should override
                    decorationGenerators.Clear();
                    AppDomain.CurrentDomain.UnhandledException -= LogUnhandledException;
                }
                else { // first time initialization only
                    assertionListener = new AssertionListener(configuration.AssertionVerbosity, configuration.IncludeCallerFileNameForTrace);
                    System.Diagnostics.Trace.Listeners.Add(assertionListener);
                }

                Settings = configuration;
                Theme = theme;
                CallerDecoration.InternalLocation = configuration.callerFileNameDisplayLocation;

                decorationGenerators = new List<DecorationGenerator>();
                conditionalGenerators = new List<Func<LogEntry, LogDecoration>>();
                conditionalFilters = new List<Predicate<LogEntry>>();


                if (configuration.LogUnhandledExceptions) // this covers all threads, not just the main one
                    AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;

                initialized = true;
            }
        }

        public void Reconfigure(LogPipelineSettings configuration, Theme theme) {
            lock (configurationLock) {
                decorationGenerators.Clear();
                AppDomain.CurrentDomain.UnhandledException -= LogUnhandledException;

                Settings = configuration;
                Theme = theme;
                CallerDecoration.InternalLocation = configuration.callerFileNameDisplayLocation;

                decorationGenerators = new List<DecorationGenerator>();
                conditionalGenerators = new List<Func<LogEntry, LogDecoration>>();
                conditionalFilters = new List<Predicate<LogEntry>>();

                if (configuration.LogUnhandledExceptions) // this covers all threads, not just the main one
                    AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            }
        }

        /// <summary> Optional replace for new <see cref="LogPipeline"/> for fluent style syntax. </summary>
        public static LogPipeline Create(LogPipelineSettings configuration, Theme? theme = null) {
            return new LogPipeline(configuration, theme is null ? Theme.DefaultDarkTheme() : theme.Value);
        }

        #endregion Construction

        #region Public Logging Methods

        public void Trace(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => BroadcastLog(Verbosity.Trace, message, callerPath, decorations);
        public void Debug(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => BroadcastLog(Verbosity.Debug, message, callerPath, decorations);
        public void Info(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => BroadcastLog(Verbosity.Info, message, callerPath, decorations);
        public void Warning(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => BroadcastLog(Verbosity.Warning, message, callerPath, decorations);
        public void Error(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => BroadcastLog(Verbosity.Error, message, callerPath, decorations);
        public void Fatal(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => BroadcastLog(Verbosity.Fatal, message, callerPath, decorations);

        #endregion Public Logging Methods

        public LogPipeline DropLogsWith(Predicate<LogEntry> shouldLog) {
            Filters.Add(shouldLog);
            return this;
        }

        public LogPipeline ConditionallyAttach(Predicate<LogEntry> attachmentCondition, Func<LogEntry, LogDecoration> decorationGenerator) {
            conditionalFilters.Add(attachmentCondition);
            conditionalGenerators.Add(decorationGenerator);
            return this;
        }

        public void Add(DecorationGenerator generator) {
#if DEBUG
            if (decorationGenerators.Contains(generator))
                throw new ArgumentException($"Identical generator already exists for {generator}");
#endif
            decorationGenerators.Add(generator);
        }

        /// <remarks> Registering the same logger multiple times will throw an <see cref="ArgumentException"/>. </remarks>
        /// <exception cref="ArgumentException"> Identical logger has already been registered. </exception>
        public void Add(ILogSink logger) {
#if DEBUG
            if (outputSinks.Contains(logger))
                throw new ArgumentException($"Identical logger already exists for {logger}");
#endif
            outputSinks.Add(logger);
        }

        #region Queries

        /// <summary> Returns true if this pipeline has ANY <see cref="DecorationGenerator"/> matching or deriving from the desired type. </summary>
        public bool HasDecorationGenerator<T>() where T : DecorationGenerator {
            for (int i = 0; i < decorationGenerators.Count; ++i)
                if (typeof(T).IsAssignableFrom(decorationGenerators[i].GetType()))
                    return true;

            return false;
        }

        /// <summary> Removes ALL <see cref="DecorationGenerator"/>s matching or deriving from the desired type. </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveDecorationGenerator<T>() {
            List<int> indexesToRemove = new List<int>(2);

            for (int i = 0; i < decorationGenerators.Count; ++i) {
                if (typeof(T).IsAssignableFrom(decorationGenerators[i].GetType()))
                    indexesToRemove.Add(i);
            }
               
            for(int i = indexesToRemove.Count + 1; i > 0; --i) {
                decorationGenerators.RemoveAt(i);
            }
        }

        #endregion Queries

        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs arguments) => UnhandledException(sender, (Exception)arguments.ExceptionObject);

        /// <summary> Logs an unhandled exception. </summary>
        private void UnhandledException(object caller, Exception exception) {
            // do not replace == with is because == can be overridden and is cannot
            string callerPath = caller == null ? "UnknownCaller" : caller.GetType().Name;
            var decorations = new LogDecoration[] {
                new ThreadIDDecoration(),
                new StackTraceDecoration(exception)
            };
            BroadcastLog(Verbosity.Fatal, exception.Message, callerPath, decorations);
        }

        //TODO: take this method apart and move as much work off the main thread as possible
        // replace our decoration generators with Dictionary<ThreadRequirement, List<DecorationGenerator>>
        // MessageLogged should not be invoked until all generators have finished attaching metadata

        // this method should simply record to a double buffer and then actually process logs on another thread
        // if Godot or Unity can't handle this, we should provide a separate place to store output sinks that require living on the main thread
        internal void BroadcastLog(Verbosity verbosity, string message, string callerPath, LogDecoration[] decorations = null) {
            lock (messageBuffer.FrontLock) {
                var logEntry = new LogEntry(message, verbosity, decorations);

                if (verbosity > Settings.MinimumVerbosity)
                    return;

                foreach (Predicate<LogEntry> shouldLog in Filters) {
                    if (shouldLog.Invoke(logEntry) is false)
                        return;
                }

                if (Settings.IncludeCallerFileName)
                    logEntry.Decorations.Add(new CallerDecoration(Path.GetFileNameWithoutExtension(callerPath)));

                for (int i = 0; i < decorationGenerators.Count; ++i)
                    logEntry.Decorations.Add(decorationGenerators[i].Emit(ref logEntry));


                foreach (ILogSink output in outputSinks)
                    output.Write(logEntry);

                MessageLogged?.Invoke(logEntry);
            }
        }
    }
}
