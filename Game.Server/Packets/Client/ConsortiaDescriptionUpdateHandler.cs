using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_DESCRIPTION_UPDATE, "更新介绍")]
    public class ConsortiaDescriptionUpdateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string description = packet.ReadString();
            if (System.Text.Encoding.Default.GetByteCount(description) > 300)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDescriptionUpdateHandler.Long"));
                return 1;
            }
            bool result = false;
            string msg = "ConsortiaDescriptionUpdateHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.UpdateConsortiaDescription(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, description, ref msg))
                {
                    msg = "ConsortiaDescriptionUpdateHandler.Success";
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
