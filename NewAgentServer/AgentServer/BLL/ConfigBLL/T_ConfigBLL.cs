using DAL.ConfigDAL;
using Model.SqliteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ConfigBLL
{
    public class T_ConfigBLL
    {
        T_ConfigDAL configDal = new T_ConfigDAL();
        /// <summary>
        /// 获取配置对象
        /// </summary>
        /// <returns></returns>
        public T_Config GetConfig()
        {
            return configDal.GetConfig();
        }
    }
}
