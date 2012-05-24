using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.HOTSPRING_ENTER,"礼堂数据")]
    public class HotSpringEnterHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //Lay Danh Sach tat ca cac phong
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.HOTSPRING_ROOM_LIST_GET);
            pkg.WriteInt(10);
            for (int i = 0; i < 10; i++)
            {
                //loc_2.roomNumber = _loc_3.readInt();
                pkg.WriteInt(i);
                //_loc_2.roomID = _loc_3.readInt();
                pkg.WriteInt(i);
                //_loc_2.roomName = _loc_3.readUTF();
                pkg.WriteString("Room" + i);
                //_loc_2.roomPassword = _loc_3.readUTF();
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
            }
            client.SendTCP(pkg);
            return 0;
        }

    }
}
