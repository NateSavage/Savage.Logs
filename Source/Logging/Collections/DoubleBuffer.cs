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
        readonly List<T> _bufferA;
        readonly List<T> _bufferB;

        #region Construction
            
        public DoubleBuffer() {
            _bufferA = new List<T>();
            _bufferB = new List<T>();
            Front = _bufferA;
            Back = _bufferB;
        }

        public DoubleBuffer(int startingSize) {
            _bufferA = new List<T>(startingSize);
            _bufferB = new List<T>(startingSize);
            Front = _bufferA;
            Back = _bufferB;
        }
        #endregion Construction

        /// <summary> Swaps the front and back buffer without moving their contents in memory. </summary>
        /// <remarks> Thread safe. </remarks>
        public void Swap() {
            lock (FrontLock) {
                lock (BackLock) {
                    if (ReferenceEquals(Front, _bufferA)) {
                        Front = _bufferB;
                        Back = _bufferA;
                    }
                    else {
                        Front = _bufferA;
                        Back = _bufferB;
                    }
                }
            }
        }
    }
}
