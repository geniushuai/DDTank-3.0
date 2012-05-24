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
    [PacketHandler((int)255, "防沉迷系统开关")]
    class Test30Hand : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn(255);
            pkg.WriteString(packet.ReadString());
            foreach (var item in WorldMgr.GetAllPlayers())
            {
                item.SendTCP(pkg);   
            } ;
           // client.SendTCP(pkg);
            return 0;
        }
    }
}
