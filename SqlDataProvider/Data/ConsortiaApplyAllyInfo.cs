using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaApplyAllyInfo
    {
        public int ID { get; set; }

        public int Consortia1ID { get; set; }

        public int Consortia2ID { get; set; }

        public DateTime Date { get; set; }

        public string Remark { get; set; }

        public bool IsExist { get; set; }

        public int State { get; set; }

        public string ConsortiaName { get; set; }
        public int Repute { get; set; }
        public string ChairmanName { get; set; }
        public int Count { get; set; }
        public int CelebCount { get; set; }
        public int Honor { get; set; }
        public int Level { get; set; }

        public string Description { get; set; }
    }

}
