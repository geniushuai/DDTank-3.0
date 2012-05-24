using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;
namespace Game.Logic.Effects
{
    public class ReflexDamageEquipEffect : BasePlayerEffect
    {
        private int m_count;
        private int m_probability;
        private int m_added;

        public ReflexDamageEquipEffect(int count, int probability)
            : base(eEffectType.ReflexDamageEquipEffect)
        {
            m_count = count;
            m_probability = probability;
            m_added = 0;
        }

        public override bool Start(Living living)
        {
            ReflexDamageEquipEffect effect = living.EffectList.GetOfType(eEffectType.ReflexDamageEquipEffect) as ReflexDamageEquipEffect;
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
            player.AfterKilledByLiving += new KillLivingEventHanlde(player_AfterKilledByLiving);
        }

        void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (IsTrigger)
            {
                target.AddBlood(-m_count);
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacked -= new LivingEventHandle(ChangeProperty);
        }

        public void ChangeProperty(Living living)
        {
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                living.EffectTrigger = true;
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("ReflexDamageEquipEffect.Success", m_count));
                living.Game.SendAttackEffect(living, 2);
            }
        }
    }
}
