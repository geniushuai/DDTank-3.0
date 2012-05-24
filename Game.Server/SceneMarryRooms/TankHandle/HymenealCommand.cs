using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;
using Game.Server.Packets;
using System.Timers;
using Game.Server.Enumerate;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    [MarryCommandAttbute((byte)MarryCmdType.HYMENEAL_BEGIN)]
    public class HymenealCommand:IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
            {
                return false;
            }

            if(player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID )
            {
                return false;
            }

            int needMoney = GameProperties.PRICE_PROPOSE;
            if(player.CurrentMarryRoom.Info.IsHymeneal)
            {
                if (player.PlayerCharacter.Money < needMoney)
                {
                    player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough"));
                    return false;
                }
            }

            GamePlayer Groom = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.GroomID);
            if(Groom == null)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoGroom"));
                return false;
            }

            GamePlayer Bride = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.BrideID);
            if (Bride == null)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoBride"));
                return false;
            }

            bool result = false;
            bool isFirst = false;
            GSPacketIn pkg = packet.Clone();

            int hymenealState = packet.ReadInt();

            if (1 == hymenealState)
            {
                player.CurrentMarryRoom.RoomState = eRoomState.FREE;
            }
            else
            {
                player.CurrentMarryRoom.RoomState = eRoomState.Hymeneal;
                player.CurrentMarryRoom.BeginTimerForHymeneal(170 * 1000);

                if (!player.PlayerCharacter.IsGotRing)
                {
                    isFirst = true;
                    ItemTemplateInfo ringTemplate = ItemMgr.FindItemTemplate(9022);
                    ItemInfo ring1 = ItemInfo.CreateFromTemplate(ringTemplate, 1, (int)ItemAddType.webing);
                    ring1.IsBinds = true;
                    //Groom.CurrentInventory.AddItem(ring1, 11);
                    using (PlayerBussiness pb = new PlayerBussiness())
                    {
                        ring1.UserID = 0;
                        pb.AddGoods(ring1);

                        string content = LanguageMgr.GetTranslation("HymenealCommand.Content", Bride.PlayerCharacter.NickName);

                        MailInfo mail = new MailInfo();
                        mail.Annex1 = ring1.ItemID.ToString();
                        mail.Content = content;
                        mail.Gold = 0;
                        mail.IsExist = true;
                        mail.Money = 0;
                        mail.Receiver = Groom.PlayerCharacter.NickName;
                        mail.ReceiverID = Groom.PlayerCharacter.ID;
                        mail.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender");
                        mail.SenderID = 0;
                        mail.Title = LanguageMgr.GetTranslation("HymenealCommand.Title");
                        mail.Type = (int)eMailType.Marry;
                        if (pb.SendMail(mail))
                        {
                            result = true;
                        }
                        player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
                    }


                    ItemInfo ring2 = ItemInfo.CreateFromTemplate(ringTemplate, 1, (int)ItemAddType.webing);
                    ring2.IsBinds = true;
                    //Bride.CurrentInventory.AddItem(ring2, 11);
                    using (PlayerBussiness pb = new PlayerBussiness())
                    {
                        ring2.UserID = 0;
                        pb.AddGoods(ring2);

                        string content = LanguageMgr.GetTranslation("HymenealCommand.Content", Groom.PlayerCharacter.NickName);

                        MailInfo mail = new MailInfo();
                        mail.Annex1 = ring2.ItemID.ToString();
                        mail.Content = content;
                        mail.Gold = 0;
                        mail.IsExist = true;
                        mail.Money = 0;
                        mail.Receiver = Bride.PlayerCharacter.NickName;
                        mail.ReceiverID = Bride.PlayerCharacter.ID;
                        mail.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender");
                        mail.SenderID = 0;
                        mail.Title = LanguageMgr.GetTranslation("HymenealCommand.Title");
                        mail.Type = (int)eMailType.Marry;
                        if (pb.SendMail(mail))
                        {
                            result = true;
                        }
                        player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
                    }


                    player.CurrentMarryRoom.Info.IsHymeneal = true;

                    using (PlayerBussiness db = new PlayerBussiness())
                    {
                        db.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);

                        //更新IsGotRing的数据库
                        db.UpdatePlayerGotRingProp(Groom.PlayerCharacter.ID,Bride.PlayerCharacter.ID);

                        //通过数据库读入PlayerCharacter内存
                        Groom.LoadMarryProp();
                        Bride.LoadMarryProp();
                    }

                    //Groom.PlayerCharacter.IsGotRing = true;
                    //Bride.PlayerCharacter.IsGotRing = true;
                    


                }
                else
                {
                    isFirst = false;
                    result = true;
                }

                if (!isFirst)
                {
                    //player.SetMoney(-needMoney, MoneyRemoveType.Marry);
                    player.RemoveMoney(needMoney);
                    LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Hymeneal, player.PlayerCharacter.ID, needMoney, player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                    CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, (int)eConsumerType.Marry, (int)eSubConsumerType.Marry_Hymeneal);
                }

                pkg.WriteInt(player.CurrentMarryRoom.Info.ID);
                pkg.WriteBoolean(result);
                //0 player.CurrentMarryRoom.SendToAll(pkg);
                //0 player.CurrentMarryRoom.SendToAllForScene(pkg,1);
                player.CurrentMarryRoom.SendToAll(pkg);

                if(result)
                {
                    string msg = LanguageMgr.GetTranslation("HymenealCommand.Succeed", Groom.PlayerCharacter.NickName, Bride.PlayerCharacter.NickName);
                    GSPacketIn message = player.Out.SendMessage(eMessageType.ChatNormal, msg);
                    player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(message, player);
                }
            }

            return true;
        }

    }
}
