using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Data.SqlClient;

using DBModel;
using DBPresistence;
using ons;

using Aliyun.Acs.Core.Profile;
using System.Timers;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core;
using Aliyun.Acs.Ons.Model;
using Aliyun.Acs.Core.Exceptions;

namespace Consumer_MQ
{
    public class MyTimer : Timer
    {
        public ConsumerParam OnsConsumerParam { get; set; }
        public bool IsRuning { get; set; }
    }

    public class ConsumerParam
    {
        public string Ons_RegionID { get; set; }
        public string Ons_AccessKey { get; set; }
        public string Ons_SecretKey { get; set; }
        public string TopicID { get; set; }
        public string ConsumerID { get; set; }
    }

    public class OnsConsumer
    {


        //access
        //string Ons_RegionID;
        //string Ons_AccessKey;
        //string Ons_SecretKey;
        //string TopicID;
        //string ConsumerID;
        //PushConsumer _consumer;

        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //public OnsConsumer(string regionID, string accessKey, string secretKey, string topicID, string consumerID)
        //{
        //    Ons_RegionID = regionID;
        //    Ons_AccessKey = accessKey;
        //    Ons_SecretKey = secretKey;
        //    TopicID = topicID;
        //    ConsumerID = consumerID;
        //}

        private static ONSFactoryProperty getFactoryProperty(string accessKey, string secretKey, string topicID, string consumerID)
        {
            ONSFactoryProperty factoryInfo = new ONSFactoryProperty();
            factoryInfo.setFactoryProperty(ONSFactoryProperty.AccessKey, accessKey);
            factoryInfo.setFactoryProperty(ONSFactoryProperty.SecretKey, secretKey);
            factoryInfo.setFactoryProperty(ONSFactoryProperty.PublishTopics, topicID);
            factoryInfo.setFactoryProperty(ONSFactoryProperty.ConsumerId, consumerID);
            factoryInfo.setFactoryProperty(ONSFactoryProperty.LogPath, "D://log");
            return factoryInfo;
        }

        /// <summary>
        /// 启动消息消费客户端
        /// </summary>
        public static void StartPushConsumer(string regionID, string accessKey, string secretKey, string topicID, string consumerID)
        {
            PushConsumer _consumer = ONSFactory.getInstance().createPushConsumer(getFactoryProperty(accessKey, secretKey, topicID, consumerID));
            _consumer.subscribe(topicID, "*", new ConsumerListener(consumerID));
            _consumer.start();

            ConsumerParam consumerParam = new ConsumerParam()
            {
                Ons_RegionID = regionID,
                Ons_AccessKey = accessKey,
                Ons_SecretKey = secretKey,
                TopicID = topicID,
                ConsumerID = consumerID
            };

            _Consumer_TopicDict.Add(consumerID + String_Splite_Char + topicID, _consumer);

            //MyTimer watchConsumerAccumulate = new MyTimer();
            //watchConsumerAccumulate.Elapsed += ConsumerAccumulateEvent;
            //watchConsumerAccumulate.OnsConsumerParam = consumerParam;
            //watchConsumerAccumulate.Interval = 3000;
            //watchConsumerAccumulate.Start();
        }
        public const char String_Splite_Char = '|';

        //消费者集合
        public static Dictionary<string, PushConsumer> _Consumer_TopicDict = new Dictionary<string, PushConsumer>();

        //public static void ConsumerAccumulateEvent(object sender, ElapsedEventArgs e)
        //{
        //    MyTimer timer = ((MyTimer)sender);
        //    if (timer.IsRuning)
        //        return;
        //    timer.IsRuning = true;
        //    try
        //    {
        //        //消息堆剩余消息数量
        //        if (OnsConsumerAccumulate(
        //            timer.OnsConsumerParam.Ons_RegionID,
        //            timer.OnsConsumerParam.Ons_AccessKey,
        //            timer.OnsConsumerParam.Ons_SecretKey,
        //            timer.OnsConsumerParam.TopicID,
        //            timer.OnsConsumerParam.ConsumerID
        //            ) == 0)
        //        {
        //            _Consumer_TopicDict[timer.OnsConsumerParam.ConsumerID + String_Splite_Char + timer.OnsConsumerParam.TopicID].shutdown();
        //            timer.Dispose();
        //        }
        //    }
        //    finally
        //    {
        //        timer.IsRuning = false;
        //    }
        //}

        /// <summary>
        /// 关闭消息消费客户端
        /// </summary>
        //public void ShutdownPushConsumer()
        //{
        //    _consumer.shutdown();
        //    Console.WriteLine(ConsumerID + "ShutdownPushConsumer");
        //}

        //#region AliQM 查询消息堆状态

        //private static long OnsConsumerAccumulate(string regionID, string accessKey, string secretKey, string topicID, string consumerID)
        //{
        //    try
        //    {
        //        IClientProfile profile = DefaultProfile.GetProfile(regionID, accessKey, secretKey);
        //        DefaultAcsClient iAcsClient = new DefaultAcsClient(profile);
        //        OnsConsumerAccumulateRequest request = new OnsConsumerAccumulateRequest();

        //        request.OnsRegionId = regionID;
        //        request.PreventCache = GetCurrentTimeMillis();
        //        request.AcceptFormat = FormatType.JSON;
        //        request.ConsumerId = consumerID;

        //        OnsConsumerAccumulateResponse response = iAcsClient.GetAcsResponse(request);
        //        Console.WriteLine(consumerID + "TotalDiff:" + response.Data.TotalDiff);
        //        return (long)response.Data.TotalDiff;
        //    }
        //    catch (ServerException)
        //    {
        //        throw;
        //    }
        //    catch (ClientException)
        //    {
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// 获取当前时间戳
        ///// </summary>
        ///// <returns></returns>
        //private static long GetCurrentTimeMillis()
        //{
        //    return (long)((DateTime.Now.ToUniversalTime() - Jan1st1970).TotalMilliseconds);
        //}
        //#endregion
    }
}
