using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ShopItemInfo
    {
        public ItemInfo Item { set; get; }

        public int ID { set; get; }

        public int ShopID { set; get; }

        public int TemplateID { set; get; }
        
        public int BuyType { set; get; }

        public int Sort { set; get; }

        public int IsBind { set; get; }

        public int IsVouch { set; get; }

        public decimal Beat { set; get; }

        public float Label { set; get; }

        public int AUnit { set; get; }

        public int APrice1 { set; get; }

        public int AValue1 { set; get; }

        public int APrice2 { set; get; }

        public int AValue2 { set; get; }

        public int APrice3 { set; get; }

        public int AValue3 { set; get; }

        public int BUnit { set; get; }

        public int BPrice1 { set; get; }

        public int BValue1 { set; get; }

        public int BPrice2 { set; get; }

        public int BValue2 { set; get; }

        public int BPrice3 { set; get; }

        public int BValue3 { set; get; }

        public int CUnit { set; get; }

        public int CPrice1 { set; get; }

        public int CValue1 { set; get; }

        public int CPrice2 { set; get; }

        public int CValue2 { set; get; }

        public int CPrice3 { set; get; }

        public int CValue3 { set; get; }        

    }
}
