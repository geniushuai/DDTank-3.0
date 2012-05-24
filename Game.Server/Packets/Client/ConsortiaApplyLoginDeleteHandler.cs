using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_TRYIN_DEL,"删除进入申请")]
    public class ConsortiaApplyLoginDeleteHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {              
                if (db.DeleteConsortiaApplyUsers(id,client.Player.PlayerCharacter.ID,client.Player.PlayerCharacter.ConsortiaID,ref msg))
                {
                    msg = client.Player.PlayerCharacter.ID == 0 ? "ConsortiaApplyAllyDeleteHandler.Success" : "ConsortiaApplyAllyDeleteHandler.Success2";
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
