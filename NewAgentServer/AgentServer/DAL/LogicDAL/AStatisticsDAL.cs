using Common.ORMTool;
using Dos.Common;
using Model.Comunication;
using Model.LogicModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LogicDAL
{
    public class AStatisticsDAL
    {
        /// <summary>
        /// 获取代理统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetAStatistics(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if(model == null)
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetAgentTotalLoopPage");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetACountCount_New");
                string sumSql = Common.SqlTemplateCommon.GetSql("A_GetAgentTotalDataNew");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql) || string.IsNullOrEmpty(sumSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAgentTotalLoopPage数据模板，请联系管理员";
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
                        whereSql += " and GameType = '" + GameType.Type_L + "' ";
                        break;
                    case "百家乐":
                        whereSql += " and GameType = '" + GameType.Type_B + "' ";
                        break;
                }
                strSql = strSql.Replace("${InWhereSql}", whereSql);
                countSql = countSql.Replace("${InWhereSql}", whereSql);
                sumSql = sumSql.Replace("${InWhereSql}", whereSql);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string msg;
                if (!string.IsNullOrEmpty(model.A_UserID))
                {
                    aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.A_UserID, out msg);
                    if (aList != null && aList.Count > 0)
                    {
                        model.A_ID = aList.Find(a => a.A_UserID == model.A_UserID).A_ID;
                    }
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
                strSql = strSql.Replace("${ParentID}", model.A_ID);
                countSql = countSql.Replace("${ParentID}", model.A_ID);
                sumSql = sumSql.Replace("${ParentID}", model.A_ID);
                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable()),
                    AgentNav = JSON.ToJSON(aList),
                    AgentSum = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(sumSql).ToDataTable()),
                    Count = Db.Context_SqlServer.FromSql(countSql).ToScalar<int>()
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }

        }
        /// <summary>
        /// 获取代理每日统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetADayStatistics(StatisticsModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if (model == null)
            {
                error.ErrMsg = "参数不完整";
                return null;
            }
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetAgentDayDataPage");
                string countSql = Common.SqlTemplateCommon.GetSql("A_GetADayCount");
                if (string.IsNullOrEmpty(strSql) || string.IsNullOrEmpty(countSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAgentTotalLoopPage数据模板，请联系管理员";
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
                        whereSql += " and GameType = '" + GameType.Type_L + "' ";
                        break;
                    case "百家乐":
                        whereSql += " and GameType = '" + GameType.Type_B + "' ";
                        break;
                }
                strSql = strSql.Replace("${InWhereSql}", whereSql);
                countSql = countSql.Replace("${InWhereSql}", whereSql);
                List<AgentSearchModel> aList = new List<AgentSearchModel>();
                string msg;
                aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);               
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                strSql = strSql.Replace("${ParentID}", model.A_ID);
                countSql = countSql.Replace("${ParentID}", model.A_ID);
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
                Common.LogHelper.WriteLog(typeof(AStatisticsDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        
    }
}
