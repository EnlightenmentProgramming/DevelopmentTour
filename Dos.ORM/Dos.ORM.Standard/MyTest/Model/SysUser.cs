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

namespace ITdos.Project.DataModel
{
    /// <summary>
    /// 实体类SysUser。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("Sys_User")]
    [Serializable]
    public partial class SysUser : Entity
    {
        #region Model
        private Guid _Id;
        private string _Account;
        private string _Pwd;
        private string _RealName;
        private string _Phone;
        private DateTime _CreateTime;
        private int _State;
        private string _Remark;
        private string _Avatar;
        private byte[] _Test;

        /// <summary>
        /// 
        /// </summary>
        [Field("Id")]
        public Guid Id
        {
            get { return _Id; }
            set
            {
                this.OnPropertyValueChange("Id");
                this._Id = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Account")]
        public string Account
        {
            get { return _Account; }
            set
            {
                this.OnPropertyValueChange("Account");
                this._Account = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Pwd")]
        public string Pwd
        {
            get { return _Pwd; }
            set
            {
                this.OnPropertyValueChange("Pwd");
                this._Pwd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("RealName")]
        public string RealName
        {
            get { return _RealName; }
            set
            {
                this.OnPropertyValueChange("RealName");
                this._RealName = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Phone")]
        public string Phone
        {
            get { return _Phone; }
            set
            {
                this.OnPropertyValueChange("Phone");
                this._Phone = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("CreateTime")]
        public DateTime CreateTime
        {
            get { return _CreateTime; }
            set
            {
                this.OnPropertyValueChange("CreateTime");
                this._CreateTime = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("State")]
        public int State
        {
            get { return _State; }
            set
            {
                this.OnPropertyValueChange("State");
                this._State = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Remark")]
        public string Remark
        {
            get { return _Remark; }
            set
            {
                this.OnPropertyValueChange("Remark");
                this._Remark = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Avatar")]
        public string Avatar
        {
            get { return _Avatar; }
            set
            {
                this.OnPropertyValueChange("Avatar");
                this._Avatar = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("Test")]
        public byte[] Test
        {
            get { return _Test; }
            set
            {
                this.OnPropertyValueChange("Test");
                this._Test = value;
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
                _.Id,
            };
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
                _.Id,
                _.Account,
                _.Pwd,
                _.RealName,
                _.Phone,
                _.CreateTime,
                _.State,
                _.Remark,
                _.Avatar,
                _.Test,
            };
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
                this._Id,
                this._Account,
                this._Pwd,
                this._RealName,
                this._Phone,
                this._CreateTime,
                this._State,
                this._Remark,
                this._Avatar,
                this._Test,
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
            public readonly static Field All = new Field("*", "Sys_User");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Id = new Field("Id", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Account = new Field("Account", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Pwd = new Field("Pwd", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field RealName = new Field("RealName", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Phone = new Field("Phone", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field CreateTime = new Field("CreateTime", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field State = new Field("State", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Remark = new Field("Remark", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Avatar = new Field("Avatar", "Sys_User", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field Test = new Field("Test", "Sys_User", "");
        }
        #endregion
    }
}