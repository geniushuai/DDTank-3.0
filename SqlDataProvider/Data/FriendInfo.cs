using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class FriendInfo : DataObject
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int FriendID { get; set; }

        public DateTime AddDate { get; set; }

        public string Remark { get; set; }

        public bool IsExist { get; set; }

        public string NickName { get; set; }

        public string UserName { get; set; }

        public string Style { get; set; }

        public int Sex { get; set; }

        public string DutyName { get; set; }

        public string Colors { get; set; }

        public int Grade { get; set; }

        public int Hide { get; set; }

        public int State { get; set; }

        public int Offer
        {
            get;
            set;
        }

        public string ConsortiaName
        {
            get;
            set;
        }

        public int Win
        {
            get;
            set;
        }

        public int Total
        {
            get;
            set;
        }

        public int Escape
        {
            get;
            set;
        }

        public int Relation { get; set; }

        public int Repute { get; set; }

        public int Nimbus
        {
            get;
            set;

        }
    }
}
