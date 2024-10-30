using System;

namespace Savage.Logs {

    /// <summary> 
    /// A producer of a specific type of metadata to attach to logs. <br/>
    /// For example, you may want to be able to attach the current frame number in your game engine to your log. You would create a decoration generator that produces a log decoration containing the
    /// </summary>
    public class AttachmentGenerator<T> : AttachmentGenerator where T : MessageAttachment {
        public override ThreadRequirement ThreadRequirement => ThreadRequirement.Any;
        public override Type EmittedType => typeof(T);

        public override MessageAttachment CreateAttachmentFor(ref LogEntry logEntry) => Activator.CreateInstance<T>();
    }

    /// <summary> 
    /// Please inherit from <see cref="AttachmentGenerator{T}"/>. <br/>
    /// Consider this class for internal use only.
    /// </summary>
    public abstract class AttachmentGenerator {

        public abstract ThreadRequirement ThreadRequirement { get; }

        /// <summary> Type of the decoration this generator emits. </summary>
        public abstract Type EmittedType { get; }

        public abstract MessageAttachment CreateAttachmentFor(ref LogEntry logEntry);
    }
}
