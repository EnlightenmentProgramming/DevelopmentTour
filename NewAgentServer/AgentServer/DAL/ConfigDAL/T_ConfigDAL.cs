using Common;
using Common.ORMTool;
using Model.SqliteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ConfigDAL
{
    public class T_ConfigDAL
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        public T_Config GetConfig()
        {
            try
            {
                T_Config config = new T_Config();
                //string id = Db.Context_SqlServer.FromSql("select logname from T_Agent where logname='888999'").ToScalar<string>();
                config = Db.Context_Sqlite.From<T_Config>().Where(d => d.id == 999).ToFirst();
                if(config ==null)
                {
                    config.id = 999;
                    config.Ip = "127.0.0.1";
                    config.Port = 27000;
                    config.IsAutoCut = 1;
                    config.AutoCutTime = 30;
                    config.LockPass = CommonHelper.MD5("123");
                    Db.Context_Sqlite.Insert<T_Config>();
                }
                return config;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(T_ConfigDAL), ex);
                return null;
            }
        }
        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateConfig(T_Config model)
        {
            try
            {
                return Db.Context_Sqlite.Update<T_Config>(model) > 0;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(T_ConfigDAL), ex);
                return false;
            }
        }

    }
}
