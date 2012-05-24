using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms
{
    public abstract class AbstractHotSpringProcessor : IHotSpringProcessor
    {
        public virtual void OnGameData(HotSpringRoom game, Game.Server.GameObjects.GamePlayer player, GSPacketIn packet)
        {
        }

        public virtual void OnTick(HotSpringRoom room)
        {
        }
    }
}
