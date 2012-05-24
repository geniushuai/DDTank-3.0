using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.SCENE_CHANNEL_CHANGE,"用户改变频道")]
    public class SceneChangeChannel:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte type = packet.ReadByte();
            return 0;
        }
    }
}
