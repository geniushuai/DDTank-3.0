using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class ReduceDamageEffect : BasePlayerEffect //减少伤害
    {
        private int m_count = 0;
        private int m_probability = 0;

        public ReduceDamageEffect(int count, int probability)
            : base(eEffectType.ReduceDamageEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        protected override void OnAttachedToPlayer(Player player)
        {
            player.BeforeTakeDamage += new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
        }

        void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                damageAmount -= m_count;
                living.EffectTrigger = true;
               // living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("ReduceDamageEffect.Success"));
                //living.Game.SendAttackEffect(living, 2);
            }
        }
    }
}
