using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_BANCHAT_UPDATE, "禁言")]
    public class ConsortiaIsBanChatHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int banUserID = packet.ReadInt();
            bool isBanChat = packet.ReadBoolean();
            int userID = 0;
            string userName = "";
            bool result = false;
            string msg = "ConsortiaIsBanChatHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.UpdateConsortiaIsBanChat(banUserID, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, isBanChat, ref userID, ref userName, ref msg))
                {
                    msg = "ConsortiaIsBanChatHandler.Success";
                    result = true;
                    GameServer.Instance.LoginServer.SendConsortiaBanChat(userID, userName, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName,isBanChat);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
