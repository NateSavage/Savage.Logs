using System;
using System.IO;

namespace Savage.Logs {

    /// <summary> Attaches the name of the file containing the code requesting a message be logged. </summary>
    public sealed class CallingFileAttachment : MessageAttachment {
        public override string Tag => tag;
        const string tag = "Calling File";

        public override bool ShowTag => false;

        public override string Value { get; }
        public override DisplayLocation Location { get => InternalLocation; }
        internal static DisplayLocation InternalLocation;

        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;

        public override Type Type => typeof(string);

        public override int DisplayPriority => -2_000;

        public CallingFileAttachment(string callingFile, bool includeFileExtension = false) {
            string fileName;

            if (includeFileExtension ) fileName = String.Intern(Path.GetFileName(callingFile));
            else {
                fileName = Path.GetFileName(callingFile);
                fileName = String.Intern(fileName.AsSpan(0, fileName.LastIndexOf('.')).ToString());
            }
            
            Value = fileName;
        }

        public override LoggingColor TagColor(ref Theme colorSettings) => colorSettings.TypeColor;

        public override LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.TypeColor;
    }

    public static partial class LogPipelineExtensions {
        
        public static LogPipeline AttachCallerFileName(this LogPipeline logPipeline, bool includeFileExtension = false) {
            logPipeline.Settings.IncludeCallerFileName = true;
            logPipeline.Settings.IncludeCallerFileExtension = includeFileExtension;
            return logPipeline;
        }
    }
}
