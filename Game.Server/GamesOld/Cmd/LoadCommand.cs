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
    [GameCommand((byte)TankCmdType.LOAD,"游戏加载进度")]
    public class LoadCommand:ICommandHandler
    {
        public void  HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game.GameState == eGameState.Loading)
            {
                player.LoadingProcess = packet.ReadInt();
                if (player.LoadingProcess >= 100)
                {
                    game.CheckState(0);
                }
                game.SendToAll(packet);
            }
        }
    }
}
