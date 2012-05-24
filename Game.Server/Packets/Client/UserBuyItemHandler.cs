using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.Managers;
using Game.Server.GameUtils;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.BUY_ITEM, "购买物品")]
    public class UserBuyItemHandler : IPacketHandler
    {

        public static int countConnect = 0;
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (countConnect >= 3000) { client.Disconnect(); return 0; }


            int gold = 0;            //表示金币
            int money = 0;           //表示点券
            int offer = 0;           //表示功勋
            int gifttoken = 0;       //表示礼劵
            StringBuilder payGoods = new StringBuilder();                    //表示支付物品ID
            eMessageType eMsg = eMessageType.Normal;
            string msg = "UserBuyItemHandler.Success";

            List<ItemInfo> buyitems = new List<ItemInfo>();                  //购买物品列表
            List<int> needitemsinfo = new List<int>();                       //购买所需物品列表
            List<bool> dresses = new List<bool>();
            List<int> places = new List<int>();
            StringBuilder types = new StringBuilder();

            bool isBind = false;
            ConsortiaInfo consotia = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);


            int count = packet.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int GoodsID = packet.ReadInt();
                int type = packet.ReadInt();
                string color = packet.ReadString();
                bool dress = packet.ReadBoolean();
                string skin = packet.ReadString();
                int place = packet.ReadInt();

                //这里开始处理公会商店
                ShopItemInfo shopItem = Bussiness.Managers.ShopMgr.GetShopItemInfoById(GoodsID);                   //获取商品信息
                if (shopItem.ShopID == 2 || !Bussiness.Managers.ShopMgr.CanBuy(shopItem.ShopID, consotia == null ? 1 : consotia.ShopLevel, ref isBind, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches))
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
                    return 1;
                }

                if (shopItem == null)
                {
                    continue;
                }

                ItemTemplateInfo goods = Bussiness.Managers.ItemMgr.FindItemTemplate(shopItem.TemplateID);              //获取物品属性

                ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, (int)ItemAddType.Buy);                            //创建物品模板

                ////////////////////////////////////////////////////////////////////////////////////////////////////
                //判断有限期
                if (0 == shopItem.BuyType)                              //时间购买类型
                {
                    if (1 == type)
                    {
                        item.ValidDate = shopItem.AUnit;
                    }
                    if (2 == type)
                    {
                        item.ValidDate = shopItem.BUnit;
                    }
                    if (3 == type)
                    {
                        item.ValidDate = shopItem.CUnit;
                    }
                }
                else                                                  //数量购买类型
                {
                    if (1 == type)
                    {
                        item.Count = shopItem.AUnit;
                    }
                    if (2 == type)
                    {
                        item.Count = shopItem.BUnit;
                    }
                    if (3 == type)
                    {
                        item.Count = shopItem.CUnit;
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (item == null && shopItem == null)
                    continue;
                item.Color = color == null ? "" : color;
                item.Skin = skin == null ? "" : skin;
                if (isBind == true)
                {
                    item.IsBinds = true;
                }
                else
                {
                    item.IsBinds = Convert.ToBoolean(shopItem.IsBind);
                }

                types.Append(type);
                types.Append(",");
                buyitems.Add(item);
                dresses.Add(dress);
                places.Add(place);

                foreach (var a in ItemInfo.SetItemType(shopItem, type, ref gold, ref money, ref offer, ref gifttoken))//商品的购买条件
                {
                    needitemsinfo.Add(a);

                }
            }

            if (buyitems.Count == 0)
                return 1;
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 1;
            }



            //////////////////////////////////////////////////////////////////////////////////////
            //玩家背包中是否有兑换物品所需要的物品
            int icount = client.Player.MainBag.GetItems().Count;       //获取个数
            bool result = true;
            for (int j = 0; j < needitemsinfo.Count; j += 2)
            {
                if (client.Player.GetItemCount(needitemsinfo[j]) < needitemsinfo[j + 1])
                {
                    result = false;
                }
            }

            if (!result)
            {
                eMsg = eMessageType.ERROR;
                msg = "UserBuyItemHandler.NoBuyItem";
                client.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg));
                return 1;
            }
            /////////////////////////////////////////////////////////////

            //判断金币或礼券等是否足够
            if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money && offer <= client.Player.PlayerCharacter.Offer && gifttoken <= client.Player.PlayerCharacter.GiftToken)
            {
                client.Player.RemoveMoney(money);
                client.Player.RemoveGold(gold);
                client.Player.RemoveOffer(offer);
                client.Player.RemoveGiftToken(gifttoken);

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //从玩家背包中删除兑换所需要的物品
                for (int j = 0; j < needitemsinfo.Count; j += 2)
                {
                    client.Player.RemoveTemplate(needitemsinfo[j], needitemsinfo[j + 1]);
                    payGoods.Append(needitemsinfo[j].ToString() + ":");
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                string itemIDs = "";
                int annexIndex = 0;
                MailInfo message = new MailInfo();
                StringBuilder annexRemark = new StringBuilder();
                annexRemark.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));

                for (int i = 0; i < buyitems.Count; i++)
                {
                    itemIDs += (itemIDs == "" ? buyitems[i].TemplateID.ToString() : "," + buyitems[i].TemplateID.ToString());
                    if (client.Player.AddTemplate(buyitems[i], buyitems[i].Template.BagType, buyitems[i].Count))
                    {
                        if (dresses[i] && buyitems[i].CanEquip())
                        {
                            int slot = client.Player.MainBag.FindItemEpuipSlot(buyitems[i].Template);
                            if ((slot == 9 || slot == 10) && (places[i] == 9 || places[i] == 10))
                            {
                                slot = places[i];
                            }
                            else if ((slot == 7 || slot == 8) && (places[i] == 7 || places[i] == 8))
                            {
                                slot = places[i];
                            }

                            client.Player.MainBag.MoveItem(buyitems[i].Place, slot, 0);
                            msg = "UserBuyItemHandler.Save";
                        }
                    }
                    else
                    {
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            buyitems[i].UserID = 0;
                            db.AddGoods(buyitems[i]);

                            annexIndex++;
                            annexRemark.Append(annexIndex);
                            annexRemark.Append("、");
                            annexRemark.Append(buyitems[i].Template.Name);
                            annexRemark.Append("x");
                            annexRemark.Append(buyitems[i].Count);
                            annexRemark.Append(";");
                            switch (annexIndex)
                            {
                                case 1:
                                    message.Annex1 = buyitems[i].ItemID.ToString();
                                    message.Annex1Name = buyitems[i].Template.Name;
                                    break;
                                case 2:
                                    message.Annex2 = buyitems[i].ItemID.ToString();
                                    message.Annex2Name = buyitems[i].Template.Name;
                                    break;
                                case 3:
                                    message.Annex3 = buyitems[i].ItemID.ToString();
                                    message.Annex3Name = buyitems[i].Template.Name;
                                    break;
                                case 4:
                                    message.Annex4 = buyitems[i].ItemID.ToString();
                                    message.Annex4Name = buyitems[i].Template.Name;
                                    break;
                                case 5:
                                    message.Annex5 = buyitems[i].ItemID.ToString();
                                    message.Annex5Name = buyitems[i].Template.Name;
                                    break;
                            }

                            if (annexIndex == 5)
                            {
                                annexIndex = 0;
                                message.AnnexRemark = annexRemark.ToString();
                                annexRemark.Remove(0, annexRemark.Length);
                                annexRemark.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));

                                message.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title") + message.Annex1Name + "]";
                                message.Gold = 0;
                                message.Money = 0;
                                message.Receiver = client.Player.PlayerCharacter.NickName;
                                message.ReceiverID = client.Player.PlayerCharacter.ID;
                                message.Sender = message.Receiver;
                                message.SenderID = message.ReceiverID;
                                message.Title = message.Content;
                                message.Type = (int)eMailType.BuyItem;
                                db.SendMail(message);

                                eMsg = eMessageType.ERROR;
                                msg = "UserBuyItemHandler.Mail";

                                message.Revert();
                            }
                        }
                    }
                }

                if (annexIndex > 0)
                {
                    using (PlayerBussiness db = new PlayerBussiness())
                    {
                        message.AnnexRemark = annexRemark.ToString();
                        message.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title") + message.Annex1Name + "]";
                        message.Gold = 0;
                        message.Money = 0;
                        message.Receiver = client.Player.PlayerCharacter.NickName;
                        message.ReceiverID = client.Player.PlayerCharacter.ID;
                        message.Sender = message.Receiver;
                        message.SenderID = message.ReceiverID;
                        message.Title = message.Content;
                        message.Type = (int)eMailType.BuyItem;
                        db.SendMail(message);

                        eMsg = eMessageType.ERROR;
                        msg = "UserBuyItemHandler.Mail";
                    }
                }

                if (eMsg == eMessageType.ERROR)
                {
                    client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                }

                client.Player.OnPaid(money, gold, offer, gifttoken, payGoods.ToString());//触发任务事件  


                LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, gifttoken, offer, "牌子编号", itemIDs, types.ToString());
            }
            else
            {
                if (gold > client.Player.PlayerCharacter.Gold)
                {
                    msg = "UserBuyItemHandler.NoGold";
                }
                if (offer > client.Player.PlayerCharacter.Offer)
                {
                    msg = "UserBuyItemHandler.NoOffer";
                }
                if (gifttoken > client.Player.PlayerCharacter.GiftToken)
                {
                    msg = "UserBuyItemHandler.GiftToken";
                }
                eMsg = eMessageType.ERROR;
            }

            client.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg));
            return 0;
        }
    }
}
