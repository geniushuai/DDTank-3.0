using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class RateInfo
    {
        public int ServerID { set; get; }

        public int Type { set; get; }

        public float Rate { set; get; }

        public DateTime BeginDay { set; get; }

        public DateTime EndDay { set; get; }

        public DateTime BeginTime { set; get; }

        public DateTime EndTime { set; get; }
    }
}
