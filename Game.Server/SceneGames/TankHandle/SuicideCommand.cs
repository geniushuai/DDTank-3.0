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
    [CommandAttbute((byte)TankCmdType.SUICIDE)]
    public class SuicideCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentGame.Data.Players[player].State == TankGameState.DEAD || player.CurrentGame.GameState != eGameState.PLAY)
                return false;

            player.CurrentGame.ReturnPacket(player, packet);
            player.CurrentGame.Data.Players[player].Dead();

            return true;
        }
    }
}
