using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SYS_DATE,"同步系统数据")]
    public class SyncSystemDateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ClearContext();
            packet.WriteDateTime(DateTime.Now);
            client.Out.SendTCP(packet);
            return 0;
        }
    }
}
