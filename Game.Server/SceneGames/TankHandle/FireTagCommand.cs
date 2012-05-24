using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using Game.Base.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.FIRE_TAG)]
    public class FireTagCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
                return false;
            //{
            bool tag = packet.ReadBoolean();
            if (player.CurrentGame.Data.CurrentFire == null && tag && player.CurrentGame.Data.CurrentIndex == player)
            {
                player.CurrentGame.Data.SpendTime(packet.ReadByte());
                player.CurrentGame.Data.CurrentFire = player;
            }

            if (!tag && player.CurrentGame.Data.CurrentIndex == player)
            {
                process.SendPlayFinish(player.CurrentGame, player);
            }

            player.CurrentGame.ReturnPacket(player, packet);
            //}
            return false;
        }
    }
}
