using System;
using Savage.Logs.LogDecorations;

namespace Savage.Logs {


    public sealed class WriteTimeAttachmentGenerator : AttachmentGenerator<WriteTimeAttachment>{

        readonly string _dateTimeFormat;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public WriteTimeAttachmentGenerator(string dateTimeFormat, DisplayLocation displayLocation) {
            _dateTimeFormat = dateTimeFormat;
            WriteTimeAttachment.InternalLocation = displayLocation;
        }

        public override MessageAttachment CreateAttachmentFor(ref LogEntry logEntry) => new WriteTimeAttachment(DateTime.Now.ToString(_dateTimeFormat));
    }

    public static partial class PipelineNodeExtensions {
        
        public static PipelineNode AttachWriteTime(this PipelineNode parentNode, string dateTimeFormat = "MM/dd/yyyy hh:mm:ss:ffff tt", DisplayLocation displayLocation = DisplayLocation.InlinePreceding) {
            parentNode.Attach(new WriteTimeAttachmentGenerator(dateTimeFormat, displayLocation));
            return parentNode;
        }
        
        public static PipelineNode AttachWriteTimeWhen(this PipelineNode parentNode,  Predicate<LogEntry> predicate, string dateTimeFormat = "MM/dd/yyyy hh:mm:ss:ffff tt", DisplayLocation displayLocation = DisplayLocation.InlinePreceding) {
            parentNode.AttachWhen(predicate, new WriteTimeAttachmentGenerator(dateTimeFormat, displayLocation));
            return parentNode;
        }
    }
}
