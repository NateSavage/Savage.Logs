using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace Savage.Logs {

    public sealed class ThreadIdAttachment : MessageAttachment {
        public override string Tag => "Thread ID";
        public override bool ShowTag => true;

        public override Type Type => typeof(int);



        public override string Value => contents;
        private string contents;

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.InfoColor;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Source;

        public override DisplayLocation Location => DisplayLocation.InlinePreceding;

        public override int DisplayPriority => -1_900;



        public ThreadIdAttachment() {
            contents =  Thread.CurrentThread.ManagedThreadId.ToString();
        }
    }

    public static partial class PipelineNodeExtensions {
        
        public static PipelineNode AttachThreadId(this PipelineNode parentNode) {
            parentNode.Attach(new AttachmentGenerator<ThreadIdAttachment>());
            return parentNode;
        }
        
        public static PipelineNode AttachThreadIdWhen(this PipelineNode parentNode,  Predicate<LogEntry> predicate) {
            parentNode.AttachWhen(predicate, new AttachmentGenerator<ThreadIdAttachment>());
            return parentNode;
        }
        
        public static PipelineNode AttachThreadIdWhenNotOnMainThread(this PipelineNode parentNode) {
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;
            parentNode.AttachWhen((_) => Thread.CurrentThread.ManagedThreadId != mainThreadId, new AttachmentGenerator<ThreadIdAttachment>());
            return parentNode;
        }
    }
}
