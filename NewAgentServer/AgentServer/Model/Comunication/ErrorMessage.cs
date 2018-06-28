using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Comunication
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// 错误代码 
        /// </summary>
        public string ErrNo { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}
