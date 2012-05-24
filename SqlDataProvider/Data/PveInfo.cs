using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class PveInfo
    {
        public int ID { set; get; }

        public string Name { set; get; }

        public string SimpleTemplateIds { set; get; }

        public string NormalTemplateIds { set; get; }

        public string HardTemplateIds { set; get; }

        public string TerrorTemplateIds { set; get; }

        public int Type { set; get; }

        public int LevelLimits { set; get; }

        public string Pic { get; set; }

        public string Description { set; get; }

        public string SimpleGameScript { get; set; }

        public string NormalGameScript { get; set; }

        public string HardGameScript { get; set; }

        public string TerrorGameScript { get; set; }


    }
}
