using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

using DBPresistence;
using DBModel;
using Producer_MQ;

namespace Business
{
    public class UserService
    {
        public static void RegUserServer(string userID, string topicID, string producerID)
        {
            SysUser user = new SysUser();
            user.Id = userID;
            user.Status = 1;

            using (SqlConnection connection = DBConnectionMgr.GetUserConnection())
            {
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    new SysUserDbSvc(connection).Insert(user, transaction);
                    SysMessageQueue sysMQ = new SysMessageQueue()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Keys = Guid.NewGuid().ToString(),
                        MsgCont = userID,
                        Status = OnsMQDefine.SysMessage_Status_UnSend,
                        Tags = OnsMQDefine.MessageTag_TestLog,//tag 看业务需要，可以不传入，为查询方便，建议传入
                        ProducerID = producerID,
                        TopicID = topicID
                    };
                    new SysMessageQueueDbSvc(connection).Insert(sysMQ, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}
