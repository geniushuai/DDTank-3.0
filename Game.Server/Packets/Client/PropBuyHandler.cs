using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.Statics;
using Bussiness.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.PROP_BUY, "购买道具")]
    public class PropBuyHandler : IPacketHandler
    {

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int gold = 0;            //表示金币
            int money = 0;           //表示点券
            int offer = 0;           //表示功勋
            int gifttoken = 0;       //表示礼劵
            StringBuilder payGoods = new StringBuilder();          //表示支付物品ID
            int GoodsID = packet.ReadInt();                        //商品
            int type = 1;                                           //购买类型

            

            ShopItemInfo shopItem = Bussiness.Managers.ShopMgr.GetShopItemInfoById(GoodsID);                   //获取商品信息

            if (shopItem == null)                                                                              //商品不存在
            {
                return 0;
            }
                
            ItemTemplateInfo prop = Bussiness.Managers.ItemMgr.FindItemTemplate(shopItem.TemplateID);          //获取物品模
            List<int> needitemsinfo = ItemInfo.SetItemType(shopItem, type, ref gold, ref money, ref offer, ref gifttoken);   //获取购买价格及需要物品

            eMessageType eMsg = eMessageType.Normal;
            string msg = "UserBuyItemHandler.Success";

            if (prop.CategoryID == 10)
            {
                PlayerInfo pi = client.Player.PlayerCharacter;
                //未开始
                if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked && (money > 0 || offer > 0 || gifttoken > 0 || GoodsID == 11408))
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                    return 0;
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
                    client.Out.SendMessage(eMsg,LanguageMgr.GetTranslation(msg));
                    return 1;
                }
                /////////////////////////////////////////////////////////////

                if (gold <= pi.Gold && money <= (pi.Money<0?0:pi.Money) && offer <= pi.Offer && gifttoken <= pi.GiftToken)
                {
                    ItemInfo info = ItemInfo.CreateFromTemplate(prop, 1, (int)ItemAddType.Buy);

                    ////////////////////////////////////////////////////////////////////////////////////////////////////
                    //判断有限期
                    if (0 == shopItem.BuyType)                              //时间购买类型
                    {
                        if (1 == type)
                        {
                            info.ValidDate = shopItem.AUnit;
                        }
                        if (2 == type)
                        {
                            info.ValidDate = shopItem.BUnit;
                        }
                        if (3 == type)
                        {
                            info.ValidDate = shopItem.CUnit;
                        }
                    }
                    else                                                  //数量购买类型
                    {
                        if (1 == type)
                        {
                            info.Count = shopItem.AUnit;
                        }
                        if (2 == type)
                        {
                            info.Count = shopItem.BUnit;
                        }
                        if (3 == type)
                        {
                            info.Count = shopItem.CUnit;
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (client.Player.FightBag.AddItem(info, 0))
                    {
                        client.Player.RemoveGold(gold);
                        client.Player.RemoveMoney(money);
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
                        LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "牌子编号", prop.TemplateID.ToString(), type.ToString());                        
                    }
                }
                else
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("PropBuyHandler.NoMoney"));
                }

            }
            return 0;
        }
    }
}
