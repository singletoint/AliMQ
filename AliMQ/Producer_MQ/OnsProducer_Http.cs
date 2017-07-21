using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using DBModel;
using DBPresistence;
using System.Data.SqlClient;

using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Ons.Model;
using Aliyun.Acs.Core.Http;
using System.Security.Cryptography;
using Aliyun.Acs.Core.Exceptions;
using System.Web;

namespace Producer_MQ
{
    public class Ons_Produce_Http
    {
        private const string ErrorCode_BIZ_NO_MESSAGE = "BIZ_NO_MESSAGE";
        private const string SignType_HMACSHA1 = "HMACSHA1";

        string Ons_RegionID;
        string Ons_AccessKey;
        string Ons_SecretKey;
        string TopicID;
        string ProducerID;
        string Url;

        public Ons_Produce_Http(string regionID, string accessKey, string secretKey, string topicID, string producerID, string url)
        {
            Ons_RegionID = regionID;
            Ons_AccessKey = accessKey;
            Ons_SecretKey = secretKey;
            TopicID = topicID;
            ProducerID = producerID;
            Url = url;
        }

        //时间戳起点。
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string NEWLINE = "\n";

        /// <summary>
        /// 发送消息到 aliMQ，同时修改系统发送状态
        /// </summary>
        /// <param name="sysMQ">系统MQ消息记录</param>
        public void SendMessage(SysMessageQueue sysMQ)
        {
            using (SqlConnection connection = DBConnectionMgr.GetUserConnection())
            {
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    SysMessageQueueDbSvc sysMQDbSvc = new SysMessageQueueDbSvc(connection);
                    sysMQ.Status = OnsMQDefine.SysMessage_Status_Sent;
                    sysMQDbSvc.UpdateStatus(sysMQ, transaction);
                    //根据 Key 查询，如果OnsMQ服务器上补存在消息，重发
                    //if (string.IsNullOrEmpty(GetMessageByKey(sysMQ.Keys)))
                    //{
                    //发送消息
                    if (SendMessageToAliMQ(sysMQ.MsgCont, sysMQ.Tags, sysMQ.Keys))
                    {
                        transaction.Commit();
                    }
                    else transaction.Rollback();
                    //}
                    //else { transaction.Commit(); }
                }
            }
        }

        /// <summary>
        /// 发送消息到 aliMQ
        /// </summary>
        /// <param name="msgBody">msg</param>
        /// <param name="tag">tag</param>
        /// <param name="key">key</param>
        /// <returns>是否成功</returns>
        private bool SendMessageToAliMQ(string msgBody, string tag, string key)
        {
            long time = GetCurrentTimeMillis();

            string signString = TopicID + NEWLINE + ProducerID + NEWLINE + GetMd5HashStr(msgBody) + NEWLINE + time;
            string sign = SignString(signString, Ons_SecretKey);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Signature", sign);
            headers.Add("AccessKey", Ons_AccessKey);
            headers.Add("ProducerID", ProducerID);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("topic", TopicID);
            parameters.Add("time", time.ToString());
            parameters.Add("tag", tag);
            parameters.Add("key", key);
            try
            {
                HttpWebResponseEx res = WebHttpHepper.SendRequest(Url, "POST", headers, parameters, msgBody);
                if (res.StatusCode == HttpStatusCode.Created)
                {
                    Console.WriteLine("Send Message Success:" + res.RetureValue);
                    return true;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Send Message Fair:" + msgBody);
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
        private static string SignString(String source, String accessSecret)
        {
            using (var algorithm = KeyedHashAlgorithm.Create(SignType_HMACSHA1))
            {
                algorithm.Key = Encoding.UTF8.GetBytes(accessSecret.ToCharArray());
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        /// <summary>
        /// MD5(32位加密)
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string GetMd5HashStr(string str)
        {
            string pwd = string.Empty;
            StringBuilder sb = new StringBuilder();
            //实例化一个md5对像
            MD5 md5 = MD5.Create();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] encryptedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);//个位的时候，将会少一位，需要32位必须是格式化（x2）格式，补0
            }
            return sb.ToString();
        }

        #region AliQM 查询
        /// <summary>
        /// 根据 Key 查询消息
        /// </summary>
        /// <param name="mKey"></param>
        /// <returns>返回第一个msg ID</returns>
        private string GetMessageByKey(string mKey)
        {
            try
            {
                IClientProfile profile = DefaultProfile.GetProfile(Ons_RegionID, Ons_AccessKey, Ons_SecretKey);
                DefaultAcsClient iAcsClient = new DefaultAcsClient(profile);
                OnsMessageGetByKeyRequest request = new OnsMessageGetByKeyRequest();

                request.OnsRegionId = Ons_RegionID;
                request.PreventCache = GetCurrentTimeMillis();
                request.AcceptFormat = FormatType.JSON;
                request.Topic = TopicID;
                request.Key = mKey;

                OnsMessageGetByKeyResponse response = iAcsClient.GetAcsResponse(request);

                if (response.Data == null || response.Data.Count == 0)
                {
                    return null;
                }
                return response.Data[0].MsgId;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (ClientException ex)
            {
                if (ex.ErrorCode == ErrorCode_BIZ_NO_MESSAGE)
                {
                    return null;
                }
                throw;
            }
        }
        #endregion
    }
}
