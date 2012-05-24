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
    [CommandAttbute((byte)TankCmdType.PLAYFINISH)]
    public class PlayFinishCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //int turnNum = packet.ReadInt();
            //if (turnNum != player.CurrentGame.Data.TurnNum)
            //    return false;

            //player.CurrentGame.Data.Players[player].TurnNum = turnNum;
            //int Count = player.CurrentGame.Data.GetPlayFinishCount();
            //if (Count == player.CurrentGame.Data.Count)
            //{
            //    process.SendNextFire(player.CurrentGame, player.CurrentGame.Data.CurrentIndex);
            //}

            return true;
        }
    }
}
