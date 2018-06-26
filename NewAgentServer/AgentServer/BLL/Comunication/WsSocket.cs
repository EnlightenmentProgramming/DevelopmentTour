using Model.Comunication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Comunication
{
    /// <summary>
    /// websocket通信控制
    /// </summary>
    public class WsSocket
    {
        public static ConcurrentDictionary<string, ClientOP> dic_Clients = new ConcurrentDictionary<string, ClientOP>();

        //是否停止监听
        bool isStop = false;

        //客户端连接过来的socket对象
        Socket ListenSocket;
        #region 异步监听
        /// <summary>
        /// 接收到客户端socket连接
        /// </summary>
        private void StartAccept()
        {
            if (!isStop)
            {
                SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(Asyn_Complete);
                if (!ListenSocket.AcceptAsync(acceptEventArg))
                {

                }
            }
        }
        /// <summary>
        /// 判断socket操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Asyn_Complete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:

                    break;
                case SocketAsyncOperation.Receive:

                    break;
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if(acceptEventArgs.SocketError != SocketError.Success || isStop)
            {
                if (isStop)
                    StartAccept();
                
            }
        }
        #endregion
    }
}
