using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_TRYIN_PASS, "申请进入通过")]
    public class ConsortiaApplyLoginPassHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaApplyLoginPassHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                int consortiaRepute = 0;
                ConsortiaUserInfo info = new ConsortiaUserInfo();
                if (db.PassConsortiaApplyUsers(id, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ConsortiaID, ref msg, info, ref consortiaRepute))
                {
                    msg = "ConsortiaApplyLoginPassHandler.Success";
                    result = true;
                    if (info.UserID != 0)
                    {
                        info.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                        info.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
                        GameServer.Instance.LoginServer.SendConsortiaUserPass(client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, info, false, consortiaRepute, info.LoginName, client.Player.PlayerCharacter.FightPower);
                    }
                }
            }

            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
