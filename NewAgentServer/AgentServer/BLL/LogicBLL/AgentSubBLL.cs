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
    public class AgentSubBLL
    {
        ComunicationMsg sendMsg = new ComunicationMsg();
        ErrorMessage error = new ErrorMessage();
        HeadMessage sendHead = new HeadMessage();
        AgentSubDAL aSubDal = new AgentSubDAL();
        /// <summary>
        /// 获取指定代理下子账号列表数据
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetAgentSubs(string requestMsg,HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string responseMsg = "";
                if (aSearch == null)
                {
                    error.ErrMsg = "参数不完整";
                }
                else
                {
                    responseMsg = aSubDal.GetAgentSubs(aSearch, head, out error);
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
        /// 子账号操作
        /// </summary>
        /// <param name="requsetMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string ASubOperates(string requsetMsg,HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                AgentSub aSub = JSON.ToObject<AgentSub>(requsetMsg);
                bool opResult = false;
                string pId;
                if (aSub == null)
                {
                    error.ErrMsg = "参数不完整";
                }
                else if(CommonDAL.IsSubAgent(head.LoginID,out pId))
                {
                    head.LoginID = pId;
                    error.ErrMsg = "子账号不具有此操作权限";
                }
                else if(CommonDAL.IsYes(head.LoginID))
                {
                    error.ErrMsg = "登录代理未启用，不能进行此操作";
                }
                else
                {
                   switch(sendHead.Method)
                    {
                        case "InsertAgentSub"://新增子账号
                            opResult = aSubDal.InsertASub(aSub, head, out error);
                            break;
                        case "UpdateAgentSub"://修改子账号
                        case "UpdateSubState"://修改子账号状态
                            opResult = aSubDal.UpdateASub(aSub, head, out error);
                            break;
                        case "DeleteAgentSub":
                            opResult = aSubDal.DeleteASub(aSub, head, out error);
                            break;
                    }
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { JsonData = JSON.ToJSON(new { Result = opResult }) });
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
