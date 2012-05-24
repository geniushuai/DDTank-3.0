using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingBeatAction : BaseAction
    {
        private Living m_living;
        private Living m_target;
        private int m_demageAmount;
        private int m_criticalAmount;
        private String m_action;

        public LivingBeatAction(Living living, Living target, int demageAmount, int criticalAmount, string action, int delay)
            : base(delay)
        {
            m_living = living;
            m_target = target;
            m_demageAmount = demageAmount;
            m_criticalAmount = criticalAmount;
            m_action = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            m_target.SyncAtTime = false;
            try
            {
                if (m_target.TakeDamage(m_living, ref m_demageAmount, ref m_criticalAmount, "小怪伤血"))
                {
                    int totalDemageAmount = m_demageAmount + m_criticalAmount;
                    //Console.WriteLine("LivingBeatAction ExecuteImpl totalDemageAmount: {0}", totalDemageAmount);
                    game.SendLivingBeat(m_living, m_target, totalDemageAmount, m_action);
                }
                m_target.IsFrost = false;
                Finish(tick);
            }
            finally
            {
                m_target.SyncAtTime = true;
            }
        }
    }
}
