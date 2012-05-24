using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    [MarryCommandAttbute((byte)MarryCmdType.POSITION)]
    public class Position:IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        { 
            if(player.CurrentMarryRoom != null)
            {
                player.X = packet.ReadInt();
                player.Y = packet.ReadInt();

                return true;
            }

            return false;
        }
    }
}
