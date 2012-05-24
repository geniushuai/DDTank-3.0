using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaDutyInfo
    {
        public int DutyID { get; set; }
        public int ConsortiaID { get; set; }
        public int Level { get; set; }
        public string DutyName { get; set; }
        public int Right { get; set; }
        public bool IsExist { get; set; }
    }
}
