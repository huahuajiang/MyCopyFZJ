using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCopyFZJ.ComFunction
{
    public class WorkQueue
    {
        private Thread AllThread;
        private ConcurrentQueue<SubThreadInfo> ThreadInfoQueue;
        private bool stopThreadHandler = true;
        public bool Option_parm_1 = true;
        public bool IsRuningOfThread = false;
        public delegate void ProccessMsg(string msg);
        public ProccessMsg ProccessMsgHandler;

        public WorkQueue()
        {
            ThreadInfoQueue = new ConcurrentQueue<SubThreadInfo>();
            AllThread = new Thread(AllThreadHandler);
            AllThread.IsBackground = true;
            stopThreadHandler = false;
            AllThread.Start();
        }

        private void AllThreadHandler()
        {
            bool success;
            string err_msg;
            try
            {
                while (true)
                {
                    if (stopThreadHandler == true)
                    {
                        return;
                    }
                    if (ThreadInfoQueue.Count > 0)
                    {
                        SubThreadInfo tmpSubThreadInfo;
                        success = ThreadInfoQueue.TryPeek(out tmpSubThreadInfo);
                    }
                }
            }
        }
    }

    public class SubThreadInfo
    {
        public string ThreadName;
        public delegate bool DelegateThreadInfo(out string err_msg);

        public DelegateThreadInfo DelegateThreadInfoHandler;

        public bool RunThreadInfo(out string err_msg)
        {
            bool success = true;
            err_msg = "";
            try
            {
                if (DelegateThreadInfoHandler != null)
                {
                    success = DelegateThreadInfoHandler(out err_msg);
                }
            }catch(Exception ex)
            {
                success = false;
                err_msg = ex.Message; 
            }
            return success;
        }
    }
}
