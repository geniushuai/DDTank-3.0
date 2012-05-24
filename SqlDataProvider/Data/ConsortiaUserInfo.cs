using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaUserInfo
    {
        public int ID { get; set; }

        public int ConsortiaID { get; set; }

        public string ConsortiaName { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string LoginName { get; set; }

        public int RatifierID { get; set; }

        public string RatifierName { get; set; }

        public int DutyID { get; set; }

        public bool IsBanChat { get; set; }

        public string Remark { get; set; }

        public int Grade { get; set; }
        public int GP { get; set; }
        public int Repute { get; set; }
        public int State { get; set; }
        public int Offer { get; set; }
        public string Colors { get; set; }
        public string Style { get; set; }
        public int Hide { get; set; }
        public string Skin { get; set; }

        public int Level { get; set; }
        public string DutyName { get; set; }
        public int Right { get; set; }

        public bool IsExist { get; set; }
        public DateTime LastDate { get; set; }

        public bool Sex { get; set; }

        public int Win { get; set; }

        public int Total { get; set; }

        public int Escape { get; set; }

        public int RichesOffer { get; set; }

        public int RichesRob { get; set; }

        public int Nimbus { get; set; }

        public int FightPower { get; set; }

    }
}
