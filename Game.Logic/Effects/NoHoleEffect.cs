using Game.Logic.Phy.Object;

namespace Game.Logic.Effects
{
    public class NoHoleEffect : AbstractEffect
    {
        private int m_count;

        public NoHoleEffect(int count):base(eEffectType.NoHoleEffect)
        {
            m_count = count;
        }

        public override bool Start(Living living)
        {
            NoHoleEffect effect = living.EffectList.GetOfType(eEffectType.NoHoleEffect) as NoHoleEffect;
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
            living.IsNoHole = true;
            living.BeginSelfTurn += new LivingEventHandle(player_BeginFitting);
        }

        public override void OnRemoved(Living living)
        {
            living.BeginSelfTurn -= new LivingEventHandle(player_BeginFitting);
            living.IsNoHole = false;
        }

        private void player_BeginFitting(Living player)
        {
            m_count --;
            if (m_count <= 0)
            {
                Stop();
            }
        }
    }
}
