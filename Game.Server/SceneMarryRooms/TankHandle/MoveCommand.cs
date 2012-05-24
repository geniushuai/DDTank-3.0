using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using Game.Server.SceneMarryRooms.TankHandle;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    [MarryCommandAttbute((byte)MarryCmdType.MOVE)]
    public class MoveCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom != null && player.CurrentMarryRoom.RoomState == eRoomState.FREE)
            {
                player.X = packet.ReadInt();
                player.Y = packet.ReadInt();
                //0 player.CurrentMarryRoom.ReturnPacket(player, packet);
                player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                return true;
            }
            return false;
        }
        
    }
}
