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
    /// 实体类T_PubInfo。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("T_PubInfo")]
    [Serializable]
    public partial class T_PubInfo : Entity
    {
        #region Model
		private string _InfoID;
		private string _ClientID;
		private string _CreateID;
		private string _CreateName;
		private string _CreateTime;
		private string _PubInfo;
		private string _InfoType;
		private string _IsPublish;

		/// <summary>
		/// 公告信息ID
		/// </summary>
		[Field("InfoID")]
		public string InfoID
		{
			get{ return _InfoID; }
			set
			{
				this.OnPropertyValueChange("InfoID");
				this._InfoID = value;
			}
		}
		/// <summary>
		/// 公告对象ID        为0 表示对所有人进行公告
		/// </summary>
		[Field("ClientID")]
		public string ClientID
		{
			get{ return _ClientID; }
			set
			{
				this.OnPropertyValueChange("ClientID");
				this._ClientID = value;
			}
		}
		/// <summary>
		/// 创建日期
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
		[Field("CreateName")]
		public string CreateName
		{
			get{ return _CreateName; }
			set
			{
				this.OnPropertyValueChange("CreateName");
				this._CreateName = value;
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
		/// 公告内容
		/// </summary>
		[Field("PubInfo")]
		public string PubInfo
		{
			get{ return _PubInfo; }
			set
			{
				this.OnPropertyValueChange("PubInfo");
				this._PubInfo = value;
			}
		}
		/// <summary>
		/// TABLE     桌台公告  CLIENTID 为0 全场桌台公告  CLIENT    会员公告  CLIENTID 为0 全场会员公告  AGENT     代理公告  CLIENTID 为0 全场代理公告   删除  被删除公告
		/// </summary>
		[Field("InfoType")]
		public string InfoType
		{
			get{ return _InfoType; }
			set
			{
				this.OnPropertyValueChange("InfoType");
				this._InfoType = value;
			}
		}
		/// <summary>
		/// 是否发布公告信息 YES-发布 NO-未发布
		/// </summary>
		[Field("IsPublish")]
		public string IsPublish
		{
			get{ return _IsPublish; }
			set
			{
				this.OnPropertyValueChange("IsPublish");
				this._IsPublish = value;
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
				_.InfoID,
			};
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
				_.InfoID,
				_.ClientID,
				_.CreateID,
				_.CreateName,
				_.CreateTime,
				_.PubInfo,
				_.InfoType,
				_.IsPublish,
			};
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
				this._InfoID,
				this._ClientID,
				this._CreateID,
				this._CreateName,
				this._CreateTime,
				this._PubInfo,
				this._InfoType,
				this._IsPublish,
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
			public readonly static Field All = new Field("*", "T_PubInfo");
            /// <summary>
			/// 公告信息ID
			/// </summary>
			public readonly static Field InfoID = new Field("InfoID", "T_PubInfo", "公告信息ID");
            /// <summary>
			/// 公告对象ID        为0 表示对所有人进行公告
			/// </summary>
			public readonly static Field ClientID = new Field("ClientID", "T_PubInfo", "公告对象ID        为0 表示对所有人进行公告");
            /// <summary>
			/// 创建日期
			/// </summary>
			public readonly static Field CreateID = new Field("CreateID", "T_PubInfo", "创建日期");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field CreateName = new Field("CreateName", "T_PubInfo", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field CreateTime = new Field("CreateTime", "T_PubInfo", "");
            /// <summary>
			/// 公告内容
			/// </summary>
			public readonly static Field PubInfo = new Field("PubInfo", "T_PubInfo", "公告内容");
            /// <summary>
			/// TABLE     桌台公告  CLIENTID 为0 全场桌台公告  CLIENT    会员公告  CLIENTID 为0 全场会员公告  AGENT     代理公告  CLIENTID 为0 全场代理公告   删除  被删除公告
			/// </summary>
			public readonly static Field InfoType = new Field("InfoType", "T_PubInfo", "TABLE     桌台公告  CLIENTID 为0 全场桌台公告  CLIENT    会员公告  CLIENTID 为0 全场会员公告  AGENT     代理公告  CLIENTID 为0 全场代理公告   删除  被删除公告");
            /// <summary>
			/// 是否发布公告信息 YES-发布 NO-未发布
			/// </summary>
			public readonly static Field IsPublish = new Field("IsPublish", "T_PubInfo", "是否发布公告信息 YES-发布 NO-未发布");
        }
        #endregion
	}
}