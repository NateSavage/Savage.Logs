using System.Collections.Generic;

namespace Savage.Logs {

    /// <summary> 
    /// An implementation of a thread safe buffer with separate front and back portion. <br/>
    /// Double buffers can have their front and back buffer swapped so that write operations can be done on the front buffer while a separate piece of code is reading from the back buffer.
    /// </summary>
    internal class DoubleBuffer<T> {


        /// <remarks> You should put a lock on <see cref="FrontLock"/> while performing operations on this list if it may be accessed from other threads. </remarks>
        public List<T> Front { get; private set; }
        public readonly object FrontLock = new object();

        /// <remarks> You should  put a lock on <see cref="BackLock"/> while performing operations on this list if it may be accessed from other threads. </remarks>
        public List<T> Back { get; private set; }
        public readonly object BackLock = new object();

        // state
        private List<T> bufferA;
        private List<T> bufferB;

        #region Construction
        public DoubleBuffer() {
            bufferA = new List<T>();
            bufferB = new List<T>();
            Front = bufferA;
            Back = bufferB;
        }

        public DoubleBuffer(int startingSize) {
            bufferA = new List<T>(startingSize);
            bufferB = new List<T>(startingSize);
            Front = bufferA;
            Back = bufferB;
        }
        #endregion Construction

        /// <summary> Swaps the front and back buffer without moving their contents in memory. </summary>
        /// <remarks> Thread safe. </remarks>
        public void Swap() {
            lock (FrontLock) {
                lock (BackLock) {
                    if (ReferenceEquals(Front, bufferA)) {
                        Front = bufferB;
                        Back = bufferA;
                    }
                    else {
                        Front = bufferA;
                        Back = bufferB;
                    }
                }
            }
        }
    }
}
