using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaInviteUserInfo
    {
        public int ID { get; set; }

        public int ConsortiaID { get; set; }

        public string ConsortiaName { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public int InviteID { get; set; }

        public string InviteName { get; set; }

        public DateTime InviteDate { get; set; }

        public string Remark { get; set; }

        public bool IsExist { get; set; }

        public string ChairmanName { get; set; }

        public int CelebCount { get; set; }

        public int Honor { get; set; }

        public int Repute { get; set; }

        public int Count { get; set; }
    }
}
