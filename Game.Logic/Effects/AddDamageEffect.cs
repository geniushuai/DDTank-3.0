using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class AddDamageEffect : BasePlayerEffect //增加伤害
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AddDamageEffect(int count, int probability)
            : base(eEffectType.AddDamageEffect)
        {
            m_count = count;
            m_probability = probability;
            /// indexer = 0;
        }

        public override bool Start(Living living)
        {
            AddDamageEffect effect = living.EffectList.GetOfType(eEffectType.AddDamageEffect) as AddDamageEffect;
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
            player.TakePlayerDamage += new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
            player.PlayerShoot += new PlayerEventHandle(playerShot);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.TakePlayerDamage -= new LivingTakedDamageEventHandle(player_BeforeTakeDamage);
            player.PlayerShoot -= new PlayerEventHandle(playerShot);
        }

        void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (IsTrigger)
            {
                damageAmount += m_count;
                // living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddDamageEffect.Success"));
                // living.Game.SendAttackEffect(living, 1);
            }

        }

        void playerShot(Player player)
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
