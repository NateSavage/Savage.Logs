#define DEBUG

using RunawaySystems.Logging;
using System.Diagnostics;



namespace Logging.Tests;

public class BasicRules {

    private static RunawaySystems.Logging.Logging loggingInstance;

    private void SetupTestEnvironment() {
        var settings = GlobalLoggingConfiguration.Default();
        loggingInstance = new RunawaySystems.Logging.Logging().Initialize(settings);
    }

    private class DisposableLogger : ILogger, IDisposable {
        public LoggerSettings Settings { get; } = new LoggerSettings();

        public bool Disposed = false;

        public void Dispose() {
            Disposed = true;
        }

        public void Write(LogEntry entry) { }
    }

    private class RecordingLogger : ILogger {
        public LoggerSettings Settings { get; } = new LoggerSettings();

        public bool Disposed = false;

        public List<LogEntry> LogEntries { get; } = new(4);

        public LogEntry? LastEntry => LogEntries.Count == 0 ? null : LogEntries[LogEntries.Count - 1];

        public void Write(LogEntry entry) {
            LogEntries.Add(entry);
        }
    }



    [Fact]
    public void ExceptionsAreLogged() {
        SetupTestEnvironment();

        throw new Exception("Crash");
    }


    [Fact]
    public void FailedTraceAssertionsAreLogged() {
        SetupTestEnvironment();
        var logger = new RecordingLogger();
        RunawaySystems.Logging.Logging.Register(logger);

        try{
            Trace.Assert(1 == 2, "Assertion failed");
        }
        catch(Exception ex) {

        }

        Assert.True(logger.LastEntry is not null, "Debug.Assert did not log anything on failure");
    }



    [Fact]
    public void LogsOnlyWriteToStdErr() {
        SetupTestEnvironment();

    }

    [Fact]
    public void LoggersAreDisposedOnProgramCrash() {
        SetupTestEnvironment();

        var disposableLogger = new DisposableLogger();
        Assert.False(disposableLogger.Disposed);

        RunawaySystems.Logging.Logging.Register(disposableLogger);
        RunawaySystems.Logging.Logging.DisposeLoggers();

        Assert.True(disposableLogger.Disposed, "loggers were not disposed");
    }

    [Fact]
    public void LoggersAreDisposedWhenLoggingClosed() {
        SetupTestEnvironment();

        var disposableLogger = new DisposableLogger();
        Assert.False(disposableLogger.Disposed);

        RunawaySystems.Logging.Logging.Register(disposableLogger);
        RunawaySystems.Logging.Logging.DisposeLoggers();

        Assert.True(disposableLogger.Disposed, "loggers were not disposed");
    }


    [Fact]
    public void VerbosityHidingWorks() {
        SetupTestEnvironment();

    }
}