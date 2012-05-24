using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using SqlDataProvider.Data;
using Game.Base.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.CHANGEBALL)]
    public class ChangeBallHandler:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //TankData data = player.CurrentGame.Data;
            //if (data.AddMultiple < 1 && data.CurrentFire == player)
            //{
            //    int id = packet.ReadInt();
            //    FireBallInfo fireBall = data.FireBall[id];
            //    fireBall.SetBallInfo(packet.ReadInt(), packet.ReadInt(), packet.ReadInt(), packet.ReadInt());
            //}

            return false;
        }
    }
}
