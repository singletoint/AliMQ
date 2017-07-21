using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DBModel;
using Dapper;
using System.Data.SqlClient;

namespace DBPresistence
{
    public class SysUserWelfareDbSvc : BaseDbSvc
    {

        public SysUserWelfareDbSvc()
        {

        }

        public SysUserWelfareDbSvc(System.Data.SqlClient.SqlConnection connection): base(connection)
        {

        }

        public List<SysUserWelfare> List()
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysUserWelfare>(@"Select * From SysUserWelfare")).ToList();
            }
        }

        public SysUserWelfare Get(string id)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysUserWelfare>(@"Select * From SysUserWelfare Where Id = @Id", new { Id = id })).FirstOrDefault();
            }
        }

        public void Insert(SysUserWelfare info, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Insert SysUserWelfare(UserID, WelfareNum, CreatedDate)
                                            values (@UserID, @WelfareNum, @CreatedDate)",
                                            info, transaction);
            }
        }

        public void Update(SysUserWelfare info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysUserWelfare Set 
                                            WelfareNum = @WelfareNum,
                                            CreatedDate = @CreatedDate 
                                            Where Id = @Id",
                                            new
                                            {
                                                Id = info.Id,
                                                WelfareNum = info.WelfareNum,
                                                CreatedDate = info.CreatedDate
                                            });
            }
        }

        public void Delete(string id)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Delete From SysUserWelfare Where Id = @Id", new { Id = id });
            }
        }

    }
}
