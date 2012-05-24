using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 用户消费行为
    /// </summary>
    public class LogMoney
    {
        public int ApplicationId { get; set; }
        public int SubId { get; set; }
        public int LineId { get; set; }
        public int MastType { get; set; }
        public int SonType { get; set; }
        public int UserId { get; set; }
        public DateTime EnterTime { get; set; }
        public int Moneys { get; set; }
        public int Gold { get; set; }
        public int GiftToken { get; set; }
        public int Offer{get;set;}
        public string OtherPay { get; set; }
        public string GoodId { get; set; }
        public string GoodsType { get; set; }        
    }
}
