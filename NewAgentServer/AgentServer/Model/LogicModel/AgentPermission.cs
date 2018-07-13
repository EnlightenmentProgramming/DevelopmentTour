using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LogicModel
{
    /// <summary>
    /// 代理抽水及配分权限
    /// </summary>
    public class AgentPermission
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        public string A_ID { get; set; }
        public string A_Name { get; set; }
        public string A_UserID { get; set; }
        public string A_PID { get; set; }
        /// <summary>
        /// 代理配分及抽水权限的Base64编码字符串
        /// </summary>
        public string A_Perm { get; set; }
        /// <summary>
        /// 是否抽水
        /// </summary>
        public bool? A_SetPv { get; set; }
        /// <summary>
        /// 是否配分
        /// </summary>
        public bool? A_MatchP { get; set; }
    }
}
