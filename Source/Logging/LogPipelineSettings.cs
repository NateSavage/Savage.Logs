
using System.Diagnostics;

namespace Savage.Logs {

    /// <summary> Settings to be applied to all output sinks. </summary>
    public struct LogPipelineSettings {

        /// <summary> Whether unhandled exceptions should be included in this <see cref="LogPipeline"/>. </summary>
        public bool LogUnhandledExceptions;

        /// <summary> Log entries with a verbosity lower than this will not be recorded. </summary>
        /// <remarks>
        /// <see cref="Verbosity.Trace"/> by <see cref="Default"/>. <br/>
        /// <see cref="Verbosity.Trace"/> is the lowest level verbosity, <see cref="Verbosity.Fatal"/> is the highest. 
        /// </remarks>
        public Verbosity MinimumVerbosity;

        /// <summary> Verbosity failed assertions should be logged with. </summary>
        /// <remarks> <see cref="Verbosity.Error"/> by <see cref="Default"/>. </remarks>
        public Verbosity AssertionVerbosity;


        /// <summary> Should the name of the calling file be included in all <see cref="LogEntry"/> objects? </summary>
        /// <remarks> True by default. </remarks>
        public bool IncludeCallerFileName;
        

        /// <remarks> <see cref="DisplayLocation.Preceding"/> by default. </remarks>
        public  DisplayLocation CallerFileNameDisplayLocation;


        public static LogPipelineSettings Default() {
            return new LogPipelineSettings() {
                LogUnhandledExceptions = true,
                MinimumVerbosity = Verbosity.Trace,
                AssertionVerbosity = Verbosity.Error,

                IncludeCallerFileName = true,
                CallerFileNameDisplayLocation = DisplayLocation.Preceding,
            };
        }

        public static LogPipelineSettings NoCallerName() {
            return new LogPipelineSettings() {
                LogUnhandledExceptions = true,
                MinimumVerbosity = Verbosity.Trace,
                AssertionVerbosity = Verbosity.Error,

                IncludeCallerFileName = false,
            };
        }
    }
}
