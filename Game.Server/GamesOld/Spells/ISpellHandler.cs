using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Game.Server.GameObjects;
using Phy.Object;
using SqlDataProvider.Data;
using Game.Server.Games;

namespace Game.Server.Spells
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISpellHandler
    {
        void Execute(BaseGame game, Player player,ItemTemplateInfo item);
    }
}
