using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_USERS_DELETE, "删除公会成员")]
    public class ConsortiaUserDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            bool result = false;
            string nickName = "";
            string msg = id == client.Player.PlayerCharacter.ID ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.DeleteConsortiaUser(client.Player.PlayerCharacter.ID, id, client.Player.PlayerCharacter.ConsortiaID, ref msg, ref nickName))
                {
                    msg = id == client.Player.PlayerCharacter.ID ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess";
                    int consortiaID = client.Player.PlayerCharacter.ConsortiaID;
                    if (id == client.Player.PlayerCharacter.ID)
                    {
                        client.Player.ClearConsortia();
                        client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
                    }

                   // client.Player.StoreBag.SendStoreToMail();
                    GameServer.Instance.LoginServer.SendConsortiaUserDelete(id, consortiaID, id != client.Player.PlayerCharacter.ID, nickName,client.Player.PlayerCharacter.NickName);
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
