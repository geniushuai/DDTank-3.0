using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CONSORTIA_CHAIRMAN_CHAHGE, "转让会长")]
    public class ConsortiaChangeChairmanHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            //int id = packet.ReadInt();
            string nickName = packet.ReadString();
            bool result = false;
            string msg = "ConsortiaChangeChairmanHandler.Failed";

            if (string.IsNullOrEmpty(nickName))
            {
                msg = "ConsortiaChangeChairmanHandler.NoName";
            }
            else if( nickName == client.Player.PlayerCharacter.NickName)
            {
                msg = "ConsortiaChangeChairmanHandler.Self";
            }
            else
            {
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    string tempUserName = "";
                    int tempUserID = 0;
                    ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                    if (db.UpdateConsortiaChairman(nickName, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg, ref info,ref tempUserID, ref tempUserName))
                    {
                        ConsortiaDutyInfo orderInfo = new ConsortiaDutyInfo();
                        orderInfo.Level = client.Player.PlayerCharacter.DutyLevel;
                        orderInfo.DutyName = client.Player.PlayerCharacter.DutyName;
                        orderInfo.Right = client.Player.PlayerCharacter.Right;
                        msg = "ConsortiaChangeChairmanHandler.Success1";
                        result = true;
                        GameServer.Instance.LoginServer.SendConsortiaDuty(orderInfo, 9, client.Player.PlayerCharacter.ConsortiaID, tempUserID, tempUserName, 0, "");
                        GameServer.Instance.LoginServer.SendConsortiaDuty(info, 8, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, 0, "");
                    }
                }
            }
            string temp = LanguageMgr.GetTranslation(msg);
            if (msg == "ConsortiaChangeChairmanHandler.Success1")
            {
                temp += nickName + LanguageMgr.GetTranslation("ConsortiaChangeChairmanHandler.Success2");
            }

            packet.WriteBoolean(result);
            packet.WriteString(temp);
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
