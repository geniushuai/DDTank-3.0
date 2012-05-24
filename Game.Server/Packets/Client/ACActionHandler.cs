using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.AC_ACTION,"user ac action")]
    [Obsolete("已经不用")]
    public class ACActionHandler:IPacketHandler
    {
        
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            return 1;
        }
    }
}
