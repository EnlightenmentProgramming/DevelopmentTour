using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    public class SettleModel
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public string C_ID { get; set; }
        /// <summary>
        /// 代理ID
        /// </summary>
        public string A_ID { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string C_CreID { get; set; }
        public string IP { get; set; }
        public string Address { get; set; }
        public string IPLocal { get; set; }
        /// <summary>
        /// 结算起始时间
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 结算结束时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal C_Amount { get; set; }
        /// <summary>
        /// 洗码率
        /// </summary>
        public string C_WashR { get; set; }
        /// <summary>
        /// 结算记录总数
        /// </summary>
        public int C_Counts { get; set; }
        /// <summary>
        /// 洗码量
        /// </summary>
        public decimal C_WashS { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public string C_Type { get; set; }
        /// <summary>
        /// 结算源
        /// </summary>
        public string C_Source { get; set; }
        /// <summary>
        /// 结算目标
        /// </summary>
        public string C_Target { get; set; }
    }
}
