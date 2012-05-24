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
    [SpellAttibute((int)eSpellType.ATTACKUP)]
    public class AttackUpSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            //TankData data = player.CurrentGame.Data;
            //data.AttackUp *= item.Property1;
            player.CurrentGame.Data.Players[player].SetDander( item.Template.Property2);

            GSPacketIn pkg = player.Out.SendAttackUp(player);
            player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
        }
    }
}
