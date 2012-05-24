using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.GAME_PLAYER_EXIT, "用户退出")]
    public class GameUserLeaveHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null)
            {
                RoomMgr.ExitRoom(client.Player.CurrentRoom, client.Player);
            }

            return 0;
        }
    }
}
