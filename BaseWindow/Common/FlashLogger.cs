using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using OpsCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseWindow.Common
{
    public class FlashLogger
    {
        public delegate void OtherMsgHandler(int type, string msg);
        public static event OtherMsgHandler OtherMsgHandlerFun;

        public static void SetOtherMsgHandler(OtherMsgHandler otherMsgHandler)
        {
            OtherMsgHandlerFun += otherMsgHandler;
        }

        public static OtherMsgHandler GetOtherMsgHandler()
        {
            return OtherMsgHandlerFun;
        }

        /// <summary>
        /// 记录消息queue
        /// </summary>
        private readonly ConcurrentQueue<FlashLogMessage> _que;

        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEvent _mre;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// 日志
        /// </summary>
        private static FlashLogger _flashLog = new FlashLogger();

        /// <summary>
        /// 运维监控平台日志
        /// </summary>
        private readonly Opslog.ILog _opsLog;

        private FlashLogger()
        {
            _que = new ConcurrentQueue<FlashLogMessage>();
            _mre = new ManualResetEvent(false);
            _opsLog = OpsHelper.Instance.GetLog(this.GetType());
            _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "[%thread] %-5level - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Composite;
            roller.File = @"Log\";
            roller.DatePattern = "yyyyMMddHH'.txt'";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "1MB";
            roller.StaticLogFileName = false;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        /// <summary>
        /// 实现单例
        /// </summary>
        /// <returns></returns>
        public static FlashLogger Instance()
        {
            return _flashLog;
        }

        public void Register()
        {
            Thread t = new Thread(new ThreadStart(WriteLog));
            t.IsBackground = false;
            t.Start();
        }

        /// <summary>
        /// 从队列中写日志到磁盘
        /// </summary>
        private void WriteLog()
        {
            while (true)
            {
                //等待信息通知
                _mre.WaitOne();

                FlashLogMessage msg;
                //判断是否有内容需要入磁盘 从队列中获取内容，并删除队列中的内容
                while(_que.Count>0&&_que.TryDequeue(out msg))
                {
                    //判断日志等级，然后写日志
                    switch (msg.Level)
                    {
                        case FlashLogLevel.Debug:
                            _log.Debug(msg.Message, msg.Exception);
                            _opsLog.Debug(msg.Message + msg.Exception?.ToString());
                            OpsHelper.Instance.CrashWirte(msg.Message + msg.Exception?.ToString());
                            break;
                        case FlashLogLevel.Info:
                            _log.Info(msg.Message, msg.Exception);
                            _opsLog.Info(msg.Message + msg.Exception?.ToString());
                            break;
                        case FlashLogLevel.Error:
                            _log.Error(msg.Message, msg.Exception);
                            _opsLog.Error(msg.Message + msg.Exception?.ToString());
                            break;
                        case FlashLogLevel.Warn:
                            _log.Warn(msg.Message, msg.Exception);
                            _opsLog.Warn(msg.Message + msg.Exception?.ToString());
                            break;
                        case FlashLogLevel.Fatal:
                            _log.Fatal(msg.Message, msg.Exception);
                            _opsLog.Fatal(msg.Message + msg.Exception?.ToString());
                            break;
                    }
                    if (OtherMsgHandlerFun != null)
                    {
                        OtherMsgHandlerFun((int)msg.Level, msg.Message);
                    }

                }
                // 重新设置信号
                _mre.Reset();
                Thread.Sleep(1);
            }
        }

        public void EnqueueMessage(string message,FlashLogLevel level,Exception ex=null)
        {
            if((level==FlashLogLevel.Debug&&_log.IsDebugEnabled)
                ||(level==FlashLogLevel.Error&&_log.IsErrorEnabled)
                ||(level==FlashLogLevel.Fatal&&_log.IsFatalEnabled)
                ||(level==FlashLogLevel.Info&&_log.IsInfoEnabled)
                || (level == FlashLogLevel.Warn && _log.IsWarnEnabled))
            {
                _que.Enqueue(new FlashLogMessage
                {
                    Message = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff") + "]" + message,
                    Level = level,
                    Exception = ex
                });
                // 通知线程往磁盘中写日志
                _mre.Set();
            }
        }

        public static void Debug(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, FlashLogLevel.Debug, ex);
        }

        public static void Error(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, FlashLogLevel.Error, ex);
        }

        public static void Fatal(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, FlashLogLevel.Fatal, ex);
        }

        public static void Info(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, FlashLogLevel.Info, ex);
        }

        public static void Warn(string msg, Exception ex = null)
        {
            Instance().EnqueueMessage(msg, FlashLogLevel.Warn, ex);
        }
    }
}
