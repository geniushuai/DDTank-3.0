using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_ROOM_OPEN, "开启房间位置")]
    public class GameUserOpenHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player == client.Player.CurrentGame.Player)
            {
                byte index = packet.ReadByte();
                client.Player.CurrentGame.OpenState[index] = true;
                client.Player.CurrentGame.SendToAll(packet);
                client.Player.CurrentGame.SendRoomInfo();
            }
            return 0;
        }
    }
}
