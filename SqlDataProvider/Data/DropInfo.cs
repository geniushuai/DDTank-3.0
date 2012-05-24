using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class DropInfo
    {
        public int ID { get; set; }

        public int Time { get; set; }

        public int Count { get; set; }

        public int MaxCount { get; set; }

        public DropInfo(int id, int time, int count, int maxCount)
        {
            ID = id;
            Time = time;
            Count = count;
            MaxCount = maxCount;
        }
    }
}
