
namespace Savage.Logs {

    /// <summary> Writes, displays, or sends <see cref="LogEntry"/> structs somewhere useful. </summary>
    public interface ILogSink {

        /// <summary> User configuration for this sink. </summary>
        LogSinkSettings Settings { get; }

        /// <summary> Outputs a log entry for this sink. </summary>
        void Write(LogEntry entry);
    }
}
