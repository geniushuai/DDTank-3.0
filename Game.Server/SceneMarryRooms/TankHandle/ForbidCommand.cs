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
    [MarryCommandAttbute((byte)MarryCmdType.FORBID)]
    public class ForbidCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom != null /*&& player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.PlayerID*/ )
            {
                if (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.BrideID)
                {
                    int userID = packet.ReadInt();
                    if (userID != player.CurrentMarryRoom.Info.BrideID && userID != player.CurrentMarryRoom.Info.GroomID)
                    {
                        player.CurrentMarryRoom.KickPlayerByUserID(player, userID);
                        player.CurrentMarryRoom.SetUserForbid(userID);
                    }

                    return true;
                }
            }
            return false;
        }
        
    }
}
