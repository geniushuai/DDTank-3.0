using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Phy.Object;
using Phy.Maths;
using Game.Base.Packets;

namespace Game.Server.Games
{
    public class GhostMoveAction:IAction
    {
        private Point m_target;
        private Player m_player;
        private bool m_isFinished;
        private Point m_v;
        private bool m_isSend;

        public GhostMoveAction(Player player,Point target)
        {
            m_player = player;
            m_target = target;
            m_isFinished = false;
            m_v = new Point(target.X - m_player.X, target.Y - m_player.Y);
            m_v.Normalize(2);
        }

        public void Execute(BaseGame game, long tick)
        {
            if (!m_isSend)
            {
                m_isSend = true;

                GSPacketIn pkg = m_player.PlayerDetail.Out.SendPlayerMove(m_player, 2, m_target.X, m_target.Y, (byte)(m_v.X > 0 ? 1 : -1),false);

                game.SendToAll(pkg,m_player.PlayerDetail);
            }

            if (m_target.Distance(m_player.X, m_player.Y) > 2)
            {
                m_player.SetXY(m_player.X + m_v.X, m_player.Y + m_v.Y);
            }
            else
            {
                m_player.SetXY(m_target.X, m_target.Y);
                m_isFinished = true;
            }
        }

        public bool IsFinish()
        {
            return m_isFinished;
        }

    }
}
