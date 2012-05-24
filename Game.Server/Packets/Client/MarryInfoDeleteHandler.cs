using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;
using Game.Server.GameUtils;
using SqlDataProvider.Data;


namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRYINFO_DELETE, "撤消征婚信息")]
    public class MarryInfoDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            string msg = LanguageMgr.GetTranslation("MarryInfoDeleteHandler.Fail");
            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (db.DeleteMarryInfo(id, client.Player.PlayerCharacter.ID, ref msg))
                {
                    client.Out.SendAuctionRefresh(null, id, false,null);
                }

                client.Out.SendMessage(eMessageType.Normal, msg);
            }
            return 0;
        }
    }
}
