using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.Effects;
using Phy.Object;
using Game.Server.Games;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.NOHOLE)]
    public class NoHoleSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            NoHoleEffect effect = (NoHoleEffect)player.EffectList.GetOfType(typeof(NoHoleEffect));
            if (effect != null)
            {
                effect.Count = item.Property3;
            }
            else
            {
                new NoHoleEffect(item.Property3).Start(player);
            }
        }
    }
}
