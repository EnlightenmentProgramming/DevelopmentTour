﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//     Website: http://ITdos.com/Dos/ORM/Index.html
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Dos.ORM;

namespace Model.DBModel
{
    /// <summary>
    /// 实体类T_Agent。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("T_Agent")]
    [Serializable]
    public partial class T_Agent : Entity
    {
        #region Model
		private string _AgentID;
		private string _LogName;
		private string _AgentName;
		private string _Pwd;
		private string _ParentID;
		private string _State;
		private int? _Max_Z;
		private int? _Min_Z;
		private int? _Max_X;
		private int? _Min_X;
		private int? _Max_H;
		private int? _Min_H;
		private int? _Max_ZD;
		private int? _Min_ZD;
		private int? _Max_XD;
		private int? _Min_XD;
		private decimal? _Principal;
		private string _WashType;
		private decimal? _WashRate;
		private string _IsWeb;
		private decimal? _IntoRate;
		private string _IsHide;
		private string _tmpState;
		private string _F_1;
		private string _F_2;
		private string _F_3;
		private decimal? _SourcePrincipal;
		private string _CreateID;
		private string _CreateTime;
		private decimal? _DrawRate;
		private decimal? _RebateRate;
		private string _Phone;
		private string _Memo;

		/// <summary>
		/// 代理编号
		/// </summary>
		[Field("AgentID")]
		public string AgentID
		{
			get{ return _AgentID; }
			set
			{
				this.OnPropertyValueChange("AgentID");
				this._AgentID = value;
			}
		}
		/// <summary>
		/// 登录名称
		/// </summary>
		[Field("LogName")]
		public string LogName
		{
			get{ return _LogName; }
			set
			{
				this.OnPropertyValueChange("LogName");
				this._LogName = value;
			}
		}
		/// <summary>
		/// 代理名称
		/// </summary>
		[Field("AgentName")]
		public string AgentName
		{
			get{ return _AgentName; }
			set
			{
				this.OnPropertyValueChange("AgentName");
				this._AgentName = value;
			}
		}
		/// <summary>
		/// 密码
		/// </summary>
		[Field("Pwd")]
		public string Pwd
		{
			get{ return _Pwd; }
			set
			{
				this.OnPropertyValueChange("Pwd");
				this._Pwd = value;
			}
		}
		/// <summary>
		/// 上级代理编号        根节点，编号为0
		/// </summary>
		[Field("ParentID")]
		public string ParentID
		{
			get{ return _ParentID; }
			set
			{
				this.OnPropertyValueChange("ParentID");
				this._ParentID = value;
			}
		}
		/// <summary>
		/// 代理状态          启用=YES         停用=NO         暂停=PAUSE
		/// </summary>
		[Field("State")]
		public string State
		{
			get{ return _State; }
			set
			{
				this.OnPropertyValueChange("State");
				this._State = value;
			}
		}
		/// <summary>
		/// 庄最大限红
		/// </summary>
		[Field("Max_Z")]
		public int? Max_Z
		{
			get{ return _Max_Z; }
			set
			{
				this.OnPropertyValueChange("Max_Z");
				this._Max_Z = value;
			}
		}
		/// <summary>
		/// 庄最小限红
		/// </summary>
		[Field("Min_Z")]
		public int? Min_Z
		{
			get{ return _Min_Z; }
			set
			{
				this.OnPropertyValueChange("Min_Z");
				this._Min_Z = value;
			}
		}
		/// <summary>
		/// 闲最大限红
		/// </summary>
		[Field("Max_X")]
		public int? Max_X
		{
			get{ return _Max_X; }
			set
			{
				this.OnPropertyValueChange("Max_X");
				this._Max_X = value;
			}
		}
		/// <summary>
		/// 闲最小限红
		/// </summary>
		[Field("Min_X")]
		public int? Min_X
		{
			get{ return _Min_X; }
			set
			{
				this.OnPropertyValueChange("Min_X");
				this._Min_X = value;
			}
		}
		/// <summary>
		/// 和最大限红
		/// </summary>
		[Field("Max_H")]
		public int? Max_H
		{
			get{ return _Max_H; }
			set
			{
				this.OnPropertyValueChange("Max_H");
				this._Max_H = value;
			}
		}
		/// <summary>
		/// 和最小限红
		/// </summary>
		[Field("Min_H")]
		public int? Min_H
		{
			get{ return _Min_H; }
			set
			{
				this.OnPropertyValueChange("Min_H");
				this._Min_H = value;
			}
		}
		/// <summary>
		/// 庄对最大限红
		/// </summary>
		[Field("Max_ZD")]
		public int? Max_ZD
		{
			get{ return _Max_ZD; }
			set
			{
				this.OnPropertyValueChange("Max_ZD");
				this._Max_ZD = value;
			}
		}
		/// <summary>
		/// 庄对最小限红
		/// </summary>
		[Field("Min_ZD")]
		public int? Min_ZD
		{
			get{ return _Min_ZD; }
			set
			{
				this.OnPropertyValueChange("Min_ZD");
				this._Min_ZD = value;
			}
		}
		/// <summary>
		/// 闲对最大限红
		/// </summary>
		[Field("Max_XD")]
		public int? Max_XD
		{
			get{ return _Max_XD; }
			set
			{
				this.OnPropertyValueChange("Max_XD");
				this._Max_XD = value;
			}
		}
		/// <summary>
		/// 闲对最小限红
		/// </summary>
		[Field("Min_XD")]
		public int? Min_XD
		{
			get{ return _Min_XD; }
			set
			{
				this.OnPropertyValueChange("Min_XD");
				this._Min_XD = value;
			}
		}
		/// <summary>
		/// 信用点数
		/// </summary>
		[Field("Principal")]
		public decimal? Principal
		{
			get{ return _Principal; }
			set
			{
				this.OnPropertyValueChange("Principal");
				this._Principal = value;
			}
		}
		/// <summary>
		/// 洗码类型        双边洗码=S        单边洗码=D
		/// </summary>
		[Field("WashType")]
		public string WashType
		{
			get{ return _WashType; }
			set
			{
				this.OnPropertyValueChange("WashType");
				this._WashType = value;
			}
		}
		/// <summary>
		/// 洗码率
		/// </summary>
		[Field("WashRate")]
		public decimal? WashRate
		{
			get{ return _WashRate; }
			set
			{
				this.OnPropertyValueChange("WashRate");
				this._WashRate = value;
			}
		}
		/// <summary>
		/// 是否允许Web登录        是=YES        否=NO
		/// </summary>
		[Field("IsWeb")]
		public string IsWeb
		{
			get{ return _IsWeb; }
			set
			{
				this.OnPropertyValueChange("IsWeb");
				this._IsWeb = value;
			}
		}
		/// <summary>
		/// 百家乐占成比率
		/// </summary>
		[Field("IntoRate")]
		public decimal? IntoRate
		{
			get{ return _IntoRate; }
			set
			{
				this.OnPropertyValueChange("IntoRate");
				this._IntoRate = value;
			}
		}
		/// <summary>
		/// 是否隐藏        是=YES        否=NO
		/// </summary>
		[Field("IsHide")]
		public string IsHide
		{
			get{ return _IsHide; }
			set
			{
				this.OnPropertyValueChange("IsHide");
				this._IsHide = value;
			}
		}
		/// <summary>
		/// 临时保存代理状态     批量设置代理状态时,先保存设置前的状态
		/// </summary>
		[Field("tmpState")]
		public string tmpState
		{
			get{ return _tmpState; }
			set
			{
				this.OnPropertyValueChange("tmpState");
				this._tmpState = value;
			}
		}
		/// <summary>
		/// 备用字段
		/// </summary>
		[Field("F_1")]
		public string F_1
		{
			get{ return _F_1; }
			set
			{
				this.OnPropertyValueChange("F_1");
				this._F_1 = value;
			}
		}
		/// <summary>
		/// 备用字段
		/// </summary>
		[Field("F_2")]
		public string F_2
		{
			get{ return _F_2; }
			set
			{
				this.OnPropertyValueChange("F_2");
				this._F_2 = value;
			}
		}
		/// <summary>
		/// 备用字段
		/// </summary>
		[Field("F_3")]
		public string F_3
		{
			get{ return _F_3; }
			set
			{
				this.OnPropertyValueChange("F_3");
				this._F_3 = value;
			}
		}
		/// <summary>
		/// 原始信用额度
		/// </summary>
		[Field("SourcePrincipal")]
		public decimal? SourcePrincipal
		{
			get{ return _SourcePrincipal; }
			set
			{
				this.OnPropertyValueChange("SourcePrincipal");
				this._SourcePrincipal = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("CreateID")]
		public string CreateID
		{
			get{ return _CreateID; }
			set
			{
				this.OnPropertyValueChange("CreateID");
				this._CreateID = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("CreateTime")]
		public string CreateTime
		{
			get{ return _CreateTime; }
			set
			{
				this.OnPropertyValueChange("CreateTime");
				this._CreateTime = value;
			}
		}
		/// <summary>
		/// 龙虎 和局比例
		/// </summary>
		[Field("DrawRate")]
		public decimal? DrawRate
		{
			get{ return _DrawRate; }
			set
			{
				this.OnPropertyValueChange("DrawRate");
				this._DrawRate = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("RebateRate")]
		public decimal? RebateRate
		{
			get{ return _RebateRate; }
			set
			{
				this.OnPropertyValueChange("RebateRate");
				this._RebateRate = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Phone")]
		public string Phone
		{
			get{ return _Phone; }
			set
			{
				this.OnPropertyValueChange("Phone");
				this._Phone = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Memo")]
		public string Memo
		{
			get{ return _Memo; }
			set
			{
				this.OnPropertyValueChange("Memo");
				this._Memo = value;
			}
		}
		#endregion

		#region Method
        /// <summary>
        /// 获取实体中的主键列
        /// </summary>
        public override Field[] GetPrimaryKeyFields()
        {
            return new Field[] {
				_.AgentID,
			};
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
				_.AgentID,
				_.LogName,
				_.AgentName,
				_.Pwd,
				_.ParentID,
				_.State,
				_.Max_Z,
				_.Min_Z,
				_.Max_X,
				_.Min_X,
				_.Max_H,
				_.Min_H,
				_.Max_ZD,
				_.Min_ZD,
				_.Max_XD,
				_.Min_XD,
				_.Principal,
				_.WashType,
				_.WashRate,
				_.IsWeb,
				_.IntoRate,
				_.IsHide,
				_.tmpState,
				_.F_1,
				_.F_2,
				_.F_3,
				_.SourcePrincipal,
				_.CreateID,
				_.CreateTime,
				_.DrawRate,
				_.RebateRate,
				_.Phone,
				_.Memo,
			};
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
				this._AgentID,
				this._LogName,
				this._AgentName,
				this._Pwd,
				this._ParentID,
				this._State,
				this._Max_Z,
				this._Min_Z,
				this._Max_X,
				this._Min_X,
				this._Max_H,
				this._Min_H,
				this._Max_ZD,
				this._Min_ZD,
				this._Max_XD,
				this._Min_XD,
				this._Principal,
				this._WashType,
				this._WashRate,
				this._IsWeb,
				this._IntoRate,
				this._IsHide,
				this._tmpState,
				this._F_1,
				this._F_2,
				this._F_3,
				this._SourcePrincipal,
				this._CreateID,
				this._CreateTime,
				this._DrawRate,
				this._RebateRate,
				this._Phone,
				this._Memo,
			};
        }
        /// <summary>
        /// 是否是v1.10.5.6及以上版本实体。
        /// </summary>
        /// <returns></returns>
        public override bool V1_10_5_6_Plus()
        {
            return true;
        }
        #endregion

		#region _Field
        /// <summary>
        /// 字段信息
        /// </summary>
        public class _
        {
			/// <summary>
			/// * 
			/// </summary>
			public readonly static Field All = new Field("*", "T_Agent");
            /// <summary>
			/// 代理编号
			/// </summary>
			public readonly static Field AgentID = new Field("AgentID", "T_Agent", "代理编号");
            /// <summary>
			/// 登录名称
			/// </summary>
			public readonly static Field LogName = new Field("LogName", "T_Agent", "登录名称");
            /// <summary>
			/// 代理名称
			/// </summary>
			public readonly static Field AgentName = new Field("AgentName", "T_Agent", "代理名称");
            /// <summary>
			/// 密码
			/// </summary>
			public readonly static Field Pwd = new Field("Pwd", "T_Agent", "密码");
            /// <summary>
			/// 上级代理编号        根节点，编号为0
			/// </summary>
			public readonly static Field ParentID = new Field("ParentID", "T_Agent", "上级代理编号        根节点，编号为0");
            /// <summary>
			/// 代理状态          启用=YES         停用=NO         暂停=PAUSE
			/// </summary>
			public readonly static Field State = new Field("State", "T_Agent", "代理状态          启用=YES         停用=NO         暂停=PAUSE");
            /// <summary>
			/// 庄最大限红
			/// </summary>
			public readonly static Field Max_Z = new Field("Max_Z", "T_Agent", "庄最大限红");
            /// <summary>
			/// 庄最小限红
			/// </summary>
			public readonly static Field Min_Z = new Field("Min_Z", "T_Agent", "庄最小限红");
            /// <summary>
			/// 闲最大限红
			/// </summary>
			public readonly static Field Max_X = new Field("Max_X", "T_Agent", "闲最大限红");
            /// <summary>
			/// 闲最小限红
			/// </summary>
			public readonly static Field Min_X = new Field("Min_X", "T_Agent", "闲最小限红");
            /// <summary>
			/// 和最大限红
			/// </summary>
			public readonly static Field Max_H = new Field("Max_H", "T_Agent", "和最大限红");
            /// <summary>
			/// 和最小限红
			/// </summary>
			public readonly static Field Min_H = new Field("Min_H", "T_Agent", "和最小限红");
            /// <summary>
			/// 庄对最大限红
			/// </summary>
			public readonly static Field Max_ZD = new Field("Max_ZD", "T_Agent", "庄对最大限红");
            /// <summary>
			/// 庄对最小限红
			/// </summary>
			public readonly static Field Min_ZD = new Field("Min_ZD", "T_Agent", "庄对最小限红");
            /// <summary>
			/// 闲对最大限红
			/// </summary>
			public readonly static Field Max_XD = new Field("Max_XD", "T_Agent", "闲对最大限红");
            /// <summary>
			/// 闲对最小限红
			/// </summary>
			public readonly static Field Min_XD = new Field("Min_XD", "T_Agent", "闲对最小限红");
            /// <summary>
			/// 信用点数
			/// </summary>
			public readonly static Field Principal = new Field("Principal", "T_Agent", "信用点数");
            /// <summary>
			/// 洗码类型        双边洗码=S        单边洗码=D
			/// </summary>
			public readonly static Field WashType = new Field("WashType", "T_Agent", "洗码类型        双边洗码=S        单边洗码=D");
            /// <summary>
			/// 洗码率
			/// </summary>
			public readonly static Field WashRate = new Field("WashRate", "T_Agent", "洗码率");
            /// <summary>
			/// 是否允许Web登录        是=YES        否=NO
			/// </summary>
			public readonly static Field IsWeb = new Field("IsWeb", "T_Agent", "是否允许Web登录        是=YES        否=NO");
            /// <summary>
			/// 百家乐占成比率
			/// </summary>
			public readonly static Field IntoRate = new Field("IntoRate", "T_Agent", "百家乐占成比率");
            /// <summary>
			/// 是否隐藏        是=YES        否=NO
			/// </summary>
			public readonly static Field IsHide = new Field("IsHide", "T_Agent", "是否隐藏        是=YES        否=NO");
            /// <summary>
			/// 临时保存代理状态     批量设置代理状态时,先保存设置前的状态
			/// </summary>
			public readonly static Field tmpState = new Field("tmpState", "T_Agent", "临时保存代理状态     批量设置代理状态时,先保存设置前的状态");
            /// <summary>
			/// 备用字段
			/// </summary>
			public readonly static Field F_1 = new Field("F_1", "T_Agent", "备用字段");
            /// <summary>
			/// 备用字段
			/// </summary>
			public readonly static Field F_2 = new Field("F_2", "T_Agent", "备用字段");
            /// <summary>
			/// 备用字段
			/// </summary>
			public readonly static Field F_3 = new Field("F_3", "T_Agent", "备用字段");
            /// <summary>
			/// 原始信用额度
			/// </summary>
			public readonly static Field SourcePrincipal = new Field("SourcePrincipal", "T_Agent", "原始信用额度");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field CreateID = new Field("CreateID", "T_Agent", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field CreateTime = new Field("CreateTime", "T_Agent", "");
            /// <summary>
			/// 龙虎 和局比例
			/// </summary>
			public readonly static Field DrawRate = new Field("DrawRate", "T_Agent", "龙虎 和局比例");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field RebateRate = new Field("RebateRate", "T_Agent", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Phone = new Field("Phone", "T_Agent", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Memo = new Field("Memo", "T_Agent", "");
        }
        #endregion
	}
}