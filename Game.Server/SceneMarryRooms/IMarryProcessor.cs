using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;

namespace Game.Server.SceneMarryRooms
{
    public interface IMarryProcessor
    {
        void OnGameData(MarryRoom game, GamePlayer player, GSPacketIn packet);

        void OnTick(MarryRoom room);
    }
}
