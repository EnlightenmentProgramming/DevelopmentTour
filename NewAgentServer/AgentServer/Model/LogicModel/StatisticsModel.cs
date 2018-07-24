using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    public class StatisticsModel
    {
        private int? _pagesize;
        private int? _curepage;
        public string A_ID { get; set; }
        public string A_PID { get; set; }
        public string GameT { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string C_ID { get; set; }
        public string C_UserID { get; set; }
        public string A_UserID { get; set; }
        public int? PageSize
        {
            get { return _pagesize; }
            set
            {
                if(value != null && value<0)
                {
                    _pagesize = 20;
                }
                else
                {
                    _pagesize = value;
                }
            }
        }

        public int? CurePage
        {
            get { return _curepage; }
            set
            {
                if(value !=null && value<0)
                {
                    _curepage = 1;
                }
                else
                {
                    _curepage = value;
                }
            }
        }
        /// <summary>
        /// 上下分统计时，是否查询包含上下分金额为0的数据
        /// </summary>
        public bool? IsAll { get; set; }
        /// <summary>
        /// 根据最小值进行过滤 大于此最小值
        /// </summary>
        public int? Minum { get; set; }
        public string T_ID { get; set; }
        public string T_Ju { get; set; }
        /// <summary>
        /// 上下分方式
        /// All = 所有  Third = 第三方上下分  Ohter = 非第三方上下分
        /// </summary>
        public string PointWay { get; set; }
        /// <summary>
        /// BD = 上分 XD = 下分  QK = 清卡  QL = 清零
        /// </summary>
        public string PointType { get; set; }

        /// <summary>
        /// Self = 自己的上下分  OwnA = 查询代理直属代理的上下分  OwnC = 查询直属会员的上下分  All = 代理分支下的上下分（包含代理自己）
        /// </summary>
        public string PointRange { get; set; }
    }
}
