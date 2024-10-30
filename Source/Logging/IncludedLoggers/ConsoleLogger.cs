using System;
using System.IO;
using System.Threading.Tasks;

//using static System.Drawing.ColorTranslator;

namespace Savage.Logs {

    /// <summary>
    /// Displays logs through the system's console through stderr. <br/>
    /// Thread safe and significantly lower latency than calling <see cref="System.Console.WriteLine()"/>.
    /// </summary>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    public class ConsoleLogger : PipelineNode, ILogSink {
        
        public LogSinkSettings Settings { get; set; }

        public Theme Theme { 
            get => _theme;
            set {
                LoadColorsFrom(value);
                _theme = value;
            } 
        }
        Theme _theme;

        readonly TextWriter _errorStream = Console.Error;

        ConsoleColor _monochromeColor;
        
        /// <summary> Color for all text that doesn't fall into the other catagories. </summary>
        ConsoleColor _textColor;

        /// <summary> Color for text that represents a type. </summary>
        ConsoleColor TypeColor;

        ConsoleColor _auditColor;
        ConsoleColor _traceColor;
        ConsoleColor _debugColor;
        ConsoleColor _infoColor;
        ConsoleColor _warningColor;
        ConsoleColor _errorColor;
        ConsoleColor _fatalColor;

        // state
        private DoubleBuffer<LogEntry> buffer = new DoubleBuffer<LogEntry>();
        private ConsoleColor previousConsoleColor;

        /// <summary> Task for moving data from the buffer and into the system console. </summary>
        Task bufferDaemon = Task.Run(DoNothing);

        #region Construction & Destruction

        public ConsoleLogger(LogSinkSettings settings = null, Theme? theme = null) {
            Settings = settings ?? new LogSinkSettings();
            Theme = theme ?? Theme.DefaultConsole();
        }

        void LoadColorsFrom(Theme colors) {
            if (colors.Monochrome) {
                _monochromeColor = colors.MonochromeColor.ToConsoleColor();
                return;
            }
            
            _textColor    = colors.TextColor.ToConsoleColor();
            TypeColor     = colors.TypeColor.ToConsoleColor();

            _auditColor   =  colors.AuditColor.ToConsoleColor();
            _traceColor   = colors.TraceColor.ToConsoleColor();
            _debugColor   = colors.DebugColor.ToConsoleColor();
            _infoColor    = colors.InfoColor.ToConsoleColor();
            _warningColor = colors.WarningColor.ToConsoleColor();
            _errorColor   = colors.ErrorColor.ToConsoleColor();
            _fatalColor   = colors.FatalColor.ToConsoleColor();
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

        static void DoNothing() { }

        void WriteFromBuffers() {
            do { 
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
            previousConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = _monochromeColor;
            
            foreach (var decoration in entry.Attachments.InlinePreceding)
                WriteInlineDecorationMonochrome(decoration);

            if (Settings.DisplayVerbosity)
                _errorStream.Write($"{entry.Verbosity}: ");
            _errorStream.Write(entry.Message);


            foreach (var decoration in entry.Attachments.InlineTrailing)
                WriteInlineDecorationMonochrome(decoration);

            foreach (var decoration in entry.Attachments.FollowingLine)
                WriteFollowingLineMonochrome(decoration);

            _errorStream.Write('\n');
            Console.ForegroundColor = previousConsoleColor;
        }

        private void WriteEntryToConsole(LogEntry entry) {
            previousConsoleColor = Console.ForegroundColor;


            foreach (var decoration in entry.Attachments.InlinePreceding)
                WriteInlineDecoration(decoration);

            Console.ForegroundColor = ColorFor(entry.Verbosity);
            if (Settings.DisplayVerbosity)
                _errorStream.Write($"{entry.Verbosity}: ");

            _errorStream.Write(entry.Message);


            foreach (var decoration in entry.Attachments.InlineTrailing)
                WriteInlineDecoration(decoration);

            foreach (var decoration in entry.Attachments.FollowingLine)
                WriteFollowingLine(decoration);


            _errorStream.Write('\n');
            Console.ForegroundColor = previousConsoleColor;
        }

        private void WriteInlineDecorationMonochrome(MessageAttachment attachment) {
            if (attachment.ShowTag)
                _errorStream.Write($"{attachment.Tag} ");

            _errorStream.Write($"{attachment.Value}: ");
        }

        private void WriteInlineDecoration(MessageAttachment attachment) {
            if (attachment.ShowTag) {
                Console.ForegroundColor = attachment.TagColor(ref _theme).ToConsoleColor();
                _errorStream.Write($"{attachment.Tag} ");
            }

            Console.ForegroundColor = attachment.ContentColor(ref _theme).ToConsoleColor();
            _errorStream.Write($"{attachment.Value}: ");
        }

        private void WriteFollowingLine(MessageAttachment attachment) {

            Console.ForegroundColor = attachment.TagColor(ref _theme).ToConsoleColor();
            _errorStream.Write($"\n    - {attachment.Tag}: ");
            Console.ForegroundColor = attachment.ContentColor(ref _theme).ToConsoleColor();

            int indentation = attachment.Tag.Length + 8;

            string[] lines = attachment.Value.Split('\n');
            _errorStream.WriteLine(lines[0]);
            for (int i = 1; i < lines.Length; ++i) {
                for (int x = 0; x < indentation; ++x)
                    _errorStream.Write(' ');
                _errorStream.WriteLine(lines[i]);
            }
        }

        void WriteFollowingLineMonochrome(MessageAttachment attachment) => _errorStream.Write($"\n    - {attachment.Tag}: {attachment.Value}");

        ConsoleColor ColorFor(Verbosity verbosity) {
            switch (verbosity) {
                case Verbosity.Trace:   return _traceColor;
                case Verbosity.Debug:   return _debugColor;
                case Verbosity.Info:    return _infoColor;
                case Verbosity.Warning: return _warningColor;
                case Verbosity.Error:   return _errorColor;
                case Verbosity.Fatal:   return _fatalColor;
                case Verbosity.Audit:   return _auditColor;
                default:
                    throw new NotImplementedException($"Logging verbosity {verbosity} is not known by the {nameof(ConsoleLogger)}!");
            };
        }

        void OnMonochromeModeChanged((bool MonochromeMode, LoggingColor MonochromeColor) data) {

        }

        #endregion Private Utility Methods

        public override bool Equals(object obj) => obj is ConsoleLogger;

        protected bool Equals(ConsoleLogger other) {
            return Equals(Settings, other.Settings);
        }

        public override int GetHashCode() {
            return (Settings != null ? Settings.GetHashCode() : 0);
        }
    }

    public static partial class PipelineNodeExtensions {

        /// <inheritdoc cref="ConsoleLogger"/>
        public static PipelineNode WriteToSystemConsole(this PipelineNode parentNode, LogSinkSettings settings = null, Theme? theme = null) {
            var logger = new ConsoleLogger(settings, theme);
            parentNode.WriteTo(logger);
            return logger;
        }
    }
}

