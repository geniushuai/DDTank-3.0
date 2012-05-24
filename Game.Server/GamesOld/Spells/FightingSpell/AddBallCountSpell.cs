using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Phy.Object;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ADDBALL)]
    public class AddBallSpell:ISpellHandler
    {
        public void Execute(Game.Server.Games.BaseGame game, Player player, ItemTemplateInfo item)
        {
            if (!player.IsSpecialSkill)
            {
                player.BallCount = item.Property2;
            }
        }
    }
}
