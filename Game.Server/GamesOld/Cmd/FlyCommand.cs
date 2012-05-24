using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Bussiness;
using Game.Server.Statics;
using Phy.Object;

namespace Game.Server.Games.Cmd
{
    [GameCommand((byte)TankCmdType.FLY,"使用飞行技能")]
    public class FlyCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if(player.IsAttacking)
            {
                player.UseFlySkill();
            }
        }
    }
}
