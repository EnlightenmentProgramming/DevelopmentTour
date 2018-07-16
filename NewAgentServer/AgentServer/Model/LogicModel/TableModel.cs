using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    public class TableModel
    {
        public string T_Name { get; set; }
        public string T_ID { get; set; }
        public int T_MX_Z { get; set; }
        public int T_MN_Z { get; set; }
        public int T_MX_X { get; set; }
        public int T_MN_X { get; set; }
        public int T_MX_H { get; set; }
        public int T_MN_H { get; set; }
        public int T_MX_XD { get; set; }
        public int T_MN_XD { get; set; }
        public int T_MX_ZD { get; set; }
        public int T_MN_ZD { get; set; }
        public string T_GameT { get; set; }
        /// <summary>
        /// 局数
        /// </summary>
        public string T_Ju { get; set; }
    }
}
