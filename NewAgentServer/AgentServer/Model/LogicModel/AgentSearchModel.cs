using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    /// <summary>
    /// 代理相关搜索模板类
    /// </summary>
    public class AgentSearchModel
    {
        private int? _pagesize;
        private int? _curepage;
        private decimal? _washr;
        private decimal? _drawr;
        private decimal? _intor;
        private int? _mx;
        private int? _mn;
        private decimal? _princple;
        /// <summary>
        /// 代理ID
        /// </summary>
        public string A_ID { get; set; }
        /// <summary>
        /// 父级代理ID
        /// </summary>
        public string A_PID { get; set; }
        /// <summary>
        /// 代理登录账号
        /// </summary>
        public string A_UserID { get; set; }
        /// <summary>
        /// 代理名称
        /// </summary>
        public string A_Name { get; set; }
        /// <summary>
        /// 代理密码
        /// </summary>
        public string A_Pwd { get; set; }
        /// <summary>
        /// 系码率
        /// </summary>
        public decimal? A_WashR
        {
            get
            {
                return _washr;
            }
            set
            {
                if(value != null && value <0)//如果将系码率赋值为负数则强制改为0
                {
                    _washr = 0;
                }
                else
                {
                    _washr = value;
                }
            }
        }
        /// <summary>
        /// 洗码类型
        /// </summary>
        public bool? A_WashT { get; set; }
        /// <summary>
        /// 占成
        /// </summary>
        public decimal? A_IntoR
        {
            get
            {
                return _intor;
            }
            set
            {
                if (value != null && value < 0)//如果将洗码率赋值为负数则强制改为0
                {
                    _intor = 0;
                }
                else
                {
                    _intor = value;
                }
            }
        }
        /// <summary>
        /// 和局率
        /// </summary>
        public decimal? A_DrawR
        {
            get
            {
                return _drawr;
            }
            set
            {
                if (value != null && value < 0)//如果将和局率赋值为负数则强制改为0
                {
                    _drawr = 0;
                }
                else
                {
                    _drawr = value;
                }
            }
        }
        /// <summary>
        /// 最大限红
        /// </summary>
        public int? A_MX_Z
        {
            get
            {
                return _mx;
            }
            set
            {
                if(value != null && value <0 )
                {
                    _mx = 0;
                }
                else
                {
                    _mx = value;
                }
            }
        }
        /// <summary>
        /// 最小限红
        /// </summary>
        public int? A_MN_Z
        {
            get
            {
                return _mn;
            }
            set
            {
                if (value != null && value < 0)
                {
                    _mn = 0;
                }
                else
                {
                    _mn = value;
                }
            }
        }
        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal? A_Prncpl
        {
            get
            {
                return _princple;
            }
            set
            {
                if (value != null && value < 0)
                {
                    _princple = 0;
                }
                else
                {
                    _princple = value;
                }
            }
        }
        /// <summary>
        /// 搜索开始时间
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 搜索结束时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int? PageSize {
            get { return _pagesize; }
            set
            {
                if(value!=null && value <=0)
                {
                    _pagesize = 20;
                }
                else
                {
                    _pagesize = value;
                }
            }
        }
        /// <summary>
        /// 当前页
        /// </summary>
        public int? CurePage
        {
            get
            {
                return _curepage;
            }
            set
            {
                if(value != null && value <=0)
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
        /// 是否抽水
        /// </summary>
        public bool? A_SetPv { get; set; }
        /// <summary>
        /// 是否配分
        /// </summary>
        public bool? A_Matchp { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string A_State { get; set; }
        /// <summary>
        /// 原始密码
        /// </summary>
        public string A_OldPwd { get; set; }
        /// <summary>
        /// 上下分数量
        /// </summary>
        public int? A_Point { get; set; }
        /// <summary>
        /// true=下分  false = 下分
        /// </summary>
        public bool? A_IsAdd { get; set; }
        /// <summary>
        /// 是否是上级代理上分 "1"=上级代理上下分  其他=登录代理上下分
        /// 上级代理上分表示分源和去处是上级代理
        /// 登录代理上下分表示分源来源和去处是登录代理
        /// </summary>
        public string A_LevelPoint { get; set; }
        /// <summary>
        /// 配分及抽水权限Base64编码字符串值
        /// </summary>
        public string A_Perm { get; set; }
        public string A_F1 { get; set; }
        public string A_F2 { get; set; }
    }
}
