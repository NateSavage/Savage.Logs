using System;
using Savage.Logs.LogDecorations;

namespace Savage.Logs {

    /// <summary> </summary>
    public sealed class VerbosityDecorationGenerator : DecorationGenerator<VerbosityDecoration> {

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public VerbosityDecorationGenerator(DisplayLocation displayLocation) {
            VerbosityDecoration.InternalLocation = displayLocation;
        }

        public override LogDecoration Emit(ref LogEntry logEntry) => new VerbosityDecoration(logEntry.Verbosity);
    }

    public static partial class LogPipelineExtensions {
        public static LogPipeline AttachVerbosity(this LogPipeline pipeline, DisplayLocation displayLocation = DisplayLocation.InlinePreceding) {
            pipeline.Add(new VerbosityDecorationGenerator(displayLocation));
            return pipeline;
        }
    }
}
