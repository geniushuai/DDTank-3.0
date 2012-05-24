using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;
using Game.Server.Games;
using Phy.Object;

namespace Game.Server.Games.Cmd
{
    [GameCommand((byte)TankCmdType.DIRECTION,"改变方向")]
    public class DirectionCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            game.SendToAll(packet);
        }
    }
}
