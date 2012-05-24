using SqlDataProvider.Data;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.HIDE)]
    public class HideSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            switch(item.Property2)
            {
                case 0:
                    if(player.IsLiving)
                    {
                        new HideEffect(item.Property3).Start(player);
                    }
                    break;
                case 1:
                    List<Player> players = player.Game.GetAllFightPlayers();
                    foreach(Player p in players)
                    {
                        if(p.IsLiving && p.Team == player.Team)
                        {
                            new HideEffect(item.Property3).Start(p);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
