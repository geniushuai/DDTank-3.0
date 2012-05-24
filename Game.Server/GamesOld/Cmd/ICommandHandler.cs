using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;
using Game.Base.Packets;

namespace Game.Server.Games.Cmd
{
    public interface ICommandHandler
    {
        void HandleCommand(BaseGame game, Player player, GSPacketIn packet);
    }
}
