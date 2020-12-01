﻿using AForge.Video.DirectShow;
using APIFrameWork;
using BaseWindow.Common;
using InitControl;
using MachineFrameWork.Classes;
using MachineFrameWork.ExtDeviceInterface;
using MachineFrameWork.FingerprintInterface;
using MachineFrameWork.QRCodeReaderInterface;
using MyCopyFZJ.ComFunction;
using OpsCore;
using System;
using System.Collections.Generic;
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
        private string ADMINACT;
        private string ADMINACT_NAME;
        private string ADMINACT_SFZH;
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

        private bool DriverInfoHandler_Fgprint(out string err_msg)
        {
            bool success = false;
            err_msg = "";
            try
            {
                FlashLogger.Debug("初始化指纹仪");
                GlobalsFuncs.SetMachineWorkingPath(FG_PATH);
                GlobalsFuncs.InitDll();
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
    }
}
