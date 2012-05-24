using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using log4net;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CLIENT_LOG,"客户端日记")]
    public class ClientErrorLog:IPacketHandler
    {
        public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            log.Error("client log:" + packet.ReadString());
            return 0;
        }
    }
}
