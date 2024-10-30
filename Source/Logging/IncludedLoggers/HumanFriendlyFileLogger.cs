using System.IO;
using System.Text;

namespace Savage.Logs {

/// <summary> A sink that produces a log file that's formatted nicely for humans to read. </summary>
public class HumanFriendlyFileLogger : PipelineNode, ILogSink {
    
    public LogSinkSettings Settings { get; }
    
    readonly string _filePath;
    readonly TextWriter _writer;
    readonly FileStream _stream;
    
    public HumanFriendlyFileLogger(string logDirectory, string logName) {
        
        _filePath = Path.Combine(logDirectory, $"{logName}.txt");
        try { _stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.Read); }
        catch (System.Exception exception) {
            Log.Error(exception.Message);
            Log.Error($"{nameof(HumanFriendlyFileLogger)} will not be used.");
            return;
        }
        
        _writer = new StreamWriter(_stream, Encoding.UTF8);
    }

    ~HumanFriendlyFileLogger() {
        _writer?.Dispose();
        _stream?.Dispose();
    }

    public void Write(LogEntry entry) => _writer.WriteLine(entry.ToString());
}

public static partial class PipelineNodeExtensions {

    public static PipelineNode WriteToHumanFriendlyFile(this PipelineNode parentNode, string logDirectory, string logName) {
        var logger = new HumanFriendlyFileLogger(logDirectory, logName);
        parentNode.WriteTo(logger);
        return logger;
    }
}

}