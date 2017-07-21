using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBModel
{
    public class OnsMQDefine
    {
        //阿里云消息队列 Tag
        public const string MessageTag_TestLog = "Reg_JC_Test";

        //系统记录消息状态
        public const int SysMessage_Status_UnSend = 0;
        public const int SysMessage_Status_Sent = 1;
    }
}
