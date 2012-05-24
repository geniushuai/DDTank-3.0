using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.GameUtils;
using Bussiness;


namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_OVERDUE, "物品过期")]
    public class ItemOverdueHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //已经开始游戏则不处理
            if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.IsPlaying)
                return 0;
            int bagType = packet.ReadByte();
            int index = packet.ReadInt();
            PlayerInventory bag = client.Player.GetInventory((eBageType)bagType);
            ItemInfo item = bag.GetItemAt(index);

            if (item != null && !item.IsValidItem())
            {
                if (bagType == 0 && index < 11)
                {
                    int place = bag.FindFirstEmptySlot(31);
                    if (place != -1)
                    {
                        bag.RemoveItem(item);
                        //bag.MoveItem(item.Place, place);
                    }
                    else
                    {
                        using (PlayerBussiness pb = new PlayerBussiness())
                        {
                            MailInfo mail = new MailInfo();
                            mail.Annex1 = item.ItemID.ToString();
                            mail.Content = LanguageMgr.GetTranslation("ItemOverdueHandler.Content");
                            mail.Gold = 0;
                            mail.IsExist = true;
                            mail.Money = 0;
                            mail.Receiver = client.Player.PlayerCharacter.NickName;
                            mail.ReceiverID = item.UserID;
                            mail.Sender = client.Player.PlayerCharacter.NickName;
                            mail.SenderID = item.UserID;
                            mail.Title = LanguageMgr.GetTranslation("ItemOverdueHandler.Title");
                            mail.Type = (int)eMailType.ItemOverdue;
                            if (pb.SendMail(mail))
                            {
                                //item.UserID = 0;
                                bag.RemoveItem(item);
                            }
                        }
                    }
                }
                else
                {
                    bag.UpdateItem(item);
                }
            }


            return 0;
        }
    }
}
