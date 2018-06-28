using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Comunication
{
    /// <summary>
    /// 通讯报文
    /// </summary>
    public class ComunicationMsg
    {
        /// <summary>
        /// 通讯头
        /// </summary>
        public string Head { get; set; }
        /// <summary>
        /// 请求报文
        /// </summary>
        public string Request { get; set; }
        /// <summary>
        /// 应答报文
        /// </summary>
        public string Reponse { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }
    }
}
