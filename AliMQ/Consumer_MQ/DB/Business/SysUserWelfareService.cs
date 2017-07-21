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
    /// <summary>
    /// 消息队列客户端消费执行类
    /// </summary>
    public class SysUserWelfareService
    {
        public static void SendEmailExcuteService(SysConsumer sysConsumer)
        {
            using (SqlConnection connection = DBConnectionMgr.GetUserConnection())
            {
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //如果消息记录状态显示已经消费，跳出函数
                        if (sysConsumer.Status == SysConsumerService.SysMQ_CostStatus_Consumed) return;

                        //业务执行消费
                        SysUserWelfare userWelware = new SysUserWelfare()
                        {
                            UserID = sysConsumer.MsgCont,
                            WelfareNum = 10
                        };
                        SysUserWelfareDbSvc dbSvc = new SysUserWelfareDbSvc(connection);
                        dbSvc.Insert(userWelware, transaction);

                        //修改数据库消费状态为已消费。
                        SysConsumerDbSvc consumberDbSvc = new SysConsumerDbSvc(connection);
                        consumberDbSvc.UpdateStatus(sysConsumer.Id, SysConsumerService.SysMQ_CostStatus_Consumed, transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}