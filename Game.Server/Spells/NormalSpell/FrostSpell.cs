using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Server.SceneGames;
using Game.Server.Effects;
using Game.Server.Managers;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.FROST)]
    public class FrostSpell : ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            //if (player.CurrentGame.Data.CurrentSpell != this)
            if (player.CurrentGame.Data.CurrentFire == null)
            {
                player.CurrentGame.Data.CurrentSpell = this;
                player.CurrentGame.Data.CurrentPorp = item;
                //player.CurrentGame.Data.CurrentBall = Bussiness.Managers.BallMgr.FindBall(1);
                player.CurrentGame.Data.SetCurrentBall(BallMgr.FindBall(1), false);

                player.CurrentGame.Data.AddAttack = -1;
                player.CurrentGame.Data.AddBall = 1;
            }
            else
            {
                if (player.CurrentGame.Data.Players[player].IsFrost == 0)
                {
                    IceFronzeEffect ice = new IceFronzeEffect(item.Template.Property2);
                    ice.Start(player);
                }
                else
                {
                    player.CurrentGame.Data.Players[player].SetFrost(item.Template.Property2);
                }

            }
        }
    }
}
