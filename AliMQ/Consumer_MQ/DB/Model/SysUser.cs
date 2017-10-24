using System;
using System.Data;
using System.Collections.Generic;

namespace DBModel
{
    public partial class SysUser
    {
        public SysUser()
        {
            Id = string.Empty;
            Status = 0;
        }

        public string Id { get; set; }

        public int Status { get; set; }
    }
}
