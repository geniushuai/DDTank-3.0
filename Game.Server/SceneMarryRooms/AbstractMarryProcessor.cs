using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SceneMarryRooms
{
    public abstract class AbstractMarryProcessor : IMarryProcessor
    {
        public virtual void OnGameData(MarryRoom game, Game.Server.GameObjects.GamePlayer player, GSPacketIn packet)
        {
        }

        public virtual void OnTick(MarryRoom room)
        {
        }
    }
}
