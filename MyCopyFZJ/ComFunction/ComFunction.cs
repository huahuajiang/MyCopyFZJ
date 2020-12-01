using MisFrameWork.core;
using SpeechLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyCopyFZJ.ComFunction
{
    public static class ComFunction
    {
        public static ManualResetEvent WorkQueueManualResetEvent = new ManualResetEvent(false);
        public static WorkQueue BackEndJob = new WorkQueue();
        public static WorkQueue FontEndJob = new WorkQueue();
        public static SpVoice Voice = new SpVoice();

        public static Thread BackWorkerFinger;//后台指纹登录进程
        public static Thread BackWorkerCheck;//后台清点进程
        public static Thread BackWorkSFZ;//后台身份证登录进程

        public static bool IsNeedReadQR = true;//是否可以进行后台扫描二维码
        public static bool IsNeedRunFinger = true;//是否可以进行后台指纹登录进程
        public static bool IsRunningFinger = false;//是否正在运行后台指纹登录进程，与BackWorkerFinger成对出现
        public static bool IsNeedCheck = true;//是否可以进行后台清点进程
        public static bool IsRunningCheck = false;//是否正在运行后台清点进程，与BackWorkerCheck成对出现
        public static bool IsRunInitFace = true;//是否初始化人相比对库
        public static bool IsCheckSFZ = false;//身份证比对线程

        public static bool IsInt(string value)
        {
            try
            {
                int.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Speak(string str)
        {
            if (str.Length > 40)
            {
                SpeechVoiceSpeakFlags SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
                Voice.Skip("Sentence", 30);
                if (str != null)
                    Voice.Speak(str, SpFlags);
            }
        }

        public static void IsVisible(bool isvisible,Control control)
        {
            if (control != null)
            {
                control.Visibility = System.Windows.Visibility.Visible;
            }else
            {
                control.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public static UnCaseSenseHashTable GetCardGlobalValue(UnCaseSenseHashTable AllDate)
        {
            UnCaseSenseHashTable card_global_value = new UnCaseSenseHashTable();

            if (AllDate.HasKeyValue("CARD_GLOBAL_VALUE"))
            {
                card_global_value = (UnCaseSenseHashTable)AllDate["CARD_GLOBAL_VALUE"];
            }else
            {
                AllDate.Add("CARD_GLOBAL_VALUE", card_global_value);
            }
            return card_global_value;
        }

        /// <summary>
        /// 获取管理员所有信息
        /// </summary>
        /// <param name="AllDate"></param>
        /// <returns></returns>
        public static UnCaseSenseHashTable GetAdminGlobalValue(UnCaseSenseHashTable AllDate)
        {
            UnCaseSenseHashTable card_global_value = new UnCaseSenseHashTable();

            if (AllDate.HasKeyValue("ADMIN_GLOBAL_VALUE"))
            {
                card_global_value = (UnCaseSenseHashTable)AllDate["ADMIN_GLOBAL_VALUE"];
            }
            else
            {
                AllDate.Add("ADMIN_GLOBAL_VALUE", card_global_value);
            }

            return card_global_value;
        }

        /// <summary>
        /// 输出管理员所有信息
        /// </summary>
        /// <param name="AllDate"></param>
        /// <param name="Key"></param>
        /// <param name="PageParameter"></param>
        public static void OutPutParameterAdmin_Info(UnCaseSenseHashTable AllDate,string Key,string PageParameter)
        {
            if (PageParameter == null)
            {
                PageParameter = "";
            }

            UnCaseSenseHashTable card_Info = GetAdminGlobalValue(AllDate);

            if (card_Info.HasKeyValue(Key))
            {
                card_Info[Key] = PageParameter;
            }else
            {
                card_Info.Add(Key, PageParameter);
            }
        }

        public static void InPutParameterAdmin_Info(UnCaseSenseHashTable AllDate,string Key,ref string PageParameter,string DefaultValue = "")
        {
            UnCaseSenseHashTable person_info = GetAdminGlobalValue(AllDate);
            PageParameter = DefaultValue;

            if (person_info.HasKeyValue(Key))
            {
                PageParameter = person_info[Key].ToString();
            }
        }

        public static void OutPutParameterCard_Info(UnCaseSenseHashTable AllDate,string Key,string PageParameter)
        {
            if (PageParameter == null)
            {
                PageParameter = "";

            }

            UnCaseSenseHashTable card_Info = GetCardGlobalValue(AllDate);

            if (card_Info.HasKeyValue(Key))
            {
                card_Info[Key] = PageParameter;
            }else
            {
                card_Info.Add(Key, PageParameter);
            }
        }
    }
}
