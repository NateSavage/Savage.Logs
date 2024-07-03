using System.Collections.Generic;
using System.Text;

using UnityEditor;

namespace Savage.Logs {
    /// <summary> Output target for Unity's log.txt file, and also the editor console if running from the editor. </summary>
    public class UnityLogger : ILogger {

        #region Constants
        private static readonly string lightThemeTypeColor = "<color=#008060>";
        private static readonly string lightThemeFatalColor = "<color=#A729C6>";
        private static readonly string lightThemeErrorColor = "<color=#FF0000>";
        private static readonly string lightThemeWarningColor = "<color=#FF3F00>";
        private static readonly string lightThemeInfoColor = "<color=#000000>";
        private static readonly string lightThemeDebugColor = "<color=#000000>";
        private static readonly string lightThemeTraceColor = "<color=#000000>";

        private static readonly string darkThemeTypeColor = "<color=#4EC9AB>";
        private static readonly string darkThemeFatalColor = "<color=#A729C6>";
        private static readonly string darkThemeErrorColor = "<color=#FF0000>";
        private static readonly string darkThemeWarningColor = "<color=#FFF43F>";
        private static readonly string darkThemeInfoColor = "<color=#EFEFEF>";
        private static readonly string darkThemeDebugColor = "<color=#7F7F7F>";
        private static readonly string darkThemeTraceColor = "<color=#7F7F7F>";

        /// <summary> Dictionary holding predefined color/label string for every verbosity level. Prevents constantly making new strings, and should be as fast as switch. </summary>
        private static readonly Dictionary<Verbosity, string> lightThemeVerbosityColors = new Dictionary<Verbosity, string> {
            [Verbosity.Fatal] = lightThemeFatalColor,
            [Verbosity.Error] = lightThemeErrorColor,
            [Verbosity.Warning] = lightThemeWarningColor,
            [Verbosity.Info] = lightThemeInfoColor,
            [Verbosity.Debug] = lightThemeDebugColor,
            [Verbosity.Trace] = lightThemeTraceColor
        };

        /// <summary> Dictionary holding predefined color/label string for every verbosity level. Prevents constantly making new strings, and should be as fast as switch. </summary>
        private static readonly Dictionary<Verbosity, string> darkThemeVerbosityColors = new Dictionary<Verbosity, string> {
            [Verbosity.Fatal] = darkThemeFatalColor,
            [Verbosity.Error] = darkThemeErrorColor,
            [Verbosity.Warning] = darkThemeWarningColor,
            [Verbosity.Info] = darkThemeInfoColor,
            [Verbosity.Debug] = darkThemeDebugColor,
            [Verbosity.Trace] = darkThemeTraceColor
        };
        #endregion Constants

        #region Parameters
        public LoggerSettings Settings => throw new System.NotImplementedException();

        private int fontSize;
        private bool includeWriteTime;
        private bool includeCaller;
        private bool includeVerbosity;
        #endregion Parameters

        // state
        private StringBuilder logWriter = new StringBuilder();

        public UnityLogger(int fontSize, bool includeWriteTime = false, bool includeCaller = true, bool includeVerbosity = false) {
            this.fontSize = fontSize;
            this.includeWriteTime = includeWriteTime;
            this.includeCaller = includeCaller;
            this.includeVerbosity = includeVerbosity;

            LogPipeline.MessageLogged += Write;
        }

        public void Write(LogEntry entry) {
            // set the unity message icon on the far left to as large as is allowable, and then set the user's font size
            logWriter.Append($"<size=22> </size><size={fontSize}>");

            /*
            // format message colors for light or dark theme
            if (UnityEditor.EditorGUIUtility.isProSkin) { // dark skin
                if (includeWriteTime)
                    logWriter.Append($"{darkThemeTraceColor}[{entry.WriteTime}]</color> ");
                if (includeCaller)
                    logWriter.Append($"{darkThemeTypeColor}<b>{entry.Caller}: </b></color>");

                logWriter.Append(darkThemeVerbosityColors[entry.Verbosity]);
            }
            else { // light skin 
                if (includeWriteTime)
                    logWriter.Append($"{lightThemeTraceColor}[{entry.WriteTime}]</color> ");
                if (includeCaller)
                    logWriter.Append($"{lightThemeTypeColor}<b>{entry.Caller}: </b></color>");

                logWriter.Append(lightThemeVerbosityColors[entry.Verbosity]);
            }
            */
            if (includeVerbosity)
                logWriter.Append($"{entry.Verbosity}: ");

            logWriter.Append($"{entry.Message}</color></size>\n");

            switch (entry.Verbosity) {
                default:
                case Verbosity.Trace:
                case Verbosity.Debug:
                case Verbosity.Info:
                    UnityEngine.Debug.Log(logWriter.ToString());
                    break;
                case Verbosity.Warning:
                    UnityEngine.Debug.LogWarning(logWriter.ToString());
                    break;
                case Verbosity.Error:
                case Verbosity.Fatal:
                    UnityEngine.Debug.LogError(logWriter.ToString());
                    break;
            }

            logWriter.Clear();
        }

        public override bool Equals(object obj) => obj is UnityLogger;
    }

    public static partial class LoggerExtensions {
        /// <inheritdoc cref="UnityLogger"/>
        public static LogPipeline WithUnityLogging(this LogPipeline logger, int fontSize = 14, bool includeWriteTime = false, bool includeCaller = true, bool includeVerbosity = false) {
            LogPipeline.Register(new UnityLogger(fontSize, includeWriteTime, includeCaller, includeVerbosity));
            return logger;
        }
    }
}
