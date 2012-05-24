using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Road.Base
{
    public class QueueMgr<T>
    {   
        public delegate void Exectue(T msg);

        private Queue<T> _queue;

        private bool _executing;

        private Exectue _exectue;

        private object _syncRoot;

        public delegate void AsynExecute(T info);

        private AsynExecute _asynExecute;

        private AsyncCallback _asynCallBack;

        public QueueMgr(Exectue exec)
        {
            _queue = new Queue<T>();
            _executing = false;
            _exectue = exec;
            _syncRoot = new object();
            _asynExecute = new QueueMgr<T>.AsynExecute(Exectuing);
            _asynCallBack = new AsyncCallback(AsynCallBack);
        }

        public void ExecuteQueue(T info) 
        {
            lock (_syncRoot)
            {
                if (_executing)
                {
                    _queue.Enqueue(info);
                    return;
                }
                else
                {
                    _executing = true;
                }
            }
            _asynExecute.BeginInvoke(info, _asynCallBack, _asynExecute);
        }

        private void AsynCallBack(IAsyncResult ar)
        {
            ((AsynExecute)ar.AsyncState).EndInvoke(ar);
        }

        private void Exectuing(T info)
        {
            _exectue(info);

            T newInfo;
            lock (_syncRoot)
            {
                if (_queue.Count > 0)
                {
                    newInfo = _queue.Peek();
                    _queue.Dequeue();

                }
                else
                {
                    _executing = false;
                    return;
                }
            }
            Exectuing(newInfo);

        }

    }
}
