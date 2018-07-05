using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Comunication
{
    /// <summary>
    /// 通讯头部报文
    /// </summary>
    public class HeadMessage
    {
        /// <summary>
        /// 登录账号，登录成功之后产生，以后每次交易均需传递
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 登录令牌 登录成功后产生，以后每次请求均需传递
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 客户端Ip地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 交易接口名称
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 执行方式  同步/异步
        /// </summary>
        public string ExeMode { get; set; }

    }
}
