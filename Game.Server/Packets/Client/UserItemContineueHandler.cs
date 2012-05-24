using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_CONTINUE, "续费")]
    public class UserItemContineueHandler : IPacketHandler
    {

        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  续费<已测试>           
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //在游戏中不能续费
            //if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.IsPlaying)
            //    return 0;
            //二次密码
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }
            StringBuilder payGoods = new StringBuilder();                    //表示支付物品ID
            int count = packet.ReadInt();
            
            for (int i = 0; i < count; i++)            
            {
                eBageType bag = (eBageType)packet.ReadByte();
                int place = packet.ReadInt();
                int Goods = packet.ReadInt();
                int type = packet.ReadByte();
                bool isDress = packet.ReadBoolean();

                if ((bag == 0 && place >= 31) || bag == eBageType.PropBag || bag == eBageType.Bank)
                {
                    ItemInfo item = client.Player.GetItemAt(bag, place);
                    if (item != null && item.ValidDate != 0 && !item.IsValidItem() && (bag == 0 || (bag == eBageType.PropBag && item.TemplateID == 10200)))
                    //if (item != null && item.ValidDate != 0 && (bag == 0 || bag == 11 || (bag == 1 && item.TemplateID == 10200)))
                    {
                        int gold = 0;            //表示金币
                        int money = 0;           //表示点券
                        int offer = 0;           //表示功勋
                        int gifttoken = 0;       //表示礼劵
                        int oldDate = item.ValidDate;
                        int oldCount = item.Count;
                        bool isValid = item.IsValidItem();
                        List<int> needitemsinfo = new List<int>();

                        eMessageType eMsg = eMessageType.Normal;
                        string msg = "UserBuyItemHandler.Success";

                        ShopItemInfo shopitem = Bussiness.Managers.ShopMgr.GetShopItemInfoById(Goods);                             //获取商品信息
                        needitemsinfo = ItemInfo.SetItemType(shopitem, type, ref gold, ref money, ref offer,ref gifttoken);        //获取物品价格及兑换物TemplatID, Count

                        //////////////////////////////////////////////////////////////////////////////////////
                        //玩家背包中是否有兑换物品所需要的物品
                        int icount = client.Player.MainBag.GetItems().Count;       //获取个数
                        bool result = true;
                        //for (int j = 0; j < needitemsinfo.Count; j += 2)
                        //{
                        //    ItemInfo temp = client.Player.PropBag.GetItemByTemplateID(icount, needitemsinfo[j]);
                        //    int iVaule = client.Player.PropBag.GetItemCount(needitemsinfo[j]);
                        //    if (temp != null || iVaule != needitemsinfo[j + 1] || !temp.IsBinds)
                        //    {
                        //        result = false;
                        //    }
                        //}
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

                       
                        if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money && offer <= client.Player.PlayerCharacter.Offer && gifttoken <= client.Player.PlayerCharacter.GiftToken)
                        {
                            client.Player.RemoveMoney(money);
                            client.Player.RemoveGold(gold);
                            client.Player.RemoveOffer(offer);
                            client.Player.RemoveGiftToken(gifttoken);

                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            //从玩家背包中删除兑换所需要的物品
                            //for (int j = 0; j < needitemsinfo.Count; j += 2)
                            //{
                            //    ItemInfo temp = client.Player.PropBag.GetItemByTemplateID(icount, needitemsinfo[j]);
                            //    client.Player.PropBag.RemoveItem(temp);                                              /////////??????   日志
                            //    payGoods.Append(temp.TemplateID.ToString() + ";");
                            //}
                            for (int j = 0; j < needitemsinfo.Count; j += 2)
                            {
                                client.Player.RemoveTemplate(needitemsinfo[j], needitemsinfo[j + 1]);
                                payGoods.Append(needitemsinfo[j].ToString() + ":");
                            }
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            if (!isValid && item.ValidDate != 0)
                            {
                                if (1 == type)
                                {
                                    item.ValidDate = shopitem.AUnit;
                                    item.BeginDate = DateTime.Now;
                                    item.IsUsed = false;
                                }
                                if (2 == type)
                                {
                                    item.ValidDate = shopitem.BUnit;
                                    item.BeginDate = DateTime.Now;
                                    item.IsUsed = false;
                                }
                                if (3 == type)
                                {
                                    item.ValidDate = shopitem.CUnit;
                                    item.BeginDate = DateTime.Now;
                                    item.IsUsed = false;
                                }
                            }
                            

                            if (bag == 0)
                            {
                                if (isDress)
                                {
                                    int solt = client.Player.MainBag.FindItemEpuipSlot(item.Template);
                                    client.Player.MainBag.MoveItem(place, solt, item.Count);
                                }
                                else
                                {
                                    client.Player.MainBag.UpdateItem(item);
                                }
                            }
                            else if (bag == eBageType.PropBag)
                            {
                                client.Player.PropBag.UpdateItem(item);
                            }
                            else
                            {
                                client.Player.StoreBag.UpdateItem(item);
                            }
                            LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Continue, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "牌子编号", item.TemplateID.ToString(), type.ToString());
                        }
                        else
                        {
                            item.ValidDate = oldDate;
                            item.Count = oldCount;
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.NoMoney"));
                        }
                    }
                }
            }

            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.Success"));

            return 0;
        }
    }
}
