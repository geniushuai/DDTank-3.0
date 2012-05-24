using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class DailyAwardInfo
    {
        public int ID { set; get; }

        public int Type { set; get; }

        public int TemplateID { set; get; }

        public int Count { set; get; }

        public int ValidDate { set; get; }

        public bool IsBinds { set; get; }

        public int Sex { set; get; }

        public string Remark { set; get; }

        public string CountRemark { set; get; }
    }
}
