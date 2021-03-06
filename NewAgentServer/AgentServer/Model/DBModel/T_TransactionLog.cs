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
    /// 实体类T_TransactionLog。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("T_TransactionLog")]
    [Serializable]
    public partial class T_TransactionLog : Entity
    {
        #region Model
		private string _LogID;
		private string _OpType;
		private string _OpReason;
		private string _UserID;
		private string _UserName;
		private decimal? _TransactionPoint;
		private string _SourceUser;
		private decimal? _SoureInto;
		private decimal? _RealInto;
		private decimal? _RealPoint;
		private decimal? _TransactionPoint_B;
		private decimal? _TransactionPoint_E;
		private string _OpTime;
		private string _Operator;
		private string _OpInfo;
		private string _IP;
		private string _Address;
		private string _Remark;
		private string _F_1;
		private string _F_2;
		private string _OperatorID;

		/// <summary>
		/// 
		/// </summary>
		[Field("LogID")]
		public string LogID
		{
			get{ return _LogID; }
			set
			{
				this.OnPropertyValueChange("LogID");
				this._LogID = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("OpType")]
		public string OpType
		{
			get{ return _OpType; }
			set
			{
				this.OnPropertyValueChange("OpType");
				this._OpType = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("OpReason")]
		public string OpReason
		{
			get{ return _OpReason; }
			set
			{
				this.OnPropertyValueChange("OpReason");
				this._OpReason = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("UserID")]
		public string UserID
		{
			get{ return _UserID; }
			set
			{
				this.OnPropertyValueChange("UserID");
				this._UserID = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("UserName")]
		public string UserName
		{
			get{ return _UserName; }
			set
			{
				this.OnPropertyValueChange("UserName");
				this._UserName = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("TransactionPoint")]
		public decimal? TransactionPoint
		{
			get{ return _TransactionPoint; }
			set
			{
				this.OnPropertyValueChange("TransactionPoint");
				this._TransactionPoint = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("SourceUser")]
		public string SourceUser
		{
			get{ return _SourceUser; }
			set
			{
				this.OnPropertyValueChange("SourceUser");
				this._SourceUser = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("SoureInto")]
		public decimal? SoureInto
		{
			get{ return _SoureInto; }
			set
			{
				this.OnPropertyValueChange("SoureInto");
				this._SoureInto = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("RealInto")]
		public decimal? RealInto
		{
			get{ return _RealInto; }
			set
			{
				this.OnPropertyValueChange("RealInto");
				this._RealInto = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("RealPoint")]
		public decimal? RealPoint
		{
			get{ return _RealPoint; }
			set
			{
				this.OnPropertyValueChange("RealPoint");
				this._RealPoint = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("TransactionPoint_B")]
		public decimal? TransactionPoint_B
		{
			get{ return _TransactionPoint_B; }
			set
			{
				this.OnPropertyValueChange("TransactionPoint_B");
				this._TransactionPoint_B = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("TransactionPoint_E")]
		public decimal? TransactionPoint_E
		{
			get{ return _TransactionPoint_E; }
			set
			{
				this.OnPropertyValueChange("TransactionPoint_E");
				this._TransactionPoint_E = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("OpTime")]
		public string OpTime
		{
			get{ return _OpTime; }
			set
			{
				this.OnPropertyValueChange("OpTime");
				this._OpTime = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Operator")]
		public string Operator
		{
			get{ return _Operator; }
			set
			{
				this.OnPropertyValueChange("Operator");
				this._Operator = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("OpInfo")]
		public string OpInfo
		{
			get{ return _OpInfo; }
			set
			{
				this.OnPropertyValueChange("OpInfo");
				this._OpInfo = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("IP")]
		public string IP
		{
			get{ return _IP; }
			set
			{
				this.OnPropertyValueChange("IP");
				this._IP = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Address")]
		public string Address
		{
			get{ return _Address; }
			set
			{
				this.OnPropertyValueChange("Address");
				this._Address = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Field("Remark")]
		public string Remark
		{
			get{ return _Remark; }
			set
			{
				this.OnPropertyValueChange("Remark");
				this._Remark = value;
			}
		}
		/// <summary>
		/// 
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
		/// 
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
		/// 
		/// </summary>
		[Field("OperatorID")]
		public string OperatorID
		{
			get{ return _OperatorID; }
			set
			{
				this.OnPropertyValueChange("OperatorID");
				this._OperatorID = value;
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
				_.LogID,
			};
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
				_.LogID,
				_.OpType,
				_.OpReason,
				_.UserID,
				_.UserName,
				_.TransactionPoint,
				_.SourceUser,
				_.SoureInto,
				_.RealInto,
				_.RealPoint,
				_.TransactionPoint_B,
				_.TransactionPoint_E,
				_.OpTime,
				_.Operator,
				_.OpInfo,
				_.IP,
				_.Address,
				_.Remark,
				_.F_1,
				_.F_2,
				_.OperatorID,
			};
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
				this._LogID,
				this._OpType,
				this._OpReason,
				this._UserID,
				this._UserName,
				this._TransactionPoint,
				this._SourceUser,
				this._SoureInto,
				this._RealInto,
				this._RealPoint,
				this._TransactionPoint_B,
				this._TransactionPoint_E,
				this._OpTime,
				this._Operator,
				this._OpInfo,
				this._IP,
				this._Address,
				this._Remark,
				this._F_1,
				this._F_2,
				this._OperatorID,
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
			public readonly static Field All = new Field("*", "T_TransactionLog");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field LogID = new Field("LogID", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field OpType = new Field("OpType", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field OpReason = new Field("OpReason", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field UserID = new Field("UserID", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field UserName = new Field("UserName", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field TransactionPoint = new Field("TransactionPoint", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field SourceUser = new Field("SourceUser", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field SoureInto = new Field("SoureInto", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field RealInto = new Field("RealInto", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field RealPoint = new Field("RealPoint", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field TransactionPoint_B = new Field("TransactionPoint_B", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field TransactionPoint_E = new Field("TransactionPoint_E", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field OpTime = new Field("OpTime", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Operator = new Field("Operator", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field OpInfo = new Field("OpInfo", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field IP = new Field("IP", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Address = new Field("Address", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Remark = new Field("Remark", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field F_1 = new Field("F_1", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field F_2 = new Field("F_2", "T_TransactionLog", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field OperatorID = new Field("OperatorID", "T_TransactionLog", "");
        }
        #endregion
	}
}