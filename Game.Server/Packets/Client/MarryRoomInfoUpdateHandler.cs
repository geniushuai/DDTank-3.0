using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.SceneMarryRooms;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_ROOM_INFO_UPDATE, "更新礼堂信息")]
    class MarryRoomInfoUpdateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentMarryRoom != null && client.Player.PlayerCharacter.ID == client.Player.CurrentMarryRoom.Info.PlayerID)
            {
                string roomName = packet.ReadString();

                bool isPwdChanged = packet.ReadBoolean();

                string pwd = packet.ReadString();
                
                string introduction = packet.ReadString();

                MarryRoom room = client.Player.CurrentMarryRoom;

                room.Info.RoomIntroduction = introduction;
                room.Info.Name = roomName;
                if (isPwdChanged)
                {
                    room.Info.Pwd = pwd;
                }
                

                using (PlayerBussiness db = new PlayerBussiness())
                {
                    db.UpdateMarryRoomInfo(room.Info);
                }

                room.SendMarryRoomInfoUpdateToScenePlayers(room);

                client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomInfoUpdateHandler.Successed"));
                return 0;
            }

            return 1;
        }
    }
}
