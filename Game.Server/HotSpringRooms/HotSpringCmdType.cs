using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.HotSpringRooms
{
    public enum HotSpringCmdType
    {
            TARGET_POINT = 1,
         HOTSPRING_ROOM_TIME_UPDATE = 7,
         HOTSPRING_ROOM_RENEWAL_FEE = 3,
         HOTSPRING_ROOM_INVITE = 4,
         HOTSPRING_ROOM_EDIT = 6,
         HOTSPRING_ROOM_ADMIN_REMOVE_PLAYER = 9,
         HOTSPRING_ROOM_PLAYER_CONTINUE = 10,
    }
}
