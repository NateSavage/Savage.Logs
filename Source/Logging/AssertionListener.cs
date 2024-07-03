using Savage.Logs.LogDecorations;

using System.Diagnostics;

namespace Savage.Logs {

    /// <summary> Catches and logs messages from <see cref="System.Diagnostics.Trace"/> and <see cref="System.Diagnostics.Trace"/>. </summary>
    internal class AssertionListener : System.Diagnostics.TraceListener {

        private Verbosity writeVerbosity;
        private Verbosity failVerbosity;
        private bool recordCallerInformation;

        public override bool IsThreadSafe => true;

        private StackTrace trace;

        internal AssertionListener(Verbosity verbosity, bool recordCallerInformation) {
            writeVerbosity = Verbosity.Trace;
            failVerbosity = Verbosity.Warning;
            this.recordCallerInformation = recordCallerInformation;
        }

        #region Write Overrides

        /// <remarks> Called by <see cref="Debug.WriteLine(string)"/> and <see cref="Trace.WriteLine(string)"/>. </remarks>
        public override void WriteLine(string message) {
            if (recordCallerInformation)
                Log.Message(writeVerbosity, message, callerPath: GetCaller());
            else
                Log.Message(writeVerbosity, message);
        }

        /// <remarks> Called by <see cref="Debug.Write(string)"/> and <see cref="Trace.Write(string)"/>. </remarks>
        public override void Write(string message) => WriteLine(message);


        /// <remarks> Called by <see cref="Debug.WriteLine(object)"/> and <see cref="Trace.WriteLine(object)"/>. </remarks>
        public override void WriteLine(object instance) {
            if (recordCallerInformation)
                Log.Message(writeVerbosity, instance.ToString(), callerPath: GetCaller());
            else
                Log.Message(writeVerbosity, instance.ToString());
        }

        /// <remarks> Called by <see cref="Debug.Write(object)"/> and <see cref="Trace.Write(object)"/>. </remarks>
        public override void Write(object instance) => WriteLine(instance);


        /// <remarks> Called by <see cref="Debug.WriteLine(object, string)"/> and <see cref="Trace.WriteLine(object, string)"/>. </remarks>
        public override void WriteLine(object instance, string category) {
            if (recordCallerInformation)
                Log.Message(writeVerbosity, instance.ToString(), new[] { new LogDecoration<string>("Trace Event Category", category, DisplayLocation.FollowingLine) }, GetCaller());
            else
                Log.Message(writeVerbosity, instance.ToString(), new[] { new LogDecoration<string>("Trace Event Category", category, DisplayLocation.FollowingLine) });
        }

        /// <remarks> Called by <see cref="Debug.Write(object, string)"/> and <see cref="Trace.Write(object, string)"/>. </remarks>
        public override void Write(object instance, string category) => WriteLine(instance, category);



        /// <remarks> Called by <see cref="Debug.WriteLine(string, string)"/> and <see cref="Trace.WriteLine(string, string)"/>. </remarks>
        public override void WriteLine(string message, string category) {
            if (recordCallerInformation)
                Log.Message(writeVerbosity, message, new[] { new LogDecoration<string>("Trace Event Category", category, DisplayLocation.FollowingLine) }, callerPath: GetCaller());
            else
                Log.Message(writeVerbosity, message, new[] { new LogDecoration<string>("Trace Event Category", category, DisplayLocation.FollowingLine) });
        }

        /// <remarks> Called by <see cref="Debug.Write(string, string)"/> and <see cref="Trace.Write(string, string)"/>. </remarks>
        public override void Write(string message, string category) => WriteLine(message, category);

        #endregion Write Overrides

        // we always want to attach a stack trace when an assertion fails I think
        /// <remarks> Called when <see cref="Debug.Assert(bool, string)"/> and <see cref="Trace.Assert(bool, string)"/> evaluates to false, or fail method is called directly on either type. </remarks>
        public override void Fail(string message) {
            Log.Message(failVerbosity, message, new[] { new StackTraceDecoration(skipFrames: 1) });
        }

        // we always want to attach a stack trace when an assertion fails I think
        /// <remarks> Called when <see cref="Debug.Assert(bool, string, string)"/> and <see cref="Trace.Assert(bool, string, string)"/> evaluates to false, or fail method is called directly on either type. </remarks>
        public override void Fail(string message, string detailMessage) {
            Log.Message(failVerbosity, message, new LogDecoration[] {
                new LogDecoration<string>("Detail Message", detailMessage, displayLocation: DisplayLocation.FollowingLine),
                new StackTraceDecoration(skipFrames: 1)
                });
        }

        // does this cause problems?
        //public override void Close() {
        //    Trace.Listeners.Remove(this);
        //}

        private string GetCaller() {
            trace = new StackTrace(skipFrames: 1, fNeedFileInfo: true);
            return trace.GetFrame(trace.FrameCount - 1).GetFileName();
        }
    }
}
