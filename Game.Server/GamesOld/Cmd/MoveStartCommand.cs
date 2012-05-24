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

namespace Game.Server.Games.Cmd
{
    [GameCommand((byte)TankCmdType.MOVESTART, "开始移动")]
    public class MoveStartCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsLiving || player.IsAttacking)
            {
                GSPacketIn pkg = packet.Clone();
                pkg.ClientID = player.Id;
                game.SendToAll(pkg, player.PlayerDetail);

                byte type = packet.ReadByte();
                int tx = packet.ReadInt();
                int ty = packet.ReadInt();
                byte dir = packet.ReadByte();
                bool isLiving = packet.ReadBoolean();

                switch (type)
                {
                    case 0:
                    case 1:
                        //p.X = tx;
                        //p.Y = ty;
                        player.SetXY(tx, ty);
                        player.StartMoving();
                        if (player.Y - ty > 1 || player.IsLiving != isLiving)
                        {
                            //GameServer.log.Error(string.Format("修正掉落: 动作类型  {0}    原始Y:{1}     最新Y:{2}", type, ty, p.Y));
                            //把服务器修正的掉落指令和客户掉落指令加以区分。
                            pkg = player.PlayerDetail.Out.SendPlayerMove(player,3, player.X, player.Y, 0, player.IsLiving);
                            game.SendToAll(pkg, player.PlayerDetail);
                        }
                        break;
                    case 2:
                        if (player.IsLiving == false)
                        {
                            player.TargetPoint = new Point(tx, ty);
                        }
                        break;
                }
            }
        }
    }
}
