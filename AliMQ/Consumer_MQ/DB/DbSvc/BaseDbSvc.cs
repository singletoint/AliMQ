using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace DBPresistence
{
    public class BaseDbSvc
    {

        protected SqlConnection DbConnection = null;
        public BaseDbSvc()
        {
        }

        public BaseDbSvc(SqlConnection conn)
        {
            DbConnection = conn;
        }

        public SqlConnection EnsureUserConnection()
        {
            if (DbConnection == null)
            {
                DbConnection = DBConnectionMgr.GetUserConnection();
                return DbConnection;
            }
            else return null;
        }
    }
}
