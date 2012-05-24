
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
    public class AddTurnEquipEffect : BasePlayerEffect //立即行动
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AddTurnEquipEffect(int count, int probability)
            : base(eEffectType.AddTurnEquipEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            AddTurnEquipEffect effect = living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) as AddTurnEquipEffect;
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
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.PlayerShoot -= new PlayerEventHandle(ChangeProperty);
        }

        private void ChangeProperty(Player player)
        {
            if (rand.Next(100) < m_probability)
            {
                player.Delay = player.DefaultDelay;
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.Success"));
                player.Game.SendAttackEffect(player, 1);
            }
        }
    }
}


