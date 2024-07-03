using System;
using System.Collections.Generic;
using System.Text;

using Savage.Logs;

namespace Savage.Logs.UnityEditor {

    /// <summary> </summary>
    internal class FrameNumberLogDecoration : LogDecoration {

        public override string Tag => tag;
        private const string tag = "Frame Number";

        public override bool DisplayTag => true;

        public override string Contents => contents;
        private readonly string contents;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Main;

        public override DisplayLocation Location => DisplayLocation.InlinePreceding;

        public override Type Type => typeof(int);

        public override int DisplayPriority => 0;

        public FrameNumberLogDecoration() {
            contents = UnityEngine.Time.frameCount.ToString();
        }

    }
}
