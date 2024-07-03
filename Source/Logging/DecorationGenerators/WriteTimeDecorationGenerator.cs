using System;
using Savage.Logs.LogDecorations;

namespace Savage.Logs {


    public sealed class WriteTimeDecorationGenerator : DecorationGenerator<WriteTimeDecoration> {

        private string dateTimeFormat;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public WriteTimeDecorationGenerator(string dateTimeFormat, DisplayLocation displayLocation) {
            this.dateTimeFormat = dateTimeFormat;
            WriteTimeDecoration.InternalLocation = displayLocation;
        }

        public override LogDecoration Emit(ref LogEntry logEntry) => new WriteTimeDecoration(DateTime.Now.ToString(dateTimeFormat));
    }

    public static partial class LogPipelineExtensions {
        public static LogPipeline AttachWriteTime(this LogPipeline pipeline, string dateTimeFormat = "MM/dd/yyyy hh:mm:ss:ffff tt", DisplayLocation displayLocation = DisplayLocation.InlinePreceding) {
            pipeline.Add(new WriteTimeDecorationGenerator(dateTimeFormat, displayLocation));
            return pipeline;
        }
    }
}
