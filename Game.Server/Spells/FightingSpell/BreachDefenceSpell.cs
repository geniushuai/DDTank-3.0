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

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.BREACHDEFENCE)]
    public class BreachDefenceSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            if (player.CurrentGame.Data.CurrentSpell != null)
                return;

            TankData data = player.CurrentGame.Data;
            data.BreachDefence = true;

        }
    }
}
