using System;
using System.Data;
using System.Collections.Generic;

namespace DBModel
{
    public partial class SysMessageQueue
    {
        public SysMessageQueue()
        {
            Id = string.Empty;
            Tags = string.Empty;
            Keys = string.Empty;
            MsgCont = string.Empty;
            Status = 0;
            ProducerID = string.Empty;
            TopicID = string.Empty;
        }

        public string Id { get; set; }
        public string Tags { get; set; }
        public string Keys { get; set; }
        public string MsgCont { get; set; }
        public int Status { get; set; }
        public string ProducerID { get; set; }
        public string TopicID { get; set; }
    }
}
