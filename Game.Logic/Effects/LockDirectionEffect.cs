using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness.Managers;
using Game.Logic.Spells;
using Bussiness;

namespace Game.Logic.Effects
{
    public class LockDirectionEffect : AbstractEffect
    {
        private int m_count;

        public LockDirectionEffect(int count)
            : base(eEffectType.LockDirectionEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            LockDirectionEffect effect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as LockDirectionEffect;
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
            living.Game.SendPlayerPicture(living, 3, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, 3, false);
        }

        void player_BeginFitting(Living living)
        {
            m_count--;

            if (m_count < 0)
            {
                Stop();
            }
        }
    }
}
