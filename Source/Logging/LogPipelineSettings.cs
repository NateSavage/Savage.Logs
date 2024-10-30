
using System.Diagnostics;

namespace Savage.Logs {

    /// <summary> Settings to be applied to all output sinks. </summary>
    public struct LogPipelineSettings {
        
        public bool LogUnhandledExceptions;

        /// <summary> Log entries with a verbosity lower than this will be dropped. </summary>
        /// <remarks> See <see cref="Verbosity"/> for ordering. </remarks>
        public Verbosity MinimumVerbosity;

        /// <summary> Verbosity failed assertions should be logged with. </summary>
        /// <remarks> <see cref="Verbosity.Warning"/> by default. </remarks>
        public Verbosity AssertionVerbosity;


        /// <summary> Should the name of the calling file be included in all <see cref="LogEntry"/> objects? </summary>
        /// <remarks> True by default. </remarks>
        public bool IncludeCallerFileName;
        public bool IncludeCallerFileExtension;

        /// <summary> 
        /// Should caller information be recorded for <see cref="Trace"/> and <see cref="Debug"/> logging methods? <br/>
        /// When false these methods will be reported as having been logged by <see cref="TraceListener"/>.
        /// </summary>
        /// <remarks>
        /// False by default. <br/>
        /// There is a performance hit for including caller information for these methods because a stack trace is required. 
        /// </remarks>
        public bool IncludeCallerFileNameForTrace;

        /// <remarks> <see cref="DisplayLocation.InlinePreceding"/> by default. </remarks>
        public  DisplayLocation CallerFileNameDisplayLocation;


        public static LogPipelineSettings Default() {
            return new LogPipelineSettings() {
                LogUnhandledExceptions = true,
                MinimumVerbosity = Verbosity.Trace,
                AssertionVerbosity = Verbosity.Warning,

                IncludeCallerFileName = false,
                IncludeCallerFileNameForTrace = false,
                CallerFileNameDisplayLocation = DisplayLocation.InlinePreceding,
            };
        }

        public static LogPipelineSettings NoCallerName() {
            return new LogPipelineSettings() {
                LogUnhandledExceptions = true,
                MinimumVerbosity = Verbosity.Trace,
                AssertionVerbosity = Verbosity.Warning,

                IncludeCallerFileName = false,
                IncludeCallerFileNameForTrace = false,
            };
        }
    }
}
