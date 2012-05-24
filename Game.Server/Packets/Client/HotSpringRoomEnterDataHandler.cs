using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.HOTSPRING_ROOM_ENTER,"礼堂数据")]
    public class HotSpringRoomEnterDataHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            var roomId = packet.ReadInt();
            var passString = packet.ReadString();

            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.HOTSPRING_ROOM_ENTER);
            pkg.WriteInt(roomId);
            pkg.WriteInt(roomId);
           
            //_loc_3.roomID = _loc_2.readInt();
            //_loc_3.roomNumber = _loc_2.readInt();
            //_loc_3.roomName = _loc_2.readUTF();
            pkg.WriteString("RoomName");
            //_loc_3.roomPassword = _loc_2.readUTF();
            pkg.WriteString("");

            //_loc_2.effectiveTime = _loc_3.readInt();
            pkg.WriteInt(1);
            //_loc_2.curCount = _loc_3.readInt();
            pkg.WriteInt(1);
            //_loc_2.playerID = _loc_3.readInt();
            pkg.WriteInt(client.Player.PlayerCharacter.ID);
            //_loc_2.playerName = _loc_3.readUTF();
            pkg.WriteString("abc");
            //_loc_2.startTime = _loc_3.readDate();
            pkg.WriteDateTime(DateTime.Now.AddDays(-50));
            //_loc_2.roomIntroduction = _loc_3.readUTF();
            pkg.WriteString("Room Intro");
            //_loc_2.roomType = _loc_3.readInt();
            pkg.WriteInt(1);
            //_loc_2.maxCount = _loc_3.readInt();
            pkg.WriteInt(10);
            //this._playerEnterRoomTime = _loc_2.readDate();
            //this._playerEffectiveTime = _loc_2.readInt();
            pkg.WriteDateTime(DateTime.Now);
            pkg.WriteDateTime(DateTime.Now.AddDays(1));
            client.SendTCP(pkg);
            return 0;
        }

    }
}
