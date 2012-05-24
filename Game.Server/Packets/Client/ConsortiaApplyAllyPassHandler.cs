using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_ALLY_APPLY_UPDATE, "申请通过")]
    public class ConsortiaApplyAllyPassHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            bool result = false;
            int tempID = 0;
            int state = 0;
            string msg = "ConsortiaApplyAllyPassHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.PassConsortiaApplyAlly(id, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID,ref tempID,ref state, ref msg))
                {
                    msg = "ConsortiaApplyAllyPassHandler.Success";
                    result = true;
                    GameServer.Instance.LoginServer.SendConsortiaAlly(client.Player.PlayerCharacter.ConsortiaID, tempID, state);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
