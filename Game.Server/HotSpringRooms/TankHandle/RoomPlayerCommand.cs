using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;
using Game.Server.Packets;
using System.Timers;
using Game.Server.Enumerate;

namespace Game.Server.HotSpringRooms.TankHandle
{
    [HotSpringCommandAttribute((byte)HotSpringCmdType.HOTSPRING_ROOM_PLAYER_CONTINUE)]
    public class RoomPlayerCommand:IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            
            return true;
        }

    }
}
