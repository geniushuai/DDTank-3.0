using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Statics;
using Game.Logic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GET_MAIL_ATTACHMENT, "获取邮件到背包")]
    public class MailGetAttachHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            byte type = packet.ReadByte();
            List<int> types = new List<int>();
            string msg = "";// LanguageMgr.GetTranslation("MailGetAttachHandler.Falied");
            eMessageType eMsg = eMessageType.Normal;
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }

            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            using (PlayerBussiness db = new PlayerBussiness())
            {
                MailInfo mes = db.GetMailSingle(client.Player.PlayerCharacter.ID, id);
                if (mes != null)
                {
                    bool result = true;
                    int oldMoney = mes.Money;
                    GamePlayer player = Managers.WorldMgr.GetPlayerById(mes.ReceiverID);

                    if (mes.Type > 100 && mes.Money > client.Player.PlayerCharacter.Money)
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MailGetAttachHandler.NoMoney"));
                        return 0;
                    }

                    if (!mes.IsRead)
                    {
                        mes.IsRead = true;
                        mes.ValidDate = 3 * 24;
                        mes.SendTime = DateTime.Now;
                    }
                    if (result && (type == 0 || type == 1) && !string.IsNullOrEmpty(mes.Annex1))
                    {
                        if (GetAnnex(mes.Annex1, client.Player, ref msg, ref result, ref eMsg))
                        {
                            types.Add(1);
                            mes.Annex1 = null;
                        }
                    }

                    if (result && (type == 0 || type == 2) && !string.IsNullOrEmpty(mes.Annex2))
                    {
                        if (GetAnnex(mes.Annex2, client.Player, ref msg, ref result, ref eMsg))
                        {
                            types.Add(2);
                            mes.Annex2 = null;
                        }
                    }

                    if (result && (type == 0 || type == 3) && !string.IsNullOrEmpty(mes.Annex3))
                    {
                        if (GetAnnex(mes.Annex3, client.Player, ref msg, ref result, ref eMsg))
                        {
                            types.Add(3);
                            mes.Annex3 = null;
                        }
                    }

                    if (result && (type == 0 || type == 4) && !string.IsNullOrEmpty(mes.Annex4))
                    {
                        if (GetAnnex(mes.Annex4, client.Player, ref msg, ref result, ref eMsg))
                        {
                            types.Add(4);
                            mes.Annex4 = null;
                        }
                    }

                    if (result && (type == 0 || type == 5) && !string.IsNullOrEmpty(mes.Annex5))
                    {
                        if (GetAnnex(mes.Annex5, client.Player, ref msg, ref result, ref eMsg))
                        {
                            types.Add(5);
                            mes.Annex5 = null;
                        }
                    }

                    if ((type == 0 || type == 6) && mes.Gold > 0)
                    {
                        types.Add(6);
                        player.AddGold(mes.Gold);
                        mes.Gold = 0;
                    }

                    if ((type == 0 || type == 7) && mes.Type < 100 && mes.Money > 0)
                    {
                        types.Add(7);
                        player.AddMoney(mes.Money);
                        LogMgr.LogMoneyAdd(LogMoneyType.Mail, LogMoneyType.Mail_Money, player.PlayerCharacter.ID, mes.Money, player.PlayerCharacter.Money, 0, 0, 0, "", "", "");//添加日志
                        mes.Money = 0;
                    }

                    if (mes.Type > 100 && mes.Money > 0)
                    {
                        mes.Money = 0;
                        msg = LanguageMgr.GetTranslation("MailGetAttachHandler.Deduct") + (string.IsNullOrEmpty(msg) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success") : msg);
                    }

                    if (db.UpdateMail(mes, oldMoney))
                    {
                        if (mes.Type > 100 && oldMoney > 0)
                        {
                            player.RemoveMoney(oldMoney);
                            LogMgr.LogMoneyAdd(LogMoneyType.Mail, LogMoneyType.Mail_Pay, client.Player.PlayerCharacter.ID, oldMoney, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                            client.Out.SendMailResponse(mes.SenderID, eMailRespose.Receiver);
                            client.Out.SendMailResponse(mes.ReceiverID, eMailRespose.Send);
                        }
                    }

                    //pkg.WriteBoolean(result);
                    pkg.WriteInt(id);
                    pkg.WriteInt(types.Count);
                    foreach (int i in types)
                    {
                        pkg.WriteInt(i);
                    }

                    client.Out.SendTCP(pkg);

                    client.Out.SendMessage(eMsg, string.IsNullOrEmpty(msg) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success") : msg);
                }
                else
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MailGetAttachHandler.Falied"));
                }
            }

            return 0;
        }

        public bool GetAnnex(string value, GamePlayer player, ref string msg, ref bool result, ref eMessageType eMsg)
        {
            int gid = int.Parse(value);
            using (PlayerBussiness db = new PlayerBussiness())
            {
                ItemInfo goods = db.GetUserItemSingle(gid);
                if (goods != null)
                {
                    if (player.AddItem(goods))
                    {
                        eMsg = eMessageType.Normal;
                        return true;
                    }
                    else
                    {
                        eMsg = eMessageType.ERROR;
                        result = false;
                        msg = LanguageMgr.GetTranslation(goods.GetBagName()) + LanguageMgr.GetTranslation("MailGetAttachHandler.NoPlace");
                    }
                }
            }
            return false;
        }

    }
}
