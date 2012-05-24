using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Games
{
    public class DelayTimeAction:IAction
    {
        private long m_time;

        private bool m_isFinished;

        public DelayTimeAction(int delay)
        {
            m_time = GameMgr.GetTickCount() + delay;
        }
    
        public void  Execute(BaseGame game, long tick)
        {
 	        if(m_time <= tick)
            {
                m_isFinished = true;
            }
        }

        public bool  IsFinish()
        {
            return m_isFinished;
        }
}
}
