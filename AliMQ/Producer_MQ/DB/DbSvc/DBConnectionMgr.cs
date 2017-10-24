using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace DBPresistence
{
    public class DBConnectionMgr
    {
        public static string DbConnectionString = ConfigurationManager.ConnectionStrings["sqlConnectionString"].ToString();

        public static SqlConnection GetUserConnection()
        {
            SqlConnection mysqlConn = new SqlConnection(DbConnectionString);
            mysqlConn.Open();
            return mysqlConn;
        }
    }
}
