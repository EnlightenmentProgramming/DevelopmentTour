using Common;
using Common.ORMTool;
using Dos.Common;
using Dos.ORM;
using Model.Comunication;
using Model.DBModel;
using Model.LogicModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.LogicDAL
{
    /// <summary>
    /// 代理列表部分
    /// </summary>
    public class AgentListDAL : Repository<T_Agent>
    {
        /// <summary>
        /// 获取指定代理及它的直属代理列表数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetALists(AgentSearchModel model, HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                string pageStr = SqlTemplateCommon.GetSql("W_GetAllAgentsDataPage_New");
                string countStr = SqlTemplateCommon.GetSql("W_GetAListCount_New");
                if (string.IsNullOrEmpty(countStr) || string.IsNullOrEmpty(pageStr))
                {
                    error.ErrNo = "0004";
                    error.ErrMsg = "服务端没有读取到W_GetAllAgentsDataPage_New/W_GetAListCount_New数据模板，请联系管理员";
                    return null;
                }
                int pageSize = model.PageSize ?? 20, curePage = model.CurePage ?? 1;
                pageStr = pageStr.Replace("${pageSize}", pageSize.ToString());
                pageStr = pageStr.Replace("${curePage}", curePage.ToString());
                string whereSql = "";
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string messge;
                //判断是否有子查询
                if (!string.IsNullOrEmpty(model.A_UserID))
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.A_UserID, out messge);
                    string id = aList.Find(a => a.A_UserID == model.A_UserID).A_ID;
                    pageStr = pageStr.Replace("${AgentID}", id);
                    countStr = countStr.Replace("${AgentID}", id);
                    if (head.LoginID != id)
                        whereSql += " and LogName ='" + model.A_UserID + "'";
                }
                else
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out messge);
                    pageStr = pageStr.Replace("${AgentID}", model.A_ID);
                    countStr = countStr.Replace("${AgentID}", model.A_ID);
                }
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = messge;
                    error.ErrNo = "0004";
                    return null;
                }
                pageStr = pageStr.Replace("${WhereSql}", whereSql);
                countStr = countStr.Replace("${WhereSql}", whereSql);
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(pageStr).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    Count = Db.Context_SqlServer.FromSql(countStr).ToScalar<int>()
                });                    
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return null;
            }
        }
        /// <summary>
        /// 插入代理
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool InsertAgent(AgentSearchModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            bool res = false;
            try
            {
                #region 新增前的验证
                if (model == null)
                {
                    error.ErrMsg = "参数不完整";
                    error.ErrNo = "0003";
                    return false;
                }
                if (string.IsNullOrEmpty(model.A_UserID))
                {
                    error.ErrMsg = "必须填写登录名称";
                    error.ErrNo = "0003";
                    return false;
                }
                if (string.IsNullOrEmpty(model.A_PID))
                {
                    error.ErrMsg = "必须填写所属代理ID";
                    error.ErrNo = "0003";
                    return false;
                }
                if (string.IsNullOrEmpty(model.A_Pwd))
                {
                    error.ErrMsg = "必须填写代理登录密码";
                    error.ErrNo = "0003";
                    return false;
                }
                if (string.IsNullOrEmpty(model.A_Name))
                {
                    error.ErrMsg = "必须填写代理名称";
                    error.ErrNo = "0003";
                    return false;
                } 
                #endregion
                #region 新增代理
                T_Agent dbAgent = new T_Agent();
                AgentPermission aPerm = new AgentPermission();
                dbAgent.AgentID = Guid.NewGuid().ToString().Replace("-", "");
                dbAgent.AgentName = model.A_Name;
                dbAgent.Principal = 0;
                dbAgent.LogName = model.A_UserID;
                dbAgent.ParentID = model.A_PID;
                dbAgent.Pwd = model.A_Pwd;
                dbAgent.CreateTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                dbAgent.CreateID = head.LoginID;
                dbAgent.State = "YES";//新增代理默认为启用状态
                dbAgent.IsHide = "FALSE";
                aPerm.A_SetPv = model.A_SetPv ?? false;
                aPerm.A_MatchP = model.A_Matchp ?? false;
                dbAgent.F_3 = Common.CommonHelper.String2Base64(JSON.ToJSON(aPerm));
                dbAgent.F_1 = model.A_F1;
                dbAgent.F_2 = model.A_F2;
                if (model.A_DrawR != null && model.A_DrawR > 0)
                {
                    dbAgent.DrawRate = model.A_DrawR;
                }
                if (model.A_MN_Z != null)
                {
                    dbAgent.Min_ZD = model.A_MN_Z;
                    dbAgent.Min_Z = model.A_MN_Z;
                    dbAgent.Min_XD = model.A_MN_Z;
                    dbAgent.Min_X = model.A_MN_Z;
                    dbAgent.Min_H = model.A_MN_Z;
                }
                if (model.A_Mx_Z != null)
                {
                    dbAgent.Max_ZD = model.A_Mx_Z / 10;
                    dbAgent.Max_Z = model.A_Mx_Z;
                    dbAgent.Max_XD = model.A_Mx_Z / 10;
                    dbAgent.Max_X = model.A_Mx_Z;
                    dbAgent.Max_H = model.A_Mx_Z / 10;
                }
                if (model.A_WashR != null)
                {
                    dbAgent.WashRate = model.A_WashR;
                }
                if (model.A_WashT != null)
                {
                    dbAgent.WashType = (model.A_WashT == true) ? "S" : "D";
                }
                if (model.A_IntoR != null)
                {
                    dbAgent.IntoRate = model.A_IntoR;
                }
                if (Db.Context_SqlServer.Insert<T_Agent>(dbAgent) > 0)
                {
                    error.ErrMsg = "新增代理成功";
                    error.ErrNo = "0000";
                    res = true;
                }
                else
                {
                    error.ErrNo = "0004";
                    error.ErrMsg = "新增代理失败，请重试";
                    res = false;
                } 
                #endregion
                #region 写入新增代理操作日志
                T_OperationLog opLog = new T_OperationLog();
                opLog.LogID = System.Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                opLog.OpID = head.LoginID;
                opLog.LogType = "新增代理";
                opLog.LogInfo = DateTime.Now.ToString() + error.ErrMsg;
                if (Db.Context_SqlServer.Insert<T_OperationLog>(opLog) <= 0)
                {
                    Common.LogHelper.WriteErrLog(typeof(AgentListDAL), "新增代理操作日志写入失败");
                } 
                #endregion
                return res;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 随机生成length位登录名，然后判断是否存在，不存在则可以使用
        /// </summary>
        /// <returns></returns>
        public string GetLoginID(int length,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                length = length < 0 ? 0 : length;
                do
                {
                    string _logname = string.Empty;
                    Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
                    int tmp = 0;
                    for (int i = 0; i <= length - 1; i++)
                    {
                        tmp = ra.Next(0, 9); //随机取数
                        Thread.Sleep(100);
                        _logname = _logname.Trim() + tmp.ToString();
                    }
                    bool isRepeat = true;
                    switch(length)
                    {
                        case 5:
                            isRepeat = AgentSubDAL.Any(a => a.LogName == _logname); 
                            break;
                        case 6:
                            isRepeat = AgentListDAL.Any(a => a.LogName == _logname);
                            break;
                        case 8:
                            isRepeat = ClientListDAL.Any(a => a.LogName == _logname);
                            break;
                        default:
                            isRepeat = false;
                            break;
                    }
                    if (!isRepeat)
                    {
                        error.ErrMsg = "生成代理登录名称成功";
                        error.ErrNo = "0000";
                        return _logname;
                    }
                }
                while (true);
            }
            catch(Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return "";
            }

        }
        /// <summary>
        /// 修改代理
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateAgent(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                bool res = false;
                string sqlString = SqlTemplateCommon.GetSql("A_UpdateT_Agent");
                if (string.IsNullOrEmpty(sqlString))
                {
                    error.ErrMsg = "服务端没有读取到A_UpdateT_Agent数据模板，请联系管理员";
                    error.ErrNo = "0004";
                    return res;
                }
                if (string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrMsg = "必须传递需要修改代理的ID";
                    error.ErrNo = "0004";
                    return res;
                }               
                sqlString = sqlString.Replace("${AgentID}", model.A_ID);
                sqlString = sqlString.Replace("${LogName}", model.A_UserID);
                string isUpClntOdds = "0";//是否需要将当前代理链上的所有会员恢复为标准赔率（只有当前取消当前代理抽水权限时需要做此操作）
                T_OperationLog opLog = new T_OperationLog();
                StringBuilder logDesc = new StringBuilder();
                logDesc.Append(DateTime.Now.ToString() + " " + head.Account + "修改了代理" + model.A_UserID + ":");
                StringBuilder upBuilder = new StringBuilder();
                #region 组装修改代理信息Sql
                upBuilder.Append("UPDATE T_Agent set ");
                upBuilder.Append("AgentID = '" + model.A_ID + "' ");
                if(!string.IsNullOrEmpty(model.A_UserID))
                {
                    upBuilder.Append(",LogName = '" + model.A_UserID + "' ");
                }
                #region 代理抽水或配分权限修改处理
                AgentPermission tempPerm = new AgentPermission();
                AgentPermission newPerm = new AgentPermission();
                try
                {
                    AgentPermission oldPerm = Db.Context_SqlServer.FromSql("Select AgentID A_ID,F_3 A_Perm from T_Agent where AgentID ='" + model.A_ID + "'").ToFirst<AgentPermission>();
                    string aPerm = oldPerm.A_Perm;
                    if (!string.IsNullOrEmpty(aPerm) && aPerm.IndexOf("{") != -1 && aPerm.IndexOf("}") != -1)
                    {
                        tempPerm = JSON.ToObject<AgentPermission>(aPerm);
                    }
                    if (tempPerm != null)//此代理已经设置过配分或抽水了，先保留原来的设置
                    {
                        newPerm.A_SetPv = oldPerm.A_SetPv ?? false;
                        newPerm.A_MatchP = oldPerm.A_MatchP ?? false;
                    }
                    if (model.A_SetPv != null)
                    {
                        if (oldPerm != null && oldPerm.A_SetPv == true && model.A_SetPv == false)
                        {
                            isUpClntOdds = "1";
                        }
                        newPerm.A_SetPv = model.A_SetPv;
                        logDesc.Append("把代理" + model.A_UserID + "的抽水权限改为了" + model.A_SetPv + "；");
                    }
                    if (model.A_Matchp != null)
                    {
                        if (!isUpMatchp(model.A_ID, out error))
                        {
                            return res;
                        }
                        newPerm.A_MatchP = model.A_Matchp;
                        logDesc.Append("把代理" + model.A_UserID + "的配分权限改为了" + model.A_Matchp + "；");
                    }
                }
                catch (Exception)
                {
                    if (model.A_SetPv != null)
                    {
                        newPerm.A_SetPv = model.A_SetPv;
                        logDesc.Append("把代理" + model.A_UserID + "的抽水权限改为了" + model.A_Perm + "；");
                    }
                    if (model.A_Matchp != null)
                    {
                        if (!isUpMatchp(model.A_ID, out error))
                        {
                            return res;
                        }
                        newPerm.A_MatchP = model.A_Matchp;
                        logDesc.Append("把代理" + model.A_UserID + "的配分权限改为了" + model.A_Matchp + "；");
                    }
                }
                if (newPerm != null && (newPerm.A_SetPv != null || newPerm.A_MatchP != null))
                {
                    upBuilder.Append(",F_3 = '" + Common.CommonHelper.String2Base64(JSON.ToJSON(newPerm)) + "' ");
                }
                sqlString = sqlString.Replace("${IsUpOdds}", isUpClntOdds);
                #endregion
                if (model.A_IntoR != null)
                {
                    upBuilder.Append(",IntoRate =" + model.A_IntoR);
                    logDesc.Append("把代理" + model.A_UserID + "的占成改为了" + model.A_IntoR + "；");
                }
                if (!string.IsNullOrEmpty(model.A_Name))
                {
                    upBuilder.Append(",AgentName = '" + model.A_Name + "' ");
                }
                if (model.A_Mx_Z != null)
                {
                    upBuilder.Append(", Max_Z = " + model.A_Mx_Z);
                    upBuilder.Append(" ,Max_X = " + model.A_Mx_Z);
                    upBuilder.Append(" ,Max_H = " + model.A_Mx_Z / 10);
                    upBuilder.Append(" ,Max_XD = " + model.A_Mx_Z / 10);
                    upBuilder.Append(" ,Max_ZD = " + model.A_Mx_Z / 10);
                    logDesc.Append("把代理" + model.A_UserID + "的最大限红改为了" + model.A_Mx_Z + "；");
                }
                if (model.A_MN_Z != null)
                {
                    upBuilder.Append(", Min_Z = " + model.A_MN_Z);
                    upBuilder.Append(" ,Min_ZD = " + model.A_MN_Z);
                    upBuilder.Append(" ,Min_X = " + model.A_MN_Z);
                    upBuilder.Append(" ,Min_XD = " + model.A_MN_Z);
                    upBuilder.Append(" ,Min_H = " + model.A_MN_Z);
                    logDesc.Append("把代理" + model.A_UserID + "的最小限红改为了" + model.A_MN_Z + "；");
                }
                if (!string.IsNullOrEmpty(model.A_F2))
                {
                    upBuilder.Append(", F_2 = '" + model.A_F2 + "' ");
                }
                if (model.A_WashT != null)
                {
                    upBuilder.Append(",WashType = '" + ((model.A_WashT == true) ? "S" : "D") + "'");
                    logDesc.Append("把代理" + model.A_UserID + "的洗码类型改为了；" + ((model.A_WashT == true) ? "双边" : "单边"));
                }
                if (model.A_WashR != null)
                {
                    upBuilder.Append(" ,WashRate = " + model.A_WashR);
                    logDesc.Append("把代理" + model.A_UserID + "的洗码率改为了" + model.A_WashR + "；");
                }
                if (!string.IsNullOrEmpty(model.A_State))
                {
                    upBuilder.Append(",State = '" + model.A_State + "' ");
                    logDesc.Append("把代理" + model.A_UserID + "的状态改为了" + model.A_State + "；");
                }
                if (model.A_DrawR != null)
                {
                    upBuilder.Append(",DrawRate = " + model.A_DrawR);
                    logDesc.Append("把代理" + model.A_UserID + "的和局率改为了" + model.A_DrawR + "；");
                }
                upBuilder.Append(" where AgentID ='"+model.A_ID+"'");
                #endregion

                #region 组装修改代理限红sql
                StringBuilder authBuilder = new StringBuilder();
                authBuilder.Append("update T_Authority set Priority = 'A'");
                if (model.A_Mx_Z != null)//如果修改了配分权限则不修改限红
                {
                    authBuilder.Append(",Max_Z = " + model.A_Mx_Z);
                    authBuilder.Append(",Max_X = " + model.A_Mx_Z);
                    authBuilder.Append(",Max_H = " + model.A_Mx_Z / 10);
                    authBuilder.Append(",Max_XD = " + model.A_Mx_Z / 10);
                    authBuilder.Append(",Max_ZD = " + model.A_Mx_Z / 10);
                }
                if (model.A_MN_Z != null)//如果修改了配分权限则不修改限红
                {
                    authBuilder.Append(",Min_Z = " + model.A_MN_Z);
                    authBuilder.Append(",Min_X = " + model.A_MN_Z);
                    authBuilder.Append(",Min_H = " + model.A_MN_Z);
                    authBuilder.Append(",Min_XD = " + model.A_MN_Z);
                    authBuilder.Append(",Min_ZD = " + model.A_MN_Z);
                }
                authBuilder.Append(" where Priority = 'A' and ClientID in (select ClientID from T_Client where AgentID = '" + model.A_ID + "')");
                #endregion

                sqlString = sqlString.Replace("${updateAgent}", upBuilder.ToString());
                sqlString = sqlString.Replace("${upAuthority}", authBuilder.ToString());
                sqlString = sqlString.Replace("##'", "");
                sqlString = sqlString.Replace("'##", "");
                sqlString = sqlString.Replace("##\"", "");
                sqlString = sqlString.Replace("\"##", "");

                opLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogInfo = logDesc.ToString();
                opLog.OpID = head.LoginID;
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now); //DateTime.Now.ToString();
                opLog.LogType = "修改代理";
                if (Db.Context_SqlServer.Insert<T_OperationLog>(opLog) <= 0)
                {
                    Common.LogHelper.WriteErrLog(typeof(AgentListDAL), "修改代理时写入操作日志失败");
                }
                error.ErrMsg = "修改代理成功";
                error.ErrNo = "0000";
                Db.Context_SqlServer.FromSql(sqlString).ToDataSet();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return false;
            }
        }
        /// <summary>
        /// 判断是否可以修改配分权限，修改配分权限之前群组余额不能大于0，并且需要结算完洗码费和抽水费
        /// </summary>
        /// <param name="aId"></param>
        /// <returns></returns>
        private bool isUpMatchp(string aId ,out ErrorMessage error)
        {
            error = new ErrorMessage();
            string strSql = SqlTemplateCommon.GetSql("UpMatchpCount4A");
            if(string.IsNullOrEmpty(strSql))
            {
                error.ErrMsg = "服务端没有读取到UpMatchpCount4A数据模板，请联系管理员";
                error.ErrNo = "0004";
                return false;
            }
            strSql = strSql.Replace("${AgentID}", aId);
            AgentReport aReport = Db.Context_SqlServer.FromSql(strSql).ToFirst<AgentReport>();
            if(aReport != null)
            {
                if(aReport.A_ChouS >0)
                {
                    error.ErrMsg = "修改配分权限之前请先结算抽水";
                    error.ErrNo = "0004";
                    return false;
                }
                if(aReport.A_WashFee >0)
                {
                    error.ErrMsg = "修改配分权限之前请先结算洗码费";
                    error.ErrNo = "0004";
                    return false;
                }
                if (aReport.A_GroupPrinc > 0)
                {
                    error.ErrMsg = "修改配分权限之前请先清零此代理";
                    error.ErrNo = "0004";
                    return false;
                }
            }
            return true;

        }
        /// <summary>
        /// 修改代理密码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateAPwd(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                string strSql = SqlTemplateCommon.GetSql("A_SaveAgentModifyPassword");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_SaveAgentModifyPassword数据模板，请联系管理员";
                    error.ErrNo = "0004";
                    return false;
                }
                if (string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrMsg = "必须传入要修改代理的ID";
                    error.ErrNo = "0004";
                    return false;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                strSql = strSql.Replace("${Pwd}", model.A_Pwd);
                error.ErrMsg = "修改代理密码成功";
                error.ErrNo = "0000";
                T_OperationLog opLog = new T_OperationLog();
                opLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogInfo = DateTime.Now.ToString() + "代理" + head.Account + "修改了下级代理" + model.A_UserID + "的登录密码";
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now); 
                opLog.OpID = head.LoginID;
                opLog.LogType = "修改代理登录密码";
                if(Db.Context_SqlServer.Insert<T_OperationLog>(opLog) <= 0)
                {
                    Common.LogHelper.WriteErrLog(typeof(AgentListDAL), "插入"+head.Account +"修改下级代理密码日志失败");
                }
                Db.Context_SqlServer.FromSql(strSql).ToDataSet();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return false;
            }
        }
        /// <summary>
        /// 修改登录代理密码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateLoginAPwd(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                string strSql = SqlTemplateCommon.GetSql("A_AgentSelfModifyPassword");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_AgentSelfModifyPassword数据模板，请联系管理员";
                    error.ErrNo = "0004";
                    return false;
                }
                if (string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrMsg = "必须传入要修改代理的ID";
                    error.ErrNo = "0004";
                    return false;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                strSql = strSql.Replace("${OldPwd}", model.A_OldPwd);
                strSql = strSql.Replace("${Pwd}", model.A_Pwd);
                error.ErrMsg = "修改代理密码成功";
                error.ErrNo = "0000";
                T_OperationLog opLog = new T_OperationLog();
                opLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogInfo = DateTime.Now.ToString() + "代理" + head.Account + "修改登录密码";
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                opLog.OpID = head.LoginID;
                opLog.LogType = "修改代理登录密码";
                if (Db.Context_SqlServer.Insert<T_OperationLog>(opLog) <= 0)
                {
                    Common.LogHelper.WriteErrLog(typeof(AgentListDAL), "插入"+head.Account + "修改登录密码日志失败");
                }
                Db.Context_SqlServer.FromSql(strSql).ToDataSet();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return false;
            }
        }
        /// <summary>
        /// 代理上下分
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool AgentPoint(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                if (string.IsNullOrEmpty(model.A_ID) || model.A_IsAdd == null || model.A_Point == null || string.IsNullOrEmpty(head.LoginID) || string.IsNullOrEmpty(model.A_PID))
                {
                    error.ErrMsg = "参数不完整";
                    error.ErrNo = "0004";
                    return false;
                }
                if (model.A_Point <= 0)
                {
                    error.ErrMsg = "上下分不在正确范围";
                    error.ErrNo = "0004";
                    return false;
                }
                string strSql = SqlTemplateCommon.GetSql("A_SaveAgentPoint");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_SaveAgentPoint数据模板，请联系管理员";
                    error.ErrNo = "0004";
                    return false;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);//上下分对象代理ID
                strSql = strSql.Replace("${ParentID}", model.A_PID);//上下分对象代理父级代理ID
                strSql = strSql.Replace("${Point}", model.A_Point.ToString());//上下分点数
                strSql = strSql.Replace("${IsAdd}", model.A_IsAdd.ToString().ToUpper());//标记上下分 true=下分 false=上分
                strSql = strSql.Replace("${CreateID}", head.LoginID);//登录代理ID
                strSql = strSql.Replace("${LoginUser}", head.Account);//登录代理登录账号
                strSql = strSql.Replace("${IP}", head.Ip);
                strSql = strSql.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                strSql = strSql.Replace("${IPLocal}", Common.CommonHelper.ipToAddr(head.Ip));
                strSql = strSql.Replace("${LevelPoint}", model.A_LevelPoint ?? "0");//标记是否是上级上下分
                strSql = strSql.Replace("${IsAClear}", "0");//标记是否是代理清零
                error.ErrMsg = model.A_IsAdd == true ? "代理上分成功" : "代理下分成功";
                error.ErrNo = "0000";
                Db.Context_SqlServer.FromSql(strSql).ToDataSet();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return false;
            }

        }
        /// <summary>
        /// 获取指定代理下逻辑删除代理
        /// IsHide = "TRUE"
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetDeletedA(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                string strSql = SqlTemplateCommon.GetSql("deletedAgent");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrNo = "0004";
                    error.ErrMsg = "服务端没有读取到deletedAgent数据模板，请联系管理员";
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string messge;
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out messge);                
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = messge;
                    error.ErrNo = "0004";
                    return null;
                }              
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return null;
            }
        }
        /// <summary>
        /// 清零指定代理及他所有下级代理及会员
        /// </summary>
        /// <param name="creID"></param>
        /// <param name="aid"></param>
        /// <returns></returns>
        public bool ClearAgent(HeadMessage head, string aid, string pid)
        {
            try
            {
                string selcctSql = "select AgentID A_ID,ParentID A_PID from T_Agent where parentID ='" + aid + "' or AgentID = '"+aid+"'";
                List<AgentSearchModel> idSList = Db.Context_SqlServer.FromSql(selcctSql).ToList<AgentSearchModel>();
                if (idSList != null || idSList.Count > 0)
                {
                    for (int i = 0; i < idSList.Count; i++)
                    {
                        CleareA2C(head, idSList[i].A_ID, pid);//清零直属代理及直属会员
                        ClearAgent(head, idSList[i].A_ID, pid);//清零直属代理本身
                    }                    
                }               
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                return false;
            }

        }
        /// <summary>
        /// 清零指定代理直属会员及直属会员
        /// </summary>
        /// <param name="creID"></param>
        /// <param name="aid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public bool CleareA2C(HeadMessage head, string aid, string pid)
        {
            try
            {
                string cClearStr = SqlTemplateCommon.GetSql("A_SetAgentClientsClear");
                if (string.IsNullOrEmpty(cClearStr))
                {
                    return false;
                }
                cClearStr = cClearStr.Replace("${CreateID}", head.LoginID);
                cClearStr = cClearStr.Replace("${AgentID}", aid);
                cClearStr = cClearStr.Replace("${IP}", head.Ip);
                cClearStr = cClearStr.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                Db.Context_SqlServer.FromSql(cClearStr).ToDataSet();//直属会员清零
                decimal princple = Db.Context_SqlServer.FromSql("select Principal from T_Agent where AgentID ='" + aid + "'").ToScalar<decimal>();
                string aPointStr = SqlTemplateCommon.GetSql("A_SaveAgentPoint");
                if (string.IsNullOrEmpty(aPointStr))
                {
                    return false;
                }
                aPointStr = aPointStr.Replace("${AgentID}", aid);
                aPointStr = aPointStr.Replace("${Point}", princple.ToString());
                aPointStr = aPointStr.Replace("${IsAdd}", "TRUE");
                aPointStr = aPointStr.Replace("${CreateID}", head.LoginID);
                aPointStr = aPointStr.Replace("${IP}", head.Ip);
                aPointStr = aPointStr.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                aPointStr = aPointStr.Replace("${IPLocal}", Common.CommonHelper.ipToAddr(head.Ip));
                aPointStr = aPointStr.Replace("${ParentID}", pid);
                aPointStr = aPointStr.Replace("${LevelPoint}", "0");
                aPointStr = aPointStr.Replace("${IsAClear}", "1");
                if (princple > 0)
                {
                    Db.Context_SqlServer.FromSql(aPointStr).ToDataTable();//代理清零
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                return false;
            }
        }
        /// <summary>
        /// 代理清零
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ClearAgent(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID) || string.IsNullOrEmpty(model.A_PID))
                {
                    error.ErrMsg = "清零参数不完整";
                    error.ErrNo = "0004";
                    return false;
                }
                if (ClearAgent(head, model.A_ID, model.A_PID))
                {
                    error.ErrMsg = "代理清零成功";
                    error.ErrNo = "0000";
                    return true;
                }
                else
                {
                    error.ErrMsg = "代理清零失败";
                    error.ErrNo = "0004";
                    return true;
                }
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return false;
            }
        }
        /// <summary>
        /// 判断指定代理直属代理是否有配分权限
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool IsSubAMatchP(string agentID)
        {
            try
            {
                if (string.IsNullOrEmpty(agentID))
                {
                    return true;
                }
                string strSql = "select AgentID A_ID,AgentName A_Name, LogName A_UserID ,[dbo].[Base64Decode](F_3) A_Perm from T_Agent where ParentID = '" + agentID + "'";
                List<AgentPermission> aList = Db.Context_SqlServer.FromSql(strSql).ToList<AgentPermission>();
                if (aList == null || aList.Count <= 0)
                {
                    return false;
                }
                bool flag = false;//标记是否有配分权限
                AgentPermission setOdd = new AgentPermission();
                for (int i = 0; i < aList.Count; i++)
                {
                    if(aList[i].A_Perm != null && aList[i].A_Perm.IndexOf("\"MatchPoint\":true") != -1)
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 判断指定代理下级代理是否有抽水权限
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool IsSubAOdds(string agentID)
        {
            try
            {
                if (string.IsNullOrEmpty(agentID))
                {
                    return true;
                }
                string strSql = "select AgentID A_ID,AgentName A_Name, LogName A_UserID ,[dbo].[Base64Decode](F_3) A_Perm from T_Agent where ParentID = '" + agentID + "'";
                List<AgentPermission> aList = Db.Context_SqlServer.FromSql(strSql).ToList<AgentPermission>();
                if (aList == null || aList.Count <= 0)
                {
                    return false;
                }
                bool flag = false;//标记是否有配分权限
                AgentPermission setOdd = new AgentPermission();
                for (int i = 0; i < aList.Count; i++)
                {
                    if (aList[i].A_Perm != null && aList[i].A_Perm.IndexOf("\"SetPV\":true") != -1)
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取下级代理最大占成
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public decimal GetSubMaxIntoR(string agentID)
        {
            try
            {
                if (string.IsNullOrEmpty(agentID))
                {
                    return 0;
                }
                string str = "select MAX(IntoRate) maxIntoR from T_Agent where ParentID ='" + agentID + "' ";
                return Db.Context_SqlServer.FromSql(str).ToScalar<decimal?>() ?? 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取指定代理下级代理
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool GetSubAgent(string agentID)
        {
            try
            {
                if (string.IsNullOrEmpty(agentID))
                {
                    return false;
                }
                string strSql = "select AgentID A_ID,ParentID A_PID,AgentName A_Name, LogName A_UserID ,F_3 A_Perm from T_Agent where ParentID = '" + agentID + "'";
                List<AgentPermission> aList = Db.Context_SqlServer.FromSql(strSql).ToList<AgentPermission>();
                return (aList!= null && aList.Count >0 );

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        /// <summary>
        /// 判断两个代理之间是否有配分权限
        /// </summary>
        /// <param name="aID"></param>
        /// <param name="loginID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool PHasAnyMatchP(string loginID, string aID)
        {
            if (string.IsNullOrEmpty(aID) || string.IsNullOrEmpty(loginID))
            {
                return true;
            }
            List<AgentPermission> aList = new List<AgentPermission>();
            string strSql = "select AgentID A_ID,ParentID A_PID ,AgentName A_Name, LogName A_UserID ,[dbo].[Base64Decode](F_3) A_Perm from T_Agent where AgentID = '" + aID + "'";
            AgentPermission firstAgent = Db.Context_SqlServer.FromSql(strSql).ToFirst<AgentPermission>();
            if (firstAgent == null)
            {
                return true;
            }
            if (firstAgent.A_PID == loginID)
            {
                return false;
            }
            if (!aList.Contains(firstAgent)) aList.Add(firstAgent);
            string pID = firstAgent.A_PID;
            AgentPermission secondAgent = new AgentPermission();
            while (true)
            {
                if (firstAgent.A_ID == loginID)
                {
                    break;
                }
                string strPSql = "select AgentID A_ID,ParentID A_PID ,AgentName A_Name, LogName A_UserID ,[dbo].[Base64Decode](F_3) A_Perm from T_Agent where AgentID = '" + pID + "'";
                secondAgent = Db.Context_SqlServer.FromSql(strPSql).ToFirst<AgentPermission>();
                if (secondAgent == null)
                {
                    return true;
                }
                if (secondAgent.A_ID == loginID)
                {
                    break;
                }
                if (!aList.Contains(secondAgent))
                {
                    aList.Add(secondAgent);
                }
                pID = secondAgent.A_PID;
            }
            if (aList == null || aList.Count <= 0)
            {
                return true;
            }
            bool flag = false;//标记是否有配分权限
            AgentPermission setOdd = new AgentPermission();
            for (int i = 0; i < aList.Count; i++)
            {
                if(aList[i].A_Perm != null && aList[i].A_Perm.IndexOf("\"MatchPoint\":true") != -1)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        /// <summary>
        /// 获取指定代理相关权限
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetAPermission(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID) || string.IsNullOrEmpty(head.LoginID))
                {
                    error.ErrMsg = "请求参数不完整";
                    error.ErrNo = "0004";
                    return null;
                }
                error.ErrMsg = "获取代理相关权限成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    SubMacthP = IsSubAMatchP(model.A_ID),
                    IsSubOdds = IsSubAOdds(model.A_ID),
                    PMacthP = PHasAnyMatchP(head.LoginID, model.A_ID),
                    HasSubAgent = GetSubAgent(model.A_ID),
                    SubMaxIntoR = GetSubMaxIntoR(model.A_ID)
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                error.ErrNo = "0004";
                return null;
            }
        }
    }
}
