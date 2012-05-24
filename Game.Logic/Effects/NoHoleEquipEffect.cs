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
    public class NoHoleEquipEffect : BasePlayerEffect  //免坑
    {
        private int m_count = 0;
        private int m_probability = 0;

        public NoHoleEquipEffect(int count, int probability)
            : base(eEffectType.NoHoleEquipEffect)
        {
            m_count = count;
            m_probability = probability;
        }

        public override bool Start(Living living)
        {
            NoHoleEquipEffect effect = living.EffectList.GetOfType(eEffectType.NoHoleEquipEffect) as NoHoleEquipEffect;
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
            player.AfterKilledByLiving += new KillLivingEventHanlde(player_AfterKilledByLiving);
        }

        protected override void OnRemovedFromPlayer(Player player)
        {
            player.AfterKilledByLiving -= new KillLivingEventHanlde(player_AfterKilledByLiving);
        }

        void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
        {
            if (rand.Next(100) < m_probability)
            {
                living.ShootMovieDelay = 50;
                SpellMgr.ExecuteSpell(living.Game, living as Player, ItemMgr.FindItemTemplate(10021));
               // living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("NoHoleEquipEffect.Success"));
            }
        }

        
    }
}
