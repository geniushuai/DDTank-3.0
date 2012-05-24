using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Bussiness;
using Game.Server.Rooms;
using Game.Logic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.GAME_ROOM_CREATE, "游戏创建")]
    public class GameUserCreateHandler : IPacketHandler
    {
       
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte roomType = packet.ReadByte();
            byte timeType = packet.ReadByte();
            string room = packet.ReadString();
            string pwd = packet.ReadString();
        
            RoomMgr.CreateRoom(client.Player, room, pwd, (eRoomType)roomType, timeType);

            return 1;
        }
    }
}
