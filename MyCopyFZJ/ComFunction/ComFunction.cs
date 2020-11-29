using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCopyFZJ.ComFunction
{
    public static class ComFunction
    {
        public static ManualResetEvent WorkQueueManualResetEvent = new ManualResetEvent(false);
        public static WorkQueue BackEndJob = new WorkQueue();

    }
}
