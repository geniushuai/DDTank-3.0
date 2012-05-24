using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_ROOM_KICK,"踢出房间")]
    public class GameUserKickHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
            {
                RoomMgr.KickPlayer(client.Player.CurrentRoom, packet.ReadByte());
            }
            return 0;
        }
    }
}
