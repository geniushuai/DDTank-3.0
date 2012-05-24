using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;


namespace Game.Server.HotSpringRooms.TankHandle
{
    public interface IHotSpringCommandHandler
    {
        bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet);
    }
}
