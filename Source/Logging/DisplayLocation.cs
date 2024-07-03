
namespace Savage.Logs {

    /// <summary> Where a <see cref="LogDecoration"/> should be displayed relative to a <see cref="LogEntry"/> object's <see cref="LogEntry.Message"/>. </summary>
    public enum DisplayLocation {
        /// <summary> Decoration intended to be displayed on the same line as and before the message text. </summary>
        InlinePreceding,
        /// <summary> Decoration intended to be displayed on the same line as and after the message text. </summary>
        InlineTrailing,
        /// <summary> Decoration intended to be displayed on it's own line(s) following the message text. </summary>
        FollowingLine,
    }
}
