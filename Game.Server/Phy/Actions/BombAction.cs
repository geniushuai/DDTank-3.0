using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phy.Actions
{
    public class BombAction
    {
        public float Time;

        public int Type;

        public int Param1;

        public int Param2;

        public int Param3;

        public int Param4;

        public BombAction(float time, ActionType type, int para1,int para2,int para3,int para4)
        {
            Time = time;
            Type = (int)type;
            Param1 = para1;
            Param2 = para2;
            Param3 = para3;
            Param4 = para4;
        }

        public int TimeInt
        {
            get
            {
                return (int)Math.Round(Time * 1000);
            }
        }
    }
}
