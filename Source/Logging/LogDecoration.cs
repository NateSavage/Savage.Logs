using System;
using System.Collections.Generic;
using System.Text;

namespace Savage.Logs {

    /// <summary> A decoration for a generic type of data. </summary>
    /// <remarks> 
    /// Not as memory efficient as declaring your own log decoration type that can statically define some of it's fields to share across instances. <br/>
    /// Intended for when you need to attach additional data to a log quickly in one off situations
    /// </remarks>
    public sealed class LogDecoration<T> : LogDecoration {

        public override string Tag { get; }

        public override Type Type => typeof(T);

        public override string Value { get; }

        public override DisplayLocation Location { get;  }

        public override int DisplayPriority { get; }

        public LogDecoration(string value, string tag, DisplayLocation displayLocation, int displayPriority = 0) {
            Value = value;
            Tag = tag;
            Location = displayLocation;
            DisplayPriority = displayPriority;
        }
    }

    /// <summary>
    /// Log decorations can be attached to every <see cref="LogEntry"/> written by the library. <br/>
    /// these decorations are used to add additional data to each entry, they can be configured to be automatically added to every log, or to logs passing through specific sinks.
    /// </summary>
    /// <remarks> Inherit from this class to log custom data types that you need to attach to log messages regularly. </remarks>
    public abstract class LogDecoration : IComparable<LogDecoration> {

        /// <summary> What this piece of data should be called or serialized under the name of. </summary>
        /// <remarks> Tags cannot contain whitespace. </remarks>
        public abstract string Tag { get; }

        public virtual bool ShowTag { get => false; }

        /// <summary> Actual data to append to the log, may be a message or data. </summary>
        public abstract string Value { get; }

        /// <summary> The type of data contained in <see cref="Value"/>. </summary>
        public abstract Type Type { get; }

        /// <summary> What thread you need to be working from when attaching this decoration to a <see cref="LogEntry"/></summary>
        public virtual ThreadRequirement ThreadRequirement { get => ThreadRequirement.Any; } 

        /// <inheritdoc cref="DisplayLocation"/>
        public abstract DisplayLocation Location { get; }

        /// <summary> lower values are rendered first, higher values appear later. </summary>
        public virtual int DisplayPriority { get => 0; }

        /// <summary> Color to apply to the <see cref="Tag"/> of this decoration if displayed. </summary>
        /// <param name="colorSettings"> The end user's current desired color settings. </param>
        public virtual LoggingColor TagColor(ref Theme colorSettings) => colorSettings.TypeColor;

        /// <summary> Color to apply to the <see cref="Value"/> of this decoration. </summary>
        /// <param name="colorSettings"> The end user's current desired color settings. </param>
        public virtual LoggingColor ContentColor(ref Theme colorSettings) => colorSettings.TextColor;


        // https://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys
        public int CompareTo(LogDecoration other) => other.DisplayPriority < DisplayPriority ? 1 : -1;
    }
}
