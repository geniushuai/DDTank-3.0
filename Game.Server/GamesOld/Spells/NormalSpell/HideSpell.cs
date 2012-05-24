using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Server.Effects;
using Phy.Object;
using Game.Server.Games;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.HIDE)]
    public class HideSpell : ISpellHandler
    {
        private void AddHideEffect(Player player, int count)
        {
            HideEffect effect = (HideEffect)player.EffectList.GetOfType(typeof(HideEffect));
            if (effect == null)
            {
                new HideEffect(count).Start(player);
            }
            else
            {
                effect.Count = count;
            }
        }

        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            switch(item.Property2)
            {
                case 0:
                    if(player.IsLiving)
                    {
                        AddHideEffect(player,item.Property3);
                    }
                    break;
                case 1:
                    Player[] players = player.Game.GetAllFightPlayersSafe();
                    foreach(Player p in players)
                    {
                        if(p.IsLiving && p.Team == player.Team)
                        {
                            AddHideEffect(p, item.Property3);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
