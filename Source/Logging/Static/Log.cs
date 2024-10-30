
using System.Runtime.CompilerServices;

namespace Savage.Logs {

    /// <summary> A convenience handle for interacting with the <see cref="GlobalLogPipeline"/>. </summary>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    public static class Log {

        /// <summary>
        /// A global log pipeline stored for convenience. Automatically set to the first created <see cref="LogPipeline"/>. <br/>
        /// You do not need to use this if you don't want to, and can create and pass around as many local pipelines as you like. 
        /// </summary>
        /// <remarks> You can send messages to the global pipeline with the other methods on this class.</remarks>
        public static LogPipeline GlobalLogPipeline;

        #region Logging Methods
            
        /// <summary> Logs a message with its verbosity passed in as an argument. </summary>
        public static void Message(Verbosity verbosity, string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(verbosity, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Trace"/>
        public static void Trace(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Trace, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Info"/>
        public static void Info(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Info, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Debug"/>
        public static void Debug(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Debug, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Warning"/>
        public static void Warning(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Warning, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Error"/>
        public static void Error(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Error, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Fatal"/>
        public static void Fatal(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Fatal, message, attachments, callerPath);
        /// <inheritdoc cref="Verbosity.Audit"/>
        public static void Audit(string message, MessageAttachment[] attachments = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.QueueForBroadcast(Verbosity.Audit, message, attachments, callerPath);
        #endregion Logging Methods
            
        #region Benchmark Methods

        /// <summary> Used to AB text the performance of the <see cref="CallerFilePathAttribute"/>. </summary>
        internal static void MessageNoAttribute(Verbosity verbosity, string message, MessageAttachment[] attachments = null) => GlobalLogPipeline.QueueForBroadcast(verbosity, message, attachments, null);
        #endregion Benchmark Methods
    }
}