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
    public class AtomBombEquipEffect : BasePlayerEffect  //原子弹
    {
        private int m_count = 0;
        private int m_probability = 0;

        public AtomBombEquipEffect(int count, int probability)
            : base(eEffectType.AtomBomb)
        {
            m_count = count;
            m_probability = probability;
        }
        public override bool Start(Living living)
        {
            AtomBombEquipEffect effect = living.EffectList.GetOfType(eEffectType.AtomBomb) as AtomBombEquipEffect;
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
                SpellMgr.ExecuteSpell(player.Game, player, ItemMgr.FindItemTemplate(10022));
                player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AtomBombEquipEffect.Success"));
            }
        }
    }
}
