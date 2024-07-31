using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace Savage.Logs.LogDecorations {

    public sealed class ThreadIDDecoration : LogDecoration {
        public override string Tag => "Thread ID";
        public override bool ShowTag => true;

        public override Type Type => typeof(int);



        public override string Value => contents;
        private string contents;

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.InfoColor;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Source;

        public override DisplayLocation Location => DisplayLocation.Preceding;

        public override int DisplayPriority => -9_900;

        public ThreadIDDecoration() {
            contents =  Thread.CurrentThread.ManagedThreadId.ToString();
        }

    }
}
