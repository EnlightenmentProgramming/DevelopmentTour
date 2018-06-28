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
        public T_Config GetConfig()
        {
            try
            {
                T_Config config = new T_Config();
                string id = Db.Context_SqlServer.FromSql("select logname from T_Agent where logname='888999'").ToScalar<string>();
                return Db.Context_Sqlite.From<T_Config>().Where(d => d.id == 999).ToFirst();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(T_ConfigDAL), ex);
                return null;
            }
        }
    }
}
