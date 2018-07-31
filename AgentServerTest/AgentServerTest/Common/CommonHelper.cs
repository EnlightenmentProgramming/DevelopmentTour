using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
        /// <summary>
        /// dataTable转成JSON
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dateFormate">设置序列化的时间格式</param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable table,string dateFormate)
        {
            var reval = DataTableToJson(table);
            if (string.IsNullOrEmpty(dateFormate)) return reval;
            reval = Regex.Replace(reval, @"\\/Date\((\d+)\)\\/", match =>
             {
                 DateTime dt = new DateTime(1970, 1, 1);
                 dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                 dt = dt.ToLocalTime();
                 return dt.ToString(dateFormate);
             });
            return reval;
        }
        /// <summary>
        /// datatable 转换成Json
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = int.MaxValue;
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(this string input)
        {
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower(CultureInfo.CurrentCulture);
            }
        }
        /// <summary>
        /// 将普通字符串转换为base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string String2Base64(string str)
        {
            try
            {
                byte[] b = Encoding.Default.GetBytes(str);
                //转成 Base64 形式的 System.String  
                return Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(CommonHelper), ex);
                return "";
            }
        }
        /// <summary>
        /// 将base64转换为普通字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base64ToStr(string str)
        {
            try
            {
                byte[] c = Convert.FromBase64String(str);
                return Encoding.Default.GetString(c);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 根据IP获取物理地址
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public static string ipToAddr(string ip)
        {
            string addr = "";
            try
            {
                WebRequest request = WebRequest.Create("http://wap.ip138.com/ip_search138.asp?ip=" + ip);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string read = reader.ReadToEnd();
                addr = Regex.Matches(read, "<b>.*?</b>")[1].Value.Replace("<b>", "").Replace("</b>", "");
            }
            catch { addr = "获取地理位置繁忙"; }
            return addr;
        }


    }
}
