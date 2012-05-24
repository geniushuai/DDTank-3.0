using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using Game.Base.Packets;
using Game.Server.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.KILLSELF)]
    public class KillSelfCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.Players[player].State == TankGameState.DEAD || player.CurrentGame.GameState != eGameState.PLAY)
            //    return false;

            //GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Client.Player.PlayerCharacter.ID);
            //pkg.WriteByte((byte)TankCmdType.SUICIDE);
            //pkg.WriteInt(player.Client.Player.PlayerCharacter.ID);
            //player.CurrentGame.SendToAll(pkg);

            //player.CurrentGame.Data.Players[player].Dead();


            return true;
        }
    }
}
