using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness.Managers;
using SqlDataProvider.Data;
using Game.Server.GameUtils;
using Bussiness;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.LOTTERY_OPEN_BOX, "打开物品")]
    public class LotteryOpenBoxHandler : IPacketHandler
    {
        public static List<int> listTemplate = new List<int>(){
                311000,
                312000,
                311100,
                311199,
                311200,
                311299,
                311300,
                311399,
                311400,
                311499,
                311999,
                313499,
                313500,
                313599,
                313999,
                313000
        };

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int bagType = packet.ReadByte();
            int place = packet.ReadInt();
            PlayerInventory arkBag = client.Player.GetInventory((eBageType)bagType);
            PlayerInventory propBag = client.Player.PropBag;
            var totalCount = 0;
            var havelist = new List<int>();
            var listGoods = new List<ItemInfo>();
            foreach (var item in listTemplate)
            {

                var c = propBag.GetItemCount(0, item);
                if (c != 0)
                {
                    totalCount += c;
                    havelist.Add(item);
                    ItemTemplateInfo goods = Bussiness.Managers.ItemMgr.FindItemTemplate(item);
                    listGoods.Add(ItemInfo.CreateFromTemplate(goods, 1, (int)ItemAddType.Fusion));
                }
            }
            ItemInfo deleteItem;
            List<ItemInfo> infos = new List<ItemInfo>();
            if (listGoods.Count > 0)
            {
                var index = new Random().Next(0, listGoods.Count);
                var goods = listGoods[index];
                int money = 0;
                int gold = 0;
                int giftToken = 0;
                int[] bags = new int[3];     
                OpenUpItem(goods.Template.Data, bags, infos, ref gold, ref money, ref giftToken);
                //
                if(infos.Count>0){

                  

                    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CADDY_GET_AWARDS, client.Player.PlayerId);

                    pkg.WriteBoolean(true);

                    var length = 1;
                    pkg.WriteInt(length);
                    for (int i = 0; i < length; i++)
                    {
                        pkg.WriteString(infos[0].Template.Name);
                        pkg.WriteInt(infos[0].TemplateID);
                        //zoneId
                        pkg.WriteInt(4);
                        pkg.WriteBoolean(false);
                        //pkg.WriteString("ZoneName");
                    }
                    client.Out.SendTCP(pkg);

                    arkBag.AddItem(infos[0]);
                    deleteItem = propBag.GetItemByTemplateID(0, goods.TemplateID);
                    if (deleteItem.Count > 0)
                    {
                        deleteItem.Count--;
                        propBag.UpdateItem(deleteItem);
                    }
                    else propBag.RemoveItem(deleteItem);
                }
            }
         
 
           
            //GSPacketIn pkg = new GSPacketIn((byte)ePackageType.LOTTERY_ALTERNATE_LIST, client.Player.PlayerId);

            //int money = 0;
            //int gold = 0;
            //int giftToken = 0;
            //int[] bags = new int[3];
            //List<ItemInfo> infos = new List<ItemInfo>();
            //foreach (var goods in listGoods)
            //{
            //    if (goods != null && goods.IsValidItem() && goods.Template.CategoryID == 11 && goods.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= goods.Template.NeedLevel)
            //    {
            //        OpenUpItem(goods.Template.Data, bags, infos, ref gold, ref money, ref giftToken);
            //    }
            //}
            //foreach (var info in infos)
            //{
            //    pkg.WriteInt(info.TemplateID);
            //    pkg.WriteBoolean(info.IsBinds);
            //    pkg.WriteByte((byte)info.Count);
            //    pkg.WriteByte(1);
            //   // arkBag.AddItem(info);
            //}
            //if (infos.Count < 18&&infos.Count>1)
            //{
            //    for (int i = 0; i < 18-infos.Count; i++)
            //    {
            //        pkg.WriteInt(infos[0].TemplateID);
            //        pkg.WriteBoolean(infos[0].IsBinds);
            //        pkg.WriteByte((byte)infos[0].Count);
            //        pkg.WriteByte(1);

            //    }
            //}


            //client.Out.SendTCP(pkg);
            return 1;
        }

        //ID(-100表示金币,-200表示点券),数量,日期,颜色,强化等级,合成攻击,合成防御,合成幸运,合成敏捷,是否绑定(0不绑定,1绑定),出现类型(0出现,1单独机率,2总合机率),出现机率(单独机率10000为百分百)
        public void OpenUpItem(string data, int[] bag, List<ItemInfo> infos, ref int gold, ref int money, ref int giftToken)
        {

            if (!string.IsNullOrEmpty(data))
            {
                ItemBoxMgr.CreateItemBox(Convert.ToInt32(data), infos, ref gold, ref money, ref giftToken);
                return;
            }
        }
    }
}
