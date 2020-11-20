using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseWindow.Common
{
    public class FlashLogMessage
    {
        public string Message { get; set; }

        public FlashLogLevel Level { get; set; }

        public Exception Exception { get; set; }
    }
}
