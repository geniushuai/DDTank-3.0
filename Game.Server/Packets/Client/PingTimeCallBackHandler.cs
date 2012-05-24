using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.PING,"测试网络")]
    public class PingTimeCallBackHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Player.PingTime = DateTime.Now.Ticks - client.Player.PingStart;
            return 0;
        }
    }
}
