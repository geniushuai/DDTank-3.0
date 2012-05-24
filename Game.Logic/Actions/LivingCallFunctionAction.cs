using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingCallFunctionAction : BaseAction
    {
        private Living m_living;

        private LivingCallBack m_func;


        public LivingCallFunctionAction(Living living, LivingCallBack func, int delay)
            : base(delay)
        {
            m_living = living;
            m_func = func;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            try
            {
                m_func();
            }
            finally
            {
                Finish(tick);
            }
        }
    }
}
