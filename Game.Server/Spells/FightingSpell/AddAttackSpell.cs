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
    [SpellAttibute((int)eSpellType.ADDATTACK)]
    public class AddAttackSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            ItemInfo info = player.CurrentGame.Data.CurrentPorp;
            if (player.CurrentGame.Data.CurrentSpell != null && info.Template.Property1 != 10 && info.Template.Property1 != 8)
                return;

            TankData data = player.CurrentGame.Data;
            data.AddAttack = item.Template.Property2;

            if (item.Template.Property2 == 2)
                data.Modulus *= 0.6;
            else
                data.Modulus *= 0.9;
        }
    }
}
