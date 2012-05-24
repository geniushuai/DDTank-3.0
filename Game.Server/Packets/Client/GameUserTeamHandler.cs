using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Rooms;
using Game.Logic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_TEAM,"用户选队")]
    public class GameUserTeamHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom == null || client.Player.CurrentRoom.RoomType == eRoomType.Match)
                return 0;

            RoomMgr.SwitchTeam(client.Player);
            return 0;
        }
    }
}
