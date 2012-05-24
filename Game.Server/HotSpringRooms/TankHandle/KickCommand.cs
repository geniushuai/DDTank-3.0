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
    [HotSpringCommandAttribute((byte)HotSpringCmdType.HOTSPRING_ROOM_ADMIN_REMOVE_PLAYER)]
    public class KickCommand : IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom != null && player.CurrentHotSpringRoom.RoomState == eRoomState.FREE)
            {
                if(player.PlayerCharacter.ID == player.CurrentHotSpringRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentHotSpringRoom.Info.BrideID)
                {
                    int userID = packet.ReadInt();
                    player.CurrentHotSpringRoom.KickPlayerByUserID(player, userID);
                    return true;
                }
            }
            return false;
        }
        
    }
}
