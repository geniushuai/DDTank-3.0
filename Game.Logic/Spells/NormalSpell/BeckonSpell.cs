using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Logic;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.BECKON)]
    public class BeckonSpell:ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
           
        }
    }
}
