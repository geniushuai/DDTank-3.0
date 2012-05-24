using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using Game.Logic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_ROOM_SETUP_CHANGE, "房间设置")]
    public class GameUserSetUpHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host && !client.Player.CurrentRoom.IsPlaying)
            {
                //string name = packet.ReadString();
                //string pwd = packet.ReadString();
               
                int mapId = packet.ReadInt();
                if (mapId == 10000) return 0;
                byte rT = packet.ReadByte();
                eRoomType roomType;
                if (rT == 10) roomType = eRoomType.Exploration;
                else 
                 roomType = (eRoomType)rT;
                byte timeType = packet.ReadByte();
                byte hardLevel = packet.ReadByte();
                int levelLimits = packet.ReadInt();
                //eRoomType roomType = (eRoomType)packet.ReadByte();
                //eTeamType teamType = (eTeamType)packet.ReadByte();
               
                RoomMgr.UpdateRoomGameType(client.Player.CurrentRoom, packet, roomType, timeType, (eHardLevel)hardLevel, levelLimits, mapId);
                //RoomMgr.StartGame(client.Player.CurrentRoom);
            }
            return 0;
        }
    }
}
