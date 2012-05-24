using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;


namespace Game.Logic.Effects
{
    public class AddAttackEffect : BasePlayerEffect     //加攻击
    {
        private int m_count = 0;
        private int m_probability = 0;
        private int m_added = 0;

        public AddAttackEffect(int count, int probability)
            : base(eEffectType.AddAttackEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            AddAttackEffect effect = living.EffectList.GetOfType(eEffectType.AddAttackEffect) as AddAttackEffect;
            if (effect != null)
            {
                m_probability = m_probability > effect.m_probability ? m_probability : effect.m_probability;
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
            living.Attack -= m_added;
            m_added = 0;
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {

                IsTrigger = true;
                living.EffectTrigger = true;
                living.Attack += m_count;
                m_added = m_count;
                // living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddAttackEffect.Success", m_count));
                // living.Game.SendAttackEffect(living, 1);
            }
        }
    }
}
