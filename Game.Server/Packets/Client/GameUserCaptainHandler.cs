using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.SceneGames;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.GAME_CAPTAIN_CHOICE,"选择队长")]
    public class GameUserCaptainHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            bool result = packet.ReadBoolean();
            if (client.Player.CurrentGame != null && client.Player.CurrentGame.GameState == eGameState.LOAD && client.Player.CurrentGame.GameMode == eGameMode.FLAG)
            {
                packet.ClientID = (client.Player.PlayerCharacter.ID);
                if (result)
                {
                    if (!client.Player.CurrentGame.Data.FlagPlayer.Contains(client.Player))
                        client.Player.CurrentGame.Data.FlagPlayer.Add(client.Player);
                }
                else
                {
                    if (client.Player.CurrentGame.Data.FlagPlayer.Contains(client.Player))
                        client.Player.CurrentGame.Data.FlagPlayer.Remove(client.Player);
                }

                client.Player.CurrentGame.SendToAll(packet);

            }
            return 0;
        }

    }
}
