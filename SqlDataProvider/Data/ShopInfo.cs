using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ShopInfo
    {
        public int ID { set; get; }

        public int ShopID { set; get; }

        public int GoodsID { set; get; }

        public int Count { set; get; }

        public int Sort { set; get; }

        public int IsVouch { set; get; }

        public int IsHot { set; get; }

        public int IsNew { set; get; }

        public int IsLtime { set; get; }

        public int IsSpecial { set; get; }
    }
}
