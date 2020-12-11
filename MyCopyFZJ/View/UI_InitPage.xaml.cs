using AForge.Video.DirectShow;
using APIFrameWork;
using BaseWindow.Common;
using InitControl;
using MachineFrameWork.Classes;
using MachineFrameWork.DistCardMachineInterface;
using MachineFrameWork.ExtDeviceInterface;
using MachineFrameWork.FingerprintInterface;
using MachineFrameWork.ICReaderInterface;
using MachineFrameWork.QRCodeReaderInterface;
using MisFrameWork.core;
using MyCopyFZJ.ComFunction;
using OpsCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyCopyFZJ.View
{
    /// <summary>
    /// UI_InitPage.xaml 的交互逻辑
    /// </summary>
    public partial class UI_InitPage : BasePage
    {
        private string READER_PATH;
        private string FZJ_PATH;
        private string FZJREADER_PATH;
        private string FG_PATH;
        private string FZJAPI_PATH;
        private string FZJEXTAPI_PATH;
        private string QRREADER_PATH;
        private string CameraName;
        private string ISENABLE_READER;//是否启用身份证读卡器
        private string ISENABLE_FGPRINT;//是否启用指纹仪
        private string ISENABLE_CAMERA;//是否启用摄像头
        private string ISENABLE_QRREADER;//是否启用扫描枪
        private string ISENABLE_FZJ;//是否启用发证机
        private string ISENABLE_FZJREADER;//是否启用发证机读卡器
        private string ISENABLE_FZJAPI;//是否启用数据库接口
        private string ISENABLE_FZJEXTAPI;//是否启用第三方接口
        private string ISENABLE_WEB_SOCKET;//是否启用后台服务
        private string ISENABLE_RESEND_DATA;//是否启用重新上传机制
        private string BAD_CARD_BOX;
        private string DEL_DAYS;
        private IEXT_API mIEXT_API;
        private string ADMIN_QR_MODE;
        private string MACHINE_NO;
        private string QRCODE;
        private QRCodeReader QRCodeReaderInterface;
        private FingerprintReaderInterface mFingerprintReaderInterface;
        private ISFZ_IDReaderInterface isfz_IDReaderInterface;
        private string FG_RATE;
        private string MODE_ERROR_CARD;
        private string IS_START_FG;//是否开启指纹登录，默认开启
        private string IS_START_BKCHECK;//是否启动后台清单服务
        private string ADMINACT;//管理员账号
        private string ADMINACT_NAME;//管理员姓名
        private string ADMINACT_SFZH;//管理员身份证号
        private string THEME_TYPE;
        private string UPLOAD_LOG;//存储柜异常数据上传
        private string DEV_MODEUL;//设备类型 1：发证机 2：证件柜 默认为发证机
        private string IS_CHECKADMIN_SFZ;//是否开启身份证登录，默认不开启

        public UI_InitPage()
        {
            InitializeComponent();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OpsHelper.Instance.CrashClear();
                FlashLogger.Debug("进入初始化界面");
                ComFunction.ComFunction.IsNeedRunFinger = false;
                if (THEME_TYPE.Equals("ThemeType_3.json")){
                    initdriver.SetFontColor(Brushes.Gray);
                }
                this.StopTimerTick();
                ComFunction.ComFunction.WorkQueueManualResetEvent.Set();
                ComFunction.ComFunction.IsNeedCheck = false;
                InitDriver.DriverInfo tmpdriver = new InitDriver.DriverInfo();
                if (ISENABLE_READER == "1"){
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "身份证读卡器";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_SFZReader);
                    initdriver.AddDriver(tmpdriver);
                }
                if (ISENABLE_CAMERA == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "摄像头";
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_Camera);
                }
                if (ISENABLE_FGPRINT == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "指纹仪";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_Fgprint);
                }
                if (ISENABLE_QRREADER == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "扫描枪";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_Scanner);
                    initdriver.AddDriver(tmpdriver);
                }

                if (ISENABLE_FZJAPI == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "数据库接口";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_FZJAPI);
                    initdriver.AddDriver(tmpdriver);
                }


                if (ISENABLE_FZJ == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "发证机";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_FZJ);
                    initdriver.AddDriver(tmpdriver);
                }

                if (ISENABLE_FZJREADER == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "发证机读卡器";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_FZJREADER);
                    initdriver.AddDriver(tmpdriver);
                }

                if (ISENABLE_FZJEXTAPI == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "第三方接口";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_FZJEXTAPI);
                    initdriver.AddDriver(tmpdriver);
                }

                tmpdriver = new InitDriver.DriverInfo();
                tmpdriver.DriverName = "清理垃圾文件";
                tmpdriver.IsEnable = true;
                tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(JunkFilesHandler_Delete);
                initdriver.AddDriver(tmpdriver);

                if (ISENABLE_WEB_SOCKET == "1")
                {
                    tmpdriver = new InitDriver.DriverInfo();
                    tmpdriver.DriverName = "启动后台服务";
                    tmpdriver.IsEnable = true;
                    tmpdriver.DeleDriverInfoHandler = new InitDriver.DeleDriverInfo(DriverInfoHandler_WebSocket);
                    initdriver.AddDriver(tmpdriver);
                }

                initdriver.InitDriverCompliteHandle = new InitDriver.InitDriverComplite(InitDriverComplite);

                initdriver.StartInit();

                this.ParentWindow.beforeExistProgramHandler += new BaseWindow.MainWindow.BeforeExistProgramHandler(BeforeExistProgramHandlerFun);
            }
            catch(Exception ex)
            {
                FlashLogger.Error("初始化加载线程异常", ex);
            }
        }

        private bool DriverInfoHandler_WebSocket(out string err_msg)
        {
            bool success;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化WebSocket");
                GlobalsFuncs.SetMachineWorkingPath(AppDomain.CurrentDomain.BaseDirectory);
                success=
            }catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Debug("初始化WebSocket异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        private bool JunkFilesHandler_Delete(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("清理本地垃圾文件");
                string argpath = AppDomain.CurrentDomain.BaseDirectory + "Args";
                DateTime Curr_time = DateTime.Now;
                if (Directory.Exists(argpath))
                {
                    foreach(string content in Directory.GetDirectories(argpath))
                    {
                        DirectoryInfo dir = new System.IO.DirectoryInfo(content);
                        DateTime DT = dir.CreationTime;
                        TimeSpan interval = Curr_time - DT;

                        if (interval.TotalDays > double.Parse(DEL_DAYS))
                        {
                            Directory.Delete(content, true);
                        }
                    }
                }

                argpath = AppDomain.CurrentDomain.BaseDirectory + "ReadCardLog";
                if (Directory.Exists(argpath))
                {
                    foreach(string content in Directory.GetDirectories(argpath))
                    {
                        if (Directory.Exists(content))
                        {
                            DirectoryInfo dir = new DirectoryInfo(content);
                            DateTime DT = dir.CreationTime;
                            TimeSpan interval = Curr_time - DT;

                            if (interval.TotalDays > double.Parse(DEL_DAYS))
                            {
                                Directory.Delete(content, true);
                            }
                        }else if (File.Exists(content))
                        {
                            FileInfo fi = new FileInfo(content);
                            TimeSpan interval = Curr_time - fi.CreationTime;

                            if (interval.TotalDays > double.Parse(DEL_DAYS))
                            {
                                File.Delete(content);
                            }
                        }
                    }
                }
                success = true;
                err_msg = "清除完毕";
            }catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Debug("清理本地垃圾文件异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        private bool DriverInfoHandler_FZJEXTAPI(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化第三方接口");
                APIGlobalsFuncs.SetMachineWorkingPath(FZJAPI_PATH);
                APIGlobalsFuncs.InitDll();

                int err_code = APIGlobalsFuncs.GetFZJ_EXTAPIObject(out mIEXT_API, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Error("初始化第三方接口失败" + err_msg + err_code.ToString());
                    success = false;
                }else
                {
                    if (UPLOAD_LOG == "1")
                    {
                        if (FlashLogger.GetOtherMsgHandler() == null)
                        {
                            FlashLogger.SetOtherMsgHandler(new FlashLogger.OtherMsgHandler(OtherThreadMsgFunHandler));
                        }
                    }

                    success = true;
                    if (IS_START_BKCHECK == "1")
                    {
                        int count = 0;
                        while (count < 3 && ComFunction.ComFunction.IsRunningCheck == true)
                        {
                            Thread.Sleep(2000);
                            count++;
                        }
                        if (ComFunction.ComFunction.IsRunningCheck == true)
                        {
                            FlashLogger.Error("后台清点线程无法终止");
                            success = false;
                        }else
                        {
                            ComFunction.ComFunction.BackWorkerCheck = new Thread(BackWorkerCheckFun);
                            ComFunction.ComFunction.BackWorkerCheck.IsBackground = true;
                            ComFunction.ComFunction.BackWorkerCheck.Start();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Error("初始化第三方接口异常:" + ex.Message);
                success = false;
            }
            return success;
        }

        private void BackWorkerCheckFun()
        {
            try
            {
                FlashLogger.Debug("开启后台清点线程");
                while (!ComFunction.ComFunction.IsRunningCheck)
                {
                    if (ComFunction.ComFunction.IsNeedCheck == false)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    string isrun = "1";
                    //调用第三方清点接口
                    Thread.Sleep(10000);
                    UnCaseSenseHashTable caseFromServer = new UnCaseSenseHashTable();
                    UnCaseSenseHashTable argForCheckStore = new UnCaseSenseHashTable();
                    string resCase = "";
                    string err_msg = "";
                    argForCheckStore["ADMINACT"] = ADMINACT;
                    argForCheckStore["MACHINE_NO"] = MACHINE_NO;
                    bool isok = mIEXT_API.CallFunExe("GetCheckStoreInfo", argForCheckStore.ToJsonString(), out resCase, out err_msg);
                    if (isok == false)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }
                    if (isrun == "1")
                    {
                        FlashLogger.Debug("后台发送清点状态");
                        ComFunction.ComFunction.IsNeedCheck = false;
                        this.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            this.ParentWindow.GotoAdminMode();
                            ComFun.OutPutParameterComFunWithKey(this.ParentWindow.UiConfig.GetGlobalVar(), "RUNBACKCHECK", "1");
                            this.ParentWindow.OpenProtectScreen(0);
                            ComFun.OutPutParameterComFunWithKey(this.ParentWindow.UiConfig.GetGlobalVar(), "BUSTYPEBIN", "3");
                            this.ParentWindow.JumpPage("Next");
                        });
                    }
                    else
                    {
                        FlashLogger.Debug("接口返回状态不对");
                    }
                    Thread.Sleep(1000);
                }
            }
            catch(Exception ex)
            {
                FlashLogger.Debug("后台清点状态检测线程异常", ex);
            }
            return;
        }

        private void OtherThreadMsgFunHandler(int type, string msg)
        {
            try
            {
                string result = "";
                string err_msg = "";
                UnCaseSenseHashTable ErrMess = new UnCaseSenseHashTable();
                ErrMess["NOTICETYPE"] = "1";
                ErrMess["MESSAGE"] = msg;
                ErrMess["MACHINE_NO"] = MACHINE_NO;
                ErrMess["DATA"] = "";

                switch (type)
                {
                    case 2://错误信息上传
                        if (mIEXT_API != null)
                        {
                            mIEXT_API.CallFunExe("LogRecordToOrigin", ErrMess.ToJsonString(), out result, out err_msg);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                FlashLogger.Error("异常信息上传接口异常" + ex);
            }
        }

        private bool DriverInfoHandler_FZJREADER(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化发证机读卡器");
                GlobalsFuncs.SetMachineWorkingPath(FZJREADER_PATH);
                GlobalsFuncs.InitDll();

                IICReader mIICReader;

                int err_code = GlobalsFuncs.GetICReaderObject(out mIICReader, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Error("初始化发证机读卡器出错:" + err_msg.ToString());
                    success = false;
                }else
                {
                    success = true;
                }
            }
            catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Error("初始化发证机读卡器异常:" + ex.Message);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 发证机
        /// </summary>
        /// <param name="err_msg"></param>
        /// <returns></returns>
        private bool DriverInfoHandler_FZJ(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化发证机");
                IDistCardMachine tmpIDistCardMachine;
                GlobalsFuncs.SetMachineWorkingPath(FZJ_PATH);
                GlobalsFuncs.InitDll();

                int err_code = GlobalsFuncs.GetDistCardMachineObject(out tmpIDistCardMachine, out err_msg, true);
                if (err_code != 0)
                {
                    string tmpRs = "";
                    UnCaseSenseHashTable resultUncase = new UnCaseSenseHashTable();
                    resultUncase["STATUS"] = Convert.ToString(err_code);
                    tmpIDistCardMachine.CallFunExe("GetErrorMessage", resultUncase.ToString(), out tmpRs, out err_msg);
                    UnCaseSenseHashTable tmpRsUncase = new UnCaseSenseHashTable();
                    tmpRsUncase.LoadFromJson(tmpRs);
                    err_msg = Convert.ToString(tmpRsUncase["MESSAGE"]);
                    FlashLogger.Error("初始化发证机出错：" + err_code.ToString() + " " + err_msg);
                    success = false;
                }else
                {
                    FlashLogger.Debug("开始初始化InitDevice");
                    err_code = tmpIDistCardMachine.InitDevice(0);
                    if (err_code == 0)
                    {
                        FlashLogger.Debug("判断中转有无卡");

                        string str_result;
                        bool flag;

                        
                    }
                }
            }
            catch(Exception ex)
            {
                err_msg = ErrorMessage(ex);
                FlashLogger.Debug("初始化发证机异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 数据库接口
        /// </summary>
        /// <param name="err_msg"></param>
        /// <returns></returns>
        private bool DriverInfoHandler_FZJAPI(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化数据库接口");
                APIGlobalsFuncs.SetMachineWorkingPath(FZJAPI_PATH);
                APIGlobalsFuncs.InitDll();

                IFZJ_APIInterface mIFZJ_APIInterface;
                int err_code = APIGlobalsFuncs.GetFZJ_APIObject(out mIFZJ_APIInterface, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Error("初始化数据库接口失败" + err_msg + err_code.ToString());
                    success = false;
                }else
                {
                    DateTime tmpdate = DateTime.Now.AddDays(-int.Parse(DEL_DAYS));
                    string yMdhms = string.Format("{0:yyyyMMddHHmmss}", tmpdate);
                    success = mIFZJ_APIInterface.RemoveJunkData(yMdhms, out err_msg);
                    if (success == false)
                    {
                        FlashLogger.Error("删除垃圾数据出错" + err_msg + err_code.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Error("初始化数据库接口异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 扫描枪
        /// </summary>
        /// <param name="err_msg"></param>
        /// <returns></returns>
        private bool DriverInfoHandler_Scanner(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化扫描枪");
                GlobalsFuncs.SetMachineWorkingPath(QRREADER_PATH);
                GlobalsFuncs.InitDll();

                int err_code = GlobalsFuncs.GetQRCodeReaderMachineObject(out QRCodeReaderInterface, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Error("初始化扫描枪失败：" + err_code.ToString() + err_msg);
                    success = false;
                }else
                {
                    QRCodeReaderInterface.ReadQR += QRCodeReaderInterface_ReadQR;
                    success = true;
                }
            }
            catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Error("初始化扫描枪异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        private void QRCodeReaderInterface_ReadQR(object sender, EventArgs e)
        {
            try
            {
                if (ComFunction.ComFunction.IsNeedReadQR == true)
                {
                    QRCodeEventArgs tmp = e as QRCodeEventArgs;
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        switch (ADMIN_QR_MODE)
                        {
                            case "0":
                                if (this.ParentWindow.GetSystemMode() == 0)
                                {
                                    this.ParentWindow.GotoAdminMode(tmp.qr_code.Replace("\r\n", ""));
                                }
                                break;

                            case "1":
                                if (mIEXT_API != null)
                                {
                                    if (this.ParentWindow.GetSystemMode() == 0)
                                    {
                                        UnCaseSenseHashTable tmpUncase = new UnCaseSenseHashTable();
                                        tmpUncase["QRCODE"] = tmp.qr_code.Replace("\r\n", "");
                                        QRCODE = tmp.qr_code.Replace("\r\n", "");
                                        tmpUncase["MACHINE_NO"] = MACHINE_NO;
                                        string result = "";
                                        string err_msg = "";
                                        bool ok = mIEXT_API.CallFunExe("AdminLoginInfo", tmpUncase.ToJsonString(), out result, out err_msg);
                                        // 检查返回结果
                                        if (string.IsNullOrWhiteSpace(result))
                                        {
                                            FlashLogger.Debug("管理员登陆失败" + err_msg);
                                            return;
                                        }
                                        UnCaseSenseHashTable tmpresult = new UnCaseSenseHashTable();
                                        tmpresult.LoadFromJson(result);
                                        if (ok == true)
                                        {
                                            ComFunction.ComFunction.Speak("管理员登陆成功");
                                            Thread.Sleep(2000);
                                            this.ParentWindow.GotoAdminMode();
                                            ComFunction.ComFunction.OutPutParameterCard_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "QRCODE", QRCODE);
                                            ComFunction.ComFunction.OutPutParameterAdmin_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "ADMINACT", Convert.ToString(tmpresult["ADMINACT"]));
                                            ComFunction.ComFunction.OutPutParameterAdmin_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "ADMINACT_NAME", Convert.ToString(tmpresult["ADMINNAME"]));

                                            FlashLogger.Debug("管理员登陆成功:账号" + Convert.ToString(tmpresult["ADMINACT"]) + "姓名:" + Convert.ToString(tmpresult["ADMINNAME"]));
                                        }
                                        else
                                        {
                                            FlashLogger.Debug("管理员登陆失败" + err_msg);
                                        }
                                    }
                                    else
                                    {
                                        //屏保是否已打开
                                        if (this.ParentWindow.GetProtectScreenStatus() == true)
                                        {
                                            this.ParentWindow.CloseProtectScreen();
                                        }
                                    }
                                }
                                break;
                            case "2":
                                break;
                        }
                    });
                }
            }catch(Exception ex)
            {
                FlashLogger.Error("管理员登陆异常" + ComFun.ErrorMessage(ex));
            }
        }

        /// <summary>
        /// 指纹仪
        /// </summary>
        /// <param name="err_msg"></param>
        /// <returns></returns>
        private bool DriverInfoHandler_Fgprint(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化指纹仪");
                GlobalsFuncs.SetMachineWorkingPath(FG_PATH);
                GlobalsFuncs.InitDll();

                int err_code = GlobalsFuncs.GetFingerprintMachineObject(out mFingerprintReaderInterface, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Error("初始化指纹仪失败：" + err_code.ToString());
                    success = false;
                }else
                {
                    success = true;
                    if (IS_START_FG == "1")
                    {
                        int count = 0;
                        while (count < 3 && ComFunction.ComFunction.IsRunningFinger == true)
                        {
                            Thread.Sleep(2000);
                            count++;
                        }
                        if (ComFunction.ComFunction.IsRunningFinger == true)
                        {
                            FlashLogger.Error("后台指纹仪无法终止");
                            success = false;
                        }else
                        {
                            ComFunction.ComFunction.IsRunningFinger = true;
                            ComFunction.ComFunction.BackWorkerFinger = new Thread(CheckAmdin_ReadFg);
                            ComFunction.ComFunction.BackWorkerFinger.IsBackground = true;
                            ComFunction.ComFunction.BackWorkerFinger.Start();
                            success = true;
                        }
                    }
                }
            }catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Error("初始化指纹仪异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 指纹登录管理员界面
        /// </summary>
        private void CheckAmdin_ReadFg()
        {
            try
            {
                FgInfo tmpFgInfo = new FgInfo() { Fg_name = "左手拇指", Fg_type = 1 };
                FlashLogger.Debug("开始执行管理员指纹比对线程");
                int countime = 0, err_code = 0;
                string err_msg = "";
                APIGlobalsFuncs.SetMachineWorkingPath(FZJAPI_PATH);
                APIGlobalsFuncs.InitDll();
                IFZJ_APIInterface tmpIFZJ_APIInterface;
                err_code = APIGlobalsFuncs.GetFZJ_APIObject(out tmpIFZJ_APIInterface, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Error("卡机查询初始化数据接口失败：" + err_code + " " + err_msg);
                    return;
                }
                while (ComFunction.ComFunction.IsRunningFinger) {
                    if (ComFunction.ComFunction.IsRunningFinger != true)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    List<UnCaseSenseHashTable> adminInfoList = new List<UnCaseSenseHashTable>();
                    bool isOk = tmpIFZJ_APIInterface.GetAllAdminMember(out adminInfoList, out err_msg);
                    if (isOk == false)
                    {
                        FlashLogger.Error("后台指纹登录线程：获取管理员信息失败" + err_msg);
                        return;
                    }

                    FingerprintData mdata = new FingerprintData();

                    int r = mFingerprintReaderInterface.ReadFingerprint(tmpFgInfo.Fg_type, ref mdata);
                    if (r != 0)
                    {
                        FlashLogger.Error("后台指纹登录线程：读取指纹失败：" + r.ToString());
                        Thread.Sleep(2500);
                    }else if (mdata.QualityScore > int.Parse(FG_RATE))
                    {
                        FlashLogger.Debug("比对指纹质量：" + Convert.ToString(mdata.QualityScore));
                        countime++;
                        if (countime > 3)
                        {
                            countime = 0;
                            FlashLogger.Debug("比对管理员指纹");
                            foreach(UnCaseSenseHashTable admininfo in adminInfoList)
                            {
                                if (string.IsNullOrWhiteSpace(admininfo["GLY_ZWY_ZWTZSJ"].ToString()))
                                {
                                    FlashLogger.Debug("管理员登录失败：没有对应管理员指纹");
                                    return;
                                }
                                byte[] src_finger = Convert.FromBase64String(admininfo["GLY_ZWY_ZWTZSJ"].ToString());
                                bool ismatch = mFingerprintReaderInterface.FeatureMatch(mdata.FgByte, src_finger, out err_msg);
                                if (ismatch == true)
                                {
                                    FlashLogger.Debug("比对管理员指纹成功");
                                    if (mIEXT_API != null)
                                    {
                                        if (this.ParentWindow.GetSystemMode() == 0)
                                        {
                                            UnCaseSenseHashTable tmpUncase = new UnCaseSenseHashTable();
                                            tmpUncase["sfzhm"] = admininfo["GMSFHM"].ToString();
                                            tmpUncase["finger"] = admininfo["GLY_ZWY_ZWTZSJ"].ToString();
                                            tmpUncase["MACHINE_NO"] = MACHINE_NO;
                                            string result = "";
                                            string err = "";
                                            bool ok = mIEXT_API.CallFunExe("AdminLoginInfo", tmpUncase.ToJsonString(), out result, out err);
                                            //检查返回结果
                                            if (string.IsNullOrWhiteSpace(result))
                                            {
                                                FlashLogger.Debug("管理员登陆失败" + err);
                                            }
                                            else
                                            {
                                                UnCaseSenseHashTable tmpresult = new UnCaseSenseHashTable();
                                                tmpresult.LoadFromJson(result);
                                                if (ok == true)
                                                {
                                                    ComFunction.ComFunction.Speak("管理员登录成功");
                                                    Thread.Sleep(1000);
                                                    ADMINACT = Convert.ToString(tmpresult["ADMINACT"]);
                                                    ADMINACT_NAME = Convert.ToString(tmpresult["ADMINNAME"]);
                                                    ADMINACT_SFZH = Convert.ToString(admininfo["GMSFHM"]);
                                                    FlashLogger.Debug("管理员登陆成功：账号" + Convert.ToString(tmpresult["ADMINACT"]) + "姓名：" + Convert.ToString(tmpresult["ADMINNAME"]));
                                                    this.Dispatcher.BeginInvoke((Action)delegate ()
                                                    {
                                                        this.ParentWindow.GotoAdminMode();
                                                        ComFunction.ComFunction.OutPutParameterCard_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "QRCODE", QRCODE);
                                                        ComFunction.ComFunction.OutPutParameterAdmin_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "ADMINACT", Convert.ToString(tmpresult["ADMINACT"]));
                                                        ComFunction.ComFunction.OutPutParameterAdmin_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "ADMINACT_NAME", Convert.ToString(tmpresult["ADMINNAME"]));
                                                        ComFunction.ComFunction.OutPutParameterAdmin_Info(this.ParentWindow.UiConfig.GetGlobalVar(), "ADMINACT_SFZH", Convert.ToString(admininfo["GMSFHM"]));
                                                    });
                                                }
                                                else
                                                {
                                                    FlashLogger.Debug("管理员登陆失败" + err);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (this.ParentWindow.GetProtectScreenStatus() == true)
                                            {
                                                this.ParentWindow.CloseProtectScreen();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(300);
                }
            }catch(Exception ex)
            {
                FlashLogger.Debug("指纹比对线程异常：" + ex.ToString());
            }
        }

        private bool DriverInfoHandler_Camera(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化摄像头");
                var devs = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                FilterInfo tmpFilterInfo = null;
                //实例化设备控制类
                foreach(var camera_device in devs)
                {
                    tmpFilterInfo = (FilterInfo)camera_device;
                    if (tmpFilterInfo.Name == CameraName)
                    {
                        success = true;
                        break;
                    }
                }
                if (success == false)
                {
                    FlashLogger.Debug("没找到摄像头" + CameraName);
                }
            }catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Debug("初始化摄像头异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        private bool DriverInfoHandler_SFZReader(out string err_msg)
        {
            bool success;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化身份证阅读器");
                GlobalsFuncs.SetMachineWorkingPath(READER_PATH);
                GlobalsFuncs.InitDll();
                ISFZ_IDReaderInterface isfz_IDReaderInterface;
                int err_code = GlobalsFuncs.GetSFZ_IDReaderObject(out isfz_IDReaderInterface, out err_msg);
                if (err_code != 0)
                {
                    FlashLogger.Debug("初始化身份证阅读器失败：" + err_msg);
                    success = false;
                }else
                {
                    success = true;
                }
            }catch(Exception ex)
            {
                err_msg = ex.Message;
                FlashLogger.Debug("初始化身份证阅读器异常：" + ex.Message);
                success = false;
            }
            return success;
        }

        private void BeforeExistProgramHandlerFun()
        {
            try
            {
                FlashLogger.Debug("开始退出程序前清理垃圾线程");
                ComFunction.ComFunction.IsRunningFinger = false;
                ComFunction.ComFunction.IsRunningCheck = true;
                Thread.Sleep(2000);
                if (ComFunction.ComFunction.BackWorkerFinger != null)
                {
                    try
                    {
                        ComFunction.ComFunction.BackWorkerFinger.Abort();
                    }catch(Exception ex)
                    {
                        FlashLogger.Debug("关闭线程BackWorkerFinger");
                    }
                }
                if (ComFunction.ComFunction.BackWorkerCheck != null)
                {
                    try
                    {
                        ComFunction.ComFunction.BackWorkerCheck.Abort();
                    }catch(Exception ex)
                    {
                        FlashLogger.Debug("关闭线程BackWorkerCheck");
                    }
                }
            }catch(Exception ex)
            {
                FlashLogger.Debug("退出程序前清除垃圾线程异常", ex);
            }
        }

        private string ErrorMessage(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("====================EXCEPTION====================");
            stringBuilder.AppendLine("【Message】:" + exception.Message);
            stringBuilder.AppendLine("【Source】:" + exception.Source);
            stringBuilder.AppendLine("【TargetSite】:" + ((exception.TargetSite != null) ? exception.TargetSite.Name : "无"));
            stringBuilder.AppendLine("【StackTrace】:" + exception.StackTrace);
            stringBuilder.AppendLine("【exception】:" + exception);
            stringBuilder.AppendLine("=================================================");
            if (exception.InnerException != null)
            {
                stringBuilder.AppendLine("====================INNER EXCEPTION====================");
                stringBuilder.AppendLine("【Message】:" + exception.InnerException.Message);
                stringBuilder.AppendLine("【Source】:" + exception.InnerException.Source);
                stringBuilder.AppendLine("【TargetSite】:" + ((exception.InnerException.TargetSite != null) ? exception.InnerException.TargetSite.Name : "无"));
                stringBuilder.AppendLine("【StackTrace】:" + exception.InnerException.StackTrace);
                stringBuilder.AppendLine(("【InnerException】:" + exception.InnerException) ?? "");
                stringBuilder.AppendLine("=================================================");
            }
            return stringBuilder.ToString().Replace("/r", "").Replace("/n", "");
        }
    }
}
