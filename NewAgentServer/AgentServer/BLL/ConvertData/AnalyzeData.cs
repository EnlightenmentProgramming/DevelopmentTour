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
        /// <returns></returns>
        public string StartAnalyze(string msg,ClientOP client)
        {
            ComunicationMsg cmMsg = JSON.ToObject<ComunicationMsg>(msg);
            HeadMessage head;
            if (cmMsg != null)
            {
                head = JSON.ToObject<HeadMessage>(cmMsg.Head);
            }           
            return msg;
        }
    }
}
