using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class SealEffect : AbstractEffect
    {
        private int m_count;

        private int m_type;

        public SealEffect(int count,int type):base(eEffectType.SealEffect)
        {
            m_count = count;
            m_type = type;
        }

        public override bool Start(Living living)
        {
            SealEffect effect = living.EffectList.GetOfType(eEffectType.SealEffect) as SealEffect;
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
            living.SetSeal(true, m_type);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.SetSeal(false, m_type);
        }

        void player_BeginFitting(Living living)
        {
            m_count--;
            if (m_count <= 0)
            {
                Stop();
            }
        }
    }
}
