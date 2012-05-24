using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Server.SceneGames;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Logic.Spells;

namespace Game.Server.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ABOMB)]
    public class ABombSpell : ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            if (player.CurrentGame.Data.CurrentFire == null)
            {
                player.CurrentGame.Data.CurrentSpell = this;
                player.CurrentGame.Data.CurrentPorp = item;
                player.CurrentGame.Data.SetCurrentBall(BallMgr.FindBall(4), false);

            }
            //else
            //{
            //    if (player.CurrentGame.Data.Players[player].IsFrost == 0)
            //    {
            //        IceFronzeEffect ice = new IceFronzeEffect(item.Template.Property2);
            //        ice.Start(player);
            //    }
            //    else
            //    {
            //        player.CurrentGame.Data.Players[player].SetFrost(item.Template.Property2);
            //    }

            //}
        }
    }
}
