using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class ReduceStrengthEffect : AbstractEffect
    {
        private int m_count;

        public ReduceStrengthEffect(int count)
            : base(eEffectType.ReduceStrengthEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            ReduceStrengthEffect effect = living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
            if (effect != null)
            {
                effect.m_count = m_count;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, 1, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, 1, false);
        }

        void player_BeginFitting(Living living)
        {
            m_count--;
            if (living is Player)
            {
                (living as Player).Energy -= 50;
            }
            if (m_count < 0)
            {
                Stop();
            }
        }
    }
}
