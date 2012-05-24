using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_DUTY_DELETE, "删除职务")]
    public class ConsortiaDutyDeleteHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaDutyDeleteHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.DeleteConsortiaDuty(id, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref msg))
                {
                    msg = "ConsortiaDutyDeleteHandler.Success";
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
