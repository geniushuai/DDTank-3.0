using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness.Managers;
using SqlDataProvider.Data;
using Game.Server.GameUtils;
using Bussiness;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CADDY_SELL_ALL_GOODS, "打开物品")]
    public class CaddyClearAllHandler : IPacketHandler
    {


        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int bagType = packet.ReadByte();
            int place = packet.ReadInt();
            PlayerInventory arkBag = client.Player.CaddyBag;
            PlayerInventory propBag = client.Player.PropBag;
            var numItem = 0;
            for (int i = 0; i < arkBag.Capalility; i++)
            {
                var item = arkBag.GetItemAt(i);
                if (item != null)
                {
                    numItem++;
                    
                    arkBag.RemoveItem(item);
                }
            }
            client.Player.BeginChanges();
            client.Player.AddGold(numItem * 25);
            client.Player.AddGiftToken(numItem * 25);
            client.Player.CommitChanges();
            return 1;
        }

    }
}
