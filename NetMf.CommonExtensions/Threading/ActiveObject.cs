#region License
// Copyright (c) 2011 Ross McDermott
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
    /// An object that has a thread associated with it. Enables execution of a set of logic
    /// within it's own thread in a thread safe manor.
    /// </summary>
    public abstract class ActiveObject
    {
        /// <summary>
        /// Locking implementation
        /// </summary>
        private MicroLock _lock = new MicroLock();

        /// <summary>
        /// Executing thread
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// State of execution
        /// </summary>
        private bool _running = false;

        /// <summary>
        /// The trigger to enable the active thread to stop. See example implementation of Run
        /// </summary>
        protected bool Running
        {
            get { return _running; }
            private set { _running = value; }
        }

        /// <summary>
        /// Locking object. To be used with lock(...) and MonitorMicro.XXX(...)
        /// </summary>
        protected object Locker
        {
            get { return _lock; }
        }

        /// <summary>
        /// Start the active object. This will not return until the thread
        /// </summary>
        /// <remarks>If already running, will do nothing.</remarks>
        public void Start()
        {
            lock (this._lock)
            {
                if (!this.Running && this._thread == null)
                {
                    this.Running = true;

                    _thread = new Thread(new ThreadStart(this.Bootstrap));
                    _thread.Start();

                    MonitorMicro.Wait(this._lock);
                }
            }
        }

        /// <summary>
        /// Stop the active object. This will not return until the thread has joined.
        /// </summary>
        public void Stop()
        {
            lock (this._lock)
            {
                if (this.Running)
                {
                    this.Running = false;

                    MonitorMicro.Pulse(this._lock);
                }
            }

            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }
            
        }

        /// <summary>
        /// Bootstrapper to enable the start method to return.
        /// </summary>
        private void Bootstrap()
        {
            lock (this._lock)
            {
                // enable the start method to return
                MonitorMicro.Pulse(_lock);
            }

            
            try
            {
                this.Run();
            }
            finally // ensure running is reset to false
            {
                this.Running = false;
            }
        }


        /// <summary>
        /// Method to implement which will run in its own thread.
        /// </summary>
        /// <example>
        /// protected override void Run()
        /// {
        ///      lock(this.Locker)
        ///      {
        ///           while(this.Running)
        ///           {
        ///                 // Check for work, otherwise wait.
        ///                 ...
        ///                 MicroMonitor.Wait(this.Locker);
        ///           }
        ///      }
        /// }
        /// </example>
        /// <remarks>
        /// Implementations must call MicroMonitor.Wait to allow for stopping of the thread
        /// when the Stop method is called.
        /// </remarks>
        protected abstract void Run();
        

    }
}
