using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.HOTSPRING_ROOM_CREATE,"礼堂数据")]
    public class HotSpringRoomCreateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //_loc_2.writeUTF(param1.roomName);
            //_loc_2.writeUTF(param1.roomPassword);
            //_loc_2.writeUTF(param1.roomIntroduction);
            //_loc_2.writeInt(param1.maxCount);

            if (client.Player.CurrentHotSpringRoom != null)
            {
                client.Player.CurrentHotSpringRoom.ProcessData(client.Player, packet);
            }

            return 0;
        }

    }
}
