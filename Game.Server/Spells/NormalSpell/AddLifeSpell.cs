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
    [SpellAttibute((int)eSpellType.ADD_LIFE)]
    public class AddLifeSpell:ISpellHandler
    {

        public void Execute(GamePlayer player, ItemInfo item)
        {
            TankData data = player.CurrentGame.Data;

            if (item.Template.Property2 == 0 && data.Players[player].Blood > 0)
            {
                data.Players[player].Blood = item.Template.Property3;
            }

            if (item.Template.Property2 == 1)
            {
                foreach (GamePlayer p in data.Players.Keys)
                {
                    if (player.CurrentTeamIndex == p.CurrentTeamIndex && data.Players[p].Blood > 0)
                    {
                        data.Players[p].Blood = item.Template.Property3;
                    }
                }
            }

        }
    }
}
