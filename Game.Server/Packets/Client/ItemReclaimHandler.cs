using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.GameUtils;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.REClAIM_GOODS, "物品比较")]
    public class ItemReclaimHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bagType = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            //int toplace = packet.ReadInt();
            int count = packet.ReadInt();
            //}
            client.Player.BeginChanges();
            PlayerInventory bag = client.Player.GetInventory(bagType);
            if (bag != null && bag.GetItemAt(place) != null)
            {

                bag.RemoveItemAt(place);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success"));
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.NoSuccess"));
                return 1;
            }

           // client.Player.BeginChanges();
            client.Player.AddGiftToken(25);
            client.Player.AddGold(25);
            client.Player.CommitChanges();
            //LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Continue, client.Player.PlayerCharacter.ID, 25, client.Player.PlayerCharacter.Money, 25, 0, 0, "牌子编号",));
            //client.Player.SaveIntoDatabase();//保存到数据库
            return 0;
        }
    }
}
