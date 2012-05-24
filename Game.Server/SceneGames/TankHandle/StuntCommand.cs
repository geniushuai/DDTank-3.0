using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Spells;
using Game.Base.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.STUNT)]
    public class StuntCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentGame.Data.CurrentIndex == player)
            {
                if (player.CurrentGame.Data.Players[player].Dander >= 200)
                {
                    //player.CurrentGame.ReturnPacket(player, packet);
                    player.CurrentGame.Data.Players[player].SetDander( -1);

                }
                return true;
            }
            return false;
        }
    }
}
