using System;
using System.Collections.Generic;
using Savage.Logs.Collections;

namespace Savage.Logs {

    /// <summary> Settings that are applied to an individual <see cref="ILogSink"/>, may be shared between multiple <see cref="ILogSink"/>s. </summary>
    public class LogSinkSettings {

        /// <summary> 
        /// Whether verbosity information should be displayed as text for a message. <br/>
        /// Monochrome themes will always display verbosity.
        /// </summary>
        public bool DisplayVerbosity { get => _displayVerbosity || Theme.Monochrome; set => _displayVerbosity = value; }
        bool _displayVerbosity;

        /// <summary> Colors this logger should use to display messages. </summary>
        public Theme Theme;

        #region Construction

        public LogSinkSettings() {
            Theme = Theme.DefaultDark();
        }

        public LogSinkSettings(Theme colors) {
            Theme = colors;
        }

        #endregion Construction
            
    }
}
