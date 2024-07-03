using System;
using System.Collections.Generic;
using System.Text;

namespace Savage.Logs.LogDecorations {

    /// <summary> Log decoration for including the time each log was recorded as a <see cref="DateTime"/>. </summary>
    public class WriteTimeDecoration : LogDecoration {
        public override string Tag => tag;
        const string tag = "Write Time";

        public override bool ShowTag => false;

        public override string Value { get; }

        public override DisplayLocation Location { get => InternalLocation; }
        internal static DisplayLocation InternalLocation;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public override Type Type => typeof(DateTime);

        public override int DisplayPriority => -10_000;

        public WriteTimeDecoration(string writeTime) {
            Value = $"[{writeTime}]";
        }

        public override LoggingColor TagColor(ref Theme colorSettings) => colorSettings.TypeColor;

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.TraceColor;
    }
}
