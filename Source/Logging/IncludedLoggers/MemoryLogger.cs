using System.Collections.Generic;

namespace Savage.Logs {

/// <summary> For when you don't need your log messages to persist after your program has finished executing. Useful for debugging, you're not using logging as a replacement for your debugger though right? </summary>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
public class MemoryLogger : PipelineNode, ILogSink {
    // ReSharper disable once CollectionNeverQueried.Global
    public readonly List<LogEntry> LogEntries = new List<LogEntry>();
    
    /// <remarks> It takes some amount of time for messages to flow through the pipeline, don't assume this value will change immediately after you push a message to the pipeline! </remarks>
    public LogEntry LastEntry => LogEntries[LogEntries.Count - 1];

    public LogSinkSettings Settings { get; }

    public MemoryLogger(LogSinkSettings settings) {
        Settings = settings;
        LogEntries.Add(new LogEntry("", Verbosity.Trace));
    }

    public void Write(LogEntry entry) {
        LogEntries.Add(entry);
    }
}

public static partial class PipelineNodeExtensions {
    public static PipelineNode WriteToMemory(this PipelineNode parentNode, LogSinkSettings settings = null) {
        var logger = new MemoryLogger(settings);
        parentNode.WriteTo(logger);
        return logger;
    }
}

}