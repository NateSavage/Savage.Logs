using System;

namespace Savage.Logs.LogDecorations {


    public sealed class CallerDecoration : LogDecoration {
        public override string Tag => tag;
        private const string tag = "Calling File";

        public override bool ShowTag => false;

        public override string Value { get; }
        public override DisplayLocation Location { get => InternalLocation; }
        internal static DisplayLocation InternalLocation;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public override Type Type => typeof(string);

        public override int DisplayPriority => -2_000;

        public CallerDecoration(string callingFile) {
            Value = callingFile;
        }

        public override LoggingColor TagColor(ref Theme colorSettings) => colorSettings.TypeColor;

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.TypeColor;
    }
}
