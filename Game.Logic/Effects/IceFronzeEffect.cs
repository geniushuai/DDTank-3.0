using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class IceFronzeEffect : AbstractEffect
    {
        private int m_count;

        public IceFronzeEffect(int count):base(eEffectType.IceFronzeEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            IceFronzeEffect effect = living.EffectList.GetOfType(eEffectType.IceFronzeEffect) as IceFronzeEffect;
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
            living.IsFrost = true;
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.IsFrost = false;
        }

        void player_BeginFitting(Living player)
        {
            m_count--;
            if (m_count < 0)
            {
                Stop();
            }
        }
    }
}
