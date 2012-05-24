using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Rooms;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.PLAYER_STATE, "用户状态改变")]
    public class GameUserReadyHandle : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.MainWeapon == null)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                return 0;
            }
            if (client.Player.CurrentRoom != null)
            {
                RoomMgr.UpdatePlayerState(client.Player, packet.ReadByte());
            }

            return 0;
        }
    }
}
