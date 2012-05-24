using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ACTIVE_PULLDOWN,"领取奖品")]
    public class ActivePullDownHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int activeID = packet.ReadInt();
            string awardID = packet.ReadString();
            string msg = "ActivePullDownHandler.Fail";
            using (ActiveBussiness db = new ActiveBussiness())
            {
                int result = db.PullDown(activeID, awardID, client.Player.PlayerCharacter.ID,ref msg);
                if (result == 0)
                {
                    client.Out.SendMailResponse(client.Player.PlayerCharacter.ID,eMailRespose.Receiver);
                }
                if (msg != "ActiveBussiness.Msg0")
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(msg));
                }
                else
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg));

                }
            }

            return 0;
        }
    }
}
