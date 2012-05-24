using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class AddDefenceEffect : BasePlayerEffect   // 加防御
    {
        private int m_count;
        private int m_probability;
        private int m_added;

        public AddDefenceEffect(int count, int probability)
            : base(eEffectType.AddDefenceEffect)
        {
            m_count = count;
            m_probability = probability;
            m_added = 0;
        }

        public override bool Start(Living living)
        {
            AddDefenceEffect effect = living.EffectList.GetOfType(eEffectType.AddDefenceEffect) as AddDefenceEffect;
            if (effect != null)
            {
                effect.m_probability = m_probability > effect.m_probability ? m_probability : effect.m_probability;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeginAttacked += new LivingEventHandle(ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacked -= new LivingEventHandle(ChangeProperty);
        }

        public void ChangeProperty(Living living)
        {
            living.Defence -= m_added;
            m_added = 0;
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                living.Defence += m_count;
                m_added = m_count;
                living.EffectTrigger = true;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddDefenceEffect.Success", m_count));
                living.Game.SendAttackEffect(living, 2);
            }
        }
    }
}
