using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Game.Logic.Effects
{
    public class FatalEffect : BasePlayerEffect  //增加瞄准
    {
        private int m_count = 0;
        private int m_probability = 0;

        public FatalEffect(int count, int probability)
            : base(eEffectType.FatalEffect)
        {
            m_count = count;
            m_probability = probability;
        }
        public override bool Start(Living living)
        {
            FatalEffect effect = living.EffectList.GetOfType(eEffectType.FatalEffect) as FatalEffect;
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
            IsTrigger = false;
            if (rand.Next(100) < m_probability)
            {
                player.ShootMovieDelay = 50;
                IsTrigger = true;
                player.EffectTrigger = true;
                if (player.CurrentBall.ID != 3)
                    player.ControlBall = true;
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("FatalEffect.Success"));
                player.Game.SendAttackEffect(player, 1);
            }
        }
    }
}
