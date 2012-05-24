using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Phy.Object;

namespace Game.Logic.Effects
{
    public class InvinciblyEffect : AbstractEffect
    {
        private int m_count;

        public InvinciblyEffect(int count)
        {
            m_count = count;
        }
        public override void Start(Player player)
        {
            base.Start(player);
            player.BeginAttacking += new PlayerEventHandle(player_BeginFitting);
        }

        private void player_BeginFitting(Player player)
        {
            m_count--;
            if (m_count <= 0)
            {
                this.Stop();
            }
        }

        public override void Stop()
        {
            m_player.BeginAttacking -= new PlayerEventHandle(player_BeginFitting);
            base.Stop();
        }
    }
}
