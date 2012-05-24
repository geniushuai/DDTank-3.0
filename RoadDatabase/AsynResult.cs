using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RoadDatabase
{
    public class AsyncResult<T> : IAsyncResult
    {
        public AsyncResult(object asyncState)
        {
            this.AsyncState = asyncState;
            this.IsCompleted = false;
            this.AsyncWaitHandle = new ManualResetEvent(false);
        }

        public object AsyncState { get; private set; }

        public WaitHandle AsyncWaitHandle { get; private set; }

        public bool CompletedSynchronously { get { return false; } }

        public bool IsCompleted { get; private set; }

        public void Complete()
        {
            this.IsCompleted = true;
            (this.AsyncWaitHandle as ManualResetEvent).Set();
        }

        public T Result { get; set; }

        public Exception Exception { get; set; }
    }
}
