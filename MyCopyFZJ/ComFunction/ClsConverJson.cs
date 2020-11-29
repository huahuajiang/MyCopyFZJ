using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MyCopyFZJ.ComFunction
{
    public class ClsConverJson
    {
        public static Object JsonToObject(string dataStr,Type objType)
        {
            DataContractJsonSerializer jsSerializer = new DataContractJsonSerializer(objType);
            MemoryStream mStm = new MemoryStream(Encoding.UTF8.GetBytes(dataStr));
            Object obj = jsSerializer.ReadObject(mStm);
            mStm.Dispose();
            return obj;
        }

        public static string ObjectToJson(Object obj,Type objType)
        {
            DataContractJsonSerializer jsSerializer = new DataContractJsonSerializer(objType);
            MemoryStream mStm = new MemoryStream();
            jsSerializer.WriteObject(mStm, obj);
            byte[] dataBytes = new byte[mStm.Length];
            mStm.Seek(0, 0);
            mStm.Read(dataBytes, 0, (int)mStm.Length);
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}
