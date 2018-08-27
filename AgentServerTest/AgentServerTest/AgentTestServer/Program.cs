using Common;
using Common.Socket;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgentTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string ip = ConfigurationManager.AppSettings["Ip"], port = ConfigurationManager.AppSettings["Port"];
                string uri = "ws://" + ip.Trim() + ":" + port.Trim();//"ws://192.168.1.106:27000"

                Console.WriteLine(" 服务端：" + uri + "\r\n");

                Dictionary<int, string[]> packDic = FileHelper.ReadFile("Packet.txt");

                string sendMsg = "",name="";
                while (true)
                {
                    Console.WriteLine(" 输入指定的序号，调用对应接口");
                    if (packDic != null && packDic.Count > 0)
                    {
                        foreach (var item in packDic)
                        {
                            Console.WriteLine(" [" + item.Key + "] \t " + item.Value[0]);
                        }
                    }
                    else
                    {
                        Console.WriteLine(" 没有读取到包配置");
                    }

                    Console.WriteLine(" Press q key to exit...\r\n");

                    WebSocketClient.Connect(uri).GetAwaiter();
                    //AsyncContext
                    int packNum = 0;

                    if (int.TryParse(Console.ReadLine(), out packNum))
                    {
                        sendMsg = packDic[packNum][1];
                        name = packDic[packNum][0];
                        //switch (packNum)
                        //{
                        //    case 1:
                        //        //Console.WriteLine("调用1");
                        //        sendMsg = packDic[1][1];
                        //        name = packDic[1][]
                        //        break;
                        //    case 2:
                        //        sendMsg = packDic[2][1];
                        //        break;
                        //}
                        WebSocketClient.Send(sendMsg,name).GetAwaiter();
                    }

                    if (Console.ReadLine().Trim().ToUpper() == "Q")
                    {
                        Environment.Exit(0);
                    }
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

            //WebSocketWrapper wsWrapper = WebSocketWrapper.Create("ws://192.168.1.106:27000");
            //wsWrapper.Connect();

            //Console.ReadKey();
        }

    }
}
