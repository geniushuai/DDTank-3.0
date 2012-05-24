using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingDelayEffectAction:BaseAction
    {
        private AbstractEffect m_effect;
        private Living m_living;

        public LivingDelayEffectAction(Living living,AbstractEffect effect,int delay)
            :base(delay)
        {
            m_effect = effect;
            m_living = living;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            m_effect.Start(m_living);
            Finish(tick);
        }
    }
}
