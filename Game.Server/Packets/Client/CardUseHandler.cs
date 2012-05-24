using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.Statics;
using Bussiness;
using Game.Server.Buffer;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CARD_USE, "卡片使用")]
    public class CardUseHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int bagType = packet.ReadInt();
            int place = packet.ReadInt();
            string msg1 = null;

            ItemInfo item = null;
            List<ShopItemInfo> ShopItem = new List<ShopItemInfo>();

            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }
            if (place == -1)
            {
                int templateID = packet.ReadInt();
                int type = packet.ReadInt();
                int gold = 0;
                int money = 0;



                ItemTemplateInfo template = Bussiness.Managers.ItemMgr.FindItemTemplate(templateID);
                item = ItemInfo.CreateFromTemplate(template, 1, (int)ItemAddType.Buy);
                ShopItem = Bussiness.Managers.ShopMgr.FindShopbyTemplatID(templateID);
                for (int i = 0; i < ShopItem.Count; i++)
                {

                    if (ShopItem[i].APrice1 == -1 && ShopItem[i].AValue1 != 0)
                    {
                        money = ShopItem[i].AValue1;
                        item.ValidDate = ShopItem[i].AUnit;
                    }
                    
                }

                if (item != null)
                {
                   // item = ItemInfo.SetItemType(item, type, ref gold, ref money, ref offer);
                    if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money)
                    {
                        client.Player.RemoveMoney(money);
                        client.Player.RemoveGold(gold);
                        LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Card, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "牌子编号", item.TemplateID.ToString(), type.ToString());
                        msg1 = "CardUseHandler.Success";
                    }
                    else
                    {
                        item = null;
                    }
                }

            }
            else
            {
                item = client.Player.PropBag.GetItemAt(place);
                msg1 = "CardUseHandler.Success";
            }

            if (item != null)
            {
                string msg = string.Empty;
                if (item.Template.Property1 != 21)
                {
                    AbstractBuffer buffer = BufferList.CreateBuffer(item.Template, item.ValidDate);
                    if (buffer != null)
                    {
                        buffer.Start(client.Player);
                        if (place != -1)
                        {
                            client.Player.PropBag.RemoveItem(item);
                        }
                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg1));
                }
                else
                {
                    if (item.IsValidItem())
                    {
                        //client.Player.PlayerCharacter.GP += item.Template.Property1;
                        client.Player.AddGP(item.Template.Property2);

                        if (item.Template.CanDelete)
                        {
                            client.Player.RemoveItem(item);
                            msg = "GPDanUser.Success";
                        }

                    }
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg, item.Template.Property2));
                }
            }

            return 0;
        }
    }
}
