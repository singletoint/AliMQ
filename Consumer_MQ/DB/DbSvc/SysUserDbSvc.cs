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
    public class SysUserDbSvc : BaseDbSvc
    {

        public SysUserDbSvc()
        {

        }

        public SysUserDbSvc(System.Data.SqlClient.SqlConnection connection)
            : base(connection)
        {

        }

        public List<SysUser> List()
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysUser>(@"Select * From SysUser")).ToList();
            }
        }

        public SysUser Get(string id, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysUser>(@"Select * From SysUser Where Id = @Id", new { Id = id }, transaction)).FirstOrDefault();
            }
        }

        public void Insert(SysUser info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Insert SysUser(Id, Status)
                                            values (@Id, @Status)",
                                            info);
            }
        }

        public void Insert(SysUser info, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Insert SysUser(Id, Status)
                                            values (@Id, @Status)",
                                             info, transaction);
            }
        }

        public void Update(SysUser info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysUser Set 
                                            Status = @Status 
                                            Where Id = @Id",
                                            new
                                            {
                                                Id = info.Id,
                                                Status = info.Status
                                            });
            }
        }

        public void Delete(string id)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Delete From SysUser Where Id = @Id", new { Id = id });
            }
        }

    }
}
