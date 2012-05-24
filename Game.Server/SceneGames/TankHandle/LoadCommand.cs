using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using Game.Base.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.LOAD)]
    public class LoadCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.Players[player].State == TankGameState.FRIST)
            if (player.CurrentGame.GameState == eGameState.LOAD)
            {
                player.CurrentGame.ReturnPacket(player, packet);
                return true;
            }
            return false;
        }
    }
}
