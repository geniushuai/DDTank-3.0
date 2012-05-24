using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.SceneGames;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.SHOOTSTRAIGHT)]
    public class ShootStraightSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            TankData data = player.CurrentGame.Data;
            data.AddMultiple = (1- (double)item.Template.Property2 / 100);

            GSPacketIn pkg = player.Out.SendShootStraight(player);
            player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
        }
    }
}
