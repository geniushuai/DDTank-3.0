using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Bussiness.Managers;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CARDS_DATA, "防沉迷系统开关")]
    class CardDataHander : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            //return 1;
            var typeCard = packet.ReadInt();
            var param1 = packet.ReadInt();
            var param2 = 0;
            var cardBag = client.Player.CardBag;
            ItemInfo item = null;
            cardBag.BeginChanges();
            switch (typeCard)
            {
                case 0:
                    //move card
                    param2 = packet.ReadInt();
                    if (param1 > 4)
                    {
                        if (cardBag.GetItemAt(param1) != null)
                        {
                            cardBag.MoveItem(param1, param2, cardBag.GetItemAt(param1).Count);
                        }
                    }
                    else
                    {
                        item = cardBag.GetItemAt(param1);
                        if (item == null) return 1;
                        if (cardBag.GetItemByTemplateID(5, item.TemplateID) != null)
                        {
                            var itemUpdate = cardBag.GetItemByTemplateID(5, item.TemplateID);
                            itemUpdate.Count++;
                            cardBag.UpdateItem(itemUpdate);
                            cardBag.RemoveItem(item);
                            break;
                        }
                        else
                        {
                            cardBag.MoveItem(item.Place,cardBag.FindFirstEmptySlot(5),item.Count);
                        }
                    }
                    break;
                case 1:
                    //open vice card
                    //mo phong an cho card

                    break;
                case 2:
                    //OpenCardBox

                    param2 = packet.ReadInt();
                    item = client.Player.MainBag.GetItemAt(param1);
                   client.Player.MainBag.RemoveItem(item);
                    var cardTemplateID = item.Template.Property5;
                    ItemMgr.FindItemTemplate(cardTemplateID);
                    //neu co' roi` thi cong them vo
                    if (cardBag.GetItemByTemplateID(5,cardTemplateID)!=null)
                    {
                        var itemUpdate=cardBag.GetItemByTemplateID(5,cardTemplateID);
                        itemUpdate.Count++;
                        cardBag.UpdateItem(itemUpdate);
                        break;
                    }
                    //neu chua co'
                    var index = cardBag.FindFirstEmptySlot(5);
                    cardBag.AddItemTo(ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(cardTemplateID), 1, (int)ItemAddType.Buy), index);
                    break;
                case 3:
                    //UpgradeCard
                    item = cardBag.GetItemAt(param1);
                    if (item != null && item.Count > 3)
                    {
                        item.Count -= 3;
                        item.AgilityCompose += 250;
                        if (item.AgilityCompose > 500 && item.AgilityCompose < 2000)
                        {
                            item.StrengthenLevel =1;
                        }
                        else if (item.AgilityCompose > 2000 && item.AgilityCompose < 7000)
                        {
                            item.StrengthenLevel = 2;
                        }
                        else if (item.AgilityCompose > 7000 && item.AgilityCompose < 10000)
                        {
                            item.StrengthenLevel = 3;
                        }
                        cardBag.UpdateItem(item);
                    }
                    break;
                case 4:
                    //CardSort
                case 5:
                    //FirstGetCard
                    break;

                default:
                    break;
            }
            cardBag.CommitChanges();
           // cardBag.SaveToDatabase();
            //GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CARDS_DATA);
            //pkg.WriteInt(client.Player.PlayerCharacter.ID);
            //var length = 17;
            //pkg.WriteInt(17);
            //for (int i = 0; i < length; i++)
            //{
            //    pkg.WriteInt(i);
            //    if (cardBag.GetItemAt(i) != null)
            //    {
                   
            //        pkg.WriteBoolean(true);
            //        item = cardBag.GetItemAt(i);
            //        //cardId
            //        pkg.WriteInt(item.ItemID);
            //        pkg.WriteInt(client.Player.PlayerCharacter.ID);
            //        //count
            //        pkg.WriteInt(item.Count);
            //        //place
            //        pkg.WriteInt(item.Place);
            //        pkg.WriteInt(item.Template.TemplateID);
            //        //_loc_9.Attack = _loc_2.readInt();
            //        //  _loc_9.Defend = _loc_2.readInt();
            //        //  _loc_9.Agility = _loc_2.readInt();
            //        //  _loc_9.Lucy = _loc_2.readInt();
            //        //  _loc_9.Damage = _loc_2.readInt();
            //        //  _loc_9.Guard = _loc_2.readInt();
            //        //  _loc_9.Level = _loc_2.readInt();
            //        //  _loc_9.CardGP = _loc_2.readInt()
            //        pkg.WriteInt(item.Attack); pkg.WriteInt(item.Defence); pkg.WriteInt(item.Agility);
            //        pkg.WriteInt(item.Luck); pkg.WriteInt(item.AttackCompose); 
            //        pkg.WriteInt(i); pkg.WriteInt(3); pkg.WriteInt(3);
            //        pkg.WriteBoolean(true);
            //    }
            //    else
            //    {
            //        pkg.WriteBoolean(false);
            //    }
            //}
            //client.SendTCP(pkg);
            return 0;
        }
    }
}
