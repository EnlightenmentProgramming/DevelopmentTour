using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dos.ORM.WinForm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
           // var db20160927 = new DbSession(DatabaseType.MySql, "Data Source=127.0.0.1;Database=ITdos;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
            var db20160927 = new DbSession(DatabaseType.SqlServer9, "Server=DSER\\DATASER;uid=gu;pwd=System&8899HSCOREi7;database=Higo.Game.HS;Connect Timeout=10;");
            DataTable t =  db20160927.FromSql("select * from T_Agent where Len(LogName) =5").ToDataTable();
        }
    }
}
