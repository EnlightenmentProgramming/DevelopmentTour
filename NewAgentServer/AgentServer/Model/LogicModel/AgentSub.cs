using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    public class AgentSub
    {
        /// <summary>
        /// 子账号编号
        /// </summary>
        public string AS_ID { get; set; }
        /// <summary>
        /// 代理编号
        /// </summary>
        public string AS_AID { get; set; }
        /// <summary>
        /// 登录名称
        /// </summary>
        public string AS_UserID { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string AS_Pwd { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string AS_State { get; set; }
        /// <summary>
        /// 子账号名称
        /// </summary>
        public string AS_Name { get; set; }
    }
}
