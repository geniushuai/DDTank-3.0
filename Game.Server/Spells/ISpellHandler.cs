using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Spells
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISpellHandler
    {
        void Execute(GamePlayer player, ItemInfo item);
    }
}
