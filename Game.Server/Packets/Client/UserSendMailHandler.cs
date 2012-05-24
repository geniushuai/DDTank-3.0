using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.GameObjects;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{


    [PacketHandler((byte)ePackageType.SEND_MAIL, "发送邮件")]
    public class UserSendMailHandler : IPacketHandler
    {
        // <summary>
        // 修改:  Xiaov 
        // 时间:  2009-11-7
        // 描述:  发送邮件<未测试>   
       
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

           
            if (client.Player.PlayerCharacter.Gold < 100)
                return 1;

           

            string msg = "UserSendMailHandler.Success";
            eMessageType eMsg = eMessageType.Normal;
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            string nickName = packet.ReadString().Trim();
            string title = packet.ReadString();
            string content = packet.ReadString();
            bool isPay = packet.ReadBoolean();
            int validDate = packet.ReadInt();
            int money = packet.ReadInt();

            eBageType bag1 = (eBageType)packet.ReadByte();
            int place1 = packet.ReadInt();
            eBageType bag2 = (eBageType)packet.ReadByte();
            int place2 = packet.ReadInt();
            eBageType bag3 = (eBageType)packet.ReadByte();
            int place3 = packet.ReadInt();
            eBageType bag4 = (eBageType)packet.ReadByte();
            int place4 = packet.ReadInt();

            if ((money!=0||place1!=-1||place2!=-1||place3!=-1||place4!=-1)&&(client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked))
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                pkg.WriteBoolean(false);
                client.Out.SendTCP(pkg);
                return 1;
            }

            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo user;
                GamePlayer player = Managers.WorldMgr.GetClientByPlayerNickName(nickName);
                if (player == null)
                {
                    user = db.GetUserSingleByNickName(nickName);
                }
                else
                {
                    user = player.PlayerCharacter;
                }

                if (user != null && !string.IsNullOrEmpty(nickName))
                {
                    if (user.NickName != client.Player.PlayerCharacter.NickName)
                    {
                        client.Player.SaveIntoDatabase();

                        //附件内容描述（例：邮件最后一行标注附件为：1、大喇叭x5；2、强化石4级x1；3、极•烈火x1；4、点券999999；5、强化公式-朱雀x5
                        MailInfo message = new MailInfo();
                        message.SenderID = client.Player.PlayerCharacter.ID;
                        message.Sender = client.Player.PlayerCharacter.NickName;
                        message.ReceiverID = user.ID;
                        message.Receiver = user.NickName;
                        message.IsExist = true;
                        message.Gold = 0;
                        message.Money = 0;

                        message.Title = title;
                        message.Content = content;
                        List<ItemInfo> items = new List<ItemInfo>();
                        List<eBageType> bagType = new List<eBageType>();
                        StringBuilder annexRemark = new StringBuilder();
                        annexRemark.Append(LanguageMgr.GetTranslation("UserSendMailHandler.AnnexRemark"));
                        int index = 0;

                        if (place1 != -1)
                        {
                            ItemInfo goods = client.Player.GetItemAt(bag1, place1);
                            if (goods != null && !goods.IsBinds)
                            {
                                message.Annex1Name = goods.Template.Name;
                                message.Annex1 = goods.ItemID.ToString();
                                items.Add(goods);
                                bagType.Add(bag1);
                                //annexRemark = message.Annex1Name + "x" + goods.Count;
                                index++;
                                annexRemark.Append(index);
                                annexRemark.Append("、");
                                //annexRemark.Append(annexRemark.Length);
                                annexRemark.Append(message.Annex1Name);
                                annexRemark.Append("x");
                                annexRemark.Append(goods.Count);
                                annexRemark.Append(";");
                                //goods.UserID = 0;
                                //client.Player.RemoveAllItem(goods, true, ItemRemoveType.Mail);
                            }
                        }

                        if (place2 != -1)
                        {
                            ItemInfo goods = client.Player.GetItemAt(bag2, place2);
                            if (goods != null && !goods.IsBinds)
                            {
                                message.Annex2Name = goods.Template.Name;
                                message.Annex2 = goods.ItemID.ToString();
                                items.Add(goods);
                                bagType.Add(bag2);
                                index++;
                                annexRemark.Append(index);
                                annexRemark.Append("、");
                                annexRemark.Append(message.Annex2Name);
                                annexRemark.Append("x");
                                annexRemark.Append(goods.Count);
                                annexRemark.Append(";");
                                //goods.UserID = 0;
                                //client.Player.RemoveAllItem(goods, true, ItemRemoveType.Mail);
                            }
                        }

                        if (place3 != -1)
                        {
                            ItemInfo goods = client.Player.GetItemAt(bag3, place3);
                            if (goods != null && !goods.IsBinds)
                            {
                                message.Annex3Name = goods.Template.Name;
                                message.Annex3 = goods.ItemID.ToString();
                                items.Add(goods);
                                bagType.Add(bag3);
                                index++;
                                annexRemark.Append(index);
                                annexRemark.Append("、");
                                annexRemark.Append(message.Annex3Name);
                                annexRemark.Append("x");
                                annexRemark.Append(goods.Count);
                                annexRemark.Append(";");
                            }
                        }

                        if (place4 != -1)
                        {
                            ItemInfo goods = client.Player.GetItemAt(bag4, place4);
                            if (goods != null && !goods.IsBinds)
                            {
                                message.Annex4Name = goods.Template.Name;
                                message.Annex4 = goods.ItemID.ToString();
                                items.Add(goods);
                                bagType.Add(bag4);
                                index++;
                                annexRemark.Append(index);
                                annexRemark.Append("、");
                                annexRemark.Append(message.Annex4Name);
                                annexRemark.Append("x");
                                annexRemark.Append(goods.Count);
                                annexRemark.Append(";");
                            }
                        }

                        if (isPay)
                        {
                            if (money <= 0 || (string.IsNullOrEmpty(message.Annex1) && string.IsNullOrEmpty(message.Annex2) && string.IsNullOrEmpty(message.Annex3) && string.IsNullOrEmpty(message.Annex4)))
                                return 1;

                            message.ValidDate = validDate == 1 ? 1 : 6;
                            message.Type = (int)eMailType.Payment;
                            if (money > 0)
                            {
                                message.Money = money;
                                index++;
                                annexRemark.Append(index);
                                annexRemark.Append("、");
                                annexRemark.Append(LanguageMgr.GetTranslation("UserSendMailHandler.PayMoney"));
                                annexRemark.Append(money);
                                annexRemark.Append(";");
                            }
                        }
                        else
                        {
                            message.Type = (int)eMailType.Common;
                            if (client.Player.PlayerCharacter.Money >= money && money > 0)
                            {
                                message.Money = money;
                                client.Player.RemoveMoney(money);
                                LogMgr.LogMoneyAdd(LogMoneyType.Mail, LogMoneyType.Mail_Send, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                                index++;
                                annexRemark.Append(index);
                                annexRemark.Append("、");
                                annexRemark.Append(LanguageMgr.GetTranslation("UserSendMailHandler.Money"));
                                annexRemark.Append(money);
                                annexRemark.Append(";");
                            }
                        }

                        if (annexRemark.Length > 1)
                        {
                            message.AnnexRemark = annexRemark.ToString();
                        }

                        if (db.SendMail(message))
                        {
                            client.Player.RemoveGold(100);
                            for(int i=0;i<items.Count;i++)
                            {
                                items[i].UserID = 0;
                                client.Player.RemoveItem(items[i]);
                                items[i].IsExist = true;
                            }
                        }

                        client.Player.SaveIntoDatabase();
                        pkg.WriteBoolean(true);

                        //发送通知
                        if (user.State != 0)
                        {
                            client.Player.Out.SendMailResponse(user.ID, eMailRespose.Receiver);
                        }
                        client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
                    }
                    else
                    {
                        msg = "UserSendMailHandler.Failed1";
                        pkg.WriteBoolean(false);
                    }
                }
                else
                {   
                     
                    eMsg = eMessageType.ERROR;
                    msg = "UserSendMailHandler.Failed2";
                    pkg.WriteBoolean(false);
                }
            }
            client.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(pkg);
            
            return 0;
        }
    }
}
