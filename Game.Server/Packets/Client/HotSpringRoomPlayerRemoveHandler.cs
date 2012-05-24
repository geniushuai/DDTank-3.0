using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.HOTSPRING_ROOM_PLAYER_REMOVE,"礼堂数据")]
    public class HotSpringRoomPlayerRemoveHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //if (client.Player.CurrentHotSpringRoom != null)
            //{
            //    client.Player.CurrentHotSpringRoom.ProcessData(client.Player, packet);
            //}
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.HOTSPRING_ROOM_PLAYER_REMOVE);
            pkg.WriteString("REMOVE OK");
            client.SendTCP(pkg);
            return 0;
        }

    }
}
