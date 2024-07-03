

namespace Savage.Logs {

    /// <summary> Indicator of where data needs to be processed. </summary>
    public enum ThreadRequirement {
        /// <summary> Can be worked with on any thread. </summary>
        Any,
        /// <summary> Only the main thread. </summary>
        Main,
        /// <summary> Only the thread the data was received from. </summary>
        Source
    }
}
