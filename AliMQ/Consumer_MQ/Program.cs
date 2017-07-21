using Business;
using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer_MQ
{
    class Program
    {
        static void Main(string[] args)
        {
            //这里只处理接收消息相关代码
            //初始化,并启动消息客户端
            const string Ons_RegionID = "cn-qingdao";
            const string Ons_AccessKey = "LTAIaKubhEz8ilWR";
            const string Ons_SecretKey = "4hOOTW86t1ePPV9BGKlClK0PCT8ds8";

            string Ons_TopicID = "Topic-JinCi_Test";

            //域名来源于 ali https://help.aliyun.com/document_detail/29574.html?spm=5176.doc29573.6.601.HF6USe
            string Url = "http://publictest-rest.ons.aliyun.com/message";

            string Ons_ConsumerID_Mail = "CID-JinCi_Mail";
            OnsConsumer_Http consumerMail_http = new OnsConsumer_Http(Ons_RegionID, Ons_AccessKey, Ons_SecretKey, Ons_TopicID, Ons_ConsumerID_Mail, Url);
            consumerMail_http.PullMessage();

            string Ons_ConsumerID_Welfare = "CID-JinCi_Welfare";
            OnsConsumer_Http consumerWelfare_http = new OnsConsumer_Http(Ons_RegionID, Ons_AccessKey, Ons_SecretKey, Ons_TopicID, Ons_ConsumerID_Welfare, Url);
            consumerWelfare_http.PullMessage();


            //业务系统执行消费消息。
            List<SysConsumer> sysConsumerMailList = SysConsumerService.ListUnConsumeByConsumerID(Ons_ConsumerID_Mail);
            foreach (var sysConsumer in sysConsumerMailList)
            {
                SysUserWelfareService.SendEmailExcuteService(sysConsumer);
            }

            List<SysConsumer> sysConsumerWelfareList = SysConsumerService.ListUnConsumeByConsumerID(Ons_ConsumerID_Welfare);
            foreach (var sysConsumer in sysConsumerWelfareList)
            {
                SysUserWelfareService.SendEmailExcuteService(sysConsumer);
            }

            Console.ReadKey();
        }
    }
}
