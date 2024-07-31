using System;
using System.Collections.Generic;
using Savage.Logs.Collections;

namespace Savage.Logs {

    /// <summary> Settings that are applied to an individual <see cref="ILogSink"/>, may be shared between multiple <see cref="ILogSink"/>s. </summary>
    public class LogSinkSettings {

        /// <summary> Decorations that should be attached by the logger to all entries that pass through it. </summary>
        /// <remarks> <see cref="ILogSink"/> implementations should check if this is <see cref="DecorationContainer.Empty"/>. </remarks>
        public readonly List<DecorationGenerator> DecorationGenerators;

        /// <summary> Functions that this sink will use to ignore or include messages. </summary>
        public readonly List<Predicate<LogEntry>> Filters;

        /// <summary> Whether verbosity information should be displayed as text for a message. </summary>
        public bool DisplayVerbosity { get => displayVerbosity; set => displayVerbosity = value; } 
        bool displayVerbosity;

        /// <summary> Colors this logger should use to display messages. </summary>
        public Theme Theme;

        #region Construction

        public LogSinkSettings() {
            displayVerbosity = true;
            Theme = Theme.DefaultDarkTheme();
            Filters = new List<Predicate<LogEntry>>();
            DecorationGenerators = new List<DecorationGenerator>();
        }

        public LogSinkSettings(Theme colors) {
            displayVerbosity = true;
            Theme = colors;
            Filters = new List<Predicate<LogEntry>>();
            DecorationGenerators = new List<DecorationGenerator>();
        }

        #endregion Construction

        public LogSinkSettings Add(Predicate<LogEntry> filter) {
            Filters.Add(filter);
            return this;
        }
        
        public LogSinkSettings Add(DecorationGenerator generator) {
            DecorationGenerators.Add(generator);
            return this;
        }
    }
}
