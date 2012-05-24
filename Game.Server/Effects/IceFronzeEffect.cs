using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Phy.Object;

namespace Game.Logic.Effects
{
    public class IceFronzeEffect:AbstractEffect
    {
        private int m_count;

        public IceFronzeEffect(int count)
        {
            m_count = count;
        }

        public override void Start(Player player)
        {
            base.Start(player);
            player.BeginAttacking += new PlayerEventHandle(player_BeginFitting);
            player.IsFrost = true;
        }

        void player_BeginFitting(Player player)
        {
            m_count--;
            if (m_count <= 0)
            {
                Stop();
            }
        }

        public override void Stop()
        {
            m_player.BeginAttacking -= new PlayerEventHandle(player_BeginFitting);
            m_player.IsFrost = false;
            base.Stop();
        }
    }
}
