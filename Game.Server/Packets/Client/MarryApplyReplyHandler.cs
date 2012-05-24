using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_APPLY_REPLY, "求婚答复")]
    class MarryApplyReplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool result = packet.ReadBoolean();
            int UserID = packet.ReadInt();
            int AnswerId = packet.ReadInt();   //答复对话框ID
            //先判断自已有没有结婚
            if (result && client.Player.PlayerCharacter.IsMarried)
            {
                client.Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg2"));                
            }

            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo tempSpouse = db.GetUserSingleByUserID(UserID);
                //发送好人卡
                if (!result)
                {
                    SendGoodManCard(tempSpouse.NickName, tempSpouse.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ID, db);
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);                    
                }

                //判断对方有没有结婚                
                if (tempSpouse == null || tempSpouse.Sex == client.Player.PlayerCharacter.Sex)
                {
                    return 1;
                }
                if (tempSpouse.IsMarried)
                {
                    client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg3"));
                }

                MarryApplyInfo info = new MarryApplyInfo();
                info.UserID = UserID;
                info.ApplyUserID = client.Player.PlayerCharacter.ID;
                info.ApplyUserName = client.Player.PlayerCharacter.NickName;
                info.ApplyType = 2;
                info.LoveProclamation = "";
                info.ApplyResult = result;
                int id = 0;
                if (db.SavePlayerMarryNotice(info, AnswerId, ref id))
                {
                    if (result)
                    {                        
                        client.Player.Out.SendMarryApplyReply(client.Player, tempSpouse.ID, tempSpouse.NickName, result, false, id);
                        client.Player.LoadMarryProp();
                        SendSYSMessages(client.Player, tempSpouse);
                    }
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
                    return 0;
                }
            }
            return 1;
        }

        public void SendSYSMessages(GamePlayer player, PlayerInfo spouse)
        {
            string name1 = player.PlayerCharacter.Sex ? player.PlayerCharacter.NickName : spouse.NickName;
            string name2 = player.PlayerCharacter.Sex ? spouse.NickName : player.PlayerCharacter.NickName;

            string msg = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg1", name1, name2);

            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
            pkg.WriteInt(2);
            pkg.WriteString(msg);

            GameServer.Instance.LoginServer.SendPacket(pkg);

            GamePlayer[] players = Game.Server.Managers.WorldMgr.GetAllPlayers();

            foreach (GamePlayer p in players)
            {
                p.Out.SendTCP(pkg);
            }
        }

        public void SendGoodManCard(string receiverName, int receiverID, string senderName, int senderID, PlayerBussiness db)
        {
            ItemTemplateInfo goodMan = ItemMgr.FindItemTemplate(11105);
            ItemInfo goodManCard = ItemInfo.CreateFromTemplate(goodMan, 1, (int)ItemAddType.webing);
            goodManCard.IsBinds = true;

            goodManCard.UserID = 0;
            db.AddGoods(goodManCard);

            MailInfo mail = new MailInfo();
            mail.Annex1 = goodManCard.ItemID.ToString();
            mail.Content = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Content");
            mail.Gold = 0;
            mail.IsExist = true;
            mail.Money = 0;
            mail.Receiver = receiverName;
            mail.ReceiverID = receiverID;
            mail.Sender = senderName;
            mail.SenderID = senderID;
            mail.Title = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Title");
            mail.Type = (int)eMailType.Marry;
            db.SendMail(mail);
            //Spouse.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
        }
    }
}
