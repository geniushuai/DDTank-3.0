using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Rooms;
using Game.Logic;
namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_PAIRUP_ROOM_SETUP, "游戏模式")]
    public class GameUserGameTypeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom != null)
            {
                int GameStyle = packet.ReadInt();
                switch (GameStyle)
                {
                    case 0:
                        client.Player.CurrentRoom.GameType = eGameType.Free;
                        break;
                    default:
                        client.Player.CurrentRoom.GameType = eGameType.Guild;
                        break;
                }
                GSPacketIn pkg = client.Player.Out.SendRoomType(client.Player, client.Player.CurrentRoom);
                client.Player.CurrentRoom.SendToAll(pkg, client.Player);
            }
            return 0;
        }
    }
}
