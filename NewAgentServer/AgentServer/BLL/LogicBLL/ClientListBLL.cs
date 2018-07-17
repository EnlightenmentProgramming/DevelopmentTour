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
    public class ClientListBLL
    {
        ComunicationMsg sendMsg = new ComunicationMsg();
        ErrorMessage error = new ErrorMessage();
        HeadMessage sendHead = new HeadMessage();
        ClientListDAL clntDal = new ClientListDAL();
        /// <summary>
        /// 获取会员列表数据
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetClntList(string requestMsg, HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                ClientSearc cSearch = JSON.ToObject<ClientSearc>(requestMsg);
                string responseMsg = "";
                string pId;
                if (CommonDAL.IsSubAgent(head.LoginID, out pId))
                {
                    head.LoginID = pId;//如果当前登录代理是子账号则，将此子账号的所属代理ID赋值为当前登录代理ID
                }
                if (cSearch == null)
                {
                    error.ErrMsg = "请求参数不完整";
                    error.ErrNo = "0003";
                }
                else
                {
                    switch(sendHead.Method)
                    {
                        case "GetClntList_Invite":
                            responseMsg = clntDal.GetCLists(cSearch, head, out error);
                            break;
                        case "GetDeletedC":
                            responseMsg = clntDal.GetDeletedC(cSearch, head, out error);
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
        /// 获取会员标准赔率
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetClntOdds(HeadMessage head)
        {
            sendHead.Method = head.Method;
            error.ErrNo = "0004";
            try
            {
                string responseMsg = clntDal.GetClntOdds(out error);
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
        /// 会员操作
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string ClntOperates(string requestMsg,HeadMessage head)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {

                ClientSearc cSearch = JSON.ToObject<ClientSearc>(requestMsg);
                bool opResult = false;
                string pId;
                if (CommonDAL.IsSubAgent(head.LoginID, out pId))
                {
                    error.ErrMsg = "子账号不具有此权限";  
                    string h5MgrId = CommonDAL.GetH5MgrID();//H5会员管理代理的子账号可以对H5会员进行上下分&& cSearch.C_IsAdd == true
                    if (CommonDAL.IsH5Clnt(cSearch.C_ID) && sendHead.Method == "ClientPoint"  && string.Equals(h5MgrId.Trim(), head.LoginID.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        if(!CommonDAL.IsYesSub(head.LoginID))
                        {
                            error.ErrMsg = "当前子账号未启用,不能进行此操作";
                        }
                        head.LoginID = pId;
                        opResult = clntDal.ClientPoint(cSearch, head, out error);
                    }
                }
                else if (!CommonDAL.IsYes(head.LoginID))
                {
                    error.ErrMsg = "当前代理未启用，不具有此权限";
                }
                else if (cSearch == null)
                {
                    error.ErrMsg = "没有接收到正确的参数";
                }
                else
                {
                    switch(sendHead.Method)
                    {
                        case "InsertClient":
                            opResult = clntDal.InsertClient(cSearch, head, out error);
                            break;
                        case "AllClientZero":
                            opResult = clntDal.ClearClnt4Agent(cSearch, head, out error);
                            break;
                        case "ClientZero":
                            opResult = clntDal.ClearClnt(cSearch, head, out error);
                            break;
                        case "ClearCard4Clnt":
                            opResult = clntDal.ClearCard4Clnt(cSearch, head, out error);
                            break;
                        case "UpdateClient":
                            opResult = clntDal.UpdateClnt(cSearch, head, out error);
                            break;
                        case "ClientModifyPwd":
                            opResult = clntDal.UpdateClntPwd(cSearch, head, out error);
                            break;
                        case "ClientPoint":
                            opResult = clntDal.ClientPoint(cSearch, head, out error);
                            break;
                        case "SettleWashF4Clnt":
                            opResult = clntDal.SettleWashF(cSearch, head, out error);
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
