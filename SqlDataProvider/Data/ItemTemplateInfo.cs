using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public enum eBageType
    {
        MainBag = 0,
        PropBag = 1,
        TaskBag = 2,
        FightBag = 3,
        TempBag = 4,
        CaddyBag = 5,
        Bank    = 11,
        Store   =12,
        Card=15
    }

    public class ItemTemplateInfo : DataObject
    {
        public int TemplateID { get; set; }

        public string Name { get; set; }

        public int CategoryID { get; set; }

        public string Description { get; set; }

        public int Attack { get; set; }

        public int Defence { get; set; }

        public int Luck { get; set; }

        public int Agility { get; set; }

        public int Level { get; set; }

        public string Pic { get; set; }

        public string AddTime { get; set; }

        public int Quality { get; set; }

        public int MaxCount { get; set; }

        public string Data { get; set; }

        public int Property1 { get; set; }

        public int Property2 { get; set; }

        public int Property3 { get; set; }

        public int Property4 { get; set; }

        public int Property5 { get; set; }

        public int Property6 { get; set; }

        public int Property7 { get; set; }

        public int Property8 { get; set; }

        public int NeedSex { get; set; }

        public int NeedLevel { get; set; }

        public bool CanDrop { get; set; }

        public bool CanDelete { get; set; }

        public bool CanEquip { get; set; }

        public bool CanUse { get; set; }

        public string Script { get; set; }

        public string Colors { get; set; }

        public bool CanStrengthen { get; set; }

        public bool CanCompose { get; set; }

        public int BindType { get; set; }

        public int FusionType { get; set; }

        public int FusionRate { get; set; }

        public int FusionNeedRate { get; set; }

        public int RefineryType { get; set; }

        public string Hole { get; set; }

        public int RefineryLevel { get; set; }

        public eBageType BagType
        {
            get
            {
                switch (CategoryID)
                {
                    case 10:
                    case 11:
                    case 12:
                        return eBageType.PropBag;
                    default:
                        return eBageType.MainBag;
                }
            }
        }

        
    }
}