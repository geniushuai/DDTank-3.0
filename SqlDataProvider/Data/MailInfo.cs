using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class MailInfo
    {
        public int ID { set; get; }

        public int SenderID { set; get; }

        public string Sender { set; get; }

        public int ReceiverID { set; get; }

        public string Receiver { set; get; }

        public string Title { set; get; }

        public string Content { set; get; }

        public string Annex1 { set; get; }

        public string Annex2 { set; get; }

        public int Gold { set; get; }

        public int Money { set; get; }

        public bool IsExist { set; get; }

        public int Type { set; get; }

        public int ValidDate { set; get; }

        public bool IsRead { set; get; }

        public DateTime SendTime { set; get; }

        public string Annex1Name { set; get; }

        public string Annex2Name { set; get; }

        public string Annex3 { set; get; }

        public string Annex4 { set; get; }

        public string Annex5 { set; get; }

        public string Annex3Name { set; get; }

        public string Annex4Name { set; get; }

        public string Annex5Name { set; get; }

        public string AnnexRemark { set; get; }
        public int  GiftToken { set; get; }

        public void Revert()
        {
            ID = 0;
            SenderID = 0;
            Sender = "";
            ReceiverID = 0;
            Receiver = "";
            Title = "";
            Content = "";
            Annex1 = "";
            Annex2 = "";
            Gold = 0;
            Money = 0;
            GiftToken = 0;
            IsExist = false;
            Type = 0;
            ValidDate = 0;
            IsRead = false;
            SendTime = DateTime.Now;
            Annex1Name = "";
            Annex2Name = "";
            Annex3 = "";
            Annex4 = "";
            Annex5 = "";
            Annex3Name = "";
            Annex4Name = "";
            Annex5Name = "";
            AnnexRemark = "";
        }
    }
}
