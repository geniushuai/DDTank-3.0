using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using System.Configuration;
using Game.Server.Managers;
using Game.Server.Statics;
using Game.Logic;


namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_COMPOSE, "物品合成")]
    public class ItemComposeHandler : IPacketHandler
    {


        public static int countConnect = 0;public int HandlePacket(GameClient client, GSPacketIn packet){if (countConnect >= 3000){client.Disconnect();return 0;}
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            StringBuilder str = new StringBuilder();
            int mustGold = GameProperties.PRICE_COMPOSE_GOLD;
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }
            if (client.Player.PlayerCharacter.Gold < mustGold)
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemComposeHandler.NoMoney"));
                return 0;
            }

            int itemPlace = - 1;
            int godPlace = -1;
            bool isBinds = false;
            bool consortia = packet.ReadBoolean();

            ItemInfo item = client.Player.StoreBag2.GetItemAt(1);
            ItemInfo stone = client.Player.StoreBag2.GetItemAt(2);
            ItemInfo luck = null;
            ItemInfo god = null;

            string BeginProperty = null;
            string AddItem = null;
            using (ItemRecordBussiness db = new ItemRecordBussiness())
            {
                db.PropertyString(item, ref BeginProperty);
            }
            if (item != null && stone != null && item.Template.CanCompose && (item.Template.CategoryID < 10 || stone.Template.CategoryID == 11 && stone.Template.Property1 == 1))
            {

                isBinds = isBinds ? true : item.IsBinds;
                isBinds = isBinds ? true : stone.IsBinds;
                str.Append(item.ItemID + ":" + item.TemplateID + "," + stone.ItemID + ":" + stone.TemplateID + ",");
                ThreadSafeRandom random = new ThreadSafeRandom();
                bool result = false;
                byte isSuccess = 1;
                bool isGod = false;
                double probability = stone.Template.Property2;
                if (client.Player.StoreBag2.GetItemAt(0)!=null)
                {
                    luck = client.Player.StoreBag2.GetItemAt(0);
                    if (luck != null && luck.Template.CategoryID == 11 && luck.Template.Property1 == 3)
                    {
                        isBinds = isBinds ? true : luck.IsBinds;
                        //probability += luck.Template.Property2;
                        AddItem += "|" + luck.ItemID + ":" + luck.Template.Name + "|" + stone.ItemID + ":" + stone.Template.Name;
                        probability = probability * (luck.Template.Property2 + 100);
                        str.Append(luck.ItemID + ":" + luck.TemplateID + ",");
                    }
                    else
                    {
                        probability *= 100;
                        luck = null;
                    }
                }
                else
                {
                    probability *= 100;
                }
                if (godPlace != -1)
                {
                    god = client.Player.PropBag.GetItemAt(godPlace);
                    if (god != null && god.Template.CategoryID == 11 && god.Template.Property1 == 7)
                    {
                        isBinds = isBinds ? true : god.IsBinds;
                        isGod = true;
                        str.Append(god.ItemID + ":" + god.TemplateID + ",");
                        AddItem += "," + god.ItemID + ":" + god.Template.Name;
                    }
                    else
                    {
                        god = null;
                    }
                }
                //判断是公会铁匠铺还是铁匠铺
                if (consortia)
                {

                    ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);


                    //这里添加公会权限限制的判断
                    ConsortiaBussiness csbs = new ConsortiaBussiness();
                    ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(client.Player.PlayerCharacter.ConsortiaID, 0, 2);

                    if (info == null)
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail"));
                    }
                    else
                    {

                        if (client.Player.PlayerCharacter.Riches < cecInfo.Riches)
                        {
                            client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission"));
                            return 1;
                        }
                        else
                        {
                            probability *= (1 + 0.1 * info.SmithLevel);

                        }
                    }

                }


                int rand = random.Next(10000);
                switch (stone.Template.Property3)
                {
                    case 1:
                        if (stone.Template.Property4 > item.AttackCompose)
                        {
                            result = true;
                            if (probability > rand)
                            {
                                isSuccess = 0;
                                item.AttackCompose = stone.Template.Property4;
                            }
                            else if (!isGod)
                            {
                                // item.AttackCompose = 0;
                            }
                        }
                        break;
                    case 2:
                        if (stone.Template.Property4 > item.DefendCompose)
                        {
                            result = true;
                            if (probability > rand)
                            {
                                isSuccess = 0;
                                item.DefendCompose = stone.Template.Property4;
                            }
                            else if (!isGod)
                            {
                                // item.DefendCompose = 0;
                            }
                        }
                        break;
                    case 3:
                        if (stone.Template.Property4 > item.AgilityCompose)
                        {
                            result = true;
                            if (probability > rand)
                            {
                                isSuccess = 0;
                                item.AgilityCompose = stone.Template.Property4;
                            }
                            else if (!isGod)
                            {
                                //  item.AgilityCompose = 0;
                            }
                        }
                        break;
                    case 4:
                        if (stone.Template.Property4 > item.LuckCompose)
                        {
                            result = true;
                            if (probability > rand)
                            {
                                isSuccess = 0;
                                item.LuckCompose = stone.Template.Property4;
                            }
                            else if (!isGod)
                            {
                                // item.LuckCompose = 0;
                            }
                        }
                        break;
                }

                if (result)
                {
                    item.IsBinds = isBinds;
                    if (isSuccess != 0)
                    {
                        str.Append("false!");
                        result = false;
                        if (!isGod)
                        {
                            if (item.Template.Level < 3)
                            {
                                // item.AttackCompose = 0;
                                // item.DefendCompose = 0;
                                //item.AgilityCompose = 0;
                                // item.LuckCompose = 0;
                            }
                        }
                    }
                    else
                    {
                        str.Append("true!");
                        result = true;
                        client.Player.OnItemCompose(stone.TemplateID);
                    }
                    LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Compose, BeginProperty, item, AddItem, Convert.ToInt32(result));
                    client.Player.RemoveItem(stone);
                    client.Player.SaveIntoDatabase();//保存到数据库
                    if (luck != null)
                    {
                        client.Player.RemoveItem(luck);
                    }
                    if (god != null)
                    {
                        client.Player.RemoveItem(god);
                    }
                    client.Player.RemoveGold(mustGold);
                    client.Player.StoreBag2.ClearBag();
                    client.Player.MainBag.AddItem(item);
                    pkg.WriteByte(isSuccess);
                    client.Out.SendTCP(pkg);
                    if (itemPlace < 30)
                    {
                        client.Player.MainBag.UpdatePlayerProperties();
                    }
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.NoLevel"));
                }
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.Fail"));
            }

            return 0;
        }
    }

}
