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
    public class AddDanderEquipEffect : BasePlayerEffect //激怒
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AddDanderEquipEffect(int count, int probability)
            : base(eEffectType.AddDander)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            AddDanderEquipEffect effect = living.EffectList.GetOfType(eEffectType.AddDander) as AddDanderEquipEffect;
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
            // player.PlayerShoot += new PlayerEventHandle(ChangeProperty);
            player.BeginAttacked += new LivingEventHandle(ChangeProperty);

        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.BeginAttacked -= new LivingEventHandle(ChangeProperty);
        }

        private void ChangeProperty(Living player)
        {
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                if (player is Player)
                    (player as Player).AddDander(m_count);
                player.EffectTrigger = true;
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AddDanderEquipEffect.Success"));
                player.Game.SendAttackEffect(player, 2);
            }
        }

    }
}
