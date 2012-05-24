using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class AssimilateBloodEffect : BasePlayerEffect //吸血
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AssimilateBloodEffect(int count, int probability)
            : base(eEffectType.AssimilateBloodEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            AssimilateBloodEffect effect = living.EffectList.GetOfType(eEffectType.AssimilateBloodEffect) as AssimilateBloodEffect;
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
            player.TakePlayerDamage += new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
            player.PlayerShoot += new PlayerEventHandle(player_PlayerShoot);
        }



        protected override void OnRemovedFromPlayer(Player player)
        {
            player.TakePlayerDamage -= new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
            player.PlayerShoot -= new PlayerEventHandle(player_PlayerShoot);
        }

        void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (IsTrigger)
            {
                living.AddBlood(damageAmount * m_count / 100);
                living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AssimilateBloodEffect.Success"));
                living.Game.SendAttackEffect(living, 1);
            }

        }
        void player_PlayerShoot(Player player)
        {
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                player.EffectTrigger = true;
            }
        }
    }
}
