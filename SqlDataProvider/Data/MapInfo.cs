using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class MapInfo
    {
        public int ID { set; get; }

        public string Name { set; get; }

        public string PosX { set; get; }

        public string PosX1 { set; get; }

        public int Weight { set; get; }

        public int DragIndex { set; get; }

        public string Description { get; set; }

        public int ForegroundWidth { get; set; }

        public int ForegroundHeight { get; set; }

        public int BackroundWidht { get; set; }

        public int BackroundHeight { get; set; }

        public int DeadWidth { get; set; }

        public int DeadHeight { get; set; }

        public string ForePic { get; set; }

        public string BackPic { get; set; }

        public string DeadPic { get; set; }

        public string Pic { get; set; }

        public string BackMusic { get; set; }

        public string Remark { get; set; }

        public byte Type { get; set; }

    }
}
