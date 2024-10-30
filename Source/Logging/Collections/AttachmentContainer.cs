using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Savage.Logs.Collections {

    /// <summary> Decorations are organized into categories for efficient retrieval by <see cref="ILogSink"/>s. </summary>
    public class AttachmentContainer {

        /// <summary> When true there are no decorations inside this container. </summary>
       // public bool Empty { get; private set; }

        /// <inheritdoc cref="DisplayLocation.InlinePreceding"/>
        public SortedSet<MessageAttachment> InlinePreceding { get; private set; } = new SortedSet<MessageAttachment>();

        /// <inheritdoc cref="DisplayLocation.InlineTrailing"/>
        public SortedSet<MessageAttachment> InlineTrailing { get; private set; } = new SortedSet<MessageAttachment>();

        /// <inheritdoc cref="DisplayLocation.FollowingLine"/>
        public SortedSet<MessageAttachment> FollowingLine { get; private set; } = new SortedSet<MessageAttachment>();

        #region Constructors
        /// <summary>
        /// This constructor should only be used for creating a container that will never hold anything. <br/>
        /// C# 7.3 doesn't support default constructors on structs which is incredibly annoying.
        /// </summary>
        public AttachmentContainer() {
        }

        public AttachmentContainer(IEnumerable<MessageAttachment> decorations) {
            foreach (var decoration in decorations)
                Add(decoration);
        }
        #endregion Constructors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(MessageAttachment decoration) {
            switch (decoration.Location) {
                case DisplayLocation.InlinePreceding: InlinePreceding.Add(decoration); return;
                case DisplayLocation.InlineTrailing: InlineTrailing.Add(decoration); return;
                case DisplayLocation.FollowingLine: FollowingLine.Add(decoration); return;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
