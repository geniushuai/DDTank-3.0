using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CHAIN_ITEM, "装备物品")]
    public class UserChainItemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //if (client.Player.CurrentGame != null && client.Player.CurrentGame.GameState != eGameState.FREE)
            //    return 0;

            //int start = packet.ReadInt();
            //int end = packet.ReadInt();

            //ItemInfo item = client.Player.CurrentInventory.GetItemAt(start);
            //if (item == null)
            //    return 0;

            //int place = client.Player.CurrentInventory.GetItemEpuipSlot(item.Template);
            //if ((place == 9 || place == 10) && (end == 9 || end == 10))
            //{
            //    client.Player.CurrentInventory.MoveItem(start, end);
            //}
            //else if ((place == 7 || place == 8) && (end == 7 || end == 8))
            //{
            //    client.Player.CurrentInventory.MoveItem(start, end);
            //}
            //else
            //{
            //    client.Player.CurrentInventory.MoveItem(start, place);
            //}

            return 0;
        }
    }
}
