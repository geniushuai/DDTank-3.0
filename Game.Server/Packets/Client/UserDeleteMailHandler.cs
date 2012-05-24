using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.DELETE_MAIL,"删除邮件")]
    public class UserDeleteMailHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            //pkg.ClearContext();
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
           
                return 0;
            }
            int id = packet.ReadInt();
            int senderID;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (db.DeleteMail(client.Player.PlayerCharacter.ID, id, out senderID))
                {
                    client.Out.SendMailResponse(senderID,eMailRespose.Receiver);
                    pkg.WriteBoolean(true);
                }
                else
                {
                    pkg.WriteBoolean(false);
                }
            }

            client.Out.SendTCP(pkg);

            return 0;
        }
    }
}
