using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_INVITE, "公会邀请")]
    public class ConsortiaInviteAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            //int id = packet.ReadInt();
            string name = packet.ReadString();
            bool result = false;
            string msg = "ConsortiaInviteAddHandler.Failed";
            if(string.IsNullOrEmpty(name))
                return 0;
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaInviteUserInfo info = new ConsortiaInviteUserInfo();
                info.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                info.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
                info.InviteDate = DateTime.Now;
                info.InviteID = client.Player.PlayerCharacter.ID;
                info.InviteName = client.Player.PlayerCharacter.NickName;
                info.IsExist = true;
                info.Remark = "";
                info.UserID = 0;
                info.UserName = name;
                if (db.AddConsortiaInviteUsers(info, ref msg))
                {
                    msg = "ConsortiaInviteAddHandler.Success";
                    result = true;
                    GameServer.Instance.LoginServer.SendConsortiaInvite(info.ID, info.UserID, info.UserName, info.InviteID, info.InviteName, info.ConsortiaName,info.ConsortiaID);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
