using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class FileHelper
    {
        public static readonly string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//程序所在目录
         //读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
         static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        public static Dictionary<int, string[]> ReadFile(string fileName)
        {
            try
            {
                string[] allLines = File.ReadAllLines(path + "\\" + fileName);
                List<int> indexFunNames = new List<int>();//所有接口名称的行索引
                Dictionary<int, string[]> packDic = new Dictionary<int, string[]>();

                if (allLines != null && allLines.Length > 0)
                {
                    int count = 0;
                    while (count < allLines.Length)
                    {
                        if (allLines[count].IndexOf("$=>") != -1)
                        {
                            indexFunNames.Add(count);
                        }
                        count++;
                    }
                    if (indexFunNames != null && indexFunNames.Count > 0)
                    {
                        for (int i = 0; i < indexFunNames.Count; i++)
                        {
                            int requestIndex = -1, responseIndex = -1;
                            StringBuilder requestMsg = new StringBuilder();
                            StringBuilder responseMsg = new StringBuilder();                           
                            int end = (i == indexFunNames.Count - 1) ? allLines.Length : indexFunNames[i + 1];
                            for (int j = indexFunNames[i]; j < end; j++)
                            {
                                if(allLines[j].IndexOf("$->") !=-1)
                                {
                                    requestIndex = j;
                                }
                                if(allLines[j].IndexOf("$<-") !=-1)
                                {
                                    responseIndex = j;
                                }                                
                            }

                            if (requestIndex != -1 && responseIndex != -1)
                            {
                                for (int k = requestIndex; k < responseIndex; k++)
                                {
                                    requestMsg.Append(allLines[k].Replace("$->", "").Replace("$<-", "").Replace("\r", "").Replace("\n", "").Trim());
                                }
                                for (int m = responseIndex; m < end; m++)
                                {
                                    responseMsg.Append(allLines[m].Replace("$->", "").Replace("$<-", "").Replace("\r", "").Replace("\n", "").Trim());
                                }
                                if (!string.IsNullOrEmpty(requestMsg.ToString()))
                                {
                                    packDic.Add(i+1, new string[] { allLines[indexFunNames[i]].Replace("$=>", "").Replace("\r", "").Replace("\n", "").Trim(), requestMsg.ToString(), responseMsg.ToString() });
                                }
                            }
                        }
                    }
                }//判断文件内容是否为空结束
                return packDic;
            }
            catch (Exception ex)
            {
                Common.LogHelper.WriteLog(typeof(FileHelper), ex);
                throw;
            }
        }


        public static void WriteLog(string msg)
        {
            try
            {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                 //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                 //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                 //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                 LogWriteLock.EnterWriteLock();
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                FileStream fs = new FileStream(path + "\\log\\" + fileName, FileMode.Append);
                StreamWriter write = new StreamWriter(fs, Encoding.UTF8);
                write.Write(msg);
                write.Flush();
                write.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                               
            }
            finally
            {
                 //退出写入模式，释放资源占用
                //注意：一次请求对应一次释放
                //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
                 LogWriteLock.ExitWriteLock();
            }
        }

        public static void WriteLog(byte[] msg)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            FileStream fs = new FileStream(path + "\\log\\" + fileName, FileMode.Append);
            fs.Write(msg, 0, msg.Length);
            fs.Flush();
            fs.Close();
        }
    }
}
