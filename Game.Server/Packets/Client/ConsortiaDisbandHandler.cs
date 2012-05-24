using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_DELETE,"公会解散")]
    public class ConsortiaDisbandHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = client.Player.PlayerCharacter.ConsortiaID;
            string consortiaName = client.Player.PlayerCharacter.ConsortiaName;
            bool result = false;
            string msg = "ConsortiaDisbandHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.DeleteConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                {
                    result = true;
                    msg = "ConsortiaDisbandHandler.Success1";

                    client.Player.ClearConsortia();

                    GameServer.Instance.LoginServer.SendConsortiaDelete(id);
                }
            }

            string temp = LanguageMgr.GetTranslation(msg);
            if (msg == "ConsortiaDisbandHandler.Success1")
            {
                temp += consortiaName + LanguageMgr.GetTranslation("ConsortiaDisbandHandler.Success2");
            }

            packet.WriteBoolean(result);
            packet.WriteInt(client.Player.PlayerCharacter.ID);
            packet.WriteString(temp);
            client.Out.SendTCP(packet);
           
            return 0;
        }
    }
}
