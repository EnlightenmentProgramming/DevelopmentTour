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
        /// 0000 --- 操作成功
        /// 0001 --- 系统内部错误
        /// 0002 --- 系统繁忙稍后再试
        /// 0003 --- 报文格式错误
        /// 0004 --- 提示错误
        /// 0005 --- 未知错误
        /// </summary>
        public string ErrNo { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}
