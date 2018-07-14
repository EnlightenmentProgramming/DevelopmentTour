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
    public class AgentSubDAL : Repository<T_AgentSub>
    {
        /// <summary>
        /// 获取指定代理下的子账号列表数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetAgentSubs(AgentSearchModel model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (model == null || string.IsNullOrEmpty(model.A_ID))
                {
                    error.ErrMsg = "参数不完整";//"服务端没有读取到ClntOdds数据模板，请联系管理员";
                    error.ErrNo = "0004";
                    return null;
                }
                string strSql = Common.SqlTemplateCommon.GetSql("A_GetAgentSubByAgentID");
                if (string.IsNullOrEmpty(strSql))
                {
                    error.ErrMsg = "服务端没有读取到A_GetAgentSubByAgentID数据模板，请联系管理员";
                    return null;
                }
                strSql = strSql.Replace("${AgentID}", model.A_ID);
                string msg;
                List<AgentSearchModel> aList = CommonDAL.GetAgentTree(head.LoginID, "id", model.A_ID, out msg);
                if(aList == null || aList.Count<=0)
                {
                    error.ErrMsg = msg;
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
                Common.LogHelper.WriteLog(typeof(AgentSubDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return null;
            }
        }
        /// <summary>
        /// 新增子账号
        /// </summary>
        /// <param name="model"></param>
        /// <param name="head"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool InsertASub(AgentSub model,HeadMessage head,out ErrorMessage error)
        {
            error = new ErrorMessage();
            error.ErrNo = "0004";
            try
            {
                if (string.IsNullOrEmpty(model.AS_AID))
                {
                    error.ErrMsg = "新增子账号必须指定所属代理ID";
                    return false;
                }
                if (string.IsNullOrEmpty(model.AS_UserID))
                {
                    error.ErrMsg = "新增子账号必须填写子账号登录名称";
                    return false;
                }
                if (string.IsNullOrEmpty(model.AS_Name))
                {
                    error.ErrMsg = "新增子账号必须填写子账号名称";
                    return false;
                }
                if (string.IsNullOrEmpty(model.AS_Pwd))
                {
                    error.ErrMsg = "新增子账号必须填写子账号密码";
                    return false;
                }
                T_AgentSub dbASub = new T_AgentSub();
                dbASub.AgentID = model.AS_AID;
                dbASub.AgentSubID = Guid.NewGuid().ToString().Replace("-", "");
                dbASub.State = model.AS_State;
                dbASub.Pwd = model.AS_Pwd;
                dbASub.LogName = model.AS_UserID;
                dbASub.AgentSubName = model.AS_Name;
                if (Db.Context_SqlServer.Insert<T_AgentSub>(dbASub) > 0)
                {
                    error.ErrMsg = "新增子账号成功";
                    error.ErrNo = "0000";
                    return true;
                }
                error.ErrMsg = "新增子账号失败";
                return false;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(AgentSubDAL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                return false;
            }
        }
    }
}
