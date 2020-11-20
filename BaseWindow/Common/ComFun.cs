using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseWindow.Common
{
    public class ComFun
    {
        public static string ErrorMessage(Exception exception)
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
