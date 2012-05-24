using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.GameUtils;
using System.Configuration;
using Game.Server.Statics;


namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_FUSION, "熔化")]
    public class ItemFusionHandler : IPacketHandler
    {
        /// <summary>
        /// 熔炼步骤
        /// 第一步：检查四个熔炼类型与熔炼物品&是否有二级密码
        /// 第二步：检验数据是否合乎熔炼规则
        /// 第三步：生成预览或生成物品
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        /// 
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
           


            StringBuilder str = new StringBuilder();
            //第一步：传入操作类型、与四个石头
            int opertionType = packet.ReadByte();
            int count = packet.ReadInt();
            int MinValid = int.MaxValue; //默认最短有效时间
            List<ItemInfo> items = new List<ItemInfo>();
            List<ItemInfo> appendItems = new List<ItemInfo>();
            List<eBageType> bagTypes = new List<eBageType>();
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 1;
            }
            for (int i = 0; i < count; i++)
            {
                eBageType bagType = (eBageType)packet.ReadByte();
                int place = packet.ReadInt();
                ItemInfo info = client.Player.GetItemAt(bagType, place);
                if (info != null)
                {
                    //str.Append(info.ItemID + ":" + info.TemplateID + ",");
                    //items.Add(info);
                    //bagTypes.Add(bagType);
                    if (!items.Contains(info))
                    {
                        str.Append(info.ItemID + ":" + info.TemplateID + ",");
                        items.Add(info);
                        bagTypes.Add(bagType);
                        if (info.ValidDate < MinValid && info.ValidDate != 0)
                        {
                            MinValid = info.ValidDate;
                        }
                    }
                    else
                    {
                        client.Out.SendMessage(eMessageType.Normal, "Bad Input");
                        return 1;
                    }
                }
            }

            if (MinValid == int.MaxValue)
            {
                MinValid = 0;
                items.Clear();
            }
            //第二步：传入熔炼公式与背包类型
            eBageType bagformul = (eBageType)packet.ReadByte();
            int placeformul = packet.ReadInt();
            var storeBag2 = client.Player.StoreBag2;
            ItemInfo formul = storeBag2.GetItemAt(0); ;
            ItemInfo tempitem = null;
            string beginProperty = null;
            string AddItem = "";

            for (int i = 1; i <= 4; i++)
            {
                items.Add(storeBag2.GetItemAt(i));
            }
            using (ItemRecordBussiness db = new ItemRecordBussiness())
            {
                foreach (ItemInfo item in items)
                {
                    db.FusionItem(item, ref beginProperty);
                }
            }

            if (items.Count != 4 || formul == null)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.ItemNotEnough"));
                return 0;
            }

            //第三步：附加物品个数+(背包类型与附加物品位置)
            int appendCount = packet.ReadInt();
            List<eBageType> bagTypesAppend = new List<eBageType>();
            for (int i = 0; i < appendCount; i++)
            {
                eBageType bagType = (eBageType)packet.ReadByte();
                int place = packet.ReadInt();
                ItemInfo info = client.Player.GetItemAt(bagType, place);
                if (info != null)
                {
                    if (!items.Contains(info) && !appendItems.Contains(info))
                    {
                        str.Append(info.ItemID + ":" + info.TemplateID + ",");
                        appendItems.Add(info);
                        bagTypesAppend.Add(bagType);
                        AddItem += info.ItemID + ":" + info.Template.Name + "," + info.IsBinds + "|";
                    }
                    else
                    {
                        client.Out.SendMessage(eMessageType.Normal, "Bad Input");
                        return 1;
                    }
                }

            }
            //结束：预览或熔炼
            if (0 == opertionType) //预览模式
            {
                bool isBind = false;
                Dictionary<int, double> previewItemList = FusionMgr.FusionPreview(items, appendItems, formul, ref isBind);

                if (previewItemList != null)
                {
                    if (previewItemList.Count != 0)
                    {
                        client.Out.SendFusionPreview(client.Player, previewItemList, isBind, MinValid);
                    }
                }
            }
            else //生成熔炼物品
            {
                storeBag2.ClearBag();
                int mustGold = (count + appendCount) * 400;
                if (client.Player.PlayerCharacter.Gold < mustGold)
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney"));
                    return 0;
                }

                bool isBind = false;
                bool result = false;
                ItemTemplateInfo rewardItem = FusionMgr.Fusion(items, appendItems, formul, ref isBind, ref result);
                if (rewardItem != null)
                {
                    client.Player.RemoveGold(mustGold);
                    for (int i = 0; i < items.Count; i++)
                    {
                        items[i].Count--;
                        client.Player.UpdateItem(items[i]);

                    }
                    formul.Count--;
                    client.Player.UpdateItem(formul);
                    for (int i = 0; i < appendItems.Count; i++)
                    {
                        appendItems[i].Count--;
                        client.Player.UpdateItem(appendItems[i]);

                    }

                    if (result)
                    {

                        str.Append(rewardItem.TemplateID + ",");
                        ItemInfo item = ItemInfo.CreateFromTemplate(rewardItem, 1, (int)ItemAddType.Fusion);
                        if (item == null)
                            return 0;
                        tempitem = item;
                        item.IsBinds = isBind;
                        item.ValidDate = MinValid;
                        client.Player.OnItemFusion(item.Template.FusionType);  //熔炼成功
                        client.Out.SendFusionResult(client.Player, result);
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1") + item.Template.Name);

                        //系统广播
                        if (((item.TemplateID >= 8300) && (item.TemplateID <= 8999)) || ((item.TemplateID >= 9300) && (item.TemplateID <= 9999)) || ((item.TemplateID >= 14300) && (item.TemplateID <= 14999)))
                        {
                            string msg = LanguageMgr.GetTranslation("ItemFusionHandler.Notice", client.Player.PlayerCharacter.NickName, item.Template.Name);

                            //client.Out.SendSystemNotice(msg);

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

                        if (!client.Player.AddTemplate(item, item.Template.BagType, item.Count))
                        {
                            str.Append("NoPlace");
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(item.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
                        }
                    }
                    else
                    {
                        str.Append("false");

                        client.Out.SendFusionResult(client.Player, result);
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed"));
                    }
                    LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Fusion, beginProperty, tempitem, AddItem, Convert.ToInt32(result));
                    client.Player.SaveIntoDatabase();//保存到数据库
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
                }
            }

            return 0;
        }
    }
}
