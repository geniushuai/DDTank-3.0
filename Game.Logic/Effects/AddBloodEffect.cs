using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class AddBloodEffect : BasePlayerEffect //攻击,被攻击回血
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AddBloodEffect(int count, int probability)
            : base(eEffectType.AddBloodEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            AddBloodEffect effect = living.EffectList.GetOfType(eEffectType.AddBloodEffect) as AddBloodEffect;
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
            player.PlayerShoot += new PlayerEventHandle(ChangeProperty);
            player.BeginAttacked += new LivingEventHandle(ChangeProperty);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(ChangeProperty);
            player.BeginAttacked -= new LivingEventHandle(ChangeProperty);
        }

        public void ChangeProperty(Living living)
        {
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                IsTrigger = true;
                living.EffectTrigger = true;
                living.Blood += m_count;
               // living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddBloodEffect.Success", m_count));
                //living.Game.SendAttackEffect(living, 2);
            }
        }
    }
}
