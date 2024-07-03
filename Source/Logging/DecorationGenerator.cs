using System;

namespace Savage.Logs {

    /// <summary> 
    /// A producer of a specific type of metadata to attach to logs. <br/>
    /// For example, you may want to be able to attach the current frame number in your game engine to your log. You would create a decoration generator that produces a log decoration containing the
    /// </summary>
    public abstract class DecorationGenerator<T> : DecorationGenerator {

        public override Type EmittedType => typeof(T);

    }

    /// <summary> 
    /// Please inherit from <see cref="DecorationGenerator{T}"/>. <br/>
    /// Consider this class for internal use only.
    /// </summary>
    public abstract class DecorationGenerator {

        public abstract ThreadRequirement ThreadRequirement { get; }

        /// <summary> Type of the decoration this generator emits. </summary>
        public abstract Type EmittedType { get; }

        public abstract LogDecoration Emit(ref LogEntry logEntry);
    }
}
