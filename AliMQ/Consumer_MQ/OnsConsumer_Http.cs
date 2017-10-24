using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Jil;
using System.Data.SqlClient;
using DBModel;
using DBPresistence;

using System.Security.Cryptography;

namespace Consumer_MQ
{
    public class OnsConsumer_Http
    {
        private const int ErrorCode_Duplicate = -2146232060;
        private const string SignType_HMACSHA1 = "HMACSHA1";

        string Ons_RegionID;
        string Ons_AccessKey;
        string Ons_SecretKey;
        string TopicID;
        string ConsumerID;
        string Url;

        public OnsConsumer_Http(string regionID, string accessKey, string secretKey, string topicID, string consumerID, string url)
        {
            Ons_RegionID = regionID;
            Ons_AccessKey = accessKey;
            Ons_SecretKey = secretKey;
            TopicID = topicID;
            ConsumerID = consumerID;
            Url = url;
        }

        //时间戳起点。
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string NEWLINE = "\n";

        public void PullMessage()
        {
            List<OnsConsumerResult> list = PullMessageFromAli();
            while (list.Count > 0)
            {
                Dictionary<string, SysMessageQueue> sysMQDict = new SysMessageQueueDbSvc().ListByKeyList(list.Select(t => t.key).ToList()).ToDictionary(t => t.Keys);
                Dictionary<string, SysConsumer> sysConsumerDict = new SysConsumerDbSvc().ListByKeyList(ConsumerID, list.Select(t => t.key).ToList()).ToDictionary(t => t.Keys);

                foreach (var msg in list)
                {
                    string msgKey = msg.key;
                    //发送消息表，一定存在这条数据，如果不存在一定是非法添加消息
                    SysMessageQueue sysMQ;
                    if (!sysMQDict.TryGetValue(msgKey, out sysMQ))
                    {
                        continue;//非法消息
                    }

                    SysConsumer consumer;
                    //如果数据库记录已经消费，则执行消费阿里云 MQ 消费，删除消息。
                    if (sysConsumerDict.TryGetValue(msgKey, out consumer))
                    {
                        DeleteMessage(msg.msgHandle);
                        continue;
                    }

                    //客户消费消息，写入数据库记录
                    using (SqlConnection connection = DBConnectionMgr.GetUserConnection())
                    {
                        using (SqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                consumer = new SysConsumer()
                                {
                                    SysMsgID = sysMQ.Id,
                                    Keys = msgKey,
                                    MQMsgID = msg.msgId,
                                    MsgCont = sysMQ.MsgCont,
                                    ConsumerID = ConsumerID
                                };

                                SysConsumerDbSvc dbSvc = new SysConsumerDbSvc(connection);
                                dbSvc.Insert(consumer, transaction);
                                transaction.Commit();

                                if (DeleteMessage(msg.msgHandle))
                                    Console.WriteLine(consumer.ConsumerID + sysMQ.MsgCont);
                            }
                            catch (SqlException ex)
                            {
                                //如果数据库已经存在，抛出重复插入异常
                                if (ex.ErrorCode == ErrorCode_Duplicate)
                                {
                                    DeleteMessage(msg.msgHandle);
                                }
                            }
                        }
                    }
                }
                //批处理，一次读取32
                list = PullMessageFromAli();
            }
        }

        /// <summary>
        /// 从 AliMQ 拉取消息
        /// </summary>
        /// <returns>消息列表</returns>
        private List<OnsConsumerResult> PullMessageFromAli()
        {
            List<OnsConsumerResult> result = null;
            long time = GetCurrentTimeMillis();

            //数据签名
            String signString = TopicID + NEWLINE + ConsumerID + NEWLINE + time;
            string sign = SignString(signString, Ons_SecretKey);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Signature", sign);
            headers.Add("AccessKey", Ons_AccessKey);
            headers.Add("ConsumerID", ConsumerID);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("topic", TopicID);
            parameters.Add("time", time.ToString());
            parameters.Add("num", 32.ToString());
            parameters.Add("Signature", sign);

            HttpWebResponseEx res = WebHttpHepper.SendRequest(Url, "GET", headers, parameters);
            result = JSON.Deserialize<List<OnsConsumerResult>>(res.RetureValue);
            return result;
        }

        /// <summary>
        /// 删除 aliMQ 上的消息
        /// </summary>
        /// <param name="msgHandle">msgHandle 查询时返回对象的属性</param>
        /// <returns>是否成功</returns>
        private bool DeleteMessage(String msgHandle)
        {
            long time = GetCurrentTimeMillis();

            String signString = TopicID + NEWLINE + ConsumerID + NEWLINE + msgHandle + NEWLINE + time;
            string sign = SignString(signString, Ons_SecretKey);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Signature", sign);
            headers.Add("AccessKey", Ons_AccessKey);
            headers.Add("ConsumerID", ConsumerID);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("topic", TopicID);
            parameters.Add("time", time.ToString());
            parameters.Add("timeout", "300000");
            parameters.Add("msgHandle", msgHandle);

            HttpWebResponseEx res = WebHttpHepper.SendRequest(Url, "DELETE", headers, parameters);
            if (res.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("delete message success: {0}{1}", msgHandle, res.RetureValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        private static long GetCurrentTimeMillis()
        {
            return (long)((DateTime.Now.ToUniversalTime() - Jan1st1970).TotalMilliseconds);
        }

        /// <summary>
        /// ali参数签名
        /// </summary>
        /// <param name="source">需要签名数据</param>
        /// <param name="accessSecret">ali accessSecret</param>
        /// <returns>签名后的数据</returns>
        private string SignString(String source, String accessSecret)
        {
            using (var algorithm = KeyedHashAlgorithm.Create(SignType_HMACSHA1))
            {
                algorithm.Key = Encoding.UTF8.GetBytes(accessSecret.ToCharArray());
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }
    }
}
