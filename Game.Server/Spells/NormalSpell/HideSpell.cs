using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Server.SceneGames;
using Game.Server.Effects;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.HIDE)]
    public class HideSpell : ISpellHandler
    {
        public void Execute(GamePlayer player,ItemInfo item)
        {
            TankData data = player.CurrentGame.Data;
            if (item.Template.Property2 == 0)
            {
                if (player.CurrentGame.Data.Players[player].IsHide == 0)
                {
                    HideEffect hide = new HideEffect(item.Template.Property3);
                    hide.Start(player);
                }
                else
                {
                    player.CurrentGame.Data.Players[player].SetHide(item.Template.Property3);
                }
            }

            if (item.Template.Property2 == 1)
            {
                foreach (GamePlayer p in data.Players.Keys)
                {
                    if (player.CurrentTeamIndex == p.CurrentTeamIndex && data.Players[p].Blood > 0)
                    {
                        if (player.CurrentGame.Data.Players[p].IsHide == 0)
                        {
                            HideEffect hide = new HideEffect(item.Template.Property3);
                            hide.Start(p);
                        }
                        else
                        {
                            player.CurrentGame.Data.Players[p].SetHide(item.Template.Property3);
                        }
                    }


                }
            }
        }
    }
}
