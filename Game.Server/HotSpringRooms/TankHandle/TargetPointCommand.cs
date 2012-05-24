using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.HotSpringRooms;
using Game.Server.HotSpringRooms.TankHandle;

namespace Game.Server.HotSpringRooms.TankHandle
{
    [HotSpringCommandAttribute((byte)HotSpringCmdType.TARGET_POINT)]
    public class MoveCommand : IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom != null && player.CurrentHotSpringRoom.RoomState == eRoomState.FREE)
            {
                var moveString = packet.ReadString();
                var playerId = packet.ReadInt();
                player.X = packet.ReadInt();
                player.Y = packet.ReadInt();
                //0 player.CurrentHotSpringRoom.ReturnPacket(player, packet);
                player.CurrentHotSpringRoom.ReturnPacketForScene(player, packet);
                return true;
            }
            return false;
        }
        
    }
}
