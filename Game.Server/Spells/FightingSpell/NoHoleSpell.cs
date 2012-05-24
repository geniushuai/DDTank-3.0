using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Server.SceneGames;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.Effects;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.NOHOLE)]
    public class NoHoleSpell : ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            TankData data = player.CurrentGame.Data;

            if (player.CurrentGame.Data.Players[player].IsNoHole == 0)
            {
                NoHoleEffect noHole = new NoHoleEffect(item.Template.Property3);
                noHole.Start(player);
            }
            else
            {
                player.CurrentGame.Data.Players[player].SetNoHole(item.Template.Property3);
            }


        }
    }
}
