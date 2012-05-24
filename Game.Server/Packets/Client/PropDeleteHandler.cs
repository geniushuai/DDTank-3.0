using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.PROP_DELETE,"删除道具")]
    public class PropDeleteHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            int index = packet.ReadInt();
            client.Player.FightBag.RemoveItemAt(index);
            return 0;
        }
    }
}
