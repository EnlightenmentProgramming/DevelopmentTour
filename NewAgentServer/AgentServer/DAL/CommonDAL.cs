using Common;
using Common.ORMTool;
using Model.LogicModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// DAL中的公共方法
    /// </summary>
    public static class CommonDAL
    {
        /// <summary>
        /// 根据LogName 或者AgentID 获取它在当前登录代理下的层级
        /// </summary>
        /// <param name="loginID">当前登录代理AentID</param>
        /// <param name="sign">id表示通过AgentID查询 name表示通过LogName查询</param>
        /// <param name="value">LogName 获取 AgentID的值</param>
        /// <param name="msg">结果状态描述</param>
        /// <returns></returns>
        public static List<AgentSearchModel> GetAgentTree(string loginID, string sign, string value, out string msg)
        {
            try
            {
                List<AgentSearchModel> agentList = new List<AgentSearchModel>();
                string pSql = "";//,WashRate A_WashR,IntoRate A_IntoR,DrawRate A_DrawR,Max_Z A_Mx_Z,Max_X A_Mx_X
                if (sign == "name")
                {
                    pSql = "select AgentID A_ID,LogName A_UserID,AgentName A_Name,ParentID A_PID,WashRate A_WashR,IntoRate A_IntoR,DrawRate A_DrawR,Max_Z A_Mx_Z,Min_Z A_MN_Z,Principal  A_Prncpl,F_3 A_Perm from T_Agent where LogName = '" + value + "'";
                }
                else if (sign == "id")
                {
                    pSql = "select AgentID A_ID,LogName A_UserID,AgentName A_Name,ParentID A_PID,WashRate A_WashR,IntoRate A_IntoR,DrawRate A_DrawR,Max_Z A_Mx_Z,Min_Z A_MN_Z, Principal  A_Prncpl,F_3 A_Perm from T_Agent where AgentID = '" + value + "'";
                }
                else
                {
                    msg = "请传入正确的查询标记";
                    return null;
                }

                AgentSearchModel agent = Db.Context_SqlServer.FromSql(pSql).ToFirst<AgentSearchModel>();
                if (agent != null)
                {
                    if (agent.A_ID == loginID)
                    {
                        if (!agentList.Contains(agent))
                        {
                            agentList.Insert(0, agent);
                        }
                        msg = "ok";
                        return agentList;
                    }
                    else
                    {
                        if (agent.A_ID == null || agent.A_ID == "root" || agent.A_PID == null || agent.A_PID == "root")
                        {
                            msg = "你无权限查看此账号数据";
                            return null;
                        }
                        if (!agentList.Contains(agent))
                        {
                            agentList.Insert(0, agent);
                        }
                        string pID = agent.A_PID;
                        int i = 0;
                        while (true)
                        {
                            string zSql = "select AgentID A_ID,LogName A_UserID,AgentName A_Name,ParentID A_PID,WashRate A_WashR,IntoRate A_IntoR,DrawRate A_DrawR,Max_Z A_Mx_Z,Max_X A_Mx_X, Principal  A_Prncpl,F_3 A_Perm from T_Agent where AgentID = '" + pID + "'";
                            AgentSearchModel agentSon = Db.Context_SqlServer.FromSql(zSql).ToFirst<AgentSearchModel>();
                            i++;
                            if (i > 20)
                            {
                                //防止造成死循环 超过20层层级的不处理
                                msg = "层级关系超出预期范围";
                                return null;
                            }
                            if (agentSon != null)
                            {
                                if (agentSon.A_ID == loginID)
                                {
                                    if (!agentList.Contains(agentSon))
                                    {
                                        agentList.Insert(0, agentSon);
                                    }
                                    msg = "ok";
                                    return agentList;
                                }
                                if (agentSon.A_ID == null || agentSon.A_ID == "root" || agent.A_PID == null || agent.A_PID == "root")
                                {
                                    msg = "你无权限查看此账号数据";
                                    return null;
                                }
                                else
                                {
                                    if (!agentList.Contains(agentSon))
                                    {
                                        agentList.Insert(0, agentSon);
                                    }
                                    pID = agentSon.A_PID;
                                }
                            }
                            else
                            {
                                msg = "没有找到数据";
                                return null;
                            }
                        }

                    }
                }
                else
                {
                    msg = "没有找到数据";
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(CommonDAL), ex);
                throw;
            }
        }
        /// <summary>
        /// 获取会员邀请关系及会员所属代理层级关系
        /// </summary>
        /// <param name="loginID"></param>
        /// <param name="userID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<AgentSearchModel> GetAgentTree(string loginID, string userID, out string msg)
        {
            try
            {
                List<AgentSearchModel> agentList = new List<AgentSearchModel>();
                int clntInvite = Db.Context_SqlServer.FromSql("select count(*) from T_WeiTou_Detail where logName = '" + userID + "'").ToScalar<int>();
                if (clntInvite <= 0)//如果不是H5会员，则先查出此会员对应的代理ID，然后按照代理方式执行
                {
                    string aId = Db.Context_SqlServer.FromSql("select AgentID from T_Client where LogName ='" + userID + "'").ToScalar<string>();
                    return GetAgentTree(loginID, "id", aId, out msg);
                }
                string pSq = "select ClientID A_ID , logName A_UserID,InviterLogName A_Name,InviterID A_PID,AgentID3Bao A_Sub from T_WeiTou_Detail where logName = '" + userID + "'";
                AgentSearchModel agent = Db.Context_SqlServer.FromSql(pSq).ToFirst<AgentSearchModel>();
                if (agent == null)
                {
                    msg = "没有找到数据";
                    return null;
                }
                if (agent.A_PID == loginID)
                {
                    if (!agentList.Contains(agent))
                    {
                        agentList.Insert(0, agent);
                        AgentSearchModel loginA = Db.Context_SqlServer.FromSql("select AgentID A_ID,LogName A_UserID,AgentName A_Name,ParentID A_PID from T_Agent where AgentID = '" + agent.A_PID + "'").ToFirst<AgentSearchModel>();
                        if (!agentList.Contains(loginA))
                        {
                            agentList.Insert(0, loginA);
                        }
                    }
                    msg = "ok";
                    return agentList;
                }
                else
                {
                    if (agent.A_ID == null || agent.A_PID == null)
                    {
                        msg = "你无权限查看此账号数据";
                        return null;
                    }
                    if (!agentList.Contains(agent))
                    {
                        agentList.Insert(0, agent);
                    }
                    string pID = agent.A_PID;
                    int i = 0;
                    while (true)
                    {
                        string zSql = "select ClientID A_ID , logName A_UserID,InviterLogName A_Name,InviterID A_PID from T_WeiTou_Detail where ClientID = '" + pID + "'";
                        AgentSearchModel agentSon = Db.Context_SqlServer.FromSql(zSql).ToFirst<AgentSearchModel>();
                        i++;
                        AgentSearchModel loginAg = new AgentSearchModel();
                        if (i > 100)
                        {
                            msg = "超出邀请范围，无法查找到指定数据";
                            return null;
                        }
                        if (agentSon == null)
                        {
                            agentSon = loginAg = Db.Context_SqlServer.FromSql("select AgentID A_ID,LogName A_UserID,AgentName A_Name,ParentID A_PID from T_Agent where AgentID = '" + pID + "'").ToFirst<AgentSearchModel>();
                        }
                        if (agentSon == null)
                        {
                            msg = "超出邀请范围，无法查找到指定数据";
                            return null;
                        }

                        if (agentSon.A_PID == loginID)
                        {
                            if (!agentList.Contains(agentSon))
                            {
                                agentList.Insert(0, agentSon);
                                loginAg = Db.Context_SqlServer.FromSql("select AgentID A_ID,LogName A_UserID,AgentName A_Name,ParentID A_PID from T_Agent where AgentID = '" + agentSon.A_PID + "'").ToFirst<AgentSearchModel>();
                                if (!agentList.Contains(loginAg))
                                {
                                    agentList.Insert(0, loginAg);
                                }
                            }
                            msg = "ok";
                            return agentList;
                        }
                        if (!agentList.Contains(agentSon))
                        {
                            agentList.Insert(0, agentSon);

                        }
                        pID = agentSon.A_PID;
                    }//while循环结束
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(CommonDAL), ex);
                throw;
            }


        }
        /// <summary>
        /// 判断指定代理是否是子账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsSubAgent(string id,out string pId)
        {
            try
            {
                pId = Db.Context_SqlServer.FromSql("select AgentID from T_AgentSub where AgentSubID ='" + id + "'").ToScalar<string>();
                return !string.IsNullOrEmpty(pId);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(CommonDAL), ex);
                throw;
            }
        }
        /// <summary>
        /// 判断指定代理是否启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsYes(string id)
        {
            try
            {
                return Db.Context_SqlServer.FromSql("select state from T_Agent where AgentID ='" + id + "'").ToScalar<string>() == "YES";
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(CommonDAL), ex);
                throw;
            }
        }
        /// <summary>
        /// 判断子账号是否启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsYesSub(string id)
        {
            try
            {
                return Db.Context_SqlServer.FromSql("select State from T_AgentSub where AgentSubID ='" + id + "'").ToScalar<string>() == "YES";
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(CommonDAL), ex);
                throw;
            }
        }
    }
}
