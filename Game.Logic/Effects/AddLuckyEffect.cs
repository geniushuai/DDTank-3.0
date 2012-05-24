using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class AddLuckyEffect : BasePlayerEffect  //加幸运
    {
        private int m_count = 0;
        private int m_probability = 0;
        private int m_added = 0;

        public AddLuckyEffect(int count, int probability)
            : base(eEffectType.AddLuckyEffect)
        {
            m_count = count;
            m_probability = probability;
            m_added = 0;
        }


        public override bool Start(Living living)
        {
            AddLuckyEffect effect = living.EffectList.GetOfType(eEffectType.AddLuckyEffect) as AddLuckyEffect;
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
            player.BeginAttacking += new LivingEventHandle(ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacking -= new LivingEventHandle(ChangeProperty);
        }

        private void ChangeProperty(Living living)
        {
            living.Lucky -= m_added;
            m_added = 0;
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                living.Lucky += m_count;
                living.EffectTrigger = true;
                m_added = m_count;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddLuckyEffect.Success", m_count));
                living.Game.SendAttackEffect(living, 1);
            }
        }
    }
}
