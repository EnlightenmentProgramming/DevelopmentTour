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
    public class LogDAL
    {
        /// <summary>
        /// 获取登陆日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetLoginLog(LogModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            if(string.IsNullOrEmpty(model.L_PAgent))
            {
                error.ErrMsg = "没有接收到完整的请求参数";
                return null;
            }
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("LoginLog");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到LoginLog数据模板，请联系管理员";
                    return null;
                }
                string nowDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                strSql = strSql.Replace("${PageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${CurePage}", (model.CurePage ?? 20).ToString());
                strSql = strSql.Replace("${StartDate}", model.StartDate ?? nowDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? nowDate);
                string msg;
                List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "name", model.L_User, out msg);
                if (aList == null || aList.Count <= 0)
                {
                    error.ErrMsg = msg;
                    return null;
                }
                string whereSql = "";
                if (!string.IsNullOrEmpty(model.L_User))
                {
                    whereSql += " and (LoginUser ='" + model.L_User + "' or ParentAgent = '" + model.L_User + "')";
                }
                if (!string.IsNullOrEmpty(model.L_PAgent))
                {
                    whereSql += " and (ParentAgent ='" + model.L_PAgent + "%' or ParentAgent = '" + head.LoginID + "')";
                }
                strSql = strSql.Replace("${WhereSql}", whereSql);

                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LogDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetOperationLog(LogModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                string strSql = Common.SqlTemplateCommon.GetSql("OperationLog");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到OperationLog数据模板，请联系管理员";
                    return null;
                }
                string nowDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                strSql = strSql.Replace("${PageSize}", (model.PageSize ?? 20).ToString());
                strSql = strSql.Replace("${CurePage}", (model.CurePage ?? 20).ToString());
                strSql = strSql.Replace("${StartDate}", model.StartDate ?? nowDate);
                strSql = strSql.Replace("${EndDate}", model.EndDate ?? nowDate);
                string whereSql = "";
                if (!string.IsNullOrEmpty(model.L_OperatorID))
                {
                    string msg;
                    List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.L_OperatorID, out msg);
                    if(aList == null || aList.Count <1)
                    {
                        error.ErrMsg = msg;
                        return null;
                    }
                    whereSql += " and OpID = '" + model.L_OperatorID + "'";
                }
               
                strSql = strSql.Replace("${WhereSql}", whereSql);

                error.ErrMsg = "获取数据成功";
                error.ErrNo = "0000";
                return JSON.ToJSON(new
                {
                    JsonData = Common.CommonHelper.DataTableToJson(Db.Context_SqlServer.FromSql(strSql).ToDataTable())
                });
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LogDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
    }
}
