using System;
using System.IO;
using System.Threading.Tasks;

//using static System.Drawing.ColorTranslator;

namespace Savage.Logs {

    /// <summary>
    /// Displays logs through the system's console through stderr. <br/>
    /// Thread safe and significantly lower latency than calling <see cref="System.Console.WriteLine()"/>.
    /// </summary>
    public class ConsoleLogger : ILogSink {

        // parameters
        public LogSinkSettings Settings { get; set; }

        public Theme Theme { 
            get => theme;
            set {
                LoadColorsFrom(Theme);
                theme = value;
            } 
        }
        private Theme theme;

        private TextWriter errorStream = Console.Error;

        /// <summary> Color for all text that doesn't fall into the other catagories. </summary>
        private ConsoleColor TextColor;

        /// <summary> Color for text that represents a type. </summary>
        private ConsoleColor TypeColor;

        private ConsoleColor TraceColor;
        private ConsoleColor DebugColor;
        private ConsoleColor InfoColor;
        private ConsoleColor WarningColor;
        private ConsoleColor ErrorColor;
        private ConsoleColor FatalColor;

        // state
        private DoubleBuffer<LogEntry> buffer = new DoubleBuffer<LogEntry>(64);
        private ConsoleColor previousConsoleColor;

        /// <summary> Task for moving data from the buffer and into the system console. </summary>
        Task bufferDaemon = Task.Run(DoNothing);

        #region Construction & Destruction

        public ConsoleLogger(LogSinkSettings settings, Theme theme) {
            Settings = settings;
            Theme = theme;
           
            //LogPipeline.MessageLogged += Write;
        }

        private void LoadColorsFrom(Theme colors) {
            TextColor    = colors.TextColor.ToConsoleColor();
            TypeColor    = colors.TypeColor.ToConsoleColor();

            TraceColor   = colors.TraceColor.ToConsoleColor();
            DebugColor   = colors.DebugColor.ToConsoleColor();
            InfoColor    = colors.InfoColor.ToConsoleColor();
            WarningColor = colors.WarningColor.ToConsoleColor();
            ErrorColor   = colors.ErrorColor.ToConsoleColor();
            FatalColor   = colors.FatalColor.ToConsoleColor();
        }

        #endregion Construction & Destruction

        public void Write(LogEntry entry) {
            lock (buffer.FrontLock) {
                buffer.Front.Add(entry);
            }

            // we write logs immediately if the daemon isn't currently in the middle of writing
            // we'll check to see if the front buffer has more messages to write when it's finished
            if (bufferDaemon.IsCompleted)
                bufferDaemon = Task.Run(WriteFromBuffers);
        }


        #region Private Utility Methods

        private static void DoNothing() { }

        private void WriteFromBuffers() {
            do {
                if (buffer.Front.Count > 0)
                    buffer.Swap();

                /*
                // attach decorations the console is supposed to provide
                for(int i = 0; i < Settings.LoggerDecorations.Count; ++i) {
                    Console.ForegroundColor = TypeColor;
                    errorStream.Write(Settings.LoggerDecorations[i].Tag);
                    Console.ForegroundColor = TextColor;
                    errorStream.Write($": {Settings.LoggerDecorations[i].Contents} ");
                }
                */

                //lock(buffer.Back) { // locking the back buffer is unnecessary because there can only one task that accesses it right now
                for (int i = 0; i < buffer.Back.Count; ++i) {
                    if (Theme.Monochrome)
                        WriteEntryToConsoleMonochrome(buffer.Back[i]);
                    else
                        WriteEntryToConsole(buffer.Back[i]);
                }
                buffer.Back.Clear();
                //}

            } while (buffer.Front.Count > 0); // one last check to see if anything was written into the front buffer while we were slowely writing the backbuffer to the console.
        }

