using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Model.Comunication
{
    /// <summary>
    /// 通信客户端
    /// </summary>
   public class ClientOP
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        public string ConID { get; set; }       
        /// <summary>
        /// 客户端ip地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 客户端端口号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConTime { get; set; }
        /// <summary>
        /// 连接用户
        /// </summary>
        public string LogName { get; set; }
        /// <summary>
        /// 连接socket对象
        /// </summary>
        public Socket cSocket { get; set; }
        /// <summary>
        /// 数据包类型
        /// </summary>
        public int Pac_Type { get; set; }
        /// <summary>
        /// 是否最后一个数据包
        /// </summary>
        public bool Pac_Fin { get; set; }
        /// <summary>
        /// 数据包消息内容
        /// </summary>
        public string Pac_Msg { get; set; }
        /// <summary>
        /// 是否flashweb
        /// </summary>
        public bool Pac_Flash { get; set; }
        /// <summary>
        /// 心跳时间
        /// </summary>
        public DateTime HeartTime { get; set; }

        public ClientOP()
        {
            ConTime = DateTime.Now;
        }
    }
}
