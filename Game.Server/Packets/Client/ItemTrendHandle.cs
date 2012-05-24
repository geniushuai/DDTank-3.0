using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.Statics;
using Bussiness;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_TREND, "物品倾向转移")]
    public class ItemTrendHandle : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            eBageType ItemBagType = (eBageType)packet.ReadInt();

            int ItemPlace = packet.ReadInt();

            eBageType PropBagType = (eBageType)packet.ReadInt();
            ItemInfo Item = null;
            List<ShopItemInfo> ShopItem = new List<ShopItemInfo>();
            ItemInfo Prop = null;
            int PropPlace = packet.ReadInt();

            int Operation = packet.ReadInt();



             if (PropPlace == -1)
            {
                int templateID = packet.ReadInt();
                int type = packet.ReadInt();
                int gold = 0;
                int money = 0;



                ItemTemplateInfo template = Bussiness.Managers.ItemMgr.FindItemTemplate(34101);
                Prop = ItemInfo.CreateFromTemplate(template, 1, (int)ItemAddType.Buy);
                ShopItem = Bussiness.Managers.ShopMgr.FindShopbyTemplatID(34101);
                for (int i = 0; i < ShopItem.Count; i++)
                {

                    if (ShopItem[i].APrice1 == -1 && ShopItem[i].AValue1 != 0)
                    {
                        money = ShopItem[i].AValue1;
                        Prop.ValidDate = ShopItem[i].AUnit;
                    }

                }

                if (Prop != null)
                {
                    // item = ItemInfo.SetItemType(item, type, ref gold, ref money, ref offer);
                    if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money)
                    {
                        client.Player.RemoveMoney(money);
                        client.Player.RemoveGold(gold);
                        LogMgr.LogMoneyAdd(LogMoneyType.Item, LogMoneyType.Item_Move, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "牌子编号", Prop.TemplateID.ToString(), type.ToString());
                    }
                    else
                    {
                        Prop = null;
                    }
                }

            }
            else
            {
                Prop = client.Player.GetItemAt(PropBagType, PropPlace);
               
            }
            Item = client.Player.GetItemAt(ItemBagType, ItemPlace);
            StringBuilder str = new StringBuilder();

            if (Prop == null || Item == null)
                return 1;

            bool result = false;

            ItemTemplateInfo TemplateItem = Managers.RefineryMgr.RefineryTrend(Operation, Item, ref result);

            if (result && TemplateItem != null)
            {


                ItemInfo item = ItemInfo.CreateFromTemplate(TemplateItem, 1, (int)ItemAddType.RefineryTrend);
                AbstractInventory bg = client.Player.GetItemInventory(TemplateItem);
               // Managers.RefineryMgr.InheritProperty(Item, ref item);
                if (bg.AddItem(item, bg.BeginSlot))
                {
                    client.Player.UpdateItem(item);
                    client.Player.RemoveItem(Item);
                    Prop.Count--;
                    client.Player.UpdateItem(Prop);
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Success"));
                }
                else
                {
                    str.Append("NoPlace");
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(item.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));

                }
                return 1;
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Fail"));
                return 1;
            }
        }
    }
}
