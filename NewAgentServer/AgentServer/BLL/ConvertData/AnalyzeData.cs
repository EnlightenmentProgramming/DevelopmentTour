﻿using BLL.Comunication;
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
                                if (hasRequestParameters && isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = aListBll.InsertAgent(reqMsg.RequestParams, head);
                                }
                                break;
                            case "GetAgentLogName"://获取代理登录名称
                                if(isLoginAuth)
                                {
                                    isGoupRetMsg = false;
                                    methodReturn = aListBll.GetALoginID(head);
                                }
                                break;
                            #endregion
                            #region 会员列表部分

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
