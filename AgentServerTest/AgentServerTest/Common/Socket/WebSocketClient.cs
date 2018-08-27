using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Socket
{
    public class WebSocketClient
    {
        private static object consoleLock = new object();
        private const int sendChunkSize = 256;
        private const int receiveChunkSize = 1024;
        private const bool verbose = true;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(1000);
        static ClientWebSocket webSocket = null;
        private static System.Diagnostics.Stopwatch countTime = new System.Diagnostics.Stopwatch();   //定义一个计时对象 
        private static string interfaceName = string.Empty;   
        public static async Task Connect(string uri)
        {
            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Console.WriteLine(" 连接服务"+uri+"成功");
                await Task.WhenAll(Receive(webSocket));//, Send()
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                lock (consoleLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WebSocket closed.");
                    Console.ResetColor();
                }
            }
        }

        public static async Task Send(string msg,string name)
        {
            //var random = new Random();
            //byte[] buffer = new byte[sendChunkSize];

            //while (webSocket.State == WebSocketState.Open)
            //{
            //    random.NextBytes(buffer);


            //    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
            //    LogStatus(false, buffer, buffer.Length);

            //    await Task.Delay(delay);
            //}
            countTime.Start();
            interfaceName = name;
            if (webSocket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }
            Console.WriteLine("\r\n   "+"->：["+msg+"]");
            FileHelper.WriteLog("\r\n \r\n   " + "->：[" + msg + "]");
            var messageBuffer = Encoding.UTF8.GetBytes(Common.ZipHelper.GZipCompressString(msg));
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / sendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (sendChunkSize * i);
                var count = sendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                LogStatus(false, messageBuffer, messageBuffer.Length);

                await webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, CancellationToken.None);
                countTime.Stop();
                if (!string.IsNullOrEmpty(interfaceName))
                {
                    string timeMsg = "\r\n [" + interfaceName + "] Time-Counsuming:" + countTime.Elapsed.TotalMilliseconds + "  (毫秒)";
                    Console.WriteLine(timeMsg);
                    FileHelper.WriteLog(timeMsg);
                }
            }


        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            
            byte[] buffer = new byte[receiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    LogStatus(true, buffer, result.Count);
                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = receiving ? ConsoleColor.Green : ConsoleColor.Gray;
                string dataLength = string.Format("\r\n   " + "{0} {1} bytes... ", receiving ? "Received" : "Sent", length);
                Console.WriteLine(dataLength);
                FileHelper.WriteLog("\r\n" + dataLength);
                if (verbose)
                {
                    string byteStr = string.Format("\r\n   " + (receiving ? "<-" : "->") + "byteArray：[" + BitConverter.ToString(buffer, 0, length) + "]");
                    Console.WriteLine(byteStr);
                    FileHelper.WriteLog("\r\n" + byteStr);

                    string baseStr = string.Format("\r\n   " + (receiving ? "<-" : "->") + "Base64：[" + Encoding.Default.GetString(buffer, 0, length) + "]");
                    Console.WriteLine(baseStr);
                    FileHelper.WriteLog("\r\n" + baseStr);

                    //string s = "H4sIAAAAAAAEAAEVAOr/5rKh5pyJ5o6l5pS25Yiw5pWw5o2ug4v/CxUAAAA=";
                    //Console.WriteLine(ZipHelper.GZipDecompressString(s));
                    if(receiving)
                    {
                        //string strMsg = string.Format("\r\n   " + (receiving ? "<-" : "->") + "UTF8：[" + ZipHelper.GZipDecompressString(Encoding.UTF8.GetString(buffer, 0, length)) + "]");
                        Console.WriteLine("\r\n   " + (receiving ? "<-" : "->") + "UTF8：[" + ZipHelper.GZipDecompressString(Encoding.UTF8.GetString(buffer, 0, length)) + "]");
                        FileHelper.WriteLog("\r\n \r\n  " + (receiving ? "<-" : "->") + "UTF8：[" + ZipHelper.GZipDecompressString(Encoding.UTF8.GetString(buffer, 0, length)) + "]");
                    }  
                    Thread.Sleep(100);
                    Console.WriteLine("\r\n 请按任意键继续,按q推出");
                        
                }

                Console.ResetColor();
            }
        }
    }
}
