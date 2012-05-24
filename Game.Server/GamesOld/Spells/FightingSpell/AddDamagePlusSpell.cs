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
    [SpellAttibute((int)eSpellType.ADDWOUND)]
    public class AddWoudSpell:ISpellHandler
    {
        public void Execute(Game.Server.Games.BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.CurrentDamagePlus += (float)item.Property2 / 100;
        }
    }
}
