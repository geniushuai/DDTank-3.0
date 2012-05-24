using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using Game.Base.Packets;
using Game.Server.Games.Cmd;
using Phy.Object;

namespace Game.Server.Games.Cmd
{
    [GameCommand((byte)TankCmdType.SUICIDE,"自杀")]
    public class SuicideCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsLiving && game.GameState == eGameState.Playing)
            {
                game.SendToAll(packet);
                player.Die();
                game.CheckState(0);
            }
        }
    }
}
