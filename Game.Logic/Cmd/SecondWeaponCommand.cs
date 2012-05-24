using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.SECONDWEAPON, "副武器")]


    public class SecondWeaponCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            player.UseSecondWeapon();
        }
    }
}
