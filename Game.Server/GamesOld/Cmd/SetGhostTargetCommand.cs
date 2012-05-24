using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;
using Game.Base.Packets;

namespace Game.Server.Games.Cmd
{
    [GameCommand((int)TankCmdType.GHOST_TATGET,"设置鬼魂目标")]
    public class SetGhostTargetCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsLiving == false)
            {
                player.TargetPoint.X = packet.ReadInt();
                player.TargetPoint.Y = packet.ReadInt();
            }
        }
    }
}
