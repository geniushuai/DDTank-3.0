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
    [CommandAttbute((byte)TankCmdType.MOVESTOP)]
    public class MoveStopCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.CurrentIndex == player || player.CurrentGame.Data.Players[player].State == TankGameState.FRIST || player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
            if (player.CurrentGame.Data.CurrentIndex == player || player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
            {
                player.CurrentGame.ReturnPacket(player, packet);
                player.CurrentGame.Data.Players[player].SetXY(packet.ReadInt(), packet.ReadInt());
                //player.CurrentGame.Data.Players[player].StartMoving();

                //GameServer.log.Error(string.Format("StopMoving {0} {1}",player.CurrentGame.Data.Players[player].X,player.CurrentGame.Data.Players[player].Y));
                
                //由于同步问题，暂时去掉
                //if (player.CurrentGame != null)
                //    player.CurrentGame.Data.Players[player].StartMoving();
               
                // GameServer.log.Error(string.Format("StopMoving {0} {1}", player.CurrentGame.Data.Players[player].X, player.CurrentGame.Data.Players[player].Y));
                return true;
            }
            else
            {
                //if (player.CurrentGame.Data.CurrentFire != null)
                //{
                    //int x = packet.ReadInt();
                    //player.CurrentGame.Data.Players[player].PosY = packet.ReadInt();
                //}
                //return true;
            }
            return false;
        }
    }
}
