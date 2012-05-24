using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.AAS_CTRL, "防沉迷系统开关")]
    class AASControlHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Out.SendAASControl(AntiAddictionMgr.ISASSon,client.Player.IsAASInfo, client.Player.IsMinor);
            return 0;
        }
    }
}
