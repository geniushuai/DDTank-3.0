using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_INVITE_PASS, "通过邀请")]
    public class ConsortiaInvitePassHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
                return 0;

            int id = packet.ReadInt();
            bool result = false;
            int consortiaID = 0;
            string consortiaName = "";
            string msg = "ConsortiaInvitePassHandler.Failed";
            int tempID = 0;
            string tempName = "";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                int consortiaRepute = 0;
                ConsortiaUserInfo info = new ConsortiaUserInfo();
                if (db.PassConsortiaInviteUsers(id, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, ref consortiaID, ref consortiaName, ref msg, info, ref tempID, ref tempName, ref consortiaRepute))
                {
                    client.Player.PlayerCharacter.ConsortiaID = consortiaID;
                    client.Player.PlayerCharacter.ConsortiaName = consortiaName;
                    client.Player.PlayerCharacter.DutyLevel = info.Level;
                    client.Player.PlayerCharacter.DutyName = info.DutyName;
                    client.Player.PlayerCharacter.Right = info.Right;
                    ConsortiaInfo consotia = ConsortiaMgr.FindConsortiaInfo(consortiaID);
                    if (consotia != null)
                        client.Player.PlayerCharacter.ConsortiaLevel = consotia.Level;
                    msg = "ConsortiaInvitePassHandler.Success";
                    result = true;

                    info.UserID = client.Player.PlayerCharacter.ID;
                    info.UserName = client.Player.PlayerCharacter.NickName;
                    info.Grade = client.Player.PlayerCharacter.Grade;
                    info.Offer = client.Player.PlayerCharacter.Offer;
                    info.RichesOffer = client.Player.PlayerCharacter.RichesOffer;
                    info.RichesRob = client.Player.PlayerCharacter.RichesRob;
                    info.Win = client.Player.PlayerCharacter.Win;
                    info.Total = client.Player.PlayerCharacter.Total;
                    info.Escape = client.Player.PlayerCharacter.Escape;
                    info.ConsortiaID = consortiaID;
                    info.ConsortiaName = consortiaName;

                    GameServer.Instance.LoginServer.SendConsortiaUserPass(tempID, tempName, info, true, consortiaRepute, client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.FightPower);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteInt(consortiaID);
            packet.WriteString(consortiaName);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
