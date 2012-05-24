using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.FRIEND_REMOVE, "删除好友")]
    public class FriendRemoveHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (db.DeleteFriends(client.Player.PlayerCharacter.ID, id))
                {
                    client.Player.FriendsRemove(id);
                    client.Out.SendFriendRemove(id);
                }
            }

            return 0;
        }
    }
}
