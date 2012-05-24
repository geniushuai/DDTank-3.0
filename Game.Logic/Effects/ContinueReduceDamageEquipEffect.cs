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
    public class ContinueReduceDamageEquipEffect : BasePlayerEffect
    {
        private int m_count = 0;
        private int m_probability = 0;

        public ContinueReduceDamageEquipEffect(int count, int probability)
            : base(eEffectType.ContinueReduceDamageEquipEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            ContinueReduceDamageEquipEffect effect = living.EffectList.GetOfType(eEffectType.ContinueReduceDamageEquipEffect) as ContinueReduceDamageEquipEffect;
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
            player.PlayerShoot += new PlayerEventHandle(ChangeProperty);
            player.AfterKillingLiving += new KillLivingEventHanlde(player_AfterKillingLiving);
        }

        void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (IsTrigger)
            {
                target.AddEffect(new ContinueReduceDamageEffect(2), 0);
            }
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(ChangeProperty);
            player.AfterKillingLiving -= new KillLivingEventHanlde(player_AfterKillingLiving);
        }

        private void ChangeProperty(Player player)
        {
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                player.EffectTrigger = true;
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("ContinueReduceDamageEquipEffect.Success"));
                player.Game.SendAttackEffect(player, 1);
            }
        }
    }
}
