
namespace Savage.Logs {

	/// <summary> Denotes the purpose of a message, and allows filtering messages based on importance. </summary>
	public enum Verbosity : byte {

        /// <summary> 
        /// For providing context to programmers about what's happening inside a system *before* something of note occurs. <br/>
        /// This is the main verbosity you should be using to assist with debugging.
        /// </summary>
        /// <example1> Record the configuration settings that are about to be used for an operation. </example1>
        /// <example2> Record the individual steps of a multi-stage transaction. </example2>
        Trace = 6,

        /// <summary> Information that's useful for identifying big picture information about what the program is doing for programmers or users. </summary>
        /// <example> The program has successfully completed a large operation. </example>
        Info = 5,

        /// <summary> Something unexpected has happened, this is a recoverable issue and the program should still be able to continue in a normal state. </summary>
        /// <example1>
        /// A server didn't respond to a request to check if it's still alive within a reasonable amount of time, it may have been very busy and is still likely to respond to the next heartbeat check.
        /// </example1>
        /// <example2> A fallback method is being used for an operation that failed the first try. </example2>
        Warning = 4,

        /// <summary> An operation has failed that prevents the intended result from happening, the program is still running and may or may not be in a state where more issues are likely to occur. </summary>
        /// <example> Your program attempted to open a file, and discovered another program already has the file open and is blocking shared access. </example>
        Error = 3,

        /// <summary> For the most serious unrecoverable issues, the program cannot continue in a normal state and should stop. </summary>
        /// <example> Your program relies on Vulkan for rendering to the screen, the user's hardware is older and doesn't support a graphics feature required by the program to run. In this case there is no fallback currently implemented you can use. </example>
        Fatal = 2,
        
        
        /// <summary> Messages that are intended to help end users (IT, sysadmins, technical people) diagnose and solve issues with your program that shouldn't require code changes. </summary>
        /// <example> Let the user know their configuration/environment needs adjusting and how to do so. </example>
        Debug = 1,
        
        /// <summary> Messages that are intended to record information about transactions that may need to be verified or examined for record keeping purposes in the future. </summary>
        /// <example1> Recording the result of mutating a table in a database. </example1>
        Audit = 0,
	}
}
