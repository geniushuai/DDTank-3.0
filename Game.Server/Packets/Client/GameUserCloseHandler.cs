using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneGames;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_ROOM_CLOSE,"关闭房间位置")]
    public class GameUserCloseHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentGame != null && client.Player == client.Player.CurrentGame.Player)
            {

                    byte index = packet.ReadByte();
                    client.Player.CurrentGame.OpenState[index] = false;
                    client.Player.CurrentGame.KickPlayerIndex(client.Player, index);
                    client.Player.CurrentGame.SendToAll(packet);
                
            }
            return 0;
        }
    }
}
