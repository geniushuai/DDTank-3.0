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
    [SpellAttibute((int)eSpellType.ADDWOUND)]
    public class AddWoudSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            TankData data = player.CurrentGame.Data;
            data.AddWound = (data.AddWound + (double)item.Template.Property2 / 100);

            GSPacketIn pkg = player.Out.SendAddWound(player);
            player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
        }
    }
}
