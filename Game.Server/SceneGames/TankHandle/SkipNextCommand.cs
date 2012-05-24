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
    [CommandAttbute((byte)TankCmdType.SKIPNEXT)]
    public class SkipNextCommandP:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentGame.Data.CurrentIndex == player && player.CurrentGame.Data.CurrentFire == null)
            {
                player.CurrentGame.Data.SpendTime(packet.ReadByte());

                player.CurrentGame.Data.TotalDelay += 100;
                player.CurrentGame.Data.Players[player].SetDander(40);

                player.CurrentGame.ReturnPacket(player, packet);
                process.SendPlayFinish(player.CurrentGame, player);
                return true;
            }
            return false;
        }
    }
}
