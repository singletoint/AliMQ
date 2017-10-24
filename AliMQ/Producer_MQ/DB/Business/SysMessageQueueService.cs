using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DBPresistence;
using DBModel;
using System.Data.SqlClient;
using Producer_MQ;

namespace Business
{
    public class SysMessageQueueService
    {
        public static List<SysMessageQueue> ListByUnSent()
        {
            return new SysMessageQueueDbSvc().ListUnSend(OnsMQDefine.SysMessage_Status_UnSend);
        }
    }
}
