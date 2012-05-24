using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    //NewAction
    public class FocusAction:BaseAction
    {
        private Physics m_obj;

        private int m_type;

        public FocusAction(Physics obj, int type, int delay, int finishTime)
            : base(delay, finishTime)
        {
            m_obj = obj;
            m_type = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.SendPhysicalObjFocus(m_obj, m_type);
          
            Finish(tick);
        }
    }
}
