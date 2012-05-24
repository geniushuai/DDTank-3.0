using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.SceneGames;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.STUNT)]
    public class StuntSpell:ISpellHandler
    {
        public void Execute(GamePlayer player,ItemInfo item)
        {
            player.CurrentGame.Data.Players[player].SetDander(-1);
        }
    }
}
