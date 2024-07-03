using System;
using System.Collections.Generic;
using Savage.Logs.Collections;

namespace Savage.Logs {

    /// <summary> Settings that are applied to an individual <see cref="ILogSink"/>, may be shared between multiple <see cref="ILogSink"/>s. </summary>
    public class LogSinkSettings {

        /// <summary> Decorations that should be attached by the logger to all entries that pass through it. </summary>
        /// <remarks> <see cref="ILogSink"/> implementations should check if this is <see cref="DecorationContainer.Empty"/>. </remarks>
        public List<DecorationGenerator> DecorationGenerators;

        /// <summary> Functions that this sink will use to ignore or include messages. </summary>
        public List<Predicate<LogEntry>> Filters;

        /// <summary> 
        /// Whether verbosity information should be displayed as text for a message. <br/>
        /// Monochrome themes will always display verbosity.
        /// </summary>
        public bool DisplayVerbosity { get => displayVerbosity || Theme.Monochrome; set => displayVerbosity = value; }
        private bool displayVerbosity;

        /// <summary> Colors this logger should use to display messages. </summary>
        public Theme Theme;

        #region Construction

        public LogSinkSettings() {
            Theme = Theme.DefaultDarkTheme();
            Filters = new List<Predicate<LogEntry>>();
        }

        public LogSinkSettings(Theme colors) {
            Theme = colors;
            Filters = new List<Predicate<LogEntry>>();
        }

        #endregion Construction

        public LogSinkSettings AddFilter(Predicate<LogEntry> filter) {
            Filters.Add(filter);
            return this;
        }
    }
}
