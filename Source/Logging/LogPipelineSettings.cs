
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Savage.Logs {

    /// <summary> Settings to be applied to all output sinks. </summary>
    public struct LogPipelineSettings {

        /// <summary> Whether or not unhandled exceptions should be logged by all <see cref="ILogSink"/>s. </summary>
        public bool LogUnhandledExceptions;

        /// <summary> Log entries with a verbosity lower than this will not be recorded. </summary>
        /// <remarks>
        /// <see cref="Verbosity.Trace"/> by default. <br/>
        /// <see cref="Verbosity.Trace"/> is the lowest level verbosity, <see cref="Verbosity.Fatal"/> is the highest. 
        /// </remarks>
        public Verbosity MinimumVerbosity;

        /// <summary> Verbosity failed assertions should be logged with. </summary>
        /// <remarks> <see cref="Verbosity.Warning"/> by default. </remarks>
        public Verbosity AssertionVerbosity;


        /// <summary> Should the name of the calling file be included in all <see cref="LogEntry"/> objects? </summary>
        /// <remarks> True by default. </remarks>
        public bool IncludeCallerFileName;

        /// <summary> 
        /// Should caller information be recorded for <see cref="Trace"/> and <see cref="Debug"/> logging methods? <br/>
        /// When false these methods will be reported as having been logged by <see cref="TraceListener"/>.
        /// </summary>
        /// <remarks>
        /// False by default. <br/>
        /// There is a significant performance hit for including caller information for these methods because a stack trace is required. 
        /// </remarks>
        public bool IncludeCallerFileNameForTrace;

        /// <remarks> <see cref="DisplayLocation.InlinePreceding"/> by default. </remarks>
        public  DisplayLocation callerFileNameDisplayLocation;


        public static LogPipelineSettings Default() {
            return new LogPipelineSettings() {
                LogUnhandledExceptions = true,
                MinimumVerbosity = Verbosity.Trace,
                AssertionVerbosity = Verbosity.Warning,

                IncludeCallerFileName = true,
                IncludeCallerFileNameForTrace = false,
                callerFileNameDisplayLocation = DisplayLocation.InlinePreceding,
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
