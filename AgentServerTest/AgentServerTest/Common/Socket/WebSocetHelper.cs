using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Socket
{
    public class WebSocetHelper
    {
        public static ClientWebSocket ws;
        public static async Task ConnectSocket()
        {
            using (ws = new ClientWebSocket())
            {
                Uri serverUri = new Uri("ws://192.168.1.106:27000");
                await ws.ConnectAsync(serverUri, CancellationToken.None);
                while (ws.State == WebSocketState.Open)
                {
                    Console.Write("Input message ('exit' to exit): ");
                    string msg = Console.ReadLine();
                    if (msg == "exit")
                    {
                        break;
                    }
                    //ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                    //await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                    await SendMsg(msg);
                    Console.WriteLine("请求报文："+msg);
                    ArraySegment<byte> result = await ReciveMsg();
                    string encodeMsg = ZipHelper.GZipDecompressString(Encoding.Default.GetString(result.Array, 0, result.Count));//.Replace("\r", "").Replace("\n", "").Trim()
                    Console.WriteLine("应答报文：" + Encoding.Default.GetString(result.Array, 0, result.Count));
                }
            }
        }

        public static async Task<bool> SendMsg(string msg)
        {
           if(ws.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            return false;
        }


        public static async Task<ArraySegment<byte>> ReciveMsg()
        {
           if(ws.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                return bytesReceived;
            }
            return new ArraySegment<byte>();
        }

    }
}
