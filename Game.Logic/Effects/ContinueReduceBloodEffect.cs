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
    public class ContinueReduceBloodEffect : AbstractEffect
    {
        private int m_count;
        private int m_blood;
        public ContinueReduceBloodEffect(int count,int blood)
            : base(eEffectType.ContinueReduceBloodEffect)
        {
            m_count = count;
            m_blood = blood;
        }

        public override bool Start(Living living)
        {
            ContinueReduceBloodEffect effect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as ContinueReduceBloodEffect;
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
            living.Game.SendPlayerPicture(living, 2, true);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.Game.SendPlayerPicture(living, 2, false);
        }

        void player_BeginFitting(Living living)
        {
            m_count--;
            if (living is Player)
            {
                Player p = (living as Player);

                if (p.Blood < Math.Abs(m_blood))
                {
                    p.AddBlood(-p.Blood + 2);
                }
                else
                {
                    p.AddBlood(m_blood);
                }
            }
            if (m_count < 0)
            {
                Stop();
            }
        }

    }
}
