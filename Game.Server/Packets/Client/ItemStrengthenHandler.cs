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
using Game.Server.GameObjects;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_STRENGTHEN, "物品强化")]
    public class ItemStrengthenHandler : IPacketHandler
    {
        public static int countConnect = 0;

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            if (countConnect >= 3000)
            {
                client.Disconnect();
                return 0;
            }

            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            StringBuilder str = new StringBuilder();
            bool isBinds = false;
            int mustGold = GameProperties.PRICE_STRENGHTN_GOLD;
            if (client.Player.PlayerCharacter.Gold < mustGold)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoMoney"));
                return 0;
            }

            int luckPlace = 0;
            int godPlace = 0;

            //int itemPlace = client.Player.TempBag.GetItemAt(0).ItemID ;
            //int luckPlace = packet.ReadInt();
            //int godPlace = packet.ReadInt();
            //int stone1Place = packet.ReadInt();
            //int stone2Place = packet.ReadInt();
            //int stone3Place = packet.ReadInt();
            bool consortia = packet.ReadBoolean();

            List<ItemInfo> stones = new List<ItemInfo>();
            ItemInfo item = client.Player.StoreBag2.GetItemAt(5);
            ItemInfo luck = null;
            ItemInfo god = null;
            string BeginProperty = null;
            string AddItem = "";
            using (ItemRecordBussiness db = new ItemRecordBussiness())
            {
                db.PropertyString(item, ref BeginProperty);
            }

            if (item != null && item.Template.CanStrengthen && item.Template.CategoryID < 18 && item.Count == 1)
            {
                isBinds = isBinds ? true : item.IsBinds;
                str.Append(item.ItemID + ":" + item.TemplateID + ",");
                ThreadSafeRandom random = new ThreadSafeRandom();
                int result = 1;
                double probability = 0;
                bool isGod = false;

                StrengthenInfo strengthenInfo;
                StrengthenGoodsInfo strengthenGoodsInfo = null;
                if (item.Template.RefineryLevel > 0)
                {
                    strengthenInfo = StrengthenMgr.FindRefineryStrengthenInfo(item.StrengthenLevel + 1);
                }
                else
                {
                    strengthenGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(item.StrengthenLevel, item.TemplateID);
                    strengthenInfo = StrengthenMgr.FindStrengthenInfo(item.StrengthenLevel + 1);
                }
                if (strengthenInfo == null)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoStrength"));
                    return 0;
                }

                if (client.Player.StoreBag2.GetItemAt(3) != null)
                {
                    god = client.Player.StoreBag2.GetItemAt(3);
                    AddItem += "," + god.ItemID.ToString() + ":" + god.Template.Name;
                    if (god != null && god.Template.CategoryID == 11 && god.Template.Property1 == 7)
                    {
                        isBinds = isBinds ? true : god.IsBinds;
                        str.Append(god.ItemID + ":" + god.TemplateID + ",");
                        isGod = true;
                    }
                    else
                    {
                        god = null;
                    }
                }

                ItemInfo stone1 = client.Player.StoreBag2.GetItemAt(0);
                if (stone1 != null && stone1.Template.CategoryID == 11 && (stone1.Template.Property1 == 2 || stone1.Template.Property1 == 35) && !stones.Contains(stone1))
                {
                    isBinds = isBinds ? true : stone1.IsBinds;
                    AddItem += "," + stone1.ItemID.ToString() + ":" + stone1.Template.Name;
                    stones.Add(stone1);
                    probability += stone1.Template.Property2;
                }

                ItemInfo stone2 = client.Player.StoreBag2.GetItemAt(1);
                if (stone2 != null && stone2.Template.CategoryID == 11 && (stone2.Template.Property1 == 2 || stone2.Template.Property1 == 35) && !stones.Contains(stone2))
                {
                    isBinds = isBinds ? true : stone2.IsBinds;
                    AddItem += "," + stone2.ItemID.ToString() + ":" + stone2.Template.Name;
                    stones.Add(stone2);
                    probability += stone2.Template.Property2;
                }

                ItemInfo stone3 = client.Player.StoreBag2.GetItemAt(2);
                if (stone3 != null && stone3.Template.CategoryID == 11 && (stone3.Template.Property1 == 2 || stone3.Template.Property1 == 35) && !stones.Contains(stone3))
                {
                    isBinds = isBinds ? true : stone3.IsBinds;
                    AddItem += "," + stone3.ItemID + ":" + stone3.Template.Name;
                    stones.Add(stone3);
                    probability += stone3.Template.Property2;
                }
                bool RefineryStrengthen = false;

                foreach (ItemInfo stoneinfo in stones)
                {
                    if (stoneinfo.Template.Property1 == 35 && stoneinfo.Template.CategoryID == 11)
                    {
                        RefineryStrengthen = true;

                    }
                    else
                    {
                        RefineryStrengthen = false;
                    }
                }

                //if (!RefineryStrengthen && item.Template.RefineryLevel > 0)
                //{
                //    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.StoneMatch"));
                //    return 1;
                //}
                if (client.Player.StoreBag2.GetItemAt(4) != null)
                {
                    luck = client.Player.StoreBag2.GetItemAt(4);
                    AddItem += "," + luck.ItemID.ToString() + ":" + luck.Template.Name;
                    if (luck != null && luck.Template.CategoryID == 11 && luck.Template.Property1 == 3)
                    {
                        isBinds = isBinds ? true : luck.IsBinds;
                        str.Append(luck.ItemID + ":" + luck.TemplateID + ",");
                        probability = probability * (luck.Template.Property2 + 100);
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
                bool CanUpdate = false;
                ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                //判断是公会铁匠铺还是铁匠铺??
                if (consortia)
                {
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
                        CanUpdate = true;
                    }
                }

                if (stones.Count >= 1)
                {
                    probability = probability / strengthenInfo.Rock;
                    for (int i = 0; i < stones.Count; i++)
                    {
                        str.Append(stones[i].ItemID + ":" + stones[i].TemplateID + ",");

                        AbstractInventory bg = client.Player.GetItemInventory(stones[i].Template);
                        stones[i].Count--;
                        bg.UpdateItem(stones[i]);
                    }

                    if (luck != null)
                    {
                        AbstractInventory bg = client.Player.GetItemInventory(luck.Template);
                        bg.RemoveItem(luck);
                    }

                    if (god != null)
                    {
                        AbstractInventory bg = client.Player.GetItemInventory(god.Template);
                        bg.RemoveItem(god);
                    }
                    if (CanUpdate)
                    {
                        probability *= (1 + 0.1 * info.SmithLevel);
                    }
                    item.IsBinds = isBinds;

                    client.Player.StoreBag2.ClearBag();
                    if (probability > random.Next(10000))
                    {
                        str.Append("true");
                        pkg.WriteByte(0);
                        if (strengthenGoodsInfo != null)
                        {
                            ItemTemplateInfo Temp = Bussiness.Managers.ItemMgr.FindItemTemplate(strengthenGoodsInfo.GainEquip);
                            if (Temp != null)
                            {
                                ItemInfo Item = ItemInfo.CreateFromTemplate(Temp, 1, (int)ItemAddType.Strengthen);
                                Item.StrengthenLevel = item.StrengthenLevel + 1;


                                ItemInfo.OpenHole(ref Item);
                                StrengthenMgr.InheritProperty(item, ref Item);

                                client.Player.RemoveItem(item);
                                client.Player.AddTemplate(Item, Item.Template.BagType, Item.Count);
                                item = Item;
                                pkg.WriteBoolean(false);
                            }
                        }
                        else
                        {
                            pkg.WriteBoolean(true);
                            item.StrengthenLevel++;
                            ItemInfo.OpenHole(ref item);
                            //client.Player.MainBag.AddItem(item);
                            client.Player.StoreBag2.AddItemTo(item, 5);
                        }



                        client.Player.OnItemStrengthen(item.Template.CategoryID, item.StrengthenLevel);//任务<强化>                                               
                        LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Strengthen, BeginProperty, item, AddItem, 1);//强化日志
                        client.Player.SaveIntoDatabase();//保存到数据库
                        //系统广播
                        if (item.StrengthenLevel >= 7)
                        {
                            string msg = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation", client.Player.PlayerCharacter.NickName, item.Template.Name, item.StrengthenLevel);

                            GSPacketIn pkg1 = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
                            pkg1.WriteInt(1);
                            pkg1.WriteString(msg);

                            GameServer.Instance.LoginServer.SendPacket(pkg1);

                            GamePlayer[] players = Game.Server.Managers.WorldMgr.GetAllPlayers();

                            foreach (GamePlayer p in players)
                            {
                                p.Out.SendTCP(pkg1);
                            }
                        }

                    }
                    else
                    {
                        str.Append("false");
                        pkg.WriteByte(1);
                        pkg.WriteBoolean(false);
                        if (isGod == false)
                        {
                            if (item.Template.Level == 3)
                            {
                                item.StrengthenLevel = item.StrengthenLevel == 0 ? 0 : item.StrengthenLevel - 1;
                                // client.Player.MainBag.AddItem(item);
                                client.Player.StoreBag2.AddItemTo(item, 5);
                            }
                            else
                            {
                                item.Count--;
                                // client.Player.MainBag.AddItem(item);
                                client.Player.StoreBag2.AddItemTo(item, 5);
                            }
                        }
                        else
                        {

                            //client.Player.MainBag.AddItem(item);
                            client.Player.StoreBag2.AddItemTo(item, 5);
                        }
                        LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Strengthen, BeginProperty, item, AddItem, 0);

                        client.Player.SaveIntoDatabase();//保存到数据库
                    }

                    client.Out.SendTCP(pkg);
                    str.Append(item.StrengthenLevel);
                    client.Player.BeginChanges();
                    client.Player.RemoveGold(mustGold);
                    client.Player.CommitChanges();
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1") + result + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2"));
                }
                if (item.Place < 31)
                    client.Player.MainBag.UpdatePlayerProperties();
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Success"));
            }

            return 0;
        }
    }
}