        private void WriteEntryToConsoleMonochrome(LogEntry entry) {

            foreach (var decoration in entry.Decorations.InlinePreceding)
                WriteInlineDecorationMonochrome(decoration);

            if (Settings.DisplayVerbosity)
                errorStream.Write($"{entry.Verbosity}: ");
            errorStream.Write(entry.Message);


            foreach (var decoration in entry.Decorations.InlineTrailing)
                WriteInlineDecorationMonochrome(decoration);

            foreach (var decoration in entry.Decorations.FollowingLine)
                WriteFollowingLineMonochrome(decoration);

            errorStream.Write('\n');
        }

        private void WriteEntryToConsole(LogEntry entry) {
            previousConsoleColor = Console.ForegroundColor;


            foreach (var decoration in entry.Decorations.InlinePreceding)
                WriteInlineDecoration(decoration);

            Console.ForegroundColor = ColorFor(entry.Verbosity);
            if (Settings.DisplayVerbosity)
                errorStream.Write($"{entry.Verbosity}: ");

            errorStream.Write(entry.Message);


            foreach (var decoration in entry.Decorations.InlineTrailing)
                WriteInlineDecoration(decoration);

            foreach (var decoration in entry.Decorations.FollowingLine)
                WriteFollowingLine(decoration);


            errorStream.Write('\n');
            Console.ForegroundColor = previousConsoleColor;
        }

        private void WriteInlineDecorationMonochrome(LogDecoration decoration) {
            if (decoration.ShowTag)
                errorStream.Write($"{decoration.Tag} ");

            errorStream.Write($"{decoration.Value} ");
        }

        private void WriteInlineDecoration(LogDecoration decoration) {
            if (decoration.ShowTag) {
                Console.ForegroundColor = decoration.TagColor(ref theme).ToConsoleColor();
                errorStream.Write($"{decoration.Tag} ");
            }

            Console.ForegroundColor = decoration.ContentColor(ref theme).ToConsoleColor();
            errorStream.Write($"{decoration.Value}: ");
        }

        private void WriteFollowingLine(LogDecoration decoration) {

            Console.ForegroundColor = decoration.TagColor(ref theme).ToConsoleColor();
            errorStream.Write($"\n    - {decoration.Tag}: ");
            Console.ForegroundColor = decoration.ContentColor(ref theme).ToConsoleColor();

            int indentation = decoration.Tag.Length + 8;

            string[] lines = decoration.Value.Split('\n');
            errorStream.WriteLine(lines[0]);
            for (int i = 1; i < lines.Length; ++i) {
                for (int x = 0; x < indentation; ++x)
                    errorStream.Write(' ');
                errorStream.WriteLine(lines[i]);
            }
        }

        private void WriteFollowingLineMonochrome(LogDecoration decoration) => errorStream.Write($"\n    - {decoration.Tag}: {decoration.Value}");

        private ConsoleColor ColorFor(Verbosity verbosity) {
            switch (verbosity) {
                case Verbosity.Trace: return TraceColor;
                case Verbosity.Debug: return DebugColor;
                case Verbosity.Info: return InfoColor;
                case Verbosity.Warning: return WarningColor;
                case Verbosity.Error: return ErrorColor;
                case Verbosity.Fatal: return FatalColor;
                default:
                    throw new NotImplementedException($"Logging verbosity {verbosity} is not known by the {nameof(ConsoleLogger)}!");
            };
        }

        private void OnMonochromeModeChanged((bool MonochromeMode, LoggingColor MonochromeColor) data) {

        }

        #endregion Private Utility Methods

        public override bool Equals(object obj) => obj is ConsoleLogger;
    }

    public static partial class LogPipelineExtensions {

        /// <inheritdoc cref="ConsoleLogger"/>
        public static LogPipeline SinkSystemConsole(this LogPipeline pipeline, LogSinkSettings settings = null, Theme? theme = null) {
            pipeline.Add(new ConsoleLogger(settings, theme is null ? pipeline.Theme : theme.Value));
            return pipeline;
        }
    }
}

