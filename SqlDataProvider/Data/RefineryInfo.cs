using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class RefineryInfo
    {
        public int RefineryID { set; get; }

        //public int Equip1 { get; set; }

        //public int Equip2 { get; set; }

        //public int Equip3 { get; set; }

        //public int Equip4 { get; set; }

        public List<int> m_Equip = new List<int> ();

        public int Item1 { set; get; }

        public int Item1Count { get; set; }

        public int Item2 { set; get; }

        public int Item2Count { get; set; }

        public int Item3 { set; get; }

        public int Item3Count { get; set; }

        public int Item4 { set; get; }

        public int Item4Count { get; set; }

        public List<int> m_Reward = new List<int> ();

        //public int Material1 { get; set; }

        //public int Operate1 { get; set; }

        //public int Reward1 { set; get; }


        //public int Material2 { get; set; }

        //public int Operate2 { get; set; }

        //public int Reward2 { set; get; }



        //public int Material3 { get; set; }

        //public int Operate3 { get; set; }

        //public int Reward3 { set; get; }



        //public int Material4 { get; set; }

        //public int Operate4 { get; set; }

        //public int Reward4 { set; get; }


    }
}
