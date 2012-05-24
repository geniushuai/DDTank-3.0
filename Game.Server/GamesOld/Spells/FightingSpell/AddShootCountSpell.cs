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
    [SpellAttibute((int)eSpellType.ADDATTACK)]
    public class AddAttackSpell:ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.ShootCount += item.Property2;

            if (item.Property2 == 2)
                player.CurrentShootMinus *= 0.6f;
            else
                player.CurrentShootMinus *= 0.9f;
        }
    }
}
