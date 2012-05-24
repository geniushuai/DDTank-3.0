using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 铁匠铺记录
    /// </summary>
    public  class LogItem
    {
        public int ApplicationId { set; get; }
        public int SubId { set; get; }
        public int LineId { set; get; }
        public DateTime EnterTime { set; get; }
        public int UserId { set; get; }
        public int Operation { set; get; }
        public string ItemName { set; get; }
        public int ItemID { set; get; }
        public string BeginProperty { set; get; }
        public string EndProperty { set; get; }
        public int Result { set; get; }
    }
}
