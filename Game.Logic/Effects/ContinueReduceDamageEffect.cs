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
    public class ContinueReduceDamageEffect : AbstractEffect
    {
        private int m_count;

        public ContinueReduceDamageEffect(int count)
            : base(eEffectType.ContinueReduceDamageEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            ContinueReduceDamageEffect effect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as ContinueReduceDamageEffect;
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
            //living.IsFrost = true;
            if (living is Player)
            {
                (living as Player).BaseDamage = (living as Player).BaseDamage * 5 / 100;
            }
            living.Game.SendPlayerPicture(living, 4, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            //living.IsFrost = false;
            (living as Player).BaseDamage = (living as Player).BaseDamage * 100 / 5;
            living.Game.SendPlayerPicture(living, 4, false);
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


