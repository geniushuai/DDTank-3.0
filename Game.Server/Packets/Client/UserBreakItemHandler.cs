using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.BREAK_ITEM,"拆分物品")]
    public class UserBreakItemHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //int bagType = packet.ReadByte();
            //int start = packet.ReadInt();
            //int end = packet.ReadInt();
            //int count = packet.ReadInt();

            //IBag bag = client.Player.GetBag(bagType);
            //ItemInfo item = bag.GetItemAt(start);
            //bag.SplitItem(item, count, end);

            //ItemInfo item = client.Player.CurrentInventory.GetItemAt(start);
            //client.Player.CurrentInventory.SplitItem(item, count, end);

            return 0;
        }
    }
}
