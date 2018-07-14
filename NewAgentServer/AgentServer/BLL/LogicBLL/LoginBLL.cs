using BLL.Comunication;
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
    public class LoginBLL
    {
        #region 成员变量
        LoginDAL loginDal = new LoginDAL();
        ComunicationMsg sendMsg = new ComunicationMsg();
        ErrorMessage error = new ErrorMessage();
        HeadMessage sendHead = new HeadMessage();
        #endregion   
        public string LoginLogic(string requestMsg, HeadMessage head, ClientOP client)
        {
            sendHead.Method = head.Method ?? "";
            error.ErrNo = "0004";
            try
            {
                sendHead.Token = Guid.NewGuid().ToString().Replace("-", "");
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string responseMsg = "";
                string pId;
                if (CommonDAL.IsSubAgent(head.LoginID, out pId))
                {
                    head.LoginID = pId;//如果当前登录代理是子账号则，将此子账号的所属代理ID赋值为当前登录代理ID
                }
                if (aSearch == null)
                {
                    error.ErrMsg = "没有接收到登录账号或密码";
                    error.ErrNo = "0003";
                }
                else
                {
                    switch(sendHead.Method)
                    {
                        case "Login":
                            responseMsg = loginDal.Login(aSearch, head, out error);
                            if (error.ErrNo == "0000")
                            {
                                foreach (KeyValuePair<string, ClientOP> kv in WsSocket.dic_Clients)
                                {
                                    if (!string.IsNullOrEmpty(aSearch.A_UserID) && kv.Value.LogName == aSearch.A_UserID)
                                    {
                                        ErrorMessage outErr = new ErrorMessage();
                                        outErr.ErrNo = "0000";
                                        outErr.ErrMsg = "此账号已在别处登录";
                                        HeadMessage outHead = new HeadMessage();
                                        outHead.Method = "GoOut";
                                        ComunicationMsg outSend = new ComunicationMsg();
                                        outSend.Head = JSON.ToJSON(outHead);
                                        outSend.Error = JSON.ToJSON(outErr);
                                        WsSocket ws = new WsSocket();
                                        ws.Send(JSON.ToJSON(outSend), kv.Value.cSocket, true);
                                    }
                                }
                                client.LogName = aSearch.A_UserID;
                                client.Token = sendHead.Token;
                                //将成功登录的客户端添加到字典中
                                WsSocket.dic_Clients.AddOrUpdate(client.ConID, client, (key, oldv) => client);
                            }
                            break;
                        case "GetSelfCenterInfo":
                            responseMsg = loginDal.GetATodayBillCount(aSearch, head, out error);
                            break;
                        case "CenterPlayList":
                            responseMsg = loginDal.GetOnlineClntList(aSearch, head, out error);
                            break;
                        case "GetLoginACount":
                            responseMsg = loginDal.GetACountByDate(aSearch, head, out error);
                            break;
                        case "AgentClientCount":
                            responseMsg = loginDal.GetAClntCount(aSearch, head, out error);
                            break;
                        case "GetAorCAgentData":
                            responseMsg = loginDal.GetAorCAgentData(aSearch, head, out error);
                            break;
                        default:
                            responseMsg = "";
                            break;
                    }
                    
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new {JsonData = (string.IsNullOrEmpty(responseMsg)) ? "{}" : responseMsg });
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
        /// 获取公告
        /// </summary>
        /// <returns></returns>
        public string GetPubInfo(HeadMessage head)
        {
            sendHead.Method = head.Method;
            error.ErrNo = "0004";
            try
            {
                string pubInfo = loginDal.GetPubInfo(out error);
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { JsonData = (string.IsNullOrEmpty(pubInfo)) ? "{}" : pubInfo });
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
        /// 代理登录
        /// </summary>
        /// <param name="requestMsg">请求参数</param>
        /// <param name="head">报文头</param>
        /// <param name="client">当前连接客户端对象</param>
        /// <returns></returns>
        public string Login(string requestMsg, HeadMessage head,ClientOP client)
        {
            //ComunicationMsg sendMsg = new ComunicationMsg();
            //ErrorMessage error = new ErrorMessage();
            //HeadMessage sendHead = new HeadMessage();
            try
            {
                sendHead.Method = head.Method;
                sendHead.Token = Guid.NewGuid().ToString().Replace("-", "");
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string responseMsg = "";
                if (aSearch == null)
                {
                    error.ErrMsg = "没有接收到登录账号或密码";
                    error.ErrNo = "0003";
                }
                else
                {
                    responseMsg = loginDal.Login(aSearch, head, out error);
                    if (error.ErrNo == "0000")
                    {
                        foreach (KeyValuePair<string, ClientOP> kv in WsSocket.dic_Clients)
                        {
                            if (!string.IsNullOrEmpty(aSearch.A_UserID) && kv.Value.LogName == aSearch.A_UserID)
                            {
                                ErrorMessage outErr = new ErrorMessage();
                                outErr.ErrNo = "0000";
                                outErr.ErrMsg = "此账号已在别处登录";
                                HeadMessage outHead = new HeadMessage();
                                outHead.Method = "GoOut";
                                ComunicationMsg outSend = new ComunicationMsg();
                                outSend.Head = JSON.ToJSON(outHead);
                                outSend.Error = JSON.ToJSON(outErr);
                                WsSocket ws = new WsSocket();
                                ws.Send(JSON.ToJSON(outSend), kv.Value.cSocket, true);
                            }
                        }
                        client.LogName = aSearch.A_UserID;
                        client.Token = sendHead.Token;
                        client.ConTime = DateTime.Now;
                        //将成功登录的客户端添加到字典中
                        WsSocket.dic_Clients.AddOrUpdate(client.ConID, client, (key, oldv) => client);
                    }
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new {LoginAgent = string.IsNullOrEmpty(responseMsg) ? "{}" : responseMsg });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(LoginBLL), ex);
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(head);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
        /// <summary>
        /// 获取指定代理今日下单统计
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public string GetATodayBillCount(string requestMsg, HeadMessage head)
        {
            //ComunicationMsg sendMsg = new ComunicationMsg();
            //ErrorMessage error = new ErrorMessage();
            //HeadMessage sendHead = new HeadMessage();
            sendHead.Method = head.Method;
            try
            {
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string billCount = "";
                if (aSearch == null)
                {
                    error.ErrMsg = "没有接收到正确的参数";
                    error.ErrNo = "0003";
                }
                else
                {
                    billCount = loginDal.GetATodayBillCount(aSearch, head, out error);
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { ATodayBillCount = string.IsNullOrEmpty(billCount) ? "{}" : billCount });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(head);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
          
        }
        /// <summary>
        /// 获取指定代理会员在线列表
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetOnlineClntList(string requestMsg, HeadMessage head)
        {
            try
            {
                sendHead.Method = head.Method;
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string onlineList = "";
                if (aSearch == null)
                {
                    error.ErrMsg = "没有接收到正确的参数";
                    error.ErrNo = "0003";
                }
                else
                {
                    onlineList = loginDal.GetOnlineClntList(aSearch, head, out error);
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { OnlineList = string.IsNullOrEmpty(onlineList) ? "{}" : onlineList });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(head);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
        /// <summary>
        /// 获取指定时段内指定代理统计数据
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetACountByDate(string requestMsg, HeadMessage head)
        {
            try
            {
                sendHead.Method = head.Method;
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string agentCount = "";
                if (aSearch == null)
                {
                    error.ErrMsg = "没有接收到正确的参数";
                    error.ErrNo = "0003";
                }
                else
                {
                    agentCount = loginDal.GetACountByDate(aSearch, head, out error);
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { AgentCount = string.IsNullOrEmpty(agentCount) ? "{}" : agentCount });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(head);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
        /// <summary>
        /// 获取指定时段内指定代理下会员统计数据
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public string GetAClientCount(string requestMsg,HeadMessage head)
        {
            try
            {
                sendHead.Method = head.Method;
                AgentSearchModel aSearch = JSON.ToObject<AgentSearchModel>(requestMsg);
                string onlineList = "";
                if (aSearch == null)
                {
                    error.ErrMsg = "没有接收到正确的参数";
                    error.ErrNo = "0003";
                }
                else
                {
                    onlineList = loginDal.GetOnlineClntList(aSearch, head, out error);
                }
                sendMsg.Head = JSON.ToJSON(sendHead);
                sendMsg.Error = JSON.ToJSON(error);
                sendMsg.Reponse = JSON.ToJSON(new { AgentClntCount = string.IsNullOrEmpty(onlineList) ? "{}" : onlineList });
                return JSON.ToJSON(sendMsg);
            }
            catch (Exception ex)
            {
                error.ErrNo = "0004";
                error.ErrMsg = ex.Message.Replace("\r", "").Replace("\n", "");
                sendMsg.Head = JSON.ToJSON(head);
                sendMsg.Reponse = "{}";
                sendMsg.Error = JSON.ToJSON(error);
                return JSON.ToJSON(sendMsg);
            }
        }
    }
}
 