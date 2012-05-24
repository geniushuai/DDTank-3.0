using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Phy.Object;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.ADD_LIFE)]
    public class AddLifeSpell:ISpellHandler
    {
        public void Execute(Game.Server.Games.BaseGame game, Player player, ItemTemplateInfo item)
        {
            switch (item.Property2)
            {
                case 0:
                    if (player.IsLiving)
                    {
                        player.AddBlood(item.Property3, true);
                    }
                    break;
                case 1:
                    Player[] temps = player.Game.GetAllFightPlayersSafe();
                    foreach (Player p in temps)
                    {
                        if (p.IsLiving && p.Team == player.Team)
                        {
                            p.AddBlood(item.Property3, true);
                        }
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
