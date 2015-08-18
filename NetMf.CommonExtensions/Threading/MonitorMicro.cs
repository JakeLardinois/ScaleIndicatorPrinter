#region License
// Copyright (c) 2010 Ross McDermott
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
#endregion

using System;
using Microsoft.SPOT;
using System.Threading;

namespace NetMf.CommonExtensions.Threading
{
    /// <summary>
    /// Implementation of the Pulse and Wait methods for the <see cref="Monitor"/> class.
    /// </summary>
    public abstract class MonitorMicro
    {
        /// <summary>
        /// Notifies a thread in the waiting queue of a change in the locked object's state.
        /// </summary>
        /// <param name="obj">The object a thread is waiting for.</param>
        /// <remarks>The calling thread should have ownership of the lock before entering this method</remarks>
        /// <exception cref="ArgumentNullException">Occurs if the provided lock object is null</exception>
        /// <exception cref="InvalidLockProvidedException">Occurs if the provided lock is not of type <see cref="MicroLock"/></exception>
        public static void Pulse(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (obj is MicroLock == false)
                throw new InvalidLockProvidedException();

            // re-entrant locks, so make sure we have the lock before trying anything else.
            lock(obj)
            {
                (obj as MicroLock).WaitHandle.Set();
            }
            
        }
        
        /// <summary>
        /// Releases the lock on an object and blocks the current thread until it reacquires the lock.
        /// </summary>
        /// <param name="obj">The object on which to wait.</param>
        /// <returns>true if the call returned because the caller reacquired the lock for the specified object. This method does not return if the lock is not reacquired.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the provided lock object is null</exception>
        /// <exception cref="InvalidLockProvidedException">Occurs if the provided lock is not of type <see cref="MicroLock"/></exception>
        public static bool Wait(object obj)
        {
            return Wait(obj, -1, false);
        }


        /// <summary>
        /// Releases the lock on an object and blocks the current thread until it reacquires the lock. If the specified time-out interval elapses, the thread enters the ready queue.
        /// </summary>
        /// <param name="obj">The object on which to wait.</param>
        /// <param name="timeout">A TimeSpan representing the amount of time to wait before the thread enters the ready queue. </param>
        /// <returns>true if the lock was reacquired before the specified time elapsed; false if the lock was reacquired after the specified time elapsed. The method does not return until the lock is reacquired.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the provided lock object is null</exception>
        /// <exception cref="InvalidLockProvidedException">Occurs if the provided lock is not of type <see cref="MicroLock"/></exception>
        public static bool Wait(object obj, TimeSpan timeout)
        {
            return Wait(obj, timeout, false);
        }

        /// <summary>
        /// Releases the lock on an object and blocks the current thread until it reacquires the lock. If the specified time-out interval elapses, the thread enters the ready queue.
        /// </summary>
        /// <param name="obj">The object on which to wait.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait before the thread enters the ready queue.</param>
        /// <returns>true if the lock was reacquired before the specified time elapsed; false if the lock was reacquired after the specified time elapsed. The method does not return until the lock is reacquired.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the provided lock object is null</exception>
        /// <exception cref="InvalidLockProvidedException">Occurs if the provided lock is not of type <see cref="MicroLock"/></exception>
        public static bool Wait(object obj, int millisecondsTimeout)
        {
            return Wait(obj, millisecondsTimeout, false);
        }


        /// <summary>
        /// Releases the lock on an object and blocks the current thread until it reacquires the lock. If the specified time-out interval elapses, the thread enters the ready queue. This method also specifies whether the synchronization domain for the context (if in a synchronized context) is exited before the wait and reacquired afterward.
        /// </summary>
        /// <param name="obj">The object on which to wait.</param>
        /// <param name="timeout">A TimeSpan representing the amount of time to wait before the thread enters the ready queue. </param>
        /// <param name="exitContext">true to exit and reacquire the synchronization domain for the context (if in a synchronized context) before the wait; otherwise, false.</param>
        /// <returns>true if the lock was reacquired before the specified time elapsed; false if the lock was reacquired after the specified time elapsed. The method does not return until the lock is reacquired.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the provided lock object is null</exception>
        /// <exception cref="InvalidLockProvidedException">Occurs if the provided lock is not of type <see cref="MicroLock"/></exception>
        public static bool Wait(object obj, TimeSpan timeout, bool exitContext)
        {
            long milliseconds = timeout.Days * TimeSpan.TicksPerDay;
            milliseconds = milliseconds + timeout.Hours * TimeSpan.TicksPerHour;
            milliseconds = milliseconds + timeout.Minutes * TimeSpan.TicksPerMinute;
            milliseconds = milliseconds + timeout.Seconds * TimeSpan.TicksPerSecond;
            milliseconds = milliseconds + timeout.Milliseconds * TimeSpan.TicksPerMillisecond;

            return Wait(obj, (int)(milliseconds / TimeSpan.TicksPerMillisecond), exitContext);
        }


        /// <summary>
        /// Releases the lock on an object and blocks the current thread until it reacquires the lock. If the specified time-out interval elapses, the thread enters the ready queue. This method also specifies whether the synchronization domain for the context (if in a synchronized context) is exited before the wait and reacquired afterward.
        /// </summary>
        /// <param name="obj">The object on which to wait.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait before the thread enters the ready queue. </param>
        /// <param name="exitContext">true to exit and reacquire the synchronization domain for the context (if in a synchronized context) before the wait; otherwise, false.</param>        
        /// <returns>true if the lock was reacquired before the specified time elapsed; false if the lock was reacquired after the specified time elapsed. The method does not return until the lock is reacquired.</returns>
        /// <exception cref="ArgumentNullException">Occurs if the provided lock object is null</exception>
        /// <exception cref="InvalidLockProvidedException">Occurs if the provided lock is not of type <see cref="MicroLock"/></exception>
        public static bool Wait(object obj, int millisecondsTimeout, bool exitContext)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (obj is MicroLock == false)
                throw new InvalidLockProvidedException();

            Monitor.Exit(obj);
            try
            {
                if (millisecondsTimeout != 0)
                {
                    return (obj as MicroLock).WaitHandle.WaitOne(millisecondsTimeout, exitContext);
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                Monitor.Enter(obj);
            }
        }

    }
}
