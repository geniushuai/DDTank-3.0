using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Spells
{
    public class SpellAttibute:Attribute
    {
        public int Type { get; private set; }

        public SpellAttibute(int type)
        {
            this.Type = type;
        }
    }
}
