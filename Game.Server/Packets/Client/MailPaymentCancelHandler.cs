using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MAIL_CANCEL, "取消付款邮件")]
    public class MailPaymentCancelHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //GSPacketIn pkg = packet.Clone();
            //pkg.ClearContext();

            int id = packet.ReadInt();
            int senderID = 0;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (db.CancelPaymentMail(client.Player.PlayerCharacter.ID, id, ref senderID))
                {
                    client.Out.SendMailResponse(senderID,eMailRespose.Receiver);
                    packet.WriteBoolean(true);
                }
                else
                {
                    packet.WriteBoolean(false);
                }
            }

            client.Out.SendTCP(packet);

            return 1;
        }
    }
}
