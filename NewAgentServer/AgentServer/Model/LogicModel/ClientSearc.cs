using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    public class ClientSearc
    {
        private int? _pagesize;
        private int? _curepage;
        private int? _mx;
        private int? _mn;
        private decimal? _balance;
        private decimal? _washr;
        private decimal? _drawr;
        private decimal? _odx;
        private decimal? _odxd;
        private decimal? _odz;
        private decimal? _odzd;
        private decimal? _odh;
        private decimal? _odl;
        private decimal? _odf;
        private decimal? _odhe;
        private int? _point;

        /// <summary>
        /// 会员ID
        /// </summary>
        public string C_ID { get; set; }
        /// <summary>
        /// 会员所属代理ID
        /// </summary>
        public string C_AID { get; set; }
        /// <summary>
        /// 会员登录名称
        /// </summary>
        public string C_UserID { get; set; }
        /// <summary>
        /// 会员名称
        /// </summary>
        public string C_Name { get; set; }
        /// <summary>
        /// 会员登录密码
        /// </summary>
        public string C_Pwd { get; set; }
        /// <summary>
        /// 会员状态 YES=启用 PAUSE=暂停 NO=锁定
        /// </summary>
        public string C_State { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 每页容量
        /// </summary>
        public int? PageSize
        {
            get
            {
                return _pagesize;
            }
            set
            {
                if(value != null && value <=0)
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
        /// 洗码类型
        /// </summary>
        public bool? C_WashT { get; set; }
        /// <summary>
        /// 最大限红
        /// </summary>
        public int? C_MX_Z
        {
            get
            {
                return _mx;
            }
            set
            {
                if (value != null && value < 0)
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
        public int? C_MN_Z
        {
            get
            {
                return _mn;
            }
            set
            {
                if(value != null && value< 0)
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
        /// 可用余额
        /// </summary>
        public decimal? C_Balnc
        {
            get
            {
                return _balance;
            }
            set
            {
                if(value != null && value <0)
                {
                    _balance = 0;
                }
                else
                {
                    _balance = value;
                }
            }
        }
        /// <summary>
        /// 系码率
        /// </summary>
        public decimal? C_WashR
        {
            get
            {
                return _washr;
            }
            set
            {
                if(value != null && value < 0)
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
        /// 和局率
        /// </summary>
        public decimal? C_DrawR
        {
            get
            {
                return _drawr;
            }
            set
            {
                if(value != null && value <0)
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
        /// 闲赔率
        /// </summary>
        public decimal? C_ODX
        {
            get { return _odx; }
            set
            {
                if(value != null && value <0)
                {
                    _odx = 0;
                }
                else
                {
                    _odx = value;
                }
            }
        }
        /// <summary>
        /// 闲对赔率
        /// </summary>
        public decimal? C_ODXD
        {
            get { return _odxd; }
            set
            {
                if(value != null && value <0)
                {
                    _odxd = 0;
                }
                else
                {
                    _odxd = value;
                }
            }
        }
        /// <summary>
        /// 庄赔率
        /// </summary>
        public decimal? C_ODZ
        {
            get { return _odz; }
            set
            {
                if(value !=null && value <0)
                {
                    _odz = 0;
                }
                else
                {
                    _odz = value;
                }
            }
        }
        /// <summary>
        /// 庄对赔率
        /// </summary>
        public decimal? C_ODZD
        {
            get { return _odzd; }
            set
            {
                if(value != null && value <0)
                {
                    _odzd = 0;
                }
                else
                {
                    _odzd = value;
                }
            }
        }
        /// <summary>
        /// 和赔率
        /// </summary>
        public decimal? C_ODH
        {
            get { return _odh; }
            set
            {
                if(value != null && value <0)
                {
                    _odh = 0;
                }
                else
                {
                    _odh = value;
                }
            }
        }
        /// <summary>
        /// 龙赔率
        /// </summary>
        public decimal? C_ODL
        {
            get { return _odl; }
            set
            {
                if(value != null && value <0)
                {
                    _odl = 0;
                }
                else
                {
                    _odl = value;
                }
            }
        }
        /// <summary>
        /// 虎赔率
        /// </summary>
        public decimal? C_ODF
        {
            get { return _odf; }
            set
            {
                if (value != null && value < 0)
                {
                    _odf = 0;
                }
                else
                {
                    _odf = value;
                }
            }
        }
        /// <summary>
        /// 龙虎和赔率
        /// </summary>
        public decimal? C_ODHe
        {
            get { return _odhe; }
            set
            {
                if(value != null && value<0)
                {
                    _odhe = 0;
                }
                else
                {
                    _odhe = value;
                }
            }
        }
        /// <summary>
        /// 会员类型
        /// C_Common = 常规会员  C_H5=H5会员
        /// </summary>
        public string C_Type { get; set; }
        /// <summary>
        /// A=代理直接邀请会员
        /// C=指定代理下会员邀请会员
        /// O=常规会员
        /// AC=除了常规会员
        /// OC= 除了代理直接邀请的会员
        /// AOC=所有会员
        /// </summary>
        public string C_InType { get; set; }
        /// <summary>
        /// 显示隐藏洗码
        /// true=显示
        /// false和其他 = 隐藏
        /// </summary>
        public bool? C_HdShow { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string C_F2 { get; set; }
        /// <summary>
        /// 上下分标记
        /// </summary>
        public bool? C_IsAdd { get; set; }
        /// <summary>
        /// 上下分点数
        /// </summary>
        public int? C_Point { get; set; }
        /// <summary>
        /// 1=上级代理上下分
        /// 其他=登录代理上下分
        /// </summary>
        public string C_LevelPoint { get; set; }
    }
}
