using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;


namespace Game.Server.SceneGames.TankHandle
{
    public interface ICommandHandler
    {
        bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet);
    }
}
