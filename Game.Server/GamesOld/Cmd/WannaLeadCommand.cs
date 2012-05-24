using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;

namespace Game.Server.Games.Cmd
{
    [GameCommand((int)TankCmdType.WANNA_LEADER,"希望成为队长")]
    public class WannaLeadCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game,Player player, Game.Base.Packets.GSPacketIn packet)
        {
            player.WannaLeader = packet.ReadBoolean();
            game.SendToAll(packet);
        }
    }
}
