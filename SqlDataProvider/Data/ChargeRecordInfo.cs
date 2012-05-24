using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    //#格式:【用户记录】游戏类型,分区,时间,付费方式(ebank、SMS、Post),男人数,女人数,男付费金额,女付费金额

    public class ChargeRecordInfo
    {
        public string PayWay { get; set; }

        public int TotalBoy { get; set; }

        public int TotalGirl { get; set; }

        public int BoyTotalPay { get; set; }

        public int GirlTotalPay { get; set; }
    }
}
