using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class FightRateInfo
    {
        public int ID { set; get; }

        public int ServerID { set; get; }

        public int Rate { set; get; }

        public DateTime BeginDay { set; get; }

        public DateTime EndDay { set; get; }

        public DateTime BeginTime { set; get; }

        public DateTime EndTime { set; get; }

        public int BoyTemplateID { set; get; }

        public int GirlTemplateID { set; get; }

        public string SelfCue { set; get; }

        public string EnemyCue { set; get; }

        public string Name { set; get; }
    }
}
