using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Savage.Logs {

    public sealed class StackTraceDecoration : LogDecoration {

        public override string Tag => tag;
        const string tag = "Stack Trace";

        public override bool ShowTag => true;

        public override string Value { get; }

        public override DisplayLocation Location => DisplayLocation.FollowingLine;

        public override int DisplayPriority => 10_000;


        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public override Type Type => typeof(StackFrame);

        public StackTraceDecoration(Exception exception = null, int skipFrames = 0) {

            var builder = new StringBuilder();
            StackTrace trace;
            if (exception is null)
                trace = new StackTrace(skipFrames + 1, fNeedFileInfo: true);
            else
                trace = new StackTrace(exception, skipFrames, fNeedFileInfo: true);

            StackFrame frame;
            MethodBase method;
            ParameterInfo[] parameters;
            int lineNumber;

            for (int i = 0; i < trace.FrameCount; ++i) {
                frame = trace.GetFrame(i);
                method = frame.GetMethod();
                parameters = method.GetParameters();
                builder.Append($"{method.DeclaringType.FullName}.{method.Name}(");
                for (int x = 0; x < parameters.Length; ++x) {
                    builder.Append(parameters[x].Name);
                    if (x < parameters.Length - 1)
                        builder.Append(", ");
                }
                builder.Append(')');

                lineNumber = frame.GetFileLineNumber();
                if (lineNumber != 0)
                    builder.Append($" @ line {lineNumber}");

                if (i < trace.FrameCount - 1)
                    builder.Append('\n');
            }

            Value = builder.ToString();
        }

        public override LoggingColor TagColor(ref Theme colorSettings) => colorSettings.TypeColor;

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.TraceColor;

    }

    public static partial class LogPipelineExtensions {
        public static void AttachStackTraces(this LogPipeline pipeline, Predicate<LogEntry> filter) {

        }
    }
}
