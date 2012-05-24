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
    [PacketHandler((byte)ePackageType.ITEM_OPENUP, "打开物品")]
    public class OpenUpArkHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int bagType = packet.ReadByte();
            int place = packet.ReadInt();

            PlayerInventory arkBag = client.Player.GetInventory((eBageType)bagType);

            ItemInfo goods = arkBag.GetItemAt(place);
            string full = "";

            List<ItemInfo> infos = new List<ItemInfo>();
            if (goods != null && goods.IsValidItem() && goods.Template.CategoryID == 11 && goods.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= goods.Template.NeedLevel)
            {
                int money = 0;
                int gold = 0;
                int giftToken = 0;
                int[] bags = new int[3];
                OpenUpItem(goods.Template.Data, bags, infos, ref gold, ref money,ref giftToken);
                bags[goods.GetBagType()]--;

                //if (arkBag.RemoveItem(goods) != -1)
                if (client.Player.RemoveItem(goods))
                {
                    //格式如：你获得了 数量 点券，数量 金币，武器/装备/美容，数量 道具
                     StringBuilder notice = new StringBuilder();
                    int index = 0;
                    StringBuilder msg = new StringBuilder();
                    msg.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start"));
                    
                    if (money != 0)
                    {
                        msg.Append(money + LanguageMgr.GetTranslation("OpenUpArkHandler.Money"));
                        client.Player.AddMoney(money);
                        LogMgr.LogMoneyAdd(LogMoneyType.Box, LogMoneyType.Box_Open, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "", "", "");//添加日志
                    }
                    if (gold != 0)
                    {
                        msg.Append(gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold"));
                        client.Player.AddGold(gold);
                    }
                    if (giftToken != 0)
                    {
                        msg.Append(giftToken + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken"));
                        client.Player.AddGiftToken(giftToken);
                    }

                    StringBuilder msga = new StringBuilder();
                    foreach (ItemInfo info in infos)
                    {
                        //IBag bag = client.Player.GetBag(info.GetBagType());
                        //bag.AddItem(info);
                        //msg.Append((info.GetBagType() == 0 ? "" : info.Count.ToString()) + " " + info.Template.Name + ",");

                        msga.Append(info.Template.Name + "x" + info.Count.ToString() + ",");

                      
                       if ((info.Template.Quality >= goods.Template.Property2)&(goods.Template.Property2!=0))
                       {
                            notice.Append( info.Template.Name+",");
                            index++;
                         
                        }
                        //msg.Append(info.Template.Name + "x" + info.Count.ToString() + ",");
                        if(!client.Player.AddTemplate(info,info.Template.BagType,info.Count))                       
                        {
                            using (PlayerBussiness db = new PlayerBussiness())
                            {
                                info.UserID = 0;
                                db.AddGoods(info);

                                MailInfo message = new MailInfo();
                                message.Annex1 = info.ItemID.ToString();
                                message.Content = LanguageMgr.GetTranslation("OpenUpArkHandler.Content1") + info.Template.Name + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2");
                                message.Gold = 0;
                                message.Money = 0;
                                message.Receiver = client.Player.PlayerCharacter.NickName;
                                message.ReceiverID = client.Player.PlayerCharacter.ID;
                                message.Sender = message.Receiver;
                                message.SenderID = message.ReceiverID;
                                message.Title = LanguageMgr.GetTranslation("OpenUpArkHandler.Title") + info.Template.Name + "]";
                                message.Type = (int)eMailType.OpenUpArk;
                                db.SendMail(message);

                                full = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail");
                            }
                        }
                    }
                    if (msga.Length > 0)
                    {
                        msga.Remove(msga.Length - 1, 1);
                        string[] msgstr = msga.ToString().Split(',');
                        for (int i = 0; i < msgstr.Length; i++)
                        {
                            int counts = 1;  //统计重复数量

                            //先统计重复数量
                            for (int j = i + 1; j < msgstr.Length; j++)
                            {
                                //重复，则更变字符串;
                                if (msgstr[i].Contains(msgstr[j]) && msgstr[j].Length == msgstr[i].Length)
                                {
                                    counts++;
                                    msgstr[j] = j.ToString();
                                }
                            }

                            //重复数字入串
                            if (counts > 1)
                            {
                                msgstr[i] = msgstr[i].Remove(msgstr[i].Length - 1, 1);
                                msgstr[i] = msgstr[i] + counts.ToString();
                            }

                            //判断是否留字符串
                            if (msgstr[i] != i.ToString())
                            {
                                msgstr[i] = msgstr[i] + ",";
                                msg.Append(msgstr[i]);
                            }

                        }
                    }                   
                    if (goods.Template.Property2 != 0&index>0)
                    {
                        string msg1 = LanguageMgr.GetTranslation("OpenUpArkHandler.Notice",client.Player.PlayerCharacter.NickName ,goods.Template.Name,notice ,notice.Remove(notice.Length-1,1));

                        //client.Out.SendSystemNotice(msg);

                        GSPacketIn pkg1 = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
                        pkg1.WriteInt(2);
                        pkg1.WriteString(msg1);

                        GameServer.Instance.LoginServer.SendPacket(pkg1);

                        GamePlayer[] players = Game.Server.Managers.WorldMgr.GetAllPlayers();

                        foreach (GamePlayer p in players)
                        {
                            if( p != client.Player )
                                p.Out.SendTCP(pkg1);
                        }
                    }
                    msg.Remove(msg.Length - 1, 1);
                    msg.Append(".");                 
                    client.Out.SendMessage(eMessageType.Normal, full + msg.ToString());

                    if (!string.IsNullOrEmpty(full))
                    {
                        //GameServer.Instance.LoginServer.SendMailResponse(client.Player.PlayerCharacter.ID);
                        client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }
                }
            }
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
