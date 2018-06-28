using BLL.ConvertData;
using Common;
using Model.Comunication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Comunication
{
    /// <summary>
    /// websocket通信控制
    /// </summary>
    public class WsSocket
    {
        #region 变量
        /// <summary>
        /// 客户端用户字段
        /// </summary>
        public static ConcurrentDictionary<string, ClientOP> dic_Clients = new ConcurrentDictionary<string, ClientOP>();

        /// <summary>
        /// 客户端消息队列
        /// </summary>
        public static ConcurrentQueue<ClientOP> que_msgs = new ConcurrentQueue<ClientOP>();

        //是否停止监听
        bool isStop = false;

        //客户端连接过来的socket对象
        Socket ListenSocket; 

        #endregion

        #region 异步监听
        /// <summary>
        /// 接收到客户端socket连接
        /// </summary>
        private void StartAccept()
        {
            try
            {
                if (!isStop)
                {
                    SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(Asyn_Complete);
                    if (!ListenSocket.AcceptAsync(acceptEventArg))
                    {
                        ProcessAccept(acceptEventArg);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
            }
        }
        /// <summary>
        /// 判断socket操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Asyn_Complete(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Accept:
                        ProcessAccept(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        ProcessRecive(e);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
            }
        }
        /// <summary>
        /// 接受客户端连接
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                if (acceptEventArgs.SocketError != SocketError.Success || isStop)
                {
                    if (isStop)
                        StartAccept();
                    closeConnect(acceptEventArgs.AcceptSocket);
                    acceptEventArgs = null;
                    return;
                }
                SocketAsyncEventArgs reciveEventArgs = new SocketAsyncEventArgs();
                reciveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(Asyn_Complete);
                reciveEventArgs.SetBuffer(new byte[65536], 0, 65536);//分配数据缓存空间
                reciveEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;
                acceptEventArgs = null;
                ProcessRecive(reciveEventArgs);//处理接收到的数据
                StartAccept();//继续等待下一次连接请求
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
            }
        }
        /// <summary>
        /// 准备开始接收数据
        /// </summary>
        /// <param name="reciveEventArgs"></param>
        private void StartRecive(SocketAsyncEventArgs reciveEventArgs)
        {
            try
            {
                if (!isStop)
                {
                    if (reciveEventArgs.AcceptSocket.Connected)
                    {
                        if (!reciveEventArgs.AcceptSocket.ReceiveAsync(reciveEventArgs))
                        {
                            ProcessRecive(reciveEventArgs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="reciveEventArgs"></param>
        private void ProcessRecive(SocketAsyncEventArgs reciveEventArgs)
        {
            try
            {
                #region 处理接收到的UserToken
                //如果连接不正常，或者接收到的数据长度为0 则主动断开连接
                if (reciveEventArgs.SocketError != SocketError.Success || reciveEventArgs.BytesTransferred == 0)
                {
                    closeConnect(reciveEventArgs.AcceptSocket);
                    reciveEventArgs = null;
                    return;
                }
                int len = reciveEventArgs.BytesTransferred;//记录接收到的数据包长度
                byte[] data_pac = new byte[len];
                Array.Copy(reciveEventArgs.Buffer, reciveEventArgs.Offset, data_pac, 0, len);
                Func<bool, byte[]> appendData = (ok) =>
                 {
                     byte[] newData;
                     if (reciveEventArgs.UserToken != null)
                     {
                         byte[] tmp = (byte[])reciveEventArgs.UserToken;
                         newData = new byte[len + tmp.Length];
                         Array.Copy(tmp, 0, newData, 0, tmp.Length);
                         Array.Copy(data_pac, 0, newData, tmp.Length, len);
                         if (ok)
                             reciveEventArgs.UserToken = null;
                     }
                     else
                         newData = data_pac;
                     return newData;
                 };//匿名方法appendData结束
                if (reciveEventArgs.AcceptSocket.Available != 0)
                    reciveEventArgs.UserToken = appendData(false);
                else
                {
                    data_pac = appendData(true);
                    ClientOP client = new ClientOP();//记录当前连接过来的客户端对象
                    client.cSocket = reciveEventArgs.AcceptSocket;
                    if(Analyze(data_pac,len,client))
                    {
                        string msg = client.Pac_Msg;
                        if(!string.IsNullOrEmpty(msg))
                        {
                            string decodeMsg = ZipHelper.GZipDecompressString(msg);//将接收到的数据进行Gizp解压
                            AnalyzeData analzeData = new AnalyzeData();
                            string sendMsg = analzeData.StartAnalyze(decodeMsg, client);
                            Send(sendMsg, client.cSocket);
                        }
                    }
                    else//Analyze方法的else
                    {
                        Send("数据解析失败", client.cSocket);
                        client = null;
                        closeConnect(reciveEventArgs.AcceptSocket);
                        dic_Clients.TryRemove(client.ConID, out client);
                        reciveEventArgs.AcceptSocket = null;
                    }//Analyze方法的if-else结束
                } 
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
            }
            finally
            {
                if (reciveEventArgs != null && reciveEventArgs.AcceptSocket != null)
                    StartRecive(reciveEventArgs);//继续处理接收到的数据
            }
        }
        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="len"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool Analyze(byte[] buffer,int len,ClientOP client)
        {
            try
            {
                Socket cSocket = client.cSocket;
                IPEndPoint endPoint = (IPEndPoint)cSocket.RemoteEndPoint;
                string ip = endPoint.Address.ToString();//获取Ip地址
                string port = endPoint.Port.ToString();//获取端口
                string connID = ip + ":" + port;
                string pacStr = Encoding.UTF8.GetString(buffer, 0, len);
                client.IP = ip;
                client.Port = port;
                client.ConID = connID;
                if (Regex.Match(pacStr.ToLower(), "upgrade: websocket").Value == "")//当前收到的不是握手包
                {
                    if (dic_Clients.ContainsKey(connID) && Regex.Match(pacStr.ToLower(), "policy-file-request").Value == "")//当收到的数据[不是]flash请求策略文件时
                    {
                        client.Pac_Msg = AnalyzeClientData(client, buffer, len);//解析出客户端的消息
                    }
                    else
                    {
                        if (Regex.Match(pacStr.ToLower(), "policy-file-request").Value != "")//当收到flash请求策略文件时
                        {
                            cSocket.Send(Encoding.UTF8.GetBytes("<cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0"));
                            client.Pac_Flash = true;
                            return true;
                        }
                        else
                        {
                            cSocket.Send(Encoding.UTF8.GetBytes("Sorry,Not allow,Please call me number:13888888888"));
                            return false;
                        }
                    }
                }
                else
                {
                    cSocket.Send(AnswerHandShake(pacStr));
                    client.Pac_Type = 101;//连接成功
                }
                if (dic_Clients.ContainsKey(connID) && client.Pac_Type == 101)
                {
                    //同个IP重复登录（注：您也可以自己定义key的值，这里把IP作为key）
                    Send("同个IP已重复登录：", dic_Clients[connID].cSocket);//发出提醒
                    closeConnect(dic_Clients[connID].cSocket);//关闭旧连接
                }
                else
                {
                    if (dic_Clients.TryAdd(connID, client))
                    {
                        if (!client.Pac_Flash)
                            que_msgs.Enqueue(client);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
                return false;
            }
        }
        /// <summary>
        /// 应答客户端连接包
        /// </summary>
        /// <param name="pacStr"></param>
        /// <returns></returns>
        private byte[] AnswerHandShake(string pacStr)
        {
            string handShakeText = pacStr;
            string key = string.Empty;
            Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            Match m = reg.Match(handShakeText);
            if(m.Value != "")
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
            }
            byte[] secKeyBytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            string secKey = Convert.ToBase64String(secKeyBytes);
            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + "\r\n");
            responseBuilder.Append("Upgrade: websocket" + "\r\n");
            responseBuilder.Append("Connection: Upgrade" + "\r\n");
            responseBuilder.Append("Sec-WebSocket-Accept: " + secKey + "\r\n\r\n");
            return Encoding.UTF8.GetBytes(responseBuilder.ToString());
        }
        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="buffer">数据包</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        private string AnalyzeClientData(ClientOP client, byte[] buffer, int len)
        {
            try
            {
                bool mask = false;
                int lodlen = 0;
                if (len < 2)
                    return string.Empty;
                client.Pac_Fin = (buffer[0] >> 7) > 0;
                if (!client.Pac_Fin)
                    return string.Empty;
                client.Pac_Type = buffer[0] & 0xF;
                if (client.Pac_Type == 10)//心跳包(IE10及以上特有，不处理即可)
                    return string.Empty;
                else if (client.Pac_Type == 8)//退出包
                {
                    removeConnectDic(client);
                    // Common.LogHelper.WriteLog(typeof(WSClass), new Exception("退出包主动关闭socket连接"));
                    return string.Empty;
                }
                mask = (buffer[1] >> 7) > 0;
                lodlen = buffer[1] & 0x7F;
                byte[] loddata;
                byte[] masks = new byte[4];

                if (lodlen == 126)
                {
                    Array.Copy(buffer, 4, masks, 0, 4);
                    lodlen = (UInt16)(buffer[2] << 8 | buffer[3]);
                    loddata = new byte[lodlen];
                    Array.Copy(buffer, 8, loddata, 0, lodlen);
                }
                else if (lodlen == 127)
                {
                    Array.Copy(buffer, 10, masks, 0, 4);
                    byte[] uInt64Bytes = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        uInt64Bytes[i] = buffer[9 - i];
                    }
                    lodlen = (int)BitConverter.ToUInt64(uInt64Bytes, 0);

                    loddata = new byte[lodlen];
                    try
                    {
                        for (int i = 0; i < lodlen; i++)
                        {
                            loddata[i] = buffer[i + 14];
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(typeof(WsSocket), ex);
                    }
                }
                else
                {
                    Array.Copy(buffer, 2, masks, 0, 4);
                    loddata = new byte[lodlen];
                    Array.Copy(buffer, 6, loddata, 0, lodlen);
                }
                for (var i = 0; i < lodlen; i++)
                {
                    loddata[i] = (byte)(loddata[i] ^ masks[i % 4]);
                }
                return Encoding.UTF8.GetString(loddata);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="so">连接的socket</param>
        public void closeConnect(Socket socket)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                if (socket != null && socket.Connected)
                {
                    socket.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
            }
        }
       
        public bool Send(string msg,Socket socket)
        {
            try
            {
                string zipMsg = ZipHelper.GZipCompressString(msg);//把发送信息做Gzip压缩
                byte[] bytes = Encoding.ASCII.GetBytes(zipMsg);
                bool isSend = true;
                int sendMax = 65536;//每次分片最大64kb数据
                int count = 0;//发送的次数
                int sendedLen = 0;//成功发送出去的字节长度
                while (isSend)
                {
                    byte[] contentBytes = null;//待发送的信息
                    var sendArr = bytes.Skip(count * sendMax).Take(sendMax).ToArray();
                    sendedLen += sendArr.Length;
                    if (sendArr.Length > 0)
                    {
                        isSend = bytes.Length > sendedLen;
                        if (sendArr.Length < 126)
                        {
                            contentBytes = new byte[sendArr.Length + 2];
                            contentBytes[0] = (byte)(count == 0 ? 0x81 : (!isSend ? 0x80 : 0));
                            contentBytes[1] = (byte)sendArr.Length;//1个字节存储真实长度
                            Array.Copy(sendArr, 0, contentBytes, 2, sendArr.Length);
                            isSend = false;
                        }
                        else if (sendArr.Length <= 65535)
                        {
                            contentBytes = new byte[sendArr.Length+4];
                            if (!isSend && count == 0)
                                contentBytes[0] = 0x81;//非分片发送
                            else
                                contentBytes[0] = (byte)(count == 0 ? 0x01 : (!isSend ? 0x80 : 0));//处于连续的分片发送
                            contentBytes[1] = 126;
                            byte[] slen = BitConverter.GetBytes((short)sendArr.Length);//2个字节存储真实长度
                            contentBytes[2] = slen[1];
                            contentBytes[3] = slen[0];
                            Array.Copy(sendArr, 0, contentBytes, 4, sendArr.Length);
                        }
                        else if (sendArr.LongLength < long.MaxValue)
                        {
                            contentBytes = new byte[sendArr.Length + 10];
                            contentBytes[0] = (byte)(count == 0 ? 0x01 : (!isSend ? 0x80 : 0));//处于连续的分片发送
                            contentBytes[1] = 127;
                            byte[] llen = BitConverter.GetBytes((long)sendArr.Length);//8个字节存储真实长度
                            for (int i = 7; i >= 0; i--)
                            {
                                contentBytes[9 - i] = llen[i];
                            }
                            Array.Copy(sendArr, 0, contentBytes, 10, sendArr.Length);
                        }
                    }// sendArr.Length >0 结束
                    if (socket != null && socket.Connected)
                    {
                        socket.Send(contentBytes);
                    }
                    count++;
                }//while 循环结束
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(WsSocket), ex);
                return false;
            }
        }

        /// <summary>
        /// 删除字典里的连接
        /// </summary>
        /// <param name="cp">客户端连接</param>
        private void removeConnectDic(ClientOP cp)
        {
            string ip = cp.IP;
            string conid = cp.ConID;
            string port = cp.Port;
            ClientOP _cp = new ClientOP();
            if (dic_Clients.ContainsKey(conid))
            {
                if (dic_Clients.TryRemove(conid, out _cp))
                    closeConnect(cp.cSocket);
            }
        }
        #endregion
    }
}
