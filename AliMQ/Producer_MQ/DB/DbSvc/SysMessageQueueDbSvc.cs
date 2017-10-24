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
    public class SysMessageQueueDbSvc : BaseDbSvc
    {

        public SysMessageQueueDbSvc()
        {

        }

        public SysMessageQueueDbSvc(System.Data.SqlClient.SqlConnection connection)
            : base(connection)
        {

        }

        public List<SysMessageQueue> List()
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysMessageQueue>(@"Select * From SysMessageQueue")).ToList();
            }
        }

        public List<SysMessageQueue> ListUnSend(int status)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysMessageQueue>(@"Select * From SysMessageQueue Where Status = @Status", new { Status = status })).ToList();
            }
        }

        public SysMessageQueue Get(string id)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysMessageQueue>(@"Select * From SysMessageQueue Where Id = @Id", new { Id = id })).FirstOrDefault();
            }
        }

        public SysMessageQueue GetByKey(string key)
        {
            using (EnsureUserConnection())
            {
                return (DbConnection.Query<SysMessageQueue>(@"Select * From SysMessageQueue Where Keys = @Keys", new { Keys = key })).FirstOrDefault();
            }
        }

        public void Insert(SysMessageQueue info, SqlTransaction transaction)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Insert SysMessageQueue(Id, Tags, Keys, MsgCont, Status, ProducerID, TopicID)
                                            values (@Id, @Tags, @Keys, @MsgCont, @Status, @ProducerID, @TopicID)",
                                            info, transaction);
            }
        }

        public void Update(SysMessageQueue info)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysMessageQueue Set 
                                            Tags = @Tags,
                                            Keys = @Keys,
                                            MsgCont = @MsgCont,
                                            Status = @Status 
                                            ProducerID = @ProducerID,
                                            TopicID = @TopicID,
                                            Where Id = @Id",
                                            new
                                            {
                                                Id = info.Id,
                                                Tags = info.Tags,
                                                Keys = info.Keys,
                                                MsgCont = info.MsgCont
                                            });
            }
        }

        public void UpdateStatus(SysMessageQueue info, SqlTransaction transaction = null)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Update SysMessageQueue Set 
                                            Status = @Status 
                                            Where Id = @Id",
                                           info, transaction);
            }
        }

        public void Delete(string id)
        {
            using (EnsureUserConnection())
            {
                DbConnection.Execute(@"Delete From SysMessageQueue Where Id = @Id", new { Id = id });
            }
        }

    }
}
