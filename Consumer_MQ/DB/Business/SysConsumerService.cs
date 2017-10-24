using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using DBModel;
using DBPresistence;

namespace Business
{
    public class SysConsumerService
    {
        internal const int SysMQ_CostStatus_UnConsume = 0;
        internal const int SysMQ_CostStatus_Consumed = 1;
        public static List<SysConsumer> ListUnConsumeByConsumerID(string consumerID)
        {
            return new SysConsumerDbSvc().ListUnConsumeByConsumerID(consumerID, SysMQ_CostStatus_UnConsume);
        }
    }
}