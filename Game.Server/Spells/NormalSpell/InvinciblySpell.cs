using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Server.SceneGames;
using Game.Server.Effects;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.INVINCIBLY)]
    public class InvinciblySpell : ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            TankData data = player.CurrentGame.Data;
            if (item.Template.Property2 == 0)
            {
                InvinciblyEffect hide = new InvinciblyEffect(item.Template.Property3);
                hide.Start(player);
            }

            if (item.Template.Property2 == 1)
            {
                foreach (GamePlayer p in data.Players.Keys)
                {
                    if (player.CurrentTeamIndex == p.CurrentTeamIndex)
                    {
                        InvinciblyEffect hide = new InvinciblyEffect(item.Template.Property3);
                        hide.Start(p);
                    }
                }
            }
        }
    }
}
