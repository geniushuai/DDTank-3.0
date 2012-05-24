using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class MarryApplyInfo
    {
        public int UserID { get; set; }

        public int ApplyUserID { get; set; }

        public string ApplyUserName { get; set; }

        public int ApplyType { get; set; }

        public bool ApplyResult { get; set; }
        
        public string LoveProclamation{get;set;}

        public int ID { get; set; }
    }
}
