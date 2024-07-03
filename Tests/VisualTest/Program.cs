using RunawaySystems.Logging;
using RunawaySystems.Logging.LogDecorations;
using System.Diagnostics;

namespace VisualTest {
    internal class Program {
        private static Logging loggingInstance;

        static void Main(string[] args) {

            var settings = GlobalLoggingConfiguration.Default();
            var colorSettings = LoggerColorSettings.DefaultConsoleTheme();
            var consoleSettings = new LoggerSettings(colorSettings) {
                DisplayVerbosity = true
            };
            loggingInstance = new Logging().Initialize(settings).WithConsoleLogging(consoleSettings).WithFileLogging(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), "TestLog");

            Log.Trace("For providing context to programmers about what's happening inside a system *before* something of note occurs.");
            Log.Debug("Messages that are intended to help non programmers (IT, sysadmins, users) diagnose and solve issues with your program that shouldn't require code changes.");
            Log.Info("Information that's useful for identifying big picture information about what the program is doing for programmers or users.");
            Log.Warning("Something unexpected has happened, this is a recoverable issue that the program should be able to continue in a normal state after.");
            Log.Error("An operation has failed in a way that cannot be automatically recovered from, the program is still running and may or may not be in a state where more issues are likely to occur.");

            Log.Fatal("For the most serious unrecoverable issues, the program cannot continue in a normal state and needs to stop.");

            Trace.Write("System.Diagnostics.Debug and System.Diagnostics.Trace methods are all automatically captured by this logging library");
            Trace.Assert(false, "failed assertions are automatically logged with stack trace information");

            Thread task = new Thread(() => {
                NestedCall1();
                Log.Info("information coming from other threads can also have information attached to it", new[] { new ThreadIDDecoration() });
            });

            task.Start();

            //NestedCall1();

            Console.ReadLine();
        }

        private static void NestedCall1() {
            NestedCall2();
        }

        private static void NestedCall2() {
            Log.Error("Message with stack trace", new[] { new StackTraceDecoration() });
            NestedCall3();
        }

        private static void NestedCall3() {
            throw new Exception("unhandled exceptions are automatically logged");
        }

    }
}