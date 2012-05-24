using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class HotSpringRoomInfo
    {
        public int ID { set; get; }

        public string Name { set; get; }

        public int PlayerID { set; get; }

        public string PlayerName { set; get; }

        public int GroomID { set; get; }

        public string GroomName { set; get; }

        public int BrideID { set; get; }

        public string BrideName { set; get; }

        public string Pwd { set; get; }

        public int AvailTime { set; get; }

        public int MaxCount{ set; get; }

        public bool GuestInvite { set; get; }

        public int MapIndex { set; get; }

        public DateTime BeginTime { set; get; }

        public DateTime BreakTime { set; get; }

        public string RoomIntroduction { set; get; }

        public int ServerID { set; get; }

        public bool IsHymeneal { set; get; }

        public bool IsGunsaluteUsed { set; get; }
    }
}
