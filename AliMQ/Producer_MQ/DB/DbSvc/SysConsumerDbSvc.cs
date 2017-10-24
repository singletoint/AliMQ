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
    public class SysConsumerDbSvc : BaseDbSvc
    {

        public SysConsumerDbSvc()
        {

        }

        public SysConsumerDbSvc(System.Data.SqlClient.SqlConnection connection)
            : base(connection)
        {

        }

        public List<SysConsumer> List()
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer")).ToList();
            }
        }

        public SysConsumer Get(string id)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer Where Id = @Id", new { Id = id })).FirstOrDefault();
            }
        }

        public SysConsumer Get(string consumerID, string sysMsgID, SqlTransaction transaction = null)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer Where ConsumerID = @ConsumerID and SysMsgID = @SysMsgID", new { ConsumerID = consumerID, SysMsgID = sysMsgID }, transaction)).FirstOrDefault();
            }
        }

        public void Insert(SysConsumer info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Insert SysConsumer(SysMsgID, MQMsgID, ConsumerID, Status, CreatedDate)
                                            values (@SysMsgID, @MQMsgID, @ConsumerID, @Status, @CreatedDate)",
                                            info);
            }
        }

        public void Update(SysConsumer info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysConsumer Set 
                                            SysMsgID = @SysMsgID,
                                            MQMsgID = @MQMsgID,
                                            ConsumerID = @ConsumerID,
                                            Status = @Status,
                                            CreatedDate = @CreatedDate 
                                            Where Id = @Id",
                                            info);
            }
        }

        public void UpdateStatus(SysConsumer info, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysConsumer Set                                             
                                            Status = @Status
                                            Where SysMsgID = @SysMsgID 
                                            and ConsumerID = @ConsumerID",
                                            info, transaction);
            }
        }

        public void Delete(string id)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Delete From SysConsumer Where Id = @Id", new { Id = id });
            }
        }

    }
}
