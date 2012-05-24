using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.GameUtils;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_EMBED_BACKOUT, "物品比较")]
    public class ItemEmbedBackOutHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();
            //eBageType bagType = (eBageType)packet.ReadByte();
            int holeNum = packet.ReadInt();
            //int toplace = packet.ReadInt();
            int templateID = packet.ReadInt();
            int mustGold = 500;
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
            
            PlayerInventory storeBag = client.Player.GetInventory(eBageType.Store);
            ItemInfo item = storeBag.GetItemAt(0);
            ItemInfo gem;
            ItemTemplateInfo goods = Bussiness.Managers.ItemMgr.FindItemTemplate(templateID); 
           // storeBag.a
            if (goods == null) return 11;
            var result=false;
            switch (holeNum)
            {
                case 1:
                    if (item.Hole1 > 0)
                    {
                        item.Hole1=0;
                        result = true;
                    }
                    break;
                case 2:
                    if (item.Hole2 > 0)
                    {
                        item.Hole2 = 0;
                        result = true;
                    }
                    break;
                case 3:
                    if (item.Hole3 > 0)
                    {
                        item.Hole3 = 0;
                        result = true;
                    }
                    break;
                case 4:
                    if (item.Hole4 > 0)
                    {
                        item.Hole4 = 0;
                        result = true;
                    }
                    break;
                case 5:
                    if (item.Hole5 > 0)
                    {
                        item.Hole5 = 0;
                        result = true;
                    }
                    break;
                case 6:
                    if (item.Hole6 > 0)
                    {
                        item.Hole6 = 0;
                        result = true;
                    }
                    break;
                default:
                    return 1;
                   
            }
            if (result)
            {
                pkg.WriteInt(0);
                client.Player.BeginChanges();
                gem = ItemInfo.CreateFromTemplate(goods, 1, (int)ItemAddType.Buy);
                client.Player.AddItem(gem);
                client.Player.StoreBag2.MoveToStore(client.Player.StoreBag2, 0, client.Player.MainBag.FindFirstEmptySlot(32), client.Player.MainBag, 9);
                client.Player.UpdateItem(item);
                client.Player.RemoveGold(mustGold);
                client.Player.CommitChanges();
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("OK"));
            }
            else
            {

                pkg.WriteInt(1);
            }
            client.Player.SendTCP(pkg);
        
            client.Player.SaveIntoDatabase();//保存到数据库
            
            return 0;
        }
    }
}
