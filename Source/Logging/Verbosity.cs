
namespace Savage.Logs {

	/// <summary> Denotes the purpose of a message, and allows filtering messages based on importance. </summary>
	public enum Verbosity : byte {

        /// <summary> 
        /// For providing context to programmers about what's happening inside a system *before* something of note occurs. <br/>
        /// This is the main verbosity you should be using to assist with debugging.
        /// </summary>
        /// <remarks>
        /// Example: record the configuration settings that are about to be used for an operation. <br/>
        /// Example: record the individual steps of a multi stage transaction. <br/>
        /// </remarks>
        Trace = 6,
        
        /// <summary> For recording information about data integrity or the final result of operations that may need to be reviewed in the future. </summary>
        /// <remarks>
        /// Example: record who did what in a database transaction so that it can be traced at some point in the future.
        /// </remarks>
        Audit = 5,

        /// <summary> Messages that are intended to help non programmers (IT, sysadmins, users) diagnose and solve issues with your program that shouldn't require code changes. </summary>
		/// <remarks> Example: let the user know their configuration/environment needs adjusting and how to do so. </remarks>
        Debug = 4,

        /// <summary> Information that's useful for identifying big picture information about what the program is doing for programmers or users. </summary>
        /// <remarks>
        /// Example: the program has successfully completed a large operation. <br/>
        /// Example:
        /// </remarks>
        Info = 3,

        /// <summary> Something unexpected has happened, this is a recoverable issue that the program should be able to continue in a normal state after. </summary>
        /// <remarks>
        /// Example: a server didn't respond to a request to check if it's still alive within 500ms, it may have been very busy and is still likely to respond to the next heartbeat check. <br/>
        /// Example: a fallback method is being used for an operation that failed the first try.
        /// </remarks>
        Warning = 2,

        /// <summary> An operation has failed in a way that cannot be automatically recovered from, the program is still running and may or may not be in a state where more issues are likely to occur. </summary>
        /// <remarks>
        /// Example: the program attempted to open a file and discovered it doesn't have access permission to read it's contents. <br/>
        /// Example: the program received data from a server or file but the data is corrupted or missing something important.
        /// </remarks>
        Error = 1,

        /// <summary> For the most serious unrecoverable issues, the program cannot continue in a normal state and needs to stop. </summary>
        /// <remarks>
        /// Example: the program relies on Vulkan for rendering to the screen, the user's hardware is older and doesn't support a graphics feature required by the program to run. <br/>
        /// Example: 
        /// </remarks>
        Fatal = 0
	}
}
