
namespace Savage.Logs {

    /// <summary> Where a <see cref="LogDecoration"/> should be displayed relative to a <see cref="LogEntry"/> object's <see cref="LogEntry.Message"/>. </summary>
    public enum DisplayLocation {
        /// <summary> Display on the same line as and before the message text. </summary>
        Preceding,
        /// <summary> Display on the same line as and after the message text. </summary>
        Trailing,
        /// <summary> Display on its own line(s) following the message text. </summary>
        FollowingLine,
    }
}
