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
    [GameCommand((byte)TankCmdType.FIRE_TAG,"准备开炮")]
    public class FireTagCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                game.SendToAll(packet);

                bool tag = packet.ReadBoolean();
                byte speedTime = packet.ReadByte();

                player.PrepareShoot(speedTime);
            }
        }
    }
}
