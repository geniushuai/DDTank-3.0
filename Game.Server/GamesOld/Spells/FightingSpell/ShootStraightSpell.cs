using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Phy.Object;
using Game.Server.Games;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.SHOOTSTRAIGHT)]
    public class ShootStraightSpell:ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.ControlBall = true;
        }
    }
}
