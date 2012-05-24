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
    [CommandAttbute((byte)TankCmdType.BLAST)]
    public class BlastCommand : ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.CurrentFire == player && player.CurrentGame.Data.ReduceFireBombs)
            //{
            //    player.CurrentGame.ReturnPacket(player, packet);

            //    int id = packet.ReadInt();

            //    player.CurrentGame.Data.BlastID = id;
            //    player.CurrentGame.Data.BlastX = packet.ReadInt();
            //    player.CurrentGame.Data.BlastY = packet.ReadInt();
            //    FireBallInfo ballInfo = player.CurrentGame.Data.FireBall[id];
            //    ballInfo.EndTime = packet.ReadInt();

            //    if (player.CurrentGame.Data.BlastX > 0)
            //    {
            //        int space = ballInfo.GetSpace(player.CurrentGame.Data.BlastX, player.CurrentGame.Data.BlastY, ballInfo.EndTime);
            //        if (space > 20)
            //        {
            //            GameServer.log.Error("子弹爆炸错误,距离为:" + space.ToString());
            //            player.Client.Disconnect();
            //            return false;
            //        }

            //    }

            //    process.TankFire(player.CurrentGame, player, player.CurrentGame.Data.BlastX, player.CurrentGame.Data.BlastY, ballInfo);
            //}
            return true;
        }
    }
}
