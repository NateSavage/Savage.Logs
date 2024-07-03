using System;
using System.Collections.Generic;
using System.Text;

namespace Savage.Logs.LogDecorations {

    public sealed class VerbosityDecoration : LogDecoration {


        public override int DisplayPriority => -2_000;

        public override string Value { get; }

        public override DisplayLocation Location { get => InternalLocation; }

        public override string Tag => nameof(Verbosity);

        public override Type Type => typeof(Verbosity);

        internal static DisplayLocation InternalLocation;

        private Verbosity verbosity;

        public VerbosityDecoration(Verbosity verbosity) {
            this.verbosity = verbosity;
            Value = verbosity.ToString();
        }

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.ColorFor(verbosity);
    }
}
