using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Comunication
{
    /// <summary>
    /// 请求信息中需要的基本信息
    /// </summary>
    public class RequestMsg
    {
        /// <summary>
        /// 应答报文是否需要Gzip压缩 默认是压缩的
        /// "Y" --- 压缩
        /// "N" --- 不压缩
        /// </summary>
        public string IsZ { get; set; }
        /// <summary>
        /// 应答报文数据格式 默认是Json格式
        /// "1" --- "Json"
        /// </summary>
        public string RType { get; set; }
        /// <summary>
        /// 标记请求的客户端类型，根据特定的类型做特定的处理
        /// "1" --- 安卓  "2" --- 苹果 "3" --- flash "4" --- web "5" --- 微信
        /// </summary>
        public string CFlag { get; set; }
        /// <summary>
        /// 客户端版本号
        /// </summary>
        public string CVer { get; set; }
        /// <summary>
        /// 请求参数Json字符串
        /// </summary>
        public string RequestParams { get; set; }

    }
}
