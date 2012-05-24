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

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_TRANSFER, "物品转移")]
    public class ItemTransferHandler : IPacketHandler
    {
        
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            StringBuilder str = new StringBuilder();
            int mustGold = 5000;
            int mustMoney = 1000;

          
            int ordPlace = packet.ReadInt();
            int newPlace = packet.ReadInt();

            ItemInfo ordItem = client.Player.StoreBag2.GetItemAt(0);
            ItemInfo newItem = client.Player.StoreBag2.GetItemAt(1);
            client.Player.StoreBag2.ClearBag();
            client.Player.MainBag.AddItem(ordItem);
            client.Player.MainBag.AddItem(newItem);
            //if (ordItem != null && newItem != null && ordItem.Template.CategoryID == newItem.Template.CategoryID && ordItem.Template.CategoryID < 10 && ordItem.Place > 10 &&
            //    newItem.Place > 10 && newItem.Count == 1 && ordItem.Count == 1 && ordItem.IsValidItem() && newItem.IsValidItem() && ordItem.Template.PayType != 0 && newItem.Template.PayType != 0)
            //{
            //未开始
            if (ordItem != null && newItem != null && ordItem.Template.CategoryID == newItem.Template.CategoryID && ordItem.Template.CategoryID < 10 &&
                newItem.Count == 1 && ordItem.Count == 1 && ordItem.IsValidItem() && newItem.IsValidItem())
            {
                if (ordItem.StrengthenLevel == 0 && ordItem.StrengthenLevel == 0 && ordItem.DefendCompose == 0 && ordItem.LuckCompose == 0 && ordItem.AgilityCompose == 0 && ordItem.AttackCompose == 0)
                    return 1;

                if (ordItem.Template.CategoryID == 7 && client.Player.PlayerCharacter.Money < mustMoney)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nomoney"));
                    return 1;
                }
                else if (client.Player.PlayerCharacter.Gold < mustGold)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold"));
                    return 1;
                }
             
                client.Player.BeginChanges();
                client.Player.MainBag.BeginChanges();
                try
                {
                    if (ordItem.Template.CategoryID == 7)
                    {
                        client.Player.RemoveMoney(mustMoney);
                        LogMgr.LogMoneyAdd(LogMoneyType.Item, LogMoneyType.Item_Move, client.Player.PlayerCharacter.ID, mustMoney, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                    }
                    else
                    {
                        client.Player.RemoveGold(mustGold);
                    }

                    str.Append(ordItem.ItemID + ":" + ordItem.TemplateID + ",");
                    str.Append(ordItem.StrengthenLevel + ",");
                    str.Append(ordItem.AttackCompose + ",");
                    str.Append(ordItem.DefendCompose + ",");
                    str.Append(ordItem.LuckCompose + ",");
                    str.Append(ordItem.AgilityCompose + ",");

                    newItem.StrengthenLevel = ordItem.StrengthenLevel;
                    ordItem.StrengthenLevel = 0;
                    newItem.AttackCompose = ordItem.AttackCompose;
                    ordItem.AttackCompose = 0;
                    newItem.DefendCompose = ordItem.DefendCompose;
                    ordItem.DefendCompose = 0;
                    newItem.LuckCompose = ordItem.LuckCompose;
                    ordItem.LuckCompose = 0;
                    newItem.AgilityCompose = ordItem.AgilityCompose;
                    ordItem.AgilityCompose = 0;
                    newItem.IsBinds = true;

                    client.Player.MainBag.UpdateItem(ordItem);
                    client.Player.MainBag.UpdateItem(newItem);

                    str.Append(newItem.ItemID + ":" + newItem.TemplateID + ",");
                    str.Append(newItem.StrengthenLevel + ",");
                    str.Append(newItem.AttackCompose + ",");
                    str.Append(newItem.DefendCompose + ",");
                    str.Append(newItem.LuckCompose + ",");
                    str.Append(newItem.AgilityCompose);
                    
                    pkg.WriteByte(0);
                    client.Out.SendTCP(pkg);
                }
                finally
                {
                    client.Player.CommitChanges();
                   
                    client.Player.MainBag.CommitChanges();
                }
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition"));
            }

            return 0;
        }
    }
}
