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
            string retMsg = "";
            string response = "{}";
            isZ = true;
            //hMsg.Method = "HeartBeat";
            //cMsg.Head = JSON.ToJSON(hMsg);
            //string msga = JSON.ToJSON(cMsg);
            HeadMessage head;
            if (cmMsg != null)
            {
                try
                {
                    reqMsg = cmMsg.Request !=null ? JSON.ToObject<RequestMsg>(cmMsg.Request):reqMsg;
                    if(reqMsg != null && !string.IsNullOrEmpty(reqMsg.IsZ) && reqMsg.IsZ == "N")
                    {
                        isZ = false;
                    }
                    head = JSON.ToObject<HeadMessage>(cmMsg.Head);
                    if (head != null)
                    {
                        string method = head.Method ?? "";
                        hMsg.Method = method;
                        switch (method)
                        {
                            case "HeartBeat"://心跳
                                error.ErrNo = "0000";
                                error.ErrMsg = "连接正常";
                                break;
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
            retMsg = JSON.ToJSON(cMsg);                
            return retMsg;
        }
    }
}
