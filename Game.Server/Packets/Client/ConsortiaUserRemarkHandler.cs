using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_USER_REMARK_UPDATE, "修改成员备注")]
    public class ConsortiaUserRemarkHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            string remark = packet.ReadString();
            if (string.IsNullOrEmpty(remark) || System.Text.Encoding.Default.GetByteCount(remark) > 100)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaUserRemarkHandler.Long"));
                return 1;
            }
            bool result = false;
            string msg = "ConsortiaUserRemarkHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.UpdateConsortiaUserRemark(id, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, remark, ref msg))
                {
                    msg = "ConsortiaUserRemarkHandler.Success";
                    result = true;
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
