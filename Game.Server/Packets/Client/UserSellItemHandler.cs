using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.SEll_ITEM, "出售商品")]
    public class UserSellItemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //int bagType = packet.ReadByte();
            //int start = packet.ReadInt();

            //IBag bag = client.Player.GetBag(bagType);
            //ItemInfo goods = bag.GetItemAt(start);
            //goods.IsExist = false;
            //if (bag.RemoveItem(goods) != -1)
            //{
            //    int money = goods.Template.Money * goods.Count / 2;
            //    int gold = goods.Template.Gold * goods.Count / 2;
            //    //client.Player.SetMoney(money);
            //    client.Player.SetGold(gold, Game.Server.Statics.GoldAddType.SellItem);
            //}

            //修改:  Xiaov 
            //时间:  2009-11-4
            //描述:  暂未用到，该功能用于玩家与玩家之间交易商品
            return 0;
        }
    }
}
