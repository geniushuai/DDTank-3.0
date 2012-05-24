using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_ALLY_APPLY_DELETE, "删除申请")]
    public class ConsortiaApplyAllyDeleteHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.DeleteConsortiaApplyAlly(id, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref msg))
                {
                    msg = "ConsortiaApplyAllyDeleteHandler.Success";
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
