using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_APPLY_STATE, "公会申请状态")]
    public class ConsotiaApplyStateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 1;

            bool state = packet.ReadBoolean();
            bool result = false;
            string msg = "CONSORTIA_APPLY_STATE.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.UpdateConsotiaApplyState(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, state,ref msg))
                {
                    msg = "CONSORTIA_APPLY_STATE.Success";
                    result =true;
                }
            }

            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
