using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingDieAction:BaseAction
    {
        private Living m_living;
        public LivingDieAction(Living living,int delay) : base(delay,1000) 
        {
            m_living = living;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            m_living.Die();
            Finish(tick);
        }
    }
}
