using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer_MQ
{
    public class OnsConsumerResult
    {
        public string body { get; set; }
        public string msgId { get; set; }
        public string bornTime { get; set; }
        public string msgHandle { get; set; }
        public int reconsumeTimes { get; set; }
        public string tag { get; set; }
        public string key { get; set; }
    }
}
