using System;
using System.Data;
using System.Collections.Generic;

namespace DBModel
{
    public partial class SysUserWelfare
    {
        public SysUserWelfare()
        {
            Id = 0;
            UserID = string.Empty;
            WelfareNum = 0;
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }

        public string UserID { get; set; }

        public int WelfareNum { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
