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
    [HotSpringCommandAttribute((byte)HotSpringCmdType.FORBID)]
    public class ForbidCommand : IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom != null /*&& player.PlayerCharacter.ID == player.CurrentHotSpringRoom.Info.PlayerID*/ )
            {
                if (player.PlayerCharacter.ID == player.CurrentHotSpringRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentHotSpringRoom.Info.BrideID)
                {
                    int userID = packet.ReadInt();
                    if (userID != player.CurrentHotSpringRoom.Info.BrideID && userID != player.CurrentHotSpringRoom.Info.GroomID)
                    {
                        player.CurrentHotSpringRoom.KickPlayerByUserID(player, userID);
                        player.CurrentHotSpringRoom.SetUserForbid(userID);
                    }

                    return true;
                }
            }
            return false;
        }
        
    }
}
