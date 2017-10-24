using System;
using System.Data;
using System.Collections.Generic;

namespace DBModel
{
    public partial class SysConsumer
    {
        public SysConsumer()
        {
            Id = 0;
            SysMsgID = string.Empty;
            MQMsgID = string.Empty;
            ConsumerID = string.Empty;
            Status = 0;
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        public string SysMsgID { get; set; }

        public string MQMsgID { get; set; }

        public string ConsumerID { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
