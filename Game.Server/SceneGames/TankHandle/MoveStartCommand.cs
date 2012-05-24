using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;
using Phy.Object;
using System.Drawing;
using Game.Server.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.MOVESTART)]
    public class MoveStartCommand : ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentGame.Data.CurrentIndex == player || player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
            {
                GSPacketIn pkg = packet.Clone();
                pkg.ClientID = player.PlayerCharacter.ID;
                player.CurrentGame.SendToPlayerExceptSelf(pkg, player);

                byte type = packet.ReadByte();
                int tx = packet.ReadInt();
                int ty = packet.ReadInt();
                byte dir = packet.ReadByte();
                bool isLiving = packet.ReadBoolean();
                Player p = player.CurrentGame.Data.Players[player];

                switch (type)
                {
                    case 0:
                    case 1:
                        //p.X = tx;
                        //p.Y = ty;
                        p.SetXY(tx, ty);
                        if (player.CurrentGame != null)
                        {
                            p.StartMoving();
                            if (p.Y - ty > 1 || p.IsLiving != isLiving)
                            {
                                //GameServer.log.Error(string.Format("修正掉落: 动作类型  {0}    原始Y:{1}     最新Y:{2}", type, ty, p.Y));
                                //把服务器修正的掉落指令和客户掉落指令加以区分。
                                pkg = player.Out.SendPlayerMove(3, p.X, p.Y, 0, p.IsLiving);
                                player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
                            }
                        }
                        break;
                    case 2:
                        if (player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
                        {
                            p.EndX = tx;
                            p.EndY = ty;
                        }
                        break;
                }
                return true;
            }
            return false;
        }
    }
}
