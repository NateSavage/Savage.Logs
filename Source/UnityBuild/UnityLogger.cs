using System.Collections.Generic;
using System.Text;

namespace Savage.Logs {

    // TODO: fix this class
    /// <summary> Output target for Unity's log.txt file, and also the editor console if running from the editor. </summary>
    public class UnityLogger : ILogger {

        // parameters
        public LoggerSettings Settings => throw new System.NotImplementedException();

        private bool includeWriteTime;
        private bool includeCaller;
        private bool includeVerbosity;

        // state
        private StringBuilder logWriter = new StringBuilder();


        // font size is included only to ensure method signatures match Logging.UnityEditor
        public UnityLogger(int fontSize, bool includeWriteTime = true, bool includeCaller = true, bool includeVerbosity = true) {
            this.includeWriteTime = includeWriteTime;
            this.includeCaller = includeCaller;
            this.includeVerbosity = includeVerbosity;

            LogPipeline.MessageLogged += Write;
        }

        public void Write(LogEntry entry) {

           // if (includeWriteTime)
           //     logWriter.Append($"[{entry.WriteTime}] ");
           // if (includeCaller)
           //     logWriter.Append($"{entry.Caller}: ");
           // if (includeVerbosity)
           //     logWriter.Append($"{entry.Verbosity}: ");

            logWriter.Append(entry.Message)
                     .Append('\n');

            System.Console.Write(logWriter.ToString());

            logWriter.Clear();
        }

        public override bool Equals(object obj) => obj is UnityLogger;
    }

    public static partial class LoggerExtensions {
        /// <inheritdoc cref="UnityLogger"/>
        public static LogPipeline WithUnityLogging(this LogPipeline logger, int fontSize = 14, bool includeWriteTime = true, bool includeCaller = true, bool includeVerbosity = true) {
            // font size is included only to ensure method signatures match Logging.UnityEditor
            LogPipeline.Register(new UnityLogger(fontSize));
            return logger;
        }
    }
}
