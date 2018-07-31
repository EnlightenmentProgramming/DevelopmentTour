using Dos.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ORMTool
{
    public class Db
    {
        //sqlserver上下文
        public static readonly DbSession Context_SqlServer = new DbSession("SqlServerConn");
        //sqlite上下文
        public static readonly DbSession Context_Sqlite = new DbSession("SqliteConn");
    }
}
