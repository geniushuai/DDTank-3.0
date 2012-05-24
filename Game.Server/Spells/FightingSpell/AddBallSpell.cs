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
    [SpellAttibute((int)eSpellType.ADDBALL)]
    public class AddBallSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            //if (player.CurrentGame.Data.CurrentSpell != null)
            //    return;
            ItemInfo info = player.CurrentGame.Data.CurrentPorp;
            if (player.CurrentGame.Data.CurrentSpell != null && info.Template.Property1 != 10 && info.Template.Property1 != 8)
                return;

            TankData data = player.CurrentGame.Data;
            if (data.CurrentBall != data.CurrentIndex.Ball2)
                data.AddBall = item.Template.Property2;
        }
    }
}
