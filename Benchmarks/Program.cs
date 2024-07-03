using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Savage.Logs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using ZLogger;


namespace Savage.Logs.Benchmarks {

public class Program {
    public static Microsoft.Extensions.Logging.ILogger ZLogger { get; private set;  }

    private static LogPipeline savageLogger;

    static void Main(string[] args) {

        var pipelineSettings = LogPipelineSettings.Default();
        savageLogger = new LogPipeline(pipelineSettings, Theme.DefaultConsoleTheme());


        Serilog.Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();

        using var factory = LoggerFactory.Create(logging => {
            logging.SetMinimumLevel(LogLevel.Trace);

            // Add ZLogger provider to ILoggingBuilder
            logging.AddZLoggerConsole();

        });

        ZLogger = factory.CreateLogger("Program");

        Summary? summary;
        if (TryParseArguments(args, out Mode runMode, out string argumentErrorMessage) is false) Console.WriteLine(argumentErrorMessage);
        else {
            switch (runMode) {
                case Mode.TestLatency:
                    summary = BenchmarkRunner.Run<ConsoleSinkLatency>();
                    break;

                case Mode.TestThroughput:
                    summary = BenchmarkRunner.Run<Throughput>();
                    break;
            }
        }

        Console.ReadLine();
    }


    enum Mode {
        None,
        TestLatency,
        TestThroughput
    }

    static bool TryParseArguments(string[] arguments, out Mode runMode, out string argumentErrorMessage) {
        runMode = Mode.None;
        argumentErrorMessage = "the only expected arguments are '-latency' '-l', or '-throughput' '-t'";

        if (arguments.Length == 0)
            return true;

        if (arguments.Length > 1)
            return false;

        switch (arguments[0]) {
            default:
                return false;

            case "-l":
            case "-latency":
                runMode = Mode.TestLatency;
                return true;


            case "-t":
            case "-throughput":
                runMode = Mode.TestThroughput;
                return true;
        }
    }
}

}