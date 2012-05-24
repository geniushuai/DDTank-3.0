using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.Statics;
using Bussiness.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_APPLY, "求婚")]
    class MarryApplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //判断是否结婚
            if(client.Player.PlayerCharacter.IsMarried)  
            {
                return 1;
            }
            //是否有二级密码
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 1;
            }
            //开始求婚，购买戒指
            int SpouseID = packet.ReadInt();
            string loveProclamation = packet.ReadString();
            bool Broadcast = packet.ReadBoolean();
            bool result = false;
            bool removeRing = true;
            string SpouseName = "";
            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo tempSpouse = db.GetUserSingleByUserID(SpouseID);
                if (tempSpouse == null || tempSpouse.Sex == client.Player.PlayerCharacter.Sex)
                {
                    return 1;
                }
                if (tempSpouse.IsMarried)
                {
                    client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg2"));
                    return 1;
                }
                ItemInfo WeddingRing = client.Player.PropBag.GetItemByTemplateID(0,11103);
                if (WeddingRing == null)
                {                    
                    ShopItemInfo tempRing = ShopMgr.FindShopbyTemplatID(11103).FirstOrDefault();
                    if (tempRing != null)
                    {
                        //玩家身上钱是否足够。
                        if (client.Player.PlayerCharacter.Money >= tempRing.AValue1)
                        {
                            removeRing = false;
                        }
                        else
                        {
                            client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1"));
                            return 1;
                        }
                    }
                    else
                    {
                        client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg6"));
                        return 1;
                    }
                }
                //插入结婚消息
                MarryApplyInfo info = new MarryApplyInfo();
                info.UserID = SpouseID;
                info.ApplyUserID = client.Player.PlayerCharacter.ID;
                info.ApplyUserName = client.Player.PlayerCharacter.NickName;
                info.ApplyType = 1;
                info.LoveProclamation = loveProclamation;
                info.ApplyResult = false;
                int id=0;
                if (db.SavePlayerMarryNotice(info,0,ref id))
                {
                    if (removeRing)
                    {
                        client.Player.RemoveItem(WeddingRing);
                    }
                    else
                    {
                        //未开始                        
                        ShopItemInfo tempRing = ShopMgr.FindShopbyTemplatID(11103).FirstOrDefault();
                        client.Player.RemoveMoney(tempRing.AValue1);
                        LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Spark, client.Player.PlayerCharacter.ID, tempRing.AValue1, client.Player.PlayerCharacter.Money, 0, 0, 0, "", tempRing.TemplateID.ToString(), "1");
                    }
                    client.Player.Out.SendPlayerMarryApply(client.Player, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, loveProclamation,id);  //发送求婚信息
                    //发送消息给中心服务器
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(SpouseID);
                    SpouseName = tempSpouse.NickName;
                    result = true;

                    client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg3"));
                }

            }
            if (result && Broadcast && SpouseName != "")
            {
                string msg = LanguageMgr.GetTranslation("MarryApplyHandler.Msg5", client.Player.PlayerCharacter.NickName, SpouseName);
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
                pkg.WriteInt(2);
                pkg.WriteString(msg);
                GameServer.Instance.LoginServer.SendPacket(pkg);
                GamePlayer[] players = WorldMgr.GetAllPlayers();
                foreach (GamePlayer p in players)
                {
                    p.Out.SendTCP(pkg);
                }
            }           
            return 0;
        }
    }
}
