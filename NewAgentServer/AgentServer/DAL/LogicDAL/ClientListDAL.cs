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
using System.Threading.Tasks;

namespace DAL.LogicDAL
{
    public class ClientListDAL : Repository<T_Client>
    {
        /// <summary>
        /// 获取会员列表数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetCLists(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string sqlStr = SqlTemplateCommon.GetSql("GetClntList_Invite");
                string curStr = SqlTemplateCommon.GetSql("CurrentClnt");
                if (string.IsNullOrEmpty(sqlStr) || string.IsNullOrEmpty(curStr))
                {
                    error.ErrMsg = "服务端没有读取到GetClntList_Invite/CurrentClnt数据模板，请联系管理员";
                    return null;
                }
                sqlStr = sqlStr.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                sqlStr = sqlStr.Replace("${curePage}", (model.CurePage ?? 1).ToString());
                string whereSql = string.Empty;
                if (!string.IsNullOrEmpty(model.C_Type))//如果不传则默认取H5h和常规会员
                {
                    whereSql += " and C_Type ='" + model.C_Type + "'";
                }
                if (string.IsNullOrEmpty(model.C_UserID))
                {
                    if (!string.IsNullOrEmpty(model.C_InType))//默认查询常规会员和代理直接邀请的会员
                    {
                        switch (model.C_InType)
                        {
                            case "A"://只过滤指定代理直接邀请会员
                                whereSql += " and C_InType ='A'";
                                break;
                            case "C"://只过滤指定代理下会员邀请会员
                                whereSql += " and C_InType ='C'";
                                break;
                            case "O"://只过滤常规会员
                                whereSql += " and C_InType ='O'";
                                break;
                            case "AC"://只过滤H5会员
                                whereSql += " and (C_InType ='A' or C_InType ='C')";
                                break;
                            case "OC"://只过滤H5会员
                                whereSql += " and (C_InType ='O' or C_InType ='C')";
                                break;
                            case "AOC"://只过滤H5会员
                                whereSql += " and (C_InType ='O' or C_InType ='C' or C_InType ='A')";
                                break;
                            default:
                                whereSql += " and (C_InType ='A' or C_InType ='O')";
                                break;
                        }
                    }
                    else
                    {
                        whereSql += " and (C_InType ='A' or C_InType ='O')";
                    }
                }
                else
                {
                    whereSql += " and (C_InType ='A' or C_InType ='O' or C_InType ='C')";
                }
                sqlStr = sqlStr.Replace("${whereSql}", whereSql);
                List<AgentSearchModel> aList;
                string msg;
                if (string.IsNullOrEmpty(model.C_UserID))
                {
                    sqlStr = sqlStr.Replace("${AgentID}", model.C_AID);
                    curStr = curStr.Replace("${AgentID}", model.C_AID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.C_AID, out msg);
                }
                else
                {
                    string id = Db.Context_SqlServer.FromSql("select ClientID from T_Client where LogName ='" + model.C_UserID + "'").ToScalar<string>();
                    string aId = Db.Context_SqlServer.FromSql("select AgentID from T_Client where LogName ='" + model.C_UserID + "'").ToScalar<string>();
                    curStr = curStr.Replace("${AgentID}", id);
                    sqlStr = sqlStr.Replace("${AgentID}", id);
                    aList = CommonDAL.GetAgentTree(head.LoginID, model.C_UserID, out msg);
                    if (aList == null)
                    {
                        aList = CommonDAL.GetAgentTree(head.LoginID, "id", aId, out msg);
                    }
                }
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(sqlStr).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    CurrentClnt = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(curStr).ToDataTable())
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
        /// 获取会员标准赔率
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetClntOdds(out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = SqlTemplateCommon.GetSql("ClntOdds");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到ClntOdds数据模板，请联系管理员";
                    return null;
                }
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理下逻辑删除会员
        /// IsHide = "TRUE"
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetDeletedC(ClientSearc model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = SqlTemplateCommon.GetSql("deletedClnt");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到deletedClnt数据模板，请联系管理员";
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.C_AID);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string messge;
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.C_AID, out messge);
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = messge;
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
                return null;
            }
        }
        /// <summary>
        /// 新增会员
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool InsertClient(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            DbTrans tran = null;
            DbBatch batch = null;

            try
            {
                #region 操作前验证
                if (string.IsNullOrEmpty(model.C_UserID))
                {
                    error.ErrMsg = "新增会员时登录名称不能为空";
                    return false;
                }
                if (string.IsNullOrEmpty(model.C_Name))
                {
                    error.ErrMsg = "新增会员时会员名称不能为空";
                    return false;
                }
                if (string.IsNullOrEmpty(model.C_Pwd))
                {
                    error.ErrMsg = "新增会员时密码不能为空";
                    return false;
                }
                if (string.IsNullOrEmpty(model.C_AID))
                {
                    error.ErrMsg = "新增会员时必须指定所属代理";
                    return false;
                }
                #endregion
                string tableSql = SqlTemplateCommon.GetSql("T_GetAllT_Tables");
                string oddSql = SqlTemplateCommon.GetSql("ClntOdds");
                if (string.IsNullOrEmpty(tableSql) || string.IsNullOrEmpty(oddSql))
                {
                    error.ErrMsg = "服务端没有读取到T_GetAllT_Tables/oddSql数据模板，请联系管理员";
                    return false;
                }
                List<TableModel> tableList = Db.Context_SqlServer.FromSql(tableSql).ToList<TableModel>();//获取系统中的所有桌台
                ClientOdds standardOdd = Db.Context_SqlServer.FromSql(oddSql).ToFirst<ClientOdds>();//获取会员标准赔率对象
                List<T_Authority> authList = new List<T_Authority>();//给新增会员配置所有桌台权限
                T_Client dbClnt = new T_Client();
                T_ClientEx dbClntEx = new T_ClientEx();
                T_OperationLog opLog = new T_OperationLog();
                #region 构造桌台权限
                if (tableList != null && tableList.Count > 0)
                {
                    foreach (var item in tableList)
                    {
                        T_Authority authority = new T_Authority();
                        authority.ClientID = model.C_ID;
                        authority.TableID = item.T_ID;
                        authority.Priority = "C";
                        if (model.C_MN_Z != null)
                        {
                            authority.Min_Z = model.C_MN_Z;
                            authority.Min_ZD = model.C_MN_Z / 10;
                            authority.Min_XD = model.C_MN_Z / 10;
                            authority.Min_X = model.C_MN_Z;
                            authority.Min_H = model.C_MN_Z / 10;
                        }
                        if (model.C_MX_Z != null)
                        {
                            authority.Max_ZD = model.C_MX_Z / 10;
                            authority.Max_Z = model.C_MX_Z;
                            authority.Max_XD = model.C_MX_Z / 10;
                            authority.Max_X = model.C_MX_Z;
                            authority.Max_H = model.C_MX_Z / 10;
                        }
                        authList.Add(authority);
                    }
                }
                #endregion

                #region 构造新增会员对象
                dbClnt.ClientID = Guid.NewGuid().ToString().Replace("-", "");
                dbClnt.CreateID = head.LoginID;
                dbClnt.Balance = 0;
                dbClnt.Principal = 0;
                dbClnt.State = "YES";//新增会员默认为启用状态
                dbClnt.CreateTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                dbClnt.AgentID = model.C_AID;
                dbClnt.ClientName = model.C_Name;
                dbClnt.LogName = model.C_UserID;
                dbClnt.F_2 = model.C_F2;
                dbClnt.IsHide = "FALSE";
                dbClnt.Pwd = model.C_Pwd;
                if (model.C_WashT != null)
                {
                    dbClnt.WashType = (model.C_WashT == true) ? "S" : "D";
                }
                if (model.C_WashR != null)
                {
                    dbClnt.WashRate = model.C_WashR;
                }
                if (model.C_DrawR != null)
                {
                    dbClnt.DrawRate = model.C_DrawR;
                }
                if (model.C_MN_Z != null)
                {
                    dbClnt.Min_H = model.C_MN_Z / 10;
                    dbClnt.Min_X = model.C_MN_Z;
                    dbClnt.Min_XD = model.C_MN_Z / 10;
                    dbClnt.Min_Z = model.C_MN_Z;
                    dbClnt.Min_ZD = model.C_MN_Z / 10;
                }
                if (model.C_MX_Z != null)
                {
                    dbClnt.Max_H = model.C_MX_Z / 10;
                    dbClnt.Max_X = model.C_MX_Z;
                    dbClnt.Max_XD = model.C_MX_Z / 10;
                    dbClnt.Max_Z = model.C_MX_Z;
                    dbClnt.Max_ZD = model.C_MX_Z / 10;
                }
                #endregion

                #region 构造会员扩展对象
                dbClntEx.ClientID = model.C_ID;
                dbClntEx.ClientDataShow = (model.C_HdShow == true) ? 2 : 1;
                if (model.C_ODF != null)
                {
                    dbClntEx.Odds_Fu_Client = (model.C_ODF > 0) ? model.C_ODF : standardOdd.C_ODF;
                }
                if (model.C_ODH != null)
                {
                    dbClntEx.Odds_H_Client = (model.C_ODH > 0) ? model.C_ODH : standardOdd.C_ODH;
                }
                if (model.C_ODHe != null)
                {
                    dbClntEx.Odds_He_Client = (model.C_ODHe > 0) ? model.C_ODHe : standardOdd.C_ODHe;
                }
                if (model.C_ODL != null)
                {
                    dbClntEx.Odds_Long_Client = (model.C_ODL > 0) ? model.C_ODL : standardOdd.C_ODL;
                }
                if (model.C_ODX != null)
                {
                    dbClntEx.Odds_X_Client = (model.C_ODX > 0) ? model.C_ODX : standardOdd.C_ODX;
                }
                if (model.C_ODXD != null)
                {
                    dbClntEx.Odds_XD_Client = (model.C_ODXD > 0) ? model.C_ODXD : standardOdd.C_ODXD;
                }
                if (model.C_ODZ != null)
                {
                    dbClntEx.Odds_Z_Client = (model.C_ODZ > 0) ? model.C_ODZ : standardOdd.C_ODZ;
                }
                if (model.C_ODZD != null)
                {
                    dbClntEx.Odds_ZD_Client = (model.C_ODZD > 0) ? model.C_ODZD : standardOdd.C_ODZD;
                }
                #endregion

                #region 构造操作日志对象
                opLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogInfo = DateTime.Now.ToString() + head.Account + "新增会员" + dbClnt.LogName;
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now); //DateTime.Now.ToString();
                opLog.OpID = head.LoginID;
                opLog.LogType = "新增会员";
                #endregion

                tran = Db.Context_SqlServer.BeginTransaction();
                batch = Db.Context_SqlServer.BeginBatchConnection(30, tran);
                tran.Insert<T_Client>(dbClnt);
                tran.Insert<T_ClientEx>(dbClntEx);
                batch.Insert<T_Authority>(authList.ToArray());
                tran.Insert<T_OperationLog>(opLog);
                batch.Execute();
                tran.Commit();
                error.ErrMsg = "新增会员成功";
                error.ErrNo = "0000";
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
            finally
            {
                if (batch != null)
                {
                    batch.Close();
                    batch.Dispose();
                }
                if (tran != null)
                {
                    tran.Close();
                    tran.Dispose();
                }
            }
        }
        /// <summary>
        /// 直属会员清零
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ClearClnt4Agent(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.C_AID) || string.IsNullOrEmpty(head.LoginID))
                {
                    error.ErrMsg = "参数不完整";
                    return false;
                }
                string strSql = SqlTemplateCommon.GetSql("A_SetAgentClientsClear");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_SetAgentClientsClear数据模板，请联系管理员";
                    return false;
                }
                strSql = strSql.Replace("${CreateID}", head.LoginID);
                strSql = strSql.Replace("${AgentID}", model.C_AID);
                strSql = strSql.Replace("${IP}", head.Ip);
                strSql = strSql.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                error.ErrNo = "0000";
                error.ErrMsg = "直属会员清零成功";
                Db.Context_SqlServer.FromSql(strSql).ToDataTable();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 会员清零
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ClearClnt(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.C_AID) || string.IsNullOrEmpty(head.LoginID))
                {
                    error.ErrMsg = "参数不完整";
                    return false;
                }
                if(CommonDAL.IsH5Clnt(model.C_ID))
                {
                    error.ErrMsg = "不允许对H5会员进行清零";
                    return false;
                }
                string strSql = SqlTemplateCommon.GetSql("A_Client_QL");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_Client_QL数据模板，请联系管理员";
                    return false;
                }
                strSql = strSql.Replace("${CreateID}", head.LoginID);
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                strSql = strSql.Replace("${IP}", head.Ip);
                strSql = strSql.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                error.ErrNo = "0000";
                error.ErrMsg = "直属会员清零成功";
                Db.Context_SqlServer.FromSql(strSql).ToDataTable();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 会员清卡
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ClearCard4Clnt(ClientSearc model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.C_AID) || string.IsNullOrEmpty(head.LoginID))
                {
                    error.ErrMsg = "参数不完整";
                    return false;
                }
                if (CommonDAL.IsH5Clnt(model.C_ID))
                {
                    error.ErrMsg = "不允许对H5会员进行清卡";
                    return false;
                }
                string strSql = SqlTemplateCommon.GetSql("A_Client_QK");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_Client_QK数据模板，请联系管理员";
                    return false;
                }
                strSql = strSql.Replace("${CreateID}", head.LoginID);
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                strSql = strSql.Replace("${IP}", head.Ip);
                strSql = strSql.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                error.ErrNo = "0000";
                error.ErrMsg = "直属会员清零成功";
                Db.Context_SqlServer.FromSql(strSql).ToDataTable();
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 修改会员
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateClnt(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (string.IsNullOrEmpty(model.C_ID))
                {
                    error.ErrMsg = "必须传递需要修改会员的ID";
                    return false;
                }
                string strSql = SqlTemplateCommon.GetSql("A_UpdateT_Client");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_UpdateT_Client数据模板，请联系管理员";
                    return false;
                }
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                strSql = strSql.Replace("${LogName}", model.C_UserID);

                StringBuilder clntBuilder = new StringBuilder();
                StringBuilder authBuilder = new StringBuilder();
                StringBuilder clntExBuilder = new StringBuilder();
                T_OperationLog opLog = new T_OperationLog();
                StringBuilder logDesc = new StringBuilder();
                logDesc.Append(DateTime.Now.ToString() + head.Account + "修改了会员" + model.C_UserID + " 以下信息：");

                #region 组装修改会员Sql
                clntBuilder.Append("UPDATE T_Client set ");
                clntBuilder.Append(" ClientID ='" + model.C_ID + "'");
                if (!string.IsNullOrEmpty(model.C_UserID))
                {
                    clntBuilder.Append(",LogName='" + model.C_UserID + "'");
                }
                if (!string.IsNullOrEmpty(model.C_Name))
                {
                    clntBuilder.Append(",ClientName='" + model.C_Name + "'");
                }
                if (model.C_MN_Z != null)
                {
                    clntBuilder.Append(",Min_Z =" + model.C_MN_Z);
                    clntBuilder.Append(",Min_X =" + model.C_MN_Z);
                    clntBuilder.Append(",Min_XD =" + model.C_MN_Z);
                    clntBuilder.Append(",Min_ZD =" + model.C_MN_Z);
                    clntBuilder.Append(",Min_H =" + model.C_MN_Z);
                    logDesc.Append("把最小限红改为了" + model.C_MN_Z + "；");
                }
                if (model.C_MX_Z != null)
                {
                    clntBuilder.Append(",Max_Z =" + model.C_MX_Z);
                    clntBuilder.Append(",Max_X =" + model.C_MX_Z);
                    clntBuilder.Append(",Max_XD =" + model.C_MX_Z / 10);
                    clntBuilder.Append(",Max_ZD =" + model.C_MX_Z / 10);
                    clntBuilder.Append(",Max_H =" + model.C_MX_Z / 10);
                    logDesc.Append("把最大限红改为了" + model.C_MX_Z + "；");
                }
                if (!string.IsNullOrEmpty(model.C_F2))
                {
                    clntBuilder.Append(",F_2 ='" + model.C_F2 + "'");
                }
                if (model.C_WashT != null)
                {
                    clntBuilder.Append(",WashType ='" + ((model.C_WashT) == true ? "s" : "d") + "'");
                    logDesc.Append("把洗码类型修改为了" + ((model.C_WashT == true) ? "双边" : "单边") + "；");
                }
                if (model.C_WashR != null)
                {
                    clntBuilder.Append(",WashRate =" + model.C_WashR);
                    logDesc.Append("把洗码率修改为" + model.C_WashR + "；");
                }
                if (model.C_DrawR != null)
                {
                    clntBuilder.Append(",DrawRate =" + model.C_DrawR);
                    logDesc.Append("把和局率修改为" + model.C_DrawR + "；");
                }
                if (!string.IsNullOrEmpty(model.C_State))
                {
                    clntBuilder.Append(",State ='" + model.C_State + "'");
                    logDesc.Append("把状态修改为" + model.C_State + "；");
                }
                clntBuilder.Append(" WHERE ClientID = '" + model.C_ID + "'");
                strSql = strSql.Replace("${updateClient}", clntBuilder.ToString());
                #endregion

                #region 组装修改会员限红sql
                authBuilder.Append("Update T_Authority set Priority='C'");
                if (model.C_MX_Z != null && model.C_MX_Z >= 0)
                {
                    authBuilder.Append(",Max_Z=" + model.C_MX_Z);
                    authBuilder.Append(",Max_X=" + model.C_MX_Z);
                    authBuilder.Append(",Max_H=" + model.C_MX_Z / 10);
                    authBuilder.Append(",Max_XD=" + model.C_MX_Z / 10);
                    authBuilder.Append(",Max_ZD=" + model.C_MX_Z / 10);
                }
                if (model.C_MN_Z != null && model.C_MN_Z >= 0)
                {
                    authBuilder.Append(",Min_Z=" + model.C_MN_Z);
                    authBuilder.Append(",Min_X=" + model.C_MN_Z);
                    authBuilder.Append(",Min_H=" + model.C_MN_Z);
                    authBuilder.Append(",Min_ZD=" + model.C_MN_Z);
                    authBuilder.Append(",Min_XD=" + model.C_MN_Z);
                }
                authBuilder.Append(" WHERE [Priority] = 'C' and ClientID = '" + model.C_ID + "' ");
                strSql = strSql.Replace("${upAuthority}", authBuilder.ToString());
                #endregion

                #region 组装修改会员赔率及显示隐藏洗码
                clntExBuilder.Append("update T_ClientEx set ");
                clntExBuilder.Append(" ClientID ='" + model.C_ID + "'");
                if (model.C_HdShow != null)
                {
                    clntExBuilder.Append(",ClientDataShow=" + ((model.C_HdShow == true) ? 2 : 1));
                }
                if (model.C_ODF != null)
                {
                    clntExBuilder.Append(",Odds_Fu_Client=" + model.C_ODF);
                }
                if (model.C_ODH != null)
                {
                    clntExBuilder.Append(",Odds_H_Client=" + model.C_ODH);
                }
                if (model.C_ODHe != null)
                {
                    clntExBuilder.Append(",Odds_He_Client=" + model.C_ODHe);
                }
                if (model.C_ODL != null)
                {
                    clntExBuilder.Append(",Odds_Long_Client=" + model.C_ODL);
                }
                if (model.C_ODX != null)
                {
                    clntExBuilder.Append(",Odds_X_Client=" + model.C_ODX);
                }
                if (model.C_ODXD != null)
                {
                    clntExBuilder.Append(",Odds_XD_Client=" + model.C_ODXD);
                }
                if (model.C_ODZ != null)
                {
                    clntExBuilder.Append(",Odds_Z_Client=" + model.C_ODZ);
                }
                if (model.C_ODZD != null)
                {
                    clntExBuilder.Append(",Odds_ZD_Client=" + model.C_ODZD);
                }
                clntExBuilder.Append(" where ClientID ='" + model.C_ID + "'");

                strSql = strSql.Replace("${upClientEx}", clntExBuilder.ToString());
                #endregion

                strSql = strSql.Replace("##'", "");
                strSql = strSql.Replace("##\"", "");
                strSql = strSql.Replace("'##", "");
                strSql = strSql.Replace("\"##", "");

                Db.Context_SqlServer.FromSql(strSql).ToDataSet();
                opLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                opLog.LogType = "修改会员";
                opLog.OpID = head.LoginID;
                opLog.LogInfo = logDesc.ToString();
                if (Db.Context_SqlServer.Insert<T_OperationLog>(opLog) <= 0)
                {
                    Common.LogHelper.WriteErrLog(typeof(AgentListDAL), "插入" + head.Account + "修改会员日志失败");
                }
                error.ErrMsg = "修改会员成功";
                error.ErrNo = "0000";
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 修改指定会员密码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateClntPwd(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.C_ID) || string.IsNullOrEmpty(model.C_Pwd) || string.IsNullOrEmpty(head.LoginID))
                {
                    error.ErrMsg = "参数不完整，不能修改会员密码";
                    return false;
                }
                if (CommonDAL.IsH5Clnt(model.C_ID))
                {
                    string h5Mger = Db.Context_SqlServer.FromSql("select [value] from T_Cfg where Name ='H5_ManageID'").ToScalar<string>();
                    if (!string.Equals(h5Mger.Trim(), head.LoginID.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        error.ErrMsg = "当前登录代理不能修改H5会员的密码！";
                        return false;
                    }
                }
                string strSql = SqlTemplateCommon.GetSql("A_SaveClientModifyPassword");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_SaveClientModifyPassword数据模板，请联系管理员";
                    return false;
                }
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                strSql = strSql.Replace("${Pwd}", model.C_Pwd);
                Db.Context_SqlServer.FromSql(strSql).ToDataSet();
                T_OperationLog opLog = new T_OperationLog();
                opLog.LogID = Guid.NewGuid().ToString().Replace("-", "");
                opLog.LogInfo = DateTime.Now.ToString() + "代理" + head.Account + "修改了下级代理" + model.C_UserID + "的登录密码";
                opLog.LogTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                opLog.OpID = head.LoginID;
                opLog.LogType = "修改代理登录密码";
                if (Db.Context_SqlServer.Insert<T_OperationLog>(opLog) <= 0)
                {
                    Common.LogHelper.WriteErrLog(typeof(AgentListDAL), "插入" + head.Account + "修改会员密码日志失败");
                }
                error.ErrMsg = "会员密码修改成功";
                error.ErrNo = "0000";
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 会员上下分
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ClientPoint(ClientSearc model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (string.IsNullOrEmpty(model.C_ID) || model.C_IsAdd == null || model.C_Point == null || string.IsNullOrEmpty(head.LoginID) || string.IsNullOrEmpty(model.C_AID) || string.IsNullOrEmpty(model.C_UserID))
                {
                    error.ErrMsg = "参数不完整";
                    return false;
                }
                bool isH5 = false;
                string h5MgrId = "";
                if (CommonDAL.IsH5Clnt(model.C_ID))
                {
                    isH5 = true;
                    h5MgrId = CommonDAL.GetH5MgrID();
                    if (model.C_IsAdd == true && !string.Equals(h5MgrId.Trim(), head.LoginID.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        error.ErrMsg = "当前代理不能对H5会员下分";
                        return false;
                    }
                }
                if (model.C_Point <= 0)
                {
                    error.ErrMsg = "上下分点数不在正确范围";
                    return false;
                }
                string strSql = SqlTemplateCommon.GetSql("A_SaveClientPoint");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_SaveClientPoint数据模板，请联系管理员";
                    return false;
                }
                string ownId = (model.C_LevelPoint != null && model.C_LevelPoint == "1") ? model.C_AID : head.LoginID;
                ownId = (isH5 && model.C_IsAdd == true) ? CommonDAL.GetH5LenderID() : ownId;//如果是H5会员下分则设置收分代理ID为分源代理ID
                strSql = strSql.Replace("${AgentID}", ownId);
                strSql = strSql.Replace("${CreateID}", head.LoginID);
                strSql = strSql.Replace("${Point}", model.C_Point.ToString());//上下分点数
                strSql = strSql.Replace("${IsAdd}", model.C_IsAdd.ToString().ToUpper());//标记上下分 true=下分 false=上分
                strSql = strSql.Replace("${ClientID}", model.C_ID);//登录代理ID
                strSql = strSql.Replace("${IP}", head.Ip);
                strSql = strSql.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                strSql = strSql.Replace("${IPLocal}", Common.CommonHelper.ipToAddr(head.Ip));
                string opType = "", memo = "";
                if (model.C_LevelPoint != null && model.C_LevelPoint == "1")
                {
                    memo = head.Account + "对会员" + model.C_UserID + "进行上级代理" + ((model.C_IsAdd == true) ? "下分" : "上分");
                    opType = (model.C_IsAdd == true) ? "会员上级代理下分" : "会员上级代理下分";
                }
                else
                {
                    memo = head.Account + "对会员" + model.C_UserID + "进行登录代理" + ((model.C_IsAdd == true) ? "下分" : "上分");
                    opType = (model.C_IsAdd == true) ? "会员登录代理下分" : "会员登录代理下分";
                }
                if (isH5 && model.C_IsAdd == true)
                {
                    memo = head.Account + "对H5会员" + model.C_UserID + "下分";
                    opType = "H5会员下分";
                }
                strSql = strSql.Replace("${OpType}", opType);//登录代理ID
                strSql = strSql.Replace("${Memo}", memo);
                Db.Context_SqlServer.FromSql(strSql).ToDataSet();
                error.ErrMsg = model.C_IsAdd == true ? "会员下分成功" : "会员上分成功";
                error.ErrNo = "0000";
                return true;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
        /// <summary>
        /// 结算会员洗码费
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SettleWashF(ClientSearc model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            DbTrans trans = null;
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("SetWashFlag");
                string opSql = Common.SqlTemplateCommon.GetSql("SettleWashF");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(opSql))
                {
                    error.ErrMsg = "服务端没有读取到SetWashFlag数据模板，请联系管理员";
                    return false;
                }
                string billID = Guid.NewGuid().ToString().Replace("-", "");
                if (string.IsNullOrEmpty(model.C_ID) || string.IsNullOrEmpty(model.StartDate) || string.IsNullOrEmpty(model.EndDate))
                {
                    error.ErrMsg = "没有接收到正确的请求参数";
                    return false;
                }
                strSql = strSql.Replace("${BillID}", billID);
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                strSql = strSql.Replace("${SettleDate}", model.StartDate);
                strSql = strSql.Replace("${EndDate}", model.StartDate);

                opSql = opSql.Replace("${CreateID}", head.LoginID);
                opSql = opSql.Replace("${ClientID}", model.C_ID);
                opSql = opSql.Replace("${BillID}", billID);
                opSql = opSql.Replace("${IP}", head.Ip);
                opSql = opSql.Replace("${Address}", Common.CommonHelper.ipToAddr(head.Ip));
                opSql = opSql.Replace("${Operator}", head.Account);
                opSql = opSql.Replace("${StartDate}", model.StartDate);
                opSql = opSql.Replace("${EndDate}", model.EndDate);

                trans = Db.Context_SqlServer.BeginTransaction();
                SettleModel settle = trans.FromSql(strSql).ToFirst<SettleModel>();
                if(settle != null)
                {
                    opSql = opSql.Replace("${Counts}", settle.C_Counts.ToString());
                    opSql = opSql.Replace("${Point}", ((int)settle.C_Amount).ToString());
                    opSql = opSql.Replace("${WashRate}", settle.C_WashR);
                    opSql = opSql.Replace("${WashSum}", settle.C_WashS.ToString());
                    error.ErrMsg = "洗码费结算成功";

                    trans.FromSql(opSql).ToDataSet();
                    trans.Commit();
                    return true;
                }
                else
                {
                    error.ErrMsg = "没有可结算的洗码费";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClientListDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                trans.Rollback();
                return false;
            }
            finally
            {
                if (trans != null)
                {
                    trans.Close();
                    trans.Dispose();
                }
            }
        }
    }
}
