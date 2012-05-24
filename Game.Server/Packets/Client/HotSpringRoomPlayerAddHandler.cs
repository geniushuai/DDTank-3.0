using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.HOTSPRING_ROOM_PLAYER_ADD,"礼堂数据")]
    public class HotSpringRoomPlayerAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentHotSpringRoom != null)
            {
                client.Player.CurrentHotSpringRoom.ProcessData(client.Player, packet);
            }

            return 0;
        }

    }
}
