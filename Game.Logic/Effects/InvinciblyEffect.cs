using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class InvinciblyEffect : AbstractEffect
    {
        private int m_count;

        public InvinciblyEffect(int count):base(eEffectType.InvinciblyEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            InvinciblyEffect effect = living.EffectList.GetOfType(eEffectType.InvinciblyEffect) as InvinciblyEffect;
            if (effect != null)
            {
                effect.m_count = m_count;
                return true;
            }
            else
            {
                return base.Start(living);
            }
        }

        public override void OnAttached(Living living)
        {
            living.BeginSelfTurn += new LivingEventHandle(player_BeginFitting);
            
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
        }

        private void player_BeginFitting(Living player)
        {
            m_count--;
            if (m_count <= 0)
            {
                this.Stop();
            }
        }
    }
}
