using Common;
using Common.ORMTool;
using Model.Comunication;
using Model.DBModel;
using Model.LogicModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LogicDAL
{
    public class LoginDAL
    {        
        /// <summary>
        /// 代理登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string Login(AgentSearchModel model, HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null)
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到登录账号或密码";
                    return null;
                }
                string loginSql = SqlTemplateCommon.GetSql("A_AgentLogin");
                if (string.IsNullOrEmpty(loginSql))
                {
                    error.ErrMsg = "服务端没有读取到A_AgentLogin数据模板，请联系管理员";
                    return null;
                }
                loginSql = loginSql.Replace("${LogName}", model.A_UserID);
                loginSql = loginSql.Replace("${LogPwd}", model.A_Pwd);
                DataTable agentTable = Db.Context_SqlServer.FromSql(loginSql).ToDataTable();
                #region 写登录日志
                int userLength = model.A_UserID == null ? 0 : model.A_UserID.Length;
                T_LoginLog loginLog = new T_LoginLog();
                loginLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                loginLog.UserLevel = "代理";
                loginLog.LoginIP = head.Ip;
                loginLog.LoginAddre = CommonHelper.ipToAddr(head.Ip);
                loginLog.LoginTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                loginLog.LoginUser = model.A_UserID;
                if (agentTable != null && agentTable.Rows.Count > 0)
                {
                    error.ErrMsg = loginLog.ReMark = (userLength < 6 ? "子账号" : "代理") + model.A_UserID + "登录成功";
                    //error.ErrMsg = (userLength < 6 ? "子账号" : "代理") + model.A_UserID + "登录成功";
                    error.ErrNo = "0000";
                }
                else
                {
                    error.ErrMsg = loginLog.ReMark = "用户名或密码错误" + (userLength < 6 ? "子账号" : "代理") + model.A_UserID + "登录失败";
                    //error.ErrMsg = (userLength < 6 ? "子账号" : "代理") + model.A_UserID + "登录成功";
                    error.ErrNo = "0004";
                }
                if (Db.Context_SqlServer.Insert<T_LoginLog>(loginLog) <= 0)
                {
                    LogHelper.WriteErrLog(typeof(LoginDAL), "插入登录日志失败");
                } 
                #endregion
                return CommonHelper.DataTableToJson(agentTable);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg ="代理登录异常：" + ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理今日下单统计
        /// </summary>
        /// <param name="model"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetATodayBillCount(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到参数";
                    return null;
                }
                string strSql = SqlTemplateCommon.GetSql("A_GetAgentBetData_Current_New");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAgentBetData_Current_New数据模板，请联系管理员";
                    return null;
                }
                //判断当前获取的数据是否在当前登录代理分支中
                string _msg;
                List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out _msg);
                if(aList == null || aList.Count <=0)
                {
                    error.ErrMsg = _msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg ="获取代理今日下单统计异常："+ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理会员在线列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetOnlineClntList(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到参数";
                    return null;
                }
                string strSql = SqlTemplateCommon.GetSql("A_GetClientUseInfo");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetClientUseInfo数据模板，请联系管理员";
                    return null;
                }
                string _msg;
                List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out _msg);
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = _msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg = "获取代理今日下单统计异常：" + ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定时段内指定代理统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetACountByDate(AgentSearchModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到参数";
                    return null;
                }
                string strSql = SqlTemplateCommon.GetSql("LoginACount");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到LoginACount数据模板，请联系管理员";
                    return null;
                }
                string _msg;
                List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out _msg);
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = _msg;
                    return null;
                }
                if(string.IsNullOrEmpty(model.StartDate))
                {
                    model.StartDate = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                }
                if(string.IsNullOrEmpty(model.EndDate))
                {
                    model.EndDate = model.StartDate;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                strSql = strSql.Replace("${StartDate}", model.StartDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate);
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定时段内指定代理下会员统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetAClntCount(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到参数";
                    return null;
                }
                string strSql = SqlTemplateCommon.GetSql("A_AgentClientCount");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_AgentClientCount数据模板，请联系管理员";
                    return null;
                }
                string _msg;
                List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out _msg);
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = _msg;
                    return null;
                }
                if (string.IsNullOrEmpty(model.StartDate))
                {
                    model.StartDate = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                }
                if (string.IsNullOrEmpty(model.EndDate))
                {
                    model.EndDate = model.StartDate;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                strSql = strSql.Replace("${StartDate}", model.StartDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate);
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理或指定会员所属代理统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetAorCAgentData(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到参数";
                    return null;
                }
                string strSql = SqlTemplateCommon.GetSql("AgentData_New");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到AgentData_New数据模板，请联系管理员";
                    return null;
                }
                string _msg;
                List<AgentSearchModel> aList;
               
                if (!string.IsNullOrEmpty(model.A_UserID))
                {
                    string sSql = "";
                    if (model.A_UserID.Length <= 7)
                    {
                        sSql = "select top 1 AgentID from T_Agent where LogName ='" + model.A_UserID + "'";
                    }
                    else
                    {
                        sSql = "select top 1 AgentID from T_Client where LogName ='" + model.A_UserID + "'";
                    }
                    string aid = Db.Context_SqlServer.FromSql(sSql).ToScalar<string>();
                    strSql = strSql.Replace("${AgentID}", aid);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", aid, out _msg);
                }
                else
                {
                    strSql = strSql.Replace("${AgentID}", model.A_ID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out _msg);
                }
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = _msg;
                    return null;
                }
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <returns></returns>
        public string GetPubInfo(out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            string info = "";
            string sql = "select  * from T_PubInfo where IsPublish='YES' and InfoType='AGENT' order by CreateTime desc";
            //List<Models.Log.T_PubInfo> pubInfos = db.GetList<Models.Log.T_PubInfo>(sql);
            try
            {
                List<T_PubInfo> pubInfo = Db.Context_SqlServer.FromSql(sql).ToList<T_PubInfo>();
                if (pubInfo != null && pubInfo.Count > 0)
                {
                    for (int i = 0; i < pubInfo.Count; i++)
                    {
                        info += "(" + (i + 1) + ") " + pubInfo[i].PubInfo + "  ";
                    }
                }
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return info;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r","").Replace("\n","");
                return "";
            }
        }
        /// <summary>
        /// 根据代理ID获取代理数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetAListByID(AgentSearchModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrNo = "0003";
                    error.ErrMsg = "没有接收到参数";
                    return null;
                }
                string strSql = SqlTemplateCommon.GetSql("GetAListByID");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到GetAListByID数据模板，请联系管理员";
                    return null;
                }
                string _msg;
                List<AgentSearchModel> aList;
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out _msg);            
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = _msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);

                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";

                return CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(LoginDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
    }
}
