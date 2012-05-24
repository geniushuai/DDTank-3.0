using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;

namespace Game.Server.HotSpringRooms.TankHandle
{
    [HotSpringCommandAttribute((byte)HotSpringCmdType.TARGET_POINT)]
    public class Position:IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        { 
            if(player.CurrentHotSpringRoom != null)
            {
                player.X = packet.ReadInt();
                player.Y = packet.ReadInt();

                return true;
            }

            return false;
        }
    }
}
