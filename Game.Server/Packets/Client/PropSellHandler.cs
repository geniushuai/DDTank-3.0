using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.PROP_SELL, "出售道具")]
    public class PropSellHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int gold = 0;
            int money = 0;
            int offer = 0;
            int gifttoken = 0;
            int type = 1;
            List<int> needitemsinfo = new List<int>();

            int index = packet.ReadInt();
            int GoodsID = packet.ReadInt();

            ItemInfo item = client.Player.FightBag.GetItemAt(index);

            if (item != null)
            {
                client.Player.FightBag.RemoveItem(item);

                ShopItemInfo shopitem = Bussiness.Managers.ShopMgr.GetShopItemInfoById(GoodsID);                              //获取商品信息
              
                needitemsinfo = ItemInfo.SetItemType(shopitem, type, ref gold, ref money, ref offer, ref gifttoken);          //商品的购买条件

                client.Player.AddGold(gold);
            }
            return 0;
        }
    }
}
