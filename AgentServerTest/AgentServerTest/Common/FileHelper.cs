using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileHelper
    {
        public static readonly string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//程序所在目录
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

    }
}
