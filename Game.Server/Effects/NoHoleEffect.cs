using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Phy.Object;

namespace Game.Logic.Effects
{
    public class NoHoleEffect : AbstractEffect
    {
        public int Count;

        public NoHoleEffect(int count)
        {
            Count = count;
        }

        public override void Start(Player player)
        {
            base.Start(player);
            player.BeginAttacking += new PlayerEventHandle(player_BeginFitting);
            player.IsNoHole = true;

        }

        private void player_BeginFitting(Player player)
        {
            Count --;
            if(Count <=0)
            {
                Stop();
            }
        }

        public override void Stop()
        {
            m_player.BeginAttacking -= new PlayerEventHandle(player_BeginFitting);
            base.Stop();
        }
    }
}
