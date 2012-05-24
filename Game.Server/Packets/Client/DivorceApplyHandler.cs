using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Statics;
using Game.Server.GameObjects;
using Bussiness;
using Game.Server.Managers;
using SqlDataProvider.Data;
using Game.Server.Enumerate;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.DIVORCE_APPLY, "离婚")]
    class DivorceApplyHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (!client.Player.PlayerCharacter.IsMarried)
            {
                return 1;
            }

            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }

            if(client.Player.PlayerCharacter.IsCreatedMarryRoom)
            {
                client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg2"));
                return 1;
            }

            int needMoney = GameProperties.PRICE_DIVORCED; 
            if (client.Player.PlayerCharacter.Money < needMoney)
            {
                client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg1"));
                return 1;
            }
            else
            {
                client.Player.RemoveMoney(needMoney);
                LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Unmarry, client.Player.PlayerCharacter.ID, needMoney, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, needMoney, 0, (int)eConsumerType.Marry, (int)eSubConsumerType.Marry_Divorce);

                using (PlayerBussiness db = new PlayerBussiness())
                {
                    PlayerInfo tempSpouse = db.GetUserSingleByUserID(client.Player.PlayerCharacter.SpouseID);
                    if (tempSpouse == null || tempSpouse.Sex == client.Player.PlayerCharacter.Sex)
                    {
                        return 1;
                    }

                    MarryApplyInfo info = new MarryApplyInfo();
                    info.UserID = client.Player.PlayerCharacter.SpouseID;
                    info.ApplyUserID = client.Player.PlayerCharacter.ID;
                    info.ApplyUserName = client.Player.PlayerCharacter.NickName;
                    info.ApplyType = 3;
                    info.LoveProclamation = "";
                    info.ApplyResult = false;
                    int id = 0;
                    if (db.SavePlayerMarryNotice(info,0, ref id))
                    {
                        GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
                        client.Player.LoadMarryProp();
                    }
                }

                client.Player.QuestInventory.ClearMarryQuest(); //离婚后清除结婚后任务.
                client.Player.Out.SendPlayerDivorceApply(client.Player, true,true);
            }
            
            return 0;
        }

    }
}
