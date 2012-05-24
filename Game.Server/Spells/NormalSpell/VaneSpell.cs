using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.SceneGames;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.VANE)]
    public class VaneSpell : ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            player.CurrentGame.Data.CurrentWind = player.CurrentGame.Data.CurrentWind * (-1);
            GSPacketIn pkg = player.Out.SendPlayerWind(player, player.CurrentGame.Data.CurrentWind);
            player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
        }
    }
}
