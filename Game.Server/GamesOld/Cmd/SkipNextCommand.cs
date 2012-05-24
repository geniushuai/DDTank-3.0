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
    [GameCommand((byte)TankCmdType.SKIPNEXT,"跳过")]
    public class SkipNextCommandP:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                player.Skip(packet.ReadByte());
            }
        }
    }
}
