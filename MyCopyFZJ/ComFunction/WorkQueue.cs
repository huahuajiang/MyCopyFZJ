using BaseWindow.Common;
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

        ~WorkQueue()
        {
            stopThreadHandler = true;
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

                        if (success == true)
                        {
                            FlashLogger.Debug("运行线程：" + tmpSubThreadInfo.ThreadName);
                            success = tmpSubThreadInfo.RunThreadInfo(out err_msg);
                            if (success == true)
                            {
                                if(!ThreadInfoQueue.TryDequeue(out tmpSubThreadInfo))
                                {
                                    FlashLogger.Error("无法出列线程：" + tmpSubThreadInfo.ThreadName);
                                }else
                                {
                                    FlashLogger.Debug("运行线程完成：" + tmpSubThreadInfo.ThreadName);
                                }
                            }else
                            {
                                FlashLogger.Debug("清空后续线程：" + tmpSubThreadInfo.ThreadName + "错误：" + err_msg);

                            }
                        }else
                        {
                            FlashLogger.Error("无法获取线程");
                        }
                    }
                    Thread.Sleep(500);
                }
            }catch(Exception ex)
            {
                FlashLogger.Error("AllThreadHandler" + ex.Message);
            }
        }

        /// <summary>
        /// 为线程的委托队列添加委托
        /// </summary>
        /// <param name="subthreadInfo"></param>
        /// <returns></returns>
        public bool AddWorker(SubThreadInfo subthreadInfo)
        {
            if (subthreadInfo == null)
            {
                return false;
            }
            ThreadInfoQueue.Enqueue(subthreadInfo);
            return true;
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        public void StopAllThreadHandler()
        {
            stopThreadHandler = true;
            AllThread.Abort();
            AllThread.Join();
        }

        public bool IsRunningSubThread()
        {
            if (ThreadInfoQueue.Count > 0)
            {
                return true;
            }else
            {
                return false;
            }
        }

        public void ReStartAllThread()
        {
            if (AllThread.IsAlive == false)
            {
                AllThread = new Thread(AllThreadHandler);
                AllThread.IsBackground = true;
                stopThreadHandler = false;
                AllThread.Start();
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
