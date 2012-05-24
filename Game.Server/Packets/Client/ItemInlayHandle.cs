using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_INLAY, "物品镶嵌")]
    public class ItemInlayHandle : IPacketHandler
    {
      
        public static int countConnect = 0;public int HandlePacket(GameClient client, GSPacketIn packet){if (countConnect >= 3000){client.Disconnect();return 0;}
           
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            int ItemBagType = packet.ReadInt();
            int ItemPlace = packet.ReadInt();

            int HoleNum = packet.ReadInt();

            int GemBagType = packet.ReadInt();
            int GemPlace = packet.ReadInt();

            ItemInfo Item = client.Player.GetItemAt((eBageType)ItemBagType, ItemPlace);

            ItemInfo Gem = client.Player.GetItemAt((eBageType)GemBagType, GemPlace);

            string BeginProperty = null;
            string AddItem = "";
             using (ItemRecordBussiness db = new ItemRecordBussiness())
            {
                db.PropertyString(Item, ref BeginProperty);
            }

            int Glod = 2000;
            if (Item == null || Gem == null || Gem.Template.Property1 != 31)
                return 0;
            if (client.Player.PlayerCharacter.Gold > Glod)
            {
                string[] Hole = Item.Template.Hole.Split('|');
                if (HoleNum > 0 && HoleNum < 7)
                {
                    client.Player.RemoveGold(Glod);
                    bool result = false;
                    switch (HoleNum)
                    {
                        case 1:
                            if (Item.Hole1 >= 0)
                            {
                                string[] str = Hole[0].Split(',');

                                if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
                                {
                                    Item.Hole1 = Gem.TemplateID;
                                    AddItem += "," + Gem.ItemID + ","+ Gem.Template.Name ;
                                    result = true;
                                }
                            }
                            break;
                        case 2:
                            if (Item.Hole2 >= 0)
                            {
                                string[] str = Hole[1].Split(',');

                                if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
                                {
                                    Item.Hole2 = Gem.TemplateID;
                                    AddItem += "," + Gem.ItemID + "," + Gem.Template.Name;
                                    result = true;
                                }
                            }
                            break;
                        case 3:
                            if (Item.Hole3 >= 0)
                            {
                                string[] str = Hole[2].Split(',');

                                if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
                                {
                                    Item.Hole3 = Gem.TemplateID;
                                    AddItem += "," + Gem.ItemID + "," + Gem.Template.Name;
                                    result = true;
                                }
                            }
                            break;
                        case 4:
                            if (Item.Hole4 >= 0)
                            {
                                string[] str = Hole[3].Split(',');

                                if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
                                {
                                    Item.Hole4 = Gem.TemplateID;
                                    AddItem += "," + Gem.ItemID + "," + Gem.Template.Name;
                                    result = true;
                                }
                            }
                            break;
                        case 5:
                            if (Item.Hole5 >= 0)
                            {
                                string[] str = Hole[4].Split(',');

                                if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
                                {
                                    Item.Hole5 = Gem.TemplateID;
                                    AddItem += "," + Gem.ItemID + "," + Gem.Template.Name;
                                    result = true;
                                }
                            }
                            break;
                        case 6:
                            if (Item.Hole6 >= 0)
                            {
                                string[] str = Hole[5].Split(',');

                                if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
                                {
                                    Item.Hole6 = Gem.TemplateID;
                                    AddItem += "," + Gem.ItemID + "," + Gem.Template.Name;
                                    result = true;
                                }
                            }
                            break;
                    }

                    if (result)
                    {
                        client.Player.StoreBag2.MoveToStore(client.Player.StoreBag2, 0, client.Player.MainBag.FindFirstEmptySlot(32), client.Player.MainBag, 9);
                        pkg.WriteInt(0);
                        Gem.Count--;
                        client.Player.UpdateItem(Gem);
                        client.Player.UpdateItem(Item);

                        //client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemInlayHandle.Success", Gem.Template.Name));
                    }
                    LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Insert, BeginProperty, Item, AddItem, Convert.ToInt32(result));  
                }
                else
                {
                    pkg.WriteByte(1);
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemInlayHandle.NoPlace"));
                }
                client.Player.SendTCP(pkg);
                client.Player.SaveIntoDatabase();//保存到数据库
            }

            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
            }
            return 0;
        }
    }
}
