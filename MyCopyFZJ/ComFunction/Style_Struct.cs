using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCopyFZJ.ComFunction
{
    public class Style_Struct
    {
        public Datum[] THEME { get; set; }
        public class Datum
        {
            public string FILENAME { get; set; }
        }
    }
}
