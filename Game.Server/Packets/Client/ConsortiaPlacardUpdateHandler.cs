using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_PLACARD_UPDATE, "更新公告")]
    public class ConsortiaPlacardUpdateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string placard = packet.ReadString();
            if (System.Text.Encoding.Default.GetByteCount(placard) > 300)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long"));
                return 1;
            }
            bool result = false;
            string msg = "ConsortiaPlacardUpdateHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.UpdateConsortiaPlacard(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, placard, ref msg))
                {
                    msg = "ConsortiaPlacardUpdateHandler.Success";
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
