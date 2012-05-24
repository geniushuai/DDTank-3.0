using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaEventInfo
    {
        public int ID { get; set; }

        public int ConsortiaID { get; set; }

        public string Remark { get; set; }

        public DateTime Date { get; set; }

        public bool IsExist { get; set; }

        public int Type { get; set; }
    }
}
