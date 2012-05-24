using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.MOVE)]
    public class MoveCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            byte type = packet.ReadByte();
            int x = packet.ReadInt();
            int y = packet.ReadInt();
            player.CurrentGame.Data.Players[player].SetXY(packet.ReadInt(), packet.ReadInt());

            if (player.CurrentGame.Data.CurrentIndex == player || player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
            {
                player.CurrentGame.ReturnPacket(player, packet);
                return true;
            }
            return false;
        }
    }
}
