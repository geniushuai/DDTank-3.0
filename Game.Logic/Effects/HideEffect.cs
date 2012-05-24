using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class HideEffect : AbstractEffect
    {
        private int m_count;

        public HideEffect(int count):base(eEffectType.HideEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            HideEffect effect = living.EffectList.GetOfType(eEffectType.HideEffect) as HideEffect;
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
            living.BeginSelfTurn += player_BeginFitting;
            living.IsHide = true;
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn += player_BeginFitting;
            living.IsHide = false;
        }

        private void player_BeginFitting(Living player)
        {
            m_count--;
            if (m_count <= 0)
            {
                Stop();
            }
        }
    }
}
