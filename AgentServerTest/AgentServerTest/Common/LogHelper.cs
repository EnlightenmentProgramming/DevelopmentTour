using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.DOMConfigurator(Watch = true)]
namespace Common
{
    public static class LogHelper
    {
        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void WriteLog(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error", ex);
        }
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error(msg);
        }
        /// <summary>
        /// 写提示消息日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteInfoLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }
        /// <summary>
        /// 写警告日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteWarnLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Warn(msg);
        }
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteErrLog(Type t,string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error(msg);
        }
    }
}
