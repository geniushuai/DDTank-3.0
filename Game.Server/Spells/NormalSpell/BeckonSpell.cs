using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Spells;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.SceneGames;
using Game.Server.Managers;

namespace Game.Server.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.BECKON)]
    public class BeckonSpell:ISpellHandler
    {
        public void Execute(GamePlayer player, ItemInfo item)
        {
            //if (player.CurrentGame.Data.CurrentSpell != this)
            //{
            //    player.CurrentGame.Data.CurrentSpell = this;
            //    player.CurrentGame.Data.CurrentPorp = item;
            //    //player.CurrentGame.Data.CurrentBall = Bussiness.Managers.BallMgr.FindBall(2);
            //    player.CurrentGame.Data.SetCurrentBall(BallMgr.FindBall(2),false);

            //    player.CurrentGame.Data.AddAttack = -1;
            //    player.CurrentGame.Data.AddBall = 1;
            //}
            //else
            //{
            //    if (player == player.CurrentGame.Data.CurrentFire)
            //        return;

            //    player.CurrentGame.Data.Players[player].BeckonPoint();
            //    GSPacketIn pkg = player.Out.SendPlayerBeckon(player);
            //    player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
            //}
        }
    }
}
