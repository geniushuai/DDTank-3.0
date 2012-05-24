using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_STATUS, "请求结婚状态")]
    class MarryStatusHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int UserID = packet.ReadInt();

            GamePlayer Spouse = WorldMgr.GetPlayerById(UserID);

            if (Spouse != null)
            {
                client.Player.Out.SendPlayerMarryStatus(client.Player, Spouse.PlayerCharacter.ID, Spouse.PlayerCharacter.IsMarried);
            }
            else
            { 
                using (PlayerBussiness db = new PlayerBussiness())
                {                                                                                   
                    PlayerInfo tempSpouse = db.GetUserSingleByUserID(UserID);
                    client.Player.Out.SendPlayerMarryStatus(client.Player, tempSpouse.ID, tempSpouse.IsMarried);
                }

            }

            return 0;
        }
    }
}
