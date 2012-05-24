using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.Managers;
using Game.Server.GameUtils;
using Game.Server.Statics;
using Bussiness.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_CHANGE_COLOR, "改变物品颜色")]
    public class UserChangeItemColorHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            eMessageType eMsg = eMessageType.Normal;
            string msg = "UserChangeItemColorHandler.Success";

            int Card_bagType = packet.ReadInt();
            int Card_place = packet.ReadInt();
            int bagType = packet.ReadInt();
            int place = packet.ReadInt();
            string color = packet.ReadString();
            string skin = packet.ReadString();
            int templateID = packet.ReadInt();

            ItemInfo item = client.Player.MainBag.GetItemAt(place);
            ItemInfo card = client.Player.PropBag.GetItemAt(Card_place);

            if (item != null)
            {
                client.Player.BeginChanges();

                try
                {
                    bool changed = false;
                    if (card != null && card.IsValidItem())
                    {
                        client.Player.PropBag.RemoveItem(card);
                        changed = true;
                    }
                    else
                    {
                        ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateID);
                        List<ShopItemInfo> Template = ShopMgr.FindShopbyTemplatID(templateID);
                        int Money = 0;
                        for (int i = 0; i < Template.Count; i++)
                        {

                            if (Template[i].APrice1 == -1 && Template[i].AValue1 != 0)
                            {
                                Money = Template[i].AValue1;
                            }

                        }
                        if (Money <= client.Player.PlayerCharacter.Money)
                        {
                            client.Player.RemoveMoney(Money);
                            LogMgr.LogMoneyAdd(LogMoneyType.Item, LogMoneyType.Item_Color, client.Player.PlayerCharacter.ID, Money, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                           // client.Player.RemoveGold(template.Gold, GoldRemoveType.Other);

                            changed = true;
                        }
                    }
                    if (changed)
                    {
                        item.Color = color == null ? "" : color;
                        item.Skin = skin == null ? "" : skin;
                        client.Player.MainBag.UpdateItem(item);
                    }
                }
                finally
                {
                    client.Player.CommitChanges();
                }
            }
            client.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg));
            return 0;
        }
    }
}
