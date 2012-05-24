using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_INVITE_DELETE, "删除公会邀请")]
    class ConsortiaInviteDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaInviteDeleteHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                if (db.DeleteConsortiaInviteUsers(id, client.Player.PlayerCharacter.ID))
                {
                    msg = "ConsortiaInviteDeleteHandler.Success";
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
