using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaApplyUserInfo
    {
        public int ID { get; set; }

        public int ConsortiaID { get; set; }

        public string ConsortiaName { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public DateTime ApplyDate { get; set; }

        public string Remark { get; set; }

        public bool IsExist { get; set; }

        public int UserLevel { get; set; }

        public int Win { get; set; }

        public int Total { get; set; }

        public int Repute { get; set; }

        public int FightPower { get; set; }
    }
}
