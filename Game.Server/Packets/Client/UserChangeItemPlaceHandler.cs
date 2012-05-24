using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using System.Reflection;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CHANGE_PLACE_ITEM, "改变物品位置")]
    public class UserChangeItemPlaceHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bagType = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            //int toBagTemp = packet.ReadByte();
            //if (toBagTemp == 11) toBagTemp++;
            //eBageType tobagType = (eBageType)toBagTemp;
            eBageType tobagType = (eBageType)packet.ReadByte();
            int toplace = packet.ReadInt();
            int count = packet.ReadInt();
            //pkg.writeByte(bagtype);
            //pkg.writeInt(place);
            //pkg.writeByte(tobagType);
            //pkg.writeInt(toplace);
            //pkg.writeInt(count);
            PlayerInventory bag = client.Player.GetInventory(bagType);
            PlayerInventory tobag = client.Player.GetInventory(tobagType);
            


            //chong hack chua xac dinh. do nem' vui khi di lung tung
            if (bag==null||bag.GetItemAt(place) == null) return 0;
            // chuyen do noi bo trong cung 1 tui'
            if ((bagType == tobagType)&&place!=-1)
            {
                  //.GetItemInventory(temp);
                bag.MoveItem(place, toplace, count);
                return 1;
            }
            if (place == -1&&toplace!=-1)
            {
                bag.RemoveItemAt(toplace);
                return 1;
            }
            if (place != -1 && toplace == -1&&bagType!=eBageType.CaddyBag&& tobagType !=eBageType.Bank&&bagType!=eBageType.Store&&tobagType !=eBageType.Store)
            {
                if (bagType == 0)
                {
                    bag.AddItem(client.Player.GetItemAt(bagType,place), 31); //toSolt = bag.FindFirstEmptySlot(31);
                }
                else
                {
                   bag.AddItem(client.Player.GetItemAt(bagType,place),0);
                }
                return 1;

            }
            if(place!=-1&&tobagType!=bagType&&tobagType!=eBageType.Store&&tobagType!=eBageType.MainBag)
            {
                ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
                if (info != null)
                {
                    //if(tobagType==eBageType.Store) client.Player.StoreBag.MoveToStore(bag, place, bagType, info.StoreLevel * 10);
                
                    bag.MoveToStore(bag, place, toplace, tobag, info.StoreLevel * 10);
                    return 1;
                  
                }
            }

            //danh cho store khi nang cap chuyen do ve main
            if (tobagType == eBageType.Store||bagType==eBageType.Store)
            {
                var item = client.Player.GetItemAt(bagType, place);
                if (item != null && item.Count > 1)
                {
                    item.Count--;
                    bag.UpdateItem(item);
                    var tempItem = item.Clone();
                    tempItem.Count = 1;
                    if (tobag.GetItemAt(toplace) == null)
                    {
                        tobag.AddItemTo(tempItem, toplace);
                    }
                    else
                    {
                        var tempItem2 = bag.GetItemByTemplateID(0, tobag.GetItemAt(toplace).TemplateID);
                        if (tempItem2 == null)
                        {
                            bag.MoveToStore(tobag, toplace, bag.FindFirstEmptySlot(0), bag, 999);
                        }
                        else
                        {
                            tempItem2.Count++;
                            bag.UpdateItem(tempItem2);
                            tobag.RemoveItemAt(toplace);
                        }

                        tobag.AddItemTo(tempItem, toplace);
                    }
                    //tobag.CommitChanges();
                }
                else
                {
                    if (tobagType != eBageType.Store && tobagType != eBageType.MainBag &&bag.GetItemAt(place)!=null&& bag.GetItemAt(place).Template.CategoryID == 7 && (toplace > 0 && toplace < 31 && toplace != 6))
                    {
                        return 1;
                    }
                    try
                    {
                        bag.MoveToStore(bag, place, toplace, tobag, 50);
                    }
                    catch (Exception e)
                    {
                        LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType).InfoFormat("ERROR USER CHANGEITEM placce: {0},toplace: {1},bagType: {2},ToBagType {3}",place,toplace,bagType,tobagType);
                        LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType).InfoFormat("item :{0}, toitem {1}",bag.GetItemAt(place), tobag.GetItemAt(toplace));
                    }
                }
                
                return 1;
                
            }

            //danh cho tu ngan hang chuyen do ve main
            if(tobagType==eBageType.MainBag&&bagType==eBageType.Bank)
            {
                 bag.MoveToStore(bag, place, toplace, tobag, 50);
                return 1;
            }
            return 0;
        }
    }
}
