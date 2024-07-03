using System;

namespace Savage.Logs {

    /// <summary> Informs an output sink what colors it should use for various parts of a log. </summary>
    public struct Theme {

        /// <summary> Used to identify a theme. </summary>
        /// <remarks> 'DefaultDark' and 'DefaultLight' are the names of the two built in themes. </remarks>
        public string ThemeName;

        /// <summary> Should text be displayed using only a single color? </summary>
        public bool Monochrome;

        /// <summary> When <see cref="Monochrome"/> is true all text should be this color. </summary>
        public LoggingColor MonochromeColor;

        /// <summary> </summary>
        /// <remarks> Most locations logs are viewed through are unlikely to support this feature. The <see cref="ConsoleLogger"/> however will. </remarks>
        //public LoggingColor BackgroundColor;

        /// <summary> Color for all text that doesn't fall into the other categories. </summary>
        public LoggingColor TextColor { get => Monochrome ? MonochromeColor : textColor; set => textColor = value; }
        private LoggingColor textColor;

        /// <summary> Color for text that represents a type. </summary>
        public LoggingColor TypeColor { get => Monochrome ? MonochromeColor : typeColor; set => typeColor = value; }
        private LoggingColor typeColor;

        // verbosity colors
        /// <summary> Color for the text of a <see cref="Verbosity.Fatal"/> log. </summary>
        public LoggingColor FatalColor { get => Monochrome ? MonochromeColor : fatalColor; set => fatalColor = value; }
        private LoggingColor fatalColor;

        /// <summary> Color for the message text of a <see cref="Verbosity.Error"/> log. </summary>
        public LoggingColor ErrorColor { get => Monochrome ? MonochromeColor : errorColor; set => errorColor = value; }
        private LoggingColor errorColor;

        /// <summary> Color for the message text of a <see cref="Verbosity.Warning"/> log. </summary>
        public LoggingColor WarningColor { get => Monochrome ? MonochromeColor : warningColor; set => warningColor = value; }
        private LoggingColor warningColor;

        /// <summary> Color for the message text of a <see cref="Verbosity.Info"/> log. </summary>
        public LoggingColor InfoColor { get => Monochrome ? MonochromeColor : infoColor; set => infoColor = value; }
        private LoggingColor infoColor;

        /// <summary> Color for the message text of a <see cref="Verbosity.Debug"/> log. </summary>
        public LoggingColor DebugColor { get => Monochrome ? MonochromeColor : debugColor; set => debugColor = value; }
        private LoggingColor debugColor;

        /// <summary> Color for the message text of a <see cref="Verbosity.Trace"/> log. </summary>
        public LoggingColor TraceColor { get => Monochrome ? MonochromeColor : traceColor; set => traceColor = value; }
        private LoggingColor traceColor;

        #region Construction
        /// <summary> Creates a new <see cref="Theme"/> struct with the default dark theme. </summary>
        /// <remarks> TODO: color this theme in. </remarks>
        public static Theme DefaultDarkTheme() {
            return new Theme {
                ThemeName    = "DefaultDark",
                Monochrome = false,
                MonochromeColor = new LoggingColor(239, 239, 239), // white
                TypeColor    = new LoggingColor(78, 201, 171),     // teal
                FatalColor   = new LoggingColor(167, 41, 198),     // purple
                ErrorColor   = new LoggingColor(255, 41, 198),     // red
                WarningColor = new LoggingColor(255, 244, 63),     // yellow
                InfoColor    = new LoggingColor(239, 239, 239),    // white
                DebugColor   = new LoggingColor(128, 128, 128),    // dark gray
                TraceColor   = new LoggingColor(128, 128, 128),    // dark gray
            };
        }

        /// <summary> Creates a new <see cref="Theme"/> struct with the default light theme. </summary>
        /// <remarks> TODO: color this theme in. </remarks>
        public static Theme DefaultLightTheme() {
            return new Theme {
                ThemeName    = "DefaultLight",
                Monochrome = false,
                MonochromeColor = new LoggingColor(128, 128, 128), // dark gray
                TypeColor    = new LoggingColor(0, 128, 96),       // dark teal
                FatalColor   = new LoggingColor(167, 41, 198),     // purple
                ErrorColor   = new LoggingColor(255, 41, 198),     // red
                WarningColor = new LoggingColor(255, 63, 0),       // orange
                InfoColor    = new LoggingColor(128, 128, 128),    // dark gray
                DebugColor   = new LoggingColor(128, 128, 128),    // dark gray
                TraceColor   = new LoggingColor(128, 128, 128),    // dark gray
            };
        }

        /// <summary> Creates a new <see cref="Theme"/> struct with the terminal colors that map well to <see cref="System.ConsoleColor"/>s. </summary>
        public static Theme DefaultConsoleTheme() {
            return new Theme {
                ThemeName    = "ConsoleDefault",
                Monochrome = false,
                MonochromeColor = new LoggingColor(50, 50, 50), // ConsoleColor.DarkGray
                TextColor    = new LoggingColor(50, 50, 50),    // ConsoleColor.DarkGray
                TypeColor    = new LoggingColor(0, 255, 255),   // ConsoleColor.Cyan
                FatalColor   = new LoggingColor(255, 0, 255),   // ConsoleColor.Magenta
                ErrorColor   = new LoggingColor(255, 0, 0),     // ConsoleColor.Red
                WarningColor = new LoggingColor(255, 255, 0),   // ConsoleColor.Yellow
                InfoColor    = new LoggingColor(255, 255, 255), // ConsoleColor.White
                DebugColor   = new LoggingColor(0, 0, 255),     // ConsoleColor.Blue
                TraceColor   = new LoggingColor(50, 50, 50),    // ConsoleColor.DarkGray
            };
        }

        /// <summary> Monochrome color scheme for a terminal. </summary>
        public static Theme HackerGreenTheme() {
            return new Theme {
                ThemeName    = "HackerGreen",
                Monochrome = true,
                MonochromeColor = new LoggingColor(0, 255, 0) // ConsoleColor.Green
            };
        }

        /// <summary> Monochrome color scheme for a terminal. </summary>
        public static Theme MonochromeWhiteTheme() {
            return new Theme {
                ThemeName    = "MonochromeWhite",
                Monochrome = true,
                MonochromeColor = new LoggingColor(255, 255, 255)
            };
        }

        /// <summary> Monochrome color scheme for a terminal. </summary>
        public static Theme MonochromeGrayTheme() {
            return new Theme {
                ThemeName    = "MonochromeWhite",
                Monochrome = true,
                MonochromeColor = new LoggingColor(128, 128, 128)
            };
        }
        #endregion Construction

        /// <summary> Gets the appropriate color for a specific verbosity level. </summary>
        public LoggingColor ColorFor(Verbosity verbosity) {
            if (Monochrome)
                return MonochromeColor;

            switch (verbosity) {
                case Verbosity.Trace: return traceColor;
                case Verbosity.Debug: return debugColor;
                case Verbosity.Info: return infoColor;
                case Verbosity.Warning: return warningColor;
                case Verbosity.Error: return errorColor;
                case Verbosity.Fatal: return fatalColor;
                default: // default case should never be hit
                    throw new NotImplementedException($"{nameof(Verbosity)} {verbosity} is not known!");
            };
        }
    }
}
