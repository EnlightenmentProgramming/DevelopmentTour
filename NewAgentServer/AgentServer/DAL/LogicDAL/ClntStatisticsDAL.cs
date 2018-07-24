using Common.ORMTool;
using Dos.Common;
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
    public class ClntStatisticsDAL
    {
        /// <summary>
        /// 获取会员统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetClntStatistics(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if(model== null || string.IsNullOrEmpty(head.LoginID))
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetClientTotalLoopPage_New");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetCCountCount_New");
                string sumSql = Common.SqlTemplateCommon.GetSql("A_GetClientTotalData_New");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql) || string.IsNullOrEmpty(sumSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetClientTotalLoopPage_New数据模板，请联系管理员";
                    return null;
                }
                string today = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
                string whereSql = string.Empty;
                strSql = strSql.Replace("${PageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${CurePage}", (model.PageSize ?? 1).ToString());
                strSql = strSql.Replace("${StartDate}", model.StartDate ?? today);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? today);
                countSql = countSql.Replace("${StartDate}", model.StartDate ?? today);
                countSql = countSql.Replace("${EndDate}", model.EndDate ?? today);
                sumSql = sumSql.Replace("${StartDate}", model.StartDate ?? today);
                sumSql = sumSql.Replace("${EndDate}", model.EndDate ?? today);
                switch (model.GameT.Trim())
                {
                    case "龙虎":
                        whereSql += " and  vdq.GameType = '" + GameType.Type_L + "' ";
                        break;
                    case "百家乐":
                        whereSql += " and  vdq.GameType = '" + GameType.Type_B + "' ";
                        break;
                }
                strSql = strSql.Replace("${WhereSql}", whereSql);
                countSql = countSql.Replace("${WhereSql}", whereSql);
                sumSql = sumSql.Replace("${WhereSql}", whereSql);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string msg;
                if (!string.IsNullOrEmpty(model.C_UserID))
                {
                    T_Client clnt = ClientListDAL.First(c => c.LogName == model.C_UserID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", clnt != null ? clnt.AgentID : "", out msg);
                    model.A_ID = clnt != null ? clnt.ClientID : "";
                }
                else
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                }
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                countSql = countSql.Replace("${AgentID}", model.A_ID);
                sumSql = sumSql.Replace("${AgentID}", model.A_ID);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    ClientSum = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(sumSql).ToDataTable()),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>()
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取会员每日明细
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetClntDayStatistics(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if (model == null || string.IsNullOrEmpty(head.LoginID))
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetClientDayDataPage");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetCDayCount");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetClientDayDataPage数据模板，请联系管理员";
                    return null;
                }
                string today = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
                string whereSql = string.Empty;
                strSql = strSql.Replace("${PageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${CurePage}", (model.PageSize ?? 1).ToString());
                strSql = strSql.Replace("${StartDate}", model.StartDate ?? today);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? today);
                countSql = countSql.Replace("${StartDate}", model.StartDate ?? today);
                countSql = countSql.Replace("${EndDate}", model.EndDate ?? today);
                switch (model.GameT.Trim())
                {
                    case "龙虎":
                        whereSql += " and  vdq.GameType = '" + GameType.Type_L + "' ";
                        break;
                    case "百家乐":
                        whereSql += " and  vdq.GameType = '" + GameType.Type_B + "' ";
                        break;
                }
                strSql = strSql.Replace("${WhereSql}", whereSql);
                countSql = countSql.Replace("${WhereSql}", whereSql);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string msg;                
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                countSql = countSql.Replace("${AgentID}", model.A_ID);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>()
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取会员注单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetClntBetBills(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if (model == null || string.IsNullOrEmpty(head.LoginID))
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetAllT_ClientDetailsPage_New");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetBillCount_New");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAllT_ClientDetailsPage_New数据模板，请联系管理员";
                    return null;
                }
                string today = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
                string whereSql = string.Empty;
                strSql = strSql.Replace("${PageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${CurePage}", (model.PageSize ?? 1).ToString());
                strSql = strSql.Replace("${StartDate}", model.StartDate ?? today);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? today);
                countSql = countSql.Replace("${StartDate}", model.StartDate ?? today);
                countSql = countSql.Replace("${EndDate}", model.EndDate ?? today);
                switch (model.GameT.Trim())
                {
                    case "龙虎":
                        whereSql += " and  a.GameType = '" + GameType.Type_L + "' ";
                        break;
                    case "百家乐":
                        whereSql += " and  a.GameType = '" + GameType.Type_B + "' ";
                        break;
                }
                strSql = strSql.Replace("${WhereSql}", whereSql);
                countSql = countSql.Replace("${WhereSql}", whereSql);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string msg;
                T_Client clnt = new T_Client();
                if (!string.IsNullOrEmpty(model.C_UserID))
                {
                    clnt = ClientListDAL.First(c => c.LogName == model.C_UserID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", clnt != null ? clnt.AgentID : "", out msg);
                    if(clnt == null)
                    {
                        error.ErrMsg = "没有找到此会员";
                        return null;
                    }
                    model.C_ID = clnt.ClientID;
                }
                else
                {
                    clnt = ClientListDAL.First(c => c.ClientID == model.C_ID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", clnt.AgentID, out msg);
                }
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                countSql = countSql.Replace("${ClientID}", model.C_ID);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>()
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取上下分统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetPointDetail(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if (model == null || string.IsNullOrEmpty(head.LoginID))
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string today = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
                string whereSql = string.Empty;
                string strSql = Common.SqlTemplateCommon.GetSql("A_QueryPointPage");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetPointCount");
                string sumSql = Common.SqlTemplateCommon.GetSql("A_QueryPoint");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql) || string.IsNullOrEmpty(sumSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAllT_ClientDetailsPage_New数据模板，请联系管理员";
                    return null;
                }
                strSql = strSql.Replace("${BeginDate}", model.StartDate ?? today);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? today);
                countSql = countSql.Replace("${BeginDate}", model.StartDate ?? today);
                countSql = countSql.Replace("${EndDate}", model.EndDate ?? today);
                sumSql = sumSql.Replace("${BeginDate}", model.StartDate ?? today);
                sumSql = sumSql.Replace("${EndDate}", model.EndDate ?? today);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                T_Client clnt = new T_Client();
                string idString = "";
                string msg = "没有权限获取数据";
                string strRange = "";
                if (!string.IsNullOrEmpty(model.C_ID))//按会员ID过滤上下分数据
                {
                    clnt = ClientListDAL.First(c => c.ClientID == model.C_ID);//按会员登录账号过滤上下分数据
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", clnt.AgentID, out msg);
                    whereSql += " and (SourceID = '" + model.C_ID + "' or TargetID ='" + model.C_ID + "')";
                }
                else if (!string.IsNullOrEmpty(model.C_UserID))
                {
                    if (model.C_UserID.Trim().Length <= 6)
                    {
                        aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.C_UserID, out msg);
                    }
                    else
                    {
                        clnt = ClientListDAL.First(c => c.LogName == model.C_UserID);
                        aList = CommonDAL.GetAgentTree(head.LoginID, "id", clnt.AgentID, out msg);
                    }
                    whereSql += " and (TargetLogName='" + model.C_UserID + "' or SourceLogName='" + model.C_UserID + "')";
                }
                else if (!string.IsNullOrEmpty(model.A_ID))//按代理ID过滤上下分数据
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                    if (!string.IsNullOrEmpty(model.PointRange))
                    {
                        switch (model.PointRange)
                        {
                            case "OwnA"://直属代理
                                idString = CommonDAL.GetClntOrAId("id", model.A_ID, "A");
                                if (string.IsNullOrEmpty(idString))
                                {
                                    error.ErrMsg = "没有找到此用户";
                                    return null;
                                }
                                else
                                {
                                    whereSql += " and (SourceID   in (" + idString + ") or TargetID in (" + idString + ")) ";
                                }
                                break;
                            case "OwnC"://直属会员
                                idString = CommonDAL.GetClntOrAId("id", model.A_ID, "C");
                                if (string.IsNullOrEmpty(idString))
                                {
                                    error.ErrMsg = "没有找到此用户";
                                    return null;
                                }
                                else
                                {
                                    whereSql += " and (SourceID   in (" + idString + ") or TargetID in (" + idString + ")) ";
                                }
                                break;
                            case "All"://代理整个分支
                                idString = CommonDAL.GetAid(model.A_ID,"id");
                                if (string.IsNullOrEmpty(idString))
                                {
                                    error.ErrMsg = "没有找到此用户";
                                    return null;
                                }
                                else
                                {
                                    whereSql += " and (SourceID   in (" + idString + ") or TargetID in (" + idString + ")) ";
                                }
                                break;
                            default://代理本身
                                whereSql += " and (SourceID = '" + model.A_ID + "' or TargetID ='" + model.A_ID + "')";//默认情况下查询代理本身的上下分数据
                                break;
                        }
                    }
                    else
                    {
                        whereSql += " and (SourceID = '" + model.A_ID + "' or TargetID ='" + model.A_ID + "')";//默认情况下查询代理本身的上下分数据
                    }
                    
                }
                else if(!string.IsNullOrEmpty(model.A_UserID))//按代理登录账号过滤上下分数据
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.A_UserID, out msg);
                    if (!string.IsNullOrEmpty(model.PointRange))
                    {
                        switch (model.PointRange)
                        {
                            case "OwnA"://直属代理
                                idString = CommonDAL.GetClntOrAId("name", model.A_UserID, "A");
                                if (string.IsNullOrEmpty(idString))
                                {
                                    error.ErrMsg = "没有找到此用户";
                                    return null;
                                }
                                else
                                {
                                    whereSql += " and (SourceID   in (" + idString + ") or TargetID in (" + idString + ")) ";
                                }
                                break;
                            case "OwnC"://直属会员
                                idString = CommonDAL.GetClntOrAId("name", model.A_UserID, "C");
                                if (string.IsNullOrEmpty(idString))
                                {
                                    error.ErrMsg = "没有找到此用户";
                                    return null;
                                }
                                else
                                {
                                    whereSql += " and (SourceID   in (" + idString + ") or TargetID in (" + idString + ")) ";
                                }
                                break;
                            case "All"://代理整个分支
                                idString = CommonDAL.GetAid(model.A_UserID,"name");
                                if (string.IsNullOrEmpty(idString))
                                {
                                    error.ErrMsg = "没有找到此用户";
                                    return null;
                                }
                                else
                                {
                                    whereSql += " and (SourceID   in (" + idString + ") or TargetID in (" + idString + ")) ";
                                }
                                break;
                            default://代理本身
                                whereSql += " and (SourceID = '" + model.A_ID + "' or TargetID ='" + model.A_ID + "')";//默认情况下查询代理本身的上下分数据
                                break;
                        }
                    }
                    else
                    {
                        whereSql += " and (SourceID = '" + model.A_ID + "' or TargetID ='" + model.A_ID + "')";//默认情况下查询代理本身的上下分数据
                    }
                }
                else//按登录代理ID过滤上下分数据
                {
                    strRange += " and (SourceID = '" + head.LoginID + "' or TargetID ='" + head.LoginID + "')";
                }
                if (model.IsAll == false)//是否要过滤掉上下分金额为0的上下分记录
                {
                    whereSql += " and Delta >0";
                }
                if (model.Minum != null)//是否需要以最小上下分金额进行过滤
                {
                    whereSql += " and Delta >" + model.Minum;
                }
                if(!string.IsNullOrEmpty(model.PointType))//BD ="上分" XD="下分" QL ="清零" QK ="清卡"
                {
                    whereSql += " and OperationType ='" + model.PointType + "'";
                }
                if(!string.IsNullOrEmpty(model.PointWay))
                {
                    string borrowID = Db.Context_SqlServer.FromSql("select value from T_Cfg where name ='H5_BorrowerID'").ToScalar<string>();
                    string lenderID = Db.Context_SqlServer.FromSql("select value from T_Cfg where name ='H5_LenderID'").ToScalar<string>();
                    switch(model.PointWay)
                    {
                        case "Third"://第三方上下分
                            whereSql += " and (SourceID = '" + borrowID + "' or SourceID = '" + lenderID + "')";
                            break;
                        case "Other"://非第三方上下分
                            whereSql += " and SourceID <> '" + borrowID + "' and SourceID <> '" + lenderID + "'";
                            break;
                    }
                }
                if (aList == null || aList.Count <= 0)//判断当前查询的用户是否在登录代理分支下
                {
                    error.ErrMsg = msg;
                    return null;
                }
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>(),
                    PointSum = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(sumSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取游戏结果数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetTableResult(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if (model == null || string.IsNullOrEmpty(head.LoginID))
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string today = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetAllGamesByTableIDPage");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetAllGamesByTableIDAndDate");
                string tableSql = Common.SqlTemplateCommon.GetSql("T_GetAllT_Tables");
                string juSql = Common.SqlTemplateCommon.GetSql("A_GetJu");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql) || string.IsNullOrEmpty(tableSql) || string.IsNullOrEmpty(juSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAllGamesByTableIDPage数据模板，请联系管理员";
                    return null;
                }
                strSql = strSql.Replace("${NDate}", model.StartDate ?? today);
                strSql = strSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${curePage}", (model.CurePage ?? 20).ToString());
                juSql = juSql.Replace("${NDate}", model.StartDate ?? today);
                List<TableModel> tableList = Db.Context_SqlServer.FromSql(tableSql).ToList<TableModel>();
                List<TableModel> juList = new List<TableModel>();
                if (tableList != null && tableList.Count > 0 && string.IsNullOrEmpty(model.T_ID))
                {
                    model.T_ID = tableList[0].T_Ju;
                }
                juList = Db.Context_SqlServer.FromSql(juSql).ToList<TableModel>();
                if (juList != null && juList.Count > 0 && string.IsNullOrEmpty(model.T_Ju))
                {
                    model.T_Ju = juList[0].T_Ju;
                }
                juSql = juSql.Replace("${TableID}", model.T_ID);
                strSql = strSql.Replace("${TableID}", model.T_ID);
                strSql = strSql.Replace("${Ju}", model.T_Ju);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    TableJson = JSON.ToJSON(tableList),
                    JuJson = JSON.ToJSON(juList),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>()
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理/会员下会员推广统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string PromotionA_Clnt4A(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("PromotionA_Clnt4A");
                string aStrSql = "";
                if (model != null && !string.IsNullOrEmpty(model.C_UserID) && model.C_UserID.Length > 7)
                {
                    aStrSql = Common.SqlTemplateCommon.GetSql("Promotion_Clnt");
                }
                else
                {
                    aStrSql = Common.SqlTemplateCommon.GetSql("Promotion_Agent");
                }
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(aStrSql))
                {
                    error.ErrMsg = "服务端没有读取到PromotionA_Clnt4A数据模板，请联系管理员";
                    return null;
                }             

                strSql = strSql.Replace("${StartDate}", model.StartDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate);
                strSql = strSql.Replace("${pageSize}",(model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());

                List<AgentSearchModel> aList;
                string msg;
                if (model != null && !string.IsNullOrEmpty(model.C_UserID))
                {
                    if (model.C_UserID.Length <= 7)
                    {
                        aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.C_UserID, out msg);
                        model.A_ID = Db.Context_SqlServer.FromSql("select AgentID from T_Agent where logname ='" + model.C_UserID + "'").ToScalar<string>();
                    }
                    else
                    {
                        aList = CommonDAL.GetAgentTree(head.LoginID, model.C_UserID, out msg);
                        model.A_ID = Db.Context_SqlServer.FromSql("select ClientID from T_Client where logName ='" + model.C_UserID + "'").ToScalar<string>();
                    }
                }
                else
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                }
                if (aList == null || aList.Count <=0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                aStrSql = aStrSql.Replace("${AgentID}", model.A_ID);
                aStrSql = aStrSql.Replace("${StartDate}", model.StartDate);
                aStrSql = aStrSql.Replace("${EndDate}", model.EndDate);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    Promotion_A = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aStrSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理/会员下会员推广统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string PromotionA_AllClnt4A(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("Promotion_AllClnt4A");
                string aStrSql = "";
                if (model != null && !string.IsNullOrEmpty(model.C_UserID) && model.C_UserID.Length > 7)
                {
                    aStrSql = Common.SqlTemplateCommon.GetSql("Promotion_AllClnt");
                }
                else
                {
                    aStrSql = Common.SqlTemplateCommon.GetSql("Promotion_AllAgent");
                }
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(aStrSql))
                {
                    error.ErrMsg = "服务端没有读取到Promotion_AllClnt4A数据模板，请联系管理员";
                    return null;
                }

                strSql = strSql.Replace("${StartDate}", model.StartDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate);
                strSql = strSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());

                List<AgentSearchModel> aList;
                string msg;
                if (model != null && !string.IsNullOrEmpty(model.C_UserID))
                {
                    if (model.C_UserID.Length <= 7)
                    {
                        aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.C_UserID, out msg);
                        model.A_ID = Db.Context_SqlServer.FromSql("select AgentID from T_Agent where logname ='" + model.C_UserID + "'").ToScalar<string>();
                    }
                    else
                    {
                        aList = CommonDAL.GetAgentTree(head.LoginID, model.C_UserID, out msg);
                        model.A_ID = Db.Context_SqlServer.FromSql("select ClientID from T_Client where logName ='" + model.C_UserID + "'").ToScalar<string>();
                    }
                }
                else
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                }
                if (aList == null)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                aStrSql = aStrSql.Replace("${AgentID}", model.A_ID);
                aStrSql = aStrSql.Replace("${StartDate}", model.StartDate);
                aStrSql = aStrSql.Replace("${EndDate}", model.EndDate);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    Promotion_A = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aStrSql).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList)
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理下H5会员第三方上分合计
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string H5ClntPointSum(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string aSumSql = Common.SqlTemplateCommon.GetSql("BorrowAgent_Sum");
                string cSumSql = Common.SqlTemplateCommon.GetSql("H5ClntPoint_Sum");
                if (string.IsNullOrEmpty(aSumSql) || string.IsNullOrEmpty(cSumSql))
                {
                    error.ErrMsg = "服务端没有读取到BorrowAgent_Sum数据模板，请联系管理员";
                    return null;
                }
               
                aSumSql = aSumSql.Replace("${StartDate}", model.StartDate);
                aSumSql = aSumSql.Replace("${EndDate}", model.EndDate);
                cSumSql = cSumSql.Replace("${StartDate}", model.StartDate);
                cSumSql = cSumSql.Replace("${EndDate}", model.EndDate);
                cSumSql = cSumSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                cSumSql = cSumSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());
                aSumSql = aSumSql.Replace("${IsAgent}", "1");
                List<AgentSearchModel> aList;
                string msg;
                string whereSql = " ";
                string aId = "";
                if (model != null && !string.IsNullOrEmpty(model.A_UserID))
                {
                    aId = Db.Context_SqlServer.FromSql("select AgentID from T_Agent where LogName ='" + model.A_UserID + "'").ToScalar<string>();
                    model.A_ID = aId;
                }
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                int loginCount = Db.Context_SqlServer.FromSql("select COUNT(*) from T_Cfg where (Name='H5_ManageID' or Name='H5_BorrowerID') and Value ='" + head.LoginID + "'").ToScalar<int>();
                if (aList != null)
                {
                    int aCount = Db.Context_SqlServer.FromSql("select COUNT(*) from T_Cfg where (Name='H5_ManageID' or Name='H5_BorrowerID') and Value ='" + model.A_ID + "'").ToScalar<int>();
                    //判断当前需要查询代理是否是H5会员管理代理或者H5会员放分代理
                    if (aCount <= 0)//如果不是则过滤出指定代理的H5会员
                    {
                        whereSql += " and (b.AgentID = '" + model.A_ID + "' or b.ClientID = '" + model.A_ID + "')";
                    }
                }
                else
                {
                    if (aList == null && loginCount <= 0)
                    {
                        error.ErrMsg = msg;
                        return null;
                    }
                       
                }
                aSumSql = aSumSql.Replace("${AgentID}", model.A_ID);
                aSumSql = aSumSql.Replace("${WhereSql}", whereSql);
                cSumSql = cSumSql.Replace("${WhereSql}", whereSql);
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(cSumSql).ToDataTable()),
                    AgentPointSum = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aSumSql).ToDataTable())
                });

            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定H5会员第三方上分明细
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string H5ClntPointDetail(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string detailSql = Common.SqlTemplateCommon.GetSql("H5ClentPointDetail");
                string aSumSql = Common.SqlTemplateCommon.GetSql("BorrowAgent_Sum");
                if (string.IsNullOrEmpty(detailSql) || string.IsNullOrEmpty(aSumSql))
                {
                    error.ErrMsg = "服务端没有读取到H5ClentPointDetail数据模板，请联系管理员";
                    return null;
                }
               
                detailSql = detailSql.Replace("${StartDate}", model.StartDate);
                detailSql = detailSql.Replace("${EndDate}", model.EndDate);
                detailSql = detailSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                detailSql = detailSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());
                aSumSql = aSumSql.Replace("${StartDate}", model.StartDate);
                aSumSql = aSumSql.Replace("${EndDate}", model.EndDate);
                List<AgentSearchModel> aList;
                T_Client clnt;
                string msg;
                if (model != null && !string.IsNullOrEmpty(model.C_UserID))
                {
                    clnt = Db.Context_SqlServer.FromSql("select ClientID,AgentID from T_Client where LogName='" + model.C_UserID + "'").ToFirst<T_Client>();
                    model.C_ID = clnt.ClientID;
                }
                else
                {
                    clnt = Db.Context_SqlServer.FromSql("select ClientID,AgentID from T_Client where ClientID='" + model.C_ID + "'").ToFirst<T_Client>();
                }
                if (clnt == null)
                {
                   error.ErrMsg = "没有找到数据";
                    return null;
                }
                string whereSql = " and b.ClientID ='" + clnt.ClientID + "'";
                aSumSql = aSumSql.Replace("${WhereSql}", whereSql);
                aSumSql = aSumSql.Replace("${IsAgent}", "0");
                detailSql = detailSql.Replace("${ClientID}", model.C_ID);
                aSumSql = aSumSql.Replace("${AgentID}", model.C_ID);
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", clnt.AgentID, out msg);
                if (aList == null)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(detailSql).ToDataTable()),
                    AgentPointSum = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aSumSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取指定代理下所有H5会员第三方上分明细
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string H5ClntPointDetail_A(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string aSumSql = Common.SqlTemplateCommon.GetSql("H5ClentPointDetail_A");
                string cSumSql = Common.SqlTemplateCommon.GetSql("BorrowAgent_Sum");
                if (string.IsNullOrEmpty(aSumSql) || string.IsNullOrEmpty(cSumSql))
                {
                    error.ErrMsg = "服务端没有读取到H5ClentPointDetail数据模板，请联系管理员";
                    return null;
                }
               
                aSumSql = aSumSql.Replace("${StartDate}", model.StartDate);
                aSumSql = aSumSql.Replace("${EndDate}", model.EndDate);
                aSumSql = aSumSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                aSumSql = aSumSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());
                cSumSql = cSumSql.Replace("${StartDate}", model.StartDate);
                cSumSql = cSumSql.Replace("${EndDate}", model.EndDate);
                cSumSql = cSumSql.Replace("${IsAgent}", "1");
                List<AgentSearchModel> aList;
                string whereSql = " ";
                string aId = "";
                string msg;
                if (model != null && !string.IsNullOrEmpty(model.A_UserID))
                {                     
                    if (model.A_UserID.Length <= 7)
                    {
                        aId = Db.Context_SqlServer.FromSql("select AgentID from T_Agent where LogName ='" + model.A_UserID + "'").ToScalar<string>();
                        model.A_ID = aId;
                    }
                    else
                    {
                       StatisticsModel modelClnt = new StatisticsModel();
                        modelClnt.C_UserID = model.A_UserID;
                        modelClnt.StartDate = model.StartDate;
                        modelClnt.EndDate = model.EndDate;
                        modelClnt.PageSize = model.PageSize;
                        modelClnt.CurePage = model.CurePage;
                        return H5ClntPointDetail(modelClnt, head, out error);
                    }
                }
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                int loginCount = Db.Context_SqlServer.FromSql("select COUNT(*) from T_Cfg where (Name='H5_ManageID' or Name='H5_BorrowerID') and Value ='" + head.LoginID + "'").ToScalar<int>();
                if (aList != null)
                {
                    int aCount = Db.Context_SqlServer.FromSql("select COUNT(*) from T_Cfg where (Name='H5_ManageID' or Name='H5_BorrowerID') and Value ='" + model.A_ID + "'").ToScalar<int>();
                    //判断当前需要查询代理是否是H5会员管理代理或者H5会员放分代理
                    if (aCount <= 0)//如果不是则过滤出指定代理的H5会员
                    {
                        whereSql += " and (b.AgentID = '" + model.A_ID + "' or b.ClientID = '" + model.A_ID + "')";
                    }
                }
                else
                {
                    if (aList == null && loginCount <= 0)
                    {
                        error.ErrMsg = msg;
                        return null;
                    }
                        
                }
                cSumSql = cSumSql.Replace("${AgentID}", model.A_ID);
                aSumSql = aSumSql.Replace("${WhereSql}", whereSql);
                cSumSql = cSumSql.Replace("${WhereSql}", whereSql);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aSumSql).ToDataTable()),
                    AgentPointSum = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(cSumSql).ToDataTable())
                });

            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 查询指定代理自己及它的直属代理及它的直属会员红包发放合计
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string RedEnvelopeSum(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string aSumSql = Common.SqlTemplateCommon.GetSql("RedEnvelopeSum");
                if (string.IsNullOrEmpty(aSumSql))
                {
                    error.ErrMsg = "服务端没有读取到RedEnvelopeSum数据模板，请联系管理员";
                    return null;
                }
                aSumSql = aSumSql.Replace("${StartDate}", model.StartDate);
                aSumSql = aSumSql.Replace("${EndDate}", model.EndDate);
                aSumSql = aSumSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                aSumSql = aSumSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());
                List<AgentSearchModel> aList;
                string aId = "";
                string msg;
                if (model != null && !string.IsNullOrEmpty(model.A_UserID))
                {
                    if (model.A_UserID.Length <= 7)
                    {
                        aId = Db.Context_SqlServer.FromSql("select AgentID from T_Agent where LogName ='" + model.A_UserID + "'").ToScalar<string>();
                        aSumSql = aSumSql.Replace("${AgentID}", aId);
                    }
                    else
                    {
                        T_Client tmpClnt = Db.Context_SqlServer.FromSql("select AgentID,ClientID from T_Client where LogName ='" + model.A_UserID + "'").ToFirst<T_Client>();
                        aId = tmpClnt.AgentID;
                        aSumSql = aSumSql.Replace("${AgentID}", tmpClnt.ClientID);
                    }
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", aId, out msg);
                }
                else
                {
                    aSumSql = aSumSql.Replace("${AgentID}", model.A_ID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                }
                if (aList == null)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aSumSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 查询指定代理或会员红包发放明细
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loginID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string RedEnvelopeDetail(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string aSumSql = Common.SqlTemplateCommon.GetSql("RedEnvelopeDetail");
                if (string.IsNullOrEmpty(aSumSql))
                {
                    error.ErrMsg = "服务端没有读取到RedEnvelopeSum数据模板，请联系管理员";
                    return null;
                }

                aSumSql = aSumSql.Replace("${StartDate}", model.StartDate);
                aSumSql = aSumSql.Replace("${EndDate}", model.EndDate);
                aSumSql = aSumSql.Replace("${pageSize}", (model.PageSize ?? 20).ToString());
                aSumSql = aSumSql.Replace("${curePage}", (model.CurePage ?? 1).ToString());
                List<AgentSearchModel> aList;
                string aId = "";
                string msg;
                if (model != null && !string.IsNullOrEmpty(model.A_UserID))
                {
                    if (model.A_UserID.Length <= 7)
                    {
                        aId = Db.Context_SqlServer.FromSql("select AgentID from T_Agent where LogName ='" + model.A_UserID + "'").ToScalar<string>();
                        aSumSql = aSumSql.Replace("${OpID}", aId);
                    }
                    else
                    {
                        T_Client tmpClnt = Db.Context_SqlServer.FromSql("select AgentID,ClientID from T_Client where LogName ='" + model.A_UserID + "'").ToFirst<T_Client>();
                        aId = tmpClnt.AgentID;
                        aSumSql = aSumSql.Replace("${OpID}", tmpClnt.ClientID);
                    }
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", aId, out msg);
                }
                else
                {
                    aSumSql = aSumSql.Replace("${OpID}", model.A_ID);
                    aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                }
                string whereSql = "";
                if (!string.IsNullOrEmpty(model.GameT))
                {
                    whereSql += " and cFlag in (";
                    List<string> flags = new List<string>();
                    if (model.GameT.IndexOf("REG") != -1)
                    {
                        flags.Add("'REG'");
                    }
                    if (model.GameT.IndexOf("PICKUP") != -1)
                    {
                        flags.Add("'PICKUP'");
                    }
                    if (model.GameT.IndexOf("ABO") != -1)
                    {
                        flags.Add("'ABO'");
                    }
                    if (model.GameT.IndexOf("YES") != -1)
                    {
                        flags.Add("'YES'");
                    }
                    if (model.GameT.IndexOf("UPD") != -1)
                    {
                        flags.Add("'UPD'");
                    }
                    if (flags != null && flags.Count > 0)
                    {
                        whereSql += string.Join(",", flags);
                        whereSql += ")";
                    }
                    else
                    {
                        whereSql = "";
                    }
                }
                aSumSql = aSumSql.Replace("${WhereSql}", whereSql);
                if (aList == null)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(aSumSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取结算记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetSettleAccounts(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("GetAccounts");
                string sumSql = Common.SqlTemplateCommon.GetSql("GetAccountCount");
                string countSql = Common.SqlTemplateCommon.GetSql("GetAccountTotal");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(sumSql) || string.IsNullOrEmpty(countSql))
                {
                    error.ErrMsg = "服务端没有读取到GetAccounts数据模板，请联系管理员";
                    return null;
                }
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string msg = "";
                string whereSql = "";
                aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.A_UserID, out msg);
                if (aList == null || aList.Count < 1)
                {
                    error.ErrMsg = msg;
                    return null;
                }

                strSql = strSql.Replace("${curePage}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${pageSize}", (model.CurePage ?? 1).ToString());
                strSql = strSql.Replace("${Operator}", model.A_UserID);
                strSql = strSql.Replace("${StartDate}", model.StartDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate);

                sumSql = sumSql.Replace("${Operator}", model.A_UserID);
                sumSql = sumSql.Replace("${StartDate}", model.StartDate);
                sumSql = sumSql.Replace("${EndDate}", model.EndDate);

                countSql = countSql.Replace("${Operator}", model.A_UserID);
                countSql = countSql.Replace("${StartDate}", model.StartDate);
                countSql = countSql.Replace("${EndDate}", model.EndDate);

                if (!string.IsNullOrEmpty(model.C_UserID))//按照结算源登录名称过滤
                {
                    whereSql += " and AccountSource = '" + model.C_UserID + "'";
                }
                if (!string.IsNullOrEmpty(model.GameT))//按结算类型过滤  "结算洗码" "结算抽水"
                {
                    whereSql += " and  AcType like '%" + model.GameT + "%'";
                }
                strSql = strSql.Replace("${WhereSql}", whereSql);
                sumSql = sumSql.Replace("${WhereSql}", whereSql);
                countSql = countSql.Replace("${WhereSql}", whereSql);

                error.ErrNo = "0000";
                error.ErrMsg = "获取数据成功";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    SumJson = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(sumSql).ToDataTable()),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>()
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取会员的洗码费及抽水费结算情况
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetOddsWashF4Clnt(StatisticsModel model, HeadMessage head, out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("GetWashFandOdds");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到GetAccounts数据模板，请联系管理员";
                    return null;
                }
                if (string.IsNullOrEmpty(model.C_ID))
                {
                    error.ErrMsg = "没有接收到请求参数";
                    return null;
                }
                strSql = strSql.Replace("${ClientID}", model.C_ID);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取交易记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetTransactions(LogModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("TransactionLogPage");
                string sumSql = Common.SqlTemplateCommon.GetSql("TransactionSum");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(sumSql))
                {
                    error.ErrMsg = "服务端没有读取到TransactionLogPage数据模板，请联系管理员";
                    return null;
                }
                if (string.IsNullOrEmpty(model.L_Operator))
                {
                    error.ErrMsg = "没有接收到完整的请求参数";
                    return null;
                }
                string nowDate = string.Format("yyyy-MM-dd HH:mm:ss", DateTime.Now);
                strSql = strSql.Replace("${Operator}", model.L_Operator);
                strSql = strSql.Replace("${StartDate}", model.StartDate ?? nowDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? nowDate);
                strSql = strSql.Replace("${PageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${CurePage}", (model.CurePage ?? 20).ToString());

                sumSql = sumSql.Replace("${Operator}", model.L_Operator);
                sumSql = sumSql.Replace("${StartDate}", model.StartDate ?? nowDate);
                sumSql = sumSql.Replace("${EndDate}", model.EndDate ?? nowDate);

                string whereSql = "";
                if (!string.IsNullOrEmpty(model.L_SourceUser))
                {
                    whereSql += " and SourceUser = '" + model.L_SourceUser + "'";
                }
                string msg;
                List<AgentSearchModel> alist = CommonDAL.GetAgentTree(head.LoginID, "name", model.L_Operator, out msg);
                if (alist == null || alist.Count < 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    SumJson = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(sumSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(ClntStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
    }
}
