using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using System.Drawing;

namespace Game.Logic.Actions
{
    public class LivingMoveToAction : BaseAction
    {
        private Living m_living;

        private List<Point> m_path;

        private string m_action;

        private bool m_isSent;

        private int m_index;

        private LivingCallBack m_callback;

        public LivingMoveToAction(Living living, List<Point> path, string action, int delay, LivingCallBack callback)
            : base(delay, 0)
        {
            m_living = living;
            m_path = path;
            m_action = action;
            m_isSent = false;
            m_index = 0;
            m_callback = callback;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            if (!m_isSent)
            {
                m_isSent = true;
                game.SendLivingMoveTo(m_living, m_living.X, m_living.Y, m_path[m_path.Count - 1].X, m_path[m_path.Count - 1].Y, m_action);
            }
            m_index++;
            if (m_index >= m_path.Count)
            {
                if (m_path[m_index - 1].X > m_living.X)
                {
                    m_living.Direction = 1;
                }
                else
                {
                    m_living.Direction = -1;
                }

                m_living.SetXY(m_path[m_index - 1].X, m_path[m_index - 1].Y);

                if (m_callback != null)
                {
                    m_living.CallFuction(m_callback, 0);
                }
                Finish(tick);
            }
        }

    }
}
