using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.Managers;
using Phy.Object;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.BECKON)]
    public class BeckonSpell:ISpellHandler
    {
        public void Execute(Game.Server.Games.BaseGame game, Player player, ItemTemplateInfo item)
        {
           
        }
    }
}
