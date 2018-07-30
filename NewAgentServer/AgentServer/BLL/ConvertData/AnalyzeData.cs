using BLL.Comunication;
using BLL.LogicBLL;
using Dos.Common;
using Model.Comunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ConvertData
{
    public class AnalyzeData
    {
        #region 成员变量
        LogicBLL.LoginBLL loginBll = new LogicBLL.LoginBLL();
        AgentListBLL aListBll = new AgentListBLL();
        ClientListBLL clntListBll = new ClientListBLL();
        AgentSubBLL aSubBll = new AgentSubBLL();
        StatisticsBLL sBll = new StatisticsBLL();
        #endregion
        /// <summary>
        /// 初步处理接收到的报文
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isZ">标记应答报文是否需要Gzip压缩</param>
        /// <returns></returns>
        public string StartAnalyze(string msg,ClientOP client,out bool isZ)
        {
            ComunicationMsg cmMsg = JSON.ToObject<ComunicationMsg>(msg);
            ComunicationMsg cMsg = new ComunicationMsg();
            HeadMessage hMsg = new HeadMessage();
            ErrorMessage error = new ErrorMessage();
            RequestMsg reqMsg = new RequestMsg();
            bool isGoupRetMsg = true;//是否需要组装应答报文
            string methodReturn = "";
            bool hasRequestParameters = false;//是否包含请求参数
            bool isLoginAuth = false;//是否通过了登录验证
            string retMsg = "";
            string response = "{}";
            isZ = true;
            HeadMessage head;
            if (cmMsg != null)
            {
                try
                {
                    reqMsg = cmMsg.Request !=null ? JSON.ToObject<RequestMsg>(cmMsg.Request):reqMsg;
                    //判断应答报文是否需要压缩
                    if (reqMsg != null && !string.IsNullOrEmpty(reqMsg.IsZ) && reqMsg.IsZ == "N")
                    {
                        isZ = false;
                    }
                    //判断是否包含请求参数
                    if (reqMsg != null && !string.IsNullOrEmpty(reqMsg.RequestParams))
                    {
                        hasRequestParameters = true;
                    }
                    else
                    {
                        error.ErrNo = "0003";
                        error.ErrMsg = "请求参数不完整。";
                    }   
                    //解码报文头
                    head = JSON.ToObject<HeadMessage>(cmMsg.Head);
                    if (head != null)
                    {
                        if(string.IsNullOrEmpty(head.Account) || string.IsNullOrEmpty(head.Token) || string.IsNullOrEmpty(head.LoginID))
                        {
                            isLoginAuth = false;
                            error.ErrMsg += " 没有通过登录验证，所以无权获取数据。";
                            error.ErrNo = "0004";
                        }
                        else
                        {
                            foreach (KeyValuePair<string, ClientOP> kv in WsSocket.dic_Clients)
                            {
                                if(head.Account  == kv.Value.LogName && head.Token == kv.Value.Token)
                                {
                                    isLoginAuth = true;
                                    break;
                                }                               
                            }
                            if(!isLoginAuth)
                            {
                                error.ErrMsg += " 没有通过登录验证，所以无权获取数据。";
                                error.ErrNo = "0004";
                            }
                        }
                        string method = head.Method ?? "";
                        head.Ip = client.IP;
                        hMsg.Method = method;
                        switch (method)
                        {
                            case "HeartBeat"://心跳
                                error.ErrNo = "0000";
                                error.ErrMsg = "连接正常";
                                break;
                            case "SignOut"://登出
                                if (hasRequestParameters)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = loginBll.SignOut(reqMsg.RequestParams, head);
                                }
                                break;
                            #region 登录部分
                            case "Login"://登录
                                if (hasRequestParameters)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = loginBll.LoginLogic(reqMsg.RequestParams, head, client);
                                }
                                break;
                            case "GetSelfCenterInfo"://获取指定代理今日下单统计
                            case "CenterPlayList"://获取指定代理会员在线情况
                            case "GetLoginACount"://获取指定时段内指定代理统计数据
                            case "AgentClientCount"://获取指定时段内指定代理下会员统计数据
                            case "GetAorCAgentData"://获取指定代理或指定会员所属代理统计数据
                            case "GetAListByID"://根据代理ID获取指定代理数据
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = loginBll.LoginLogic(reqMsg.RequestParams, head, client);
                                }
                                break;
                            case "GetPubInfo"://获取公告
                                if(isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = loginBll.GetPubInfo(head);
                                }
                                break;
                            #endregion
                            #region 代理列表部分
                            case "GetAllAgents"://获取指定代理及它的直属代理列表数据
                            case "GetDeletedA"://获取指定代理下逻辑删除的代理列表数据
                            case "AgentMatchP"://获取指定代理相关权限
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = aListBll.GetALists(reqMsg.RequestParams, head);
                                }
                                break;
                            case "InsertAgent"://新增代理
                            case "UpdateAgent"://修改代理
                            case "AgentModifyPwd"://修改下级代理密码
                            case "AgentSelfModifyPwd"://修改登录代理密码
                            case "AgentPoint"://代理上下分
                            case "AgentClear"://代理清零
                            case "SettleOdds4Agent"://结算代理抽水
                            case "SettleWashF4Agent"://结算代理洗码费
                            case "DeleteAgent"://删除代理
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = aListBll.InsertAgent(reqMsg.RequestParams, head);
                                }
                                break;
                            case "GetAgentLogName"://获取代理登录名称
                            case "GetClientLogName"://获取会员登录名称
                            case "GetSubLogName"://获取子账号登录名称
                                if (isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = aListBll.GetALoginID(head);
                                }
                                break;
                            #endregion
                            #region 会员列表部分
                            case "GetClntList_Invite"://获取会员列表数据
                            case "GetDeletedC"://获取逻辑删除的会员
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = clntListBll.GetClntList(reqMsg.RequestParams, head);
                                }
                                break;
                            case "GetClntOdds"://获取会员标准赔率
                               if(isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = clntListBll.GetClntOdds(head);
                                }
                                break;
                            case "InsertClient"://新增会员
                            case "AllClientZero"://直属会员清零
                            case "ClientZero"://会员清零
                            case "ClearCard4Clnt"://会员清卡
                            case "UpdateClient"://修改会员
                            case "ClientModifyPwd"://修改会员密码
                            case "ClientPoint"://会员上下分
                            case "SettleWashF4Clnt"://结算会员洗码费
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = clntListBll.ClntOperates(reqMsg.RequestParams, head);
                                }
                                break;
                            #endregion
                            #region 子账号部分
                            case "GetAgentSubs"://获取指定代理下子账号列表数据
                                aSubBll.GetAgentSubs(reqMsg.RequestParams, head);
                                break;
                            case "InsertAgentSub"://新增子账号
                            case "UpdateAgentSub"://修改子账号
                            case "UpdateSubState"://修改子账号状态
                            case "DeleteAgentSub"://删除子账号
                                aSubBll.ASubOperates(reqMsg.RequestParams, head);
                                break;
                            #endregion
                            #region 获取报表数据部分
                            case "GetAStatistics"://获取代理统计数据
                            case "GetADayStatistics"://获取代理每日统计数据
                            case "GetClntStatistics"://获取会员统计
                            case "GetClntDayStatistics"://获取会员每日统计数据
                            case "GetClntBetBills"://获取会员下注明细
                            case "GetPointDetail"://获取上下分明细  
                            case "GetTableResult"://获取游戏结果数据
                            case "PromotionA_Clnt4A"://获取指定代理下的会员推广统计数据 增量
                            case "PromotionA_AllClnt4A"://获取指定代理下的会员推广统计数据 存量
                            case "H5ClntPointDetail"://H5会员第三方上分明细
                            case "H5ClntPointSum_A"://获取指定代理下所有H5会员第三方上分明细
                            case "RedEnvelopeSum"://查询指定代理自己及直属代理及直属会员下红包发放合计
                            case "RedEnvelopeDetail"://查询指定会员下红包发送明细
                            case "GetSettleAccounts"://获取结算记录
                            case "GetWashF4Clnt"://获取会员的洗码费统计
                            case "GetOddsWashF4Agent"://获取代理抽水及洗码费统计
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    methodReturn = sBll.GetStatisticsData(reqMsg.RequestParams, head);
                                }
                                break;
                            #endregion
                            #region 获取日志记录
                            case "GetTransactions"://获取交易记录
                            case "GetLoginLog"://获取登录日志
                            case "GetOperationLog"://获取操作日志
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = sBll.GetLog(reqMsg.RequestParams, head);
                                }
                                break; 
                            #endregion
                            default:
                                error.ErrNo = "0005";
                                error.ErrMsg = "没有对应的接口：" + method;
                                break;
                        }
                    }
                    else
                    {
                        error.ErrNo = "0003";
                        error.ErrMsg = "报文格式错误，没有收到正确的报文头";
                    }
                }                
                catch (Exception ex)
                {
                    Common.LogHelper.WriteLog(typeof(AnalyzeData), ex);
                    error.ErrNo = "0001";
                    error.ErrMsg = "系统内部错误："+ ex.Message.Replace("\r", "").Replace("\n", "");
                }
            }
            cMsg.Head = JSON.ToJSON(hMsg);
            cMsg.Reponse = response;
            cMsg.Request = "{}";
            cMsg.Error = JSON.ToJSON(error);           
            retMsg = isGoupRetMsg ? JSON.ToJSON(cMsg) : methodReturn;                
            return retMsg;
        }


    }
}
