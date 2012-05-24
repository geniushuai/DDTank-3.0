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
    public class ContinueReduceBloodEquipEffect : BasePlayerEffect
    {
        private int m_blood = 0;
        private int m_probability = 0;

        public ContinueReduceBloodEquipEffect(int blood, int probability)
            : base(eEffectType.ContinueReduceBloodEquipEffect)
        {
            m_blood = blood;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            ContinueReduceBloodEquipEffect effect = living.EffectList.GetOfType(eEffectType.ContinueReduceBloodEquipEffect) as ContinueReduceBloodEquipEffect;
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

        protected void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (IsTrigger)
            {
                target.AddEffect(new ContinueReduceBloodEffect(2,-m_blood), 0);
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
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("ContinueReduceBloodEquipEffect.Success"));
               // player.Game.SendAttackEffect(player, 1);
            }
        }
    }
}
