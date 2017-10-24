using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business;
using DBModel;
using System.Threading;

namespace Producer_MQ
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 注册用户，生成测试数据
            //参数来自于阿里云控制台
            //消息队列，发送 TopicID
            string Ons_Topic_Test = "Topic-JinCi_Test";
            //消息队列，发送 ProducerID
            string Ons_ProducerID_Test = "PID-JinCi_Test";
            //for (int i = 0; i < 30; i++)
            //{
            //    //注册用户，生成数据
            //    UserService.RegUserServer(Guid.NewGuid().ToString(), Ons_Topic_Test, Ons_ProducerID_Test);
            //}
            //注册用户，生成数据
            UserService.RegUserServer(Guid.NewGuid().ToString(), Ons_Topic_Test, Ons_ProducerID_Test);
            #endregion

            const string Ons_RegionID = "cn-qingdao";
            const string Ons_AccessKey = "LTAIaKubhEz8ilWR";
            const string Ons_SecretKey = "4hOOTW86t1ePPV9BGKlClK0PCT8ds8";

            //域名来源于 ali https://help.aliyun.com/document_detail/29574.html?spm=5176.doc29573.6.601.HF6USe
            string Url = "http://publictest-rest.ons.aliyun.com/message";

            //读取数据库待发送的消息列表
            List<SysMessageQueue> sysMQList = SysMessageQueueService.ListByUnSent();

            //如果TopicID，ProducerID 一致时可以采用循环发送
            //var sysMQFirst = sysMQList.First();
            //发送方法送前初始化消息生产者

            Ons_Produce_Http onsProducer = new Ons_Produce_Http(Ons_RegionID, Ons_AccessKey, Ons_SecretKey, Ons_Topic_Test, Ons_ProducerID_Test, Url);
            foreach (var sysMQ in sysMQList)
            {
                onsProducer.SendMessage(sysMQ);
            }

            Console.ReadKey();
        }
    }
}
