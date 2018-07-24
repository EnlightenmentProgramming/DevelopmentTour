using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    /// <summary>
    /// 代理报表模板
    /// </summary>
    public class AgentReport
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        public string A_ID { get; set; }
        /// <summary>
        /// 可用余额
        /// </summary>
        public decimal A_Princple { get; set; }
        /// <summary>
        /// 群组余额
        /// </summary>
        public decimal A_GroupPrinc { get; set; }
        /// <summary>
        /// H5会员余额和
        /// </summary>
        public decimal H5Balance { get; set; }
        /// <summary>
        /// 总赢
        /// </summary>
        public decimal A_WinSum { get; set; }
        /// <summary>
        /// 洗码量
        /// </summary>
        public decimal A_WashSum { get; set; }
        /// <summary>
        /// 洗码费
        /// </summary>
        public decimal A_WashFee { get; set; }
        /// <summary>
        /// 和局量
        /// </summary>
        public decimal A_DrawSum { get; set; }
        /// <summary>
        /// 和局费
        /// </summary>
        public decimal A_DrawFee { get; set; }
        public decimal A_ChouS { get; set; }
    }
}
