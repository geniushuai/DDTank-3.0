using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Phy.Object;
using Phy.Maps;
using Phy.Actions;
using Game.Server.Packets;
using System.Drawing;
using Game.Server.Managers;
using Game.Server.Statics;
using Game.Server.Phy.Object;
using System.Collections;

namespace Game.Server.Games.Cmd
{
    [GameCommand((byte)TankCmdType.FIRE,"用户开炮")]
    public class FireCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                int x = packet.ReadInt();
                int y = packet.ReadInt();

                //检查开炮点的距离有效性
                if (player.CheckShootPoint(x, y) == false)
                    return;

                int force = packet.ReadInt();
                int angle = packet.ReadInt();
                player.Shoot(x, y, force, angle);
            }
        }
    }
}
