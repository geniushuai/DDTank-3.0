using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CONSORTIA_USER_GRADE_UPDATE,"用户等级更新")]
    public class ConsortiaUserGradeUpdateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            bool upGrade = packet.ReadBoolean();
            bool result = false;
            string msg = "ConsortiaUserGradeUpdateHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                string tempUserName = "";
                ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                if (db.UpdateConsortiaUserGrade(id, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, upGrade, ref msg, ref info, ref tempUserName))
                {
                    msg = "ConsortiaUserGradeUpdateHandler.Success";
                    result = true;
                    GameServer.Instance.LoginServer.SendConsortiaDuty(info, upGrade ? 6 : 7, client.Player.PlayerCharacter.ConsortiaID,id, tempUserName,client.Player.PlayerCharacter.ID,client.Player.PlayerCharacter.NickName);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
