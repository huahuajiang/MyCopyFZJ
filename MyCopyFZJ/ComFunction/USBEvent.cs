using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MyCopyFZJ.ComFunction
{
    public class USBEvent
    {
        private ManagementEventWatcher insertWatcher = null;

        private ManagementEventWatcher removeWatcher = null;

        public Boolean AddUSBEventWatcher(EventArrivedEventHandler usbInsertHandler,EventArrivedEventHandler usbRemoveHandler,TimeSpan withinInterval)
        {
            try
            {
                ManagementScope Scope = new ManagementScope("root\\CIMV2");
                Scope.Options.EnablePrivileges = true;

                //USB插入监视
                if (usbInsertHandler != null)
                {
                    WqlEventQuery InsertQuery = new WqlEventQuery("__InstanceCreationEvent", withinInterval, "TargetInstance isa 'Win32_USBControllerDevice'");
                    insertWatcher = new ManagementEventWatcher(Scope, InsertQuery);
                    insertWatcher.EventArrived += usbInsertHandler;
                    insertWatcher.Start();
                }

                //USB拔出监视
                if (usbRemoveHandler != null)
                {
                    WqlEventQuery RemoveQuery = new WqlEventQuery("__InstanceDeletionEvent",
                        withinInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");

                    removeWatcher = new ManagementEventWatcher(Scope, RemoveQuery);
                    removeWatcher.EventArrived += usbRemoveHandler;
                    removeWatcher.Start();

                }
                return true;
            }
            catch(Exception ex)
            {
                RemoveUSBEventWatcher();
                return false;
            }


        }

        private void RemoveUSBEventWatcher()
        {
            if (insertWatcher != null)
            {
                insertWatcher.Stop();
                insertWatcher = null;
            }

            if (removeWatcher != null)
            {
                removeWatcher.Stop();
                removeWatcher = null;
            }
        }

        public static USBControllerDevice[] WhoUSBControllerDevice(EventArrivedEventArgs e)
        {
            ManagementBaseObject mbo = e.NewEvent["TargetInstance"] as ManagementBaseObject;
            if(mbo!=null&&mbo.ClassPath.ClassName== "Win32_USBControllerDevice")
            {
                String Antecedent = (mbo["Antecedent"] as String).Replace("\"", String.Empty).Split(new char[] { '=' })[1];
                String Dependent = (mbo["Dependent"] as String).Replace("\"", String.Empty).Split(new char[] { '=' })[1];
                return new USBControllerDevice[1] { new USBControllerDevice { Antecedent = Antecedent, Dependent = Dependent } };
            }
            return null;
        }

    }

    public struct USBControllerDevice
    {
        /// <summary>
        /// USB控制器设备ID
        /// </summary>
        public String Antecedent;

        /// <summary>
        /// USB即插即用设备ID
        /// </summary>
        public String Dependent;
    }
}
