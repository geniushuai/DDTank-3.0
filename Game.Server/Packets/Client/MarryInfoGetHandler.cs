using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRYINFO_GET, "获取征婚信息")]
    class MarryInfoGetHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.MarryInfoID == 0)
            {
                return 1;
            }
            
            int id = packet.ReadInt();

            using (PlayerBussiness db = new PlayerBussiness())
            {
                MarryInfo info = db.GetMarryInfoSingle(id);
                if (info != null)
                {
                    client.Player.Out.SendMarryInfo(client.Player, info);
                    return 0;
                }
            }

            return 1;
        }

    }
}
