using DAL;
using DAL.LogicDAL;
using Dos.Common;
using Model.Comunication;
using Model.LogicModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.LogicBLL
{
    public class AgentListBLL
    {
        #region 成员变量
        AgentListDAL agentListDal = new AgentListDAL ();
        ComunicationMsg sendMsg = new ComunicationMsg();
        ErrorMessage error = new ErrorMessage();
        HeadMessage sendHead = new HeadMessage();
        #endregion
        /// <summary>
        /// 获取指定代理及它的直属代理列表数据
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetALists(string requestMsg, HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string responseMsg = "";
                string pId;
                if (CommonDAL.IsSubAgent(head.LoginID, out pId))
                {
                    head.LoginID = pId;//如果当前登录代理是子账号则，将此子账号的所属代理ID赋值为当前登录代理ID
                }
                if (aSearch == null)
                {
                    error.ErrMsg = "请求参数不完整";
                    error.ErrNo = "0003";
                }
                else
                {
                   switch(sendHead.Method)
                    {
                        case "GetAllAgents":
                            responseMsg = agentListDal.GetALists(aSearch, head, out error);
                            break;
                        case "GetDeletedA":
                            responseMsg = agentListDal.GetDeletedA(aSearch, head, out error);
                            break;
                        case "AgentMatchP":
                            responseMsg = agentListDal.GetAPermission(aSearch, head, out error);
                            break;
                    }                    
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = string.IsNullOrEmpty(responseMsg) ? "{}" : responseMsg;
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LoginBLL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
        /// <summary>
        /// 新增代理/修改代理
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string InsertAgent(string requestMsg,HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                bool insertRes = false;
                string pId;
                if (CommonDAL.IsSubAgent(head.LoginID, out pId))
                {
                    head.LoginID = pId;
                    error.ErrMsg = "子账号不具有此权限";
                }
                else if(!CommonDAL.IsYes(head.LoginID))
                {
                    error.ErrMsg = "当前代理未启用，不具有此权限";
                }
                else if(aSearch == null)
                {
                    error.ErrMsg = "没有接收到正确的参数";
                }
                else
                {
                    switch(sendHead.Method)
                    {
                        case "InsertAgent":
                            insertRes = agentListDal.InsertAgent(aSearch, head, out error);
                            break;
                        case "UpdateAgent":
                            insertRes = agentListDal.UpdateAgent(aSearch, head, out error);
                            break;
                        case "AgentModifyPwd":
                            insertRes = agentListDal.UpdateAPwd(aSearch, head, out error);
                            break;
                        case "AgentSelfModifyPwd":
                            insertRes = agentListDal.UpdateLoginAPwd(aSearch, head, out error);
                            break;
                        case "AgentPoint":
                            insertRes = agentListDal.AgentPoint(aSearch, head, out error);
                            break;
                        case "AgentClear":
                            insertRes = agentListDal.ClearAgent(aSearch, head, out error);
                            break;
                        default:
                            insertRes = false;
                            break;
                    }
                }               
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON( new { JsonData = JSON.ToJSON(new { Result = insertRes }) });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LoginBLL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }

        }
        /// <summary>
        /// 生成代理登录名称
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetALoginID(HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                string userId = "";
                switch (sendHead.Method)
                {
                    case "GetAgentLogName":
                        userId = agentListDal.GetLoginID(6,out error);
                        break;
                    case "GetClientLogName":
                        userId = agentListDal.GetLoginID(8, out error);
                        break;
                    case "GetSubLogName":
                        userId = agentListDal.GetLoginID(5, out error);
                        break;
                }                
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { JsonData = JSON.ToJSON(new { UserID = userId }) });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LoginBLL), ex);
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
       
    }
}
