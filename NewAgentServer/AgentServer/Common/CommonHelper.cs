using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class CommonHelper
    {

        /// <summary>
        /// 获取本地IP列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetLocIps()
        {
            List<string> arr = new List<string>();
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIPAddresses)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                    arr.Add(ip.ToString());
            }
            return arr.ToArray();
        }

    }
}
