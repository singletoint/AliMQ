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

        public List<SysConsumer> ListUnConsumeByConsumerID(string consumerID, int status)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer Where ConsumerID = @ConsumerID and Status = @Status", new { ConsumerID = consumerID, Status = status })).ToList();
            }
        }

        public List<SysConsumer> ListByKeyList(string consumerID, List<string> keys)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer Where ConsumerID = @ConsumerID and Keys In @Keys", new { Keys = keys, ConsumerID = consumerID })).ToList();
            }
        }

        public SysConsumer Get(string id)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer Where Id = @Id", new { Id = id })).FirstOrDefault();
            }
        }

        public SysConsumer Get(string consumerID, string Key, SqlTransaction transaction = null)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysConsumer>(@"Select * From SysConsumer Where ConsumerID = @ConsumerID and Keys = @Keys", new { ConsumerID = consumerID, Keys = Key }, transaction)).FirstOrDefault();
            }
        }

        public void Insert(SysConsumer info, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Insert SysConsumer(SysMsgID, MQMsgID, Keys, MsgCont, ConsumerID, Status, CreatedDate)
                                            values (@SysMsgID, @MQMsgID, @Keys, @MsgCont, @ConsumerID, @Status, @CreatedDate)",
                                            info, transaction);
            }
        }

        public void Update(SysConsumer info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysConsumer Set 
                                            SysMsgID = @SysMsgID,
                                            MQMsgID = @MQMsgID,
                                            Keys = @Keys,
                                            MsgCont = @MsgCont,
                                            ConsumerID = @ConsumerID,
                                            Status = @Status,
                                            CreatedDate = @CreatedDate 
                                            Where Id = @Id",
                                            info);
            }
        }

        public void UpdateStatus(int id, int status, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysConsumer Set                                             
                                            Status = @Status
                                            Where Id = @Id",
                                            new { Id = id, Status = status }, transaction);
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
