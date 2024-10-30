using System;
using System.Collections.Generic;
using System.Text;

using Savage.Logs;

namespace Savage.Logs.UnityEditor {

    /// <summary> </summary>
    internal class FrameNumberMessageAttachment : MessageAttachment {

        public override string Tag => tag;
        private const string tag = "Frame Number";
        
        public override string Value { get; }

        public  bool DisplayTag => true;

        public  string Contents => contents;
        private readonly string contents;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Main;

        public override DisplayLocation Location => DisplayLocation.InlinePreceding;


        public override Type Type => typeof(int);

        public override int DisplayPriority => 0;

        public FrameNumberMessageAttachment() {
            contents = UnityEngine.Time.frameCount.ToString();
        }

    }
}
