using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;

namespace Game.Server.Games
{
    public class WaitPlayerAttackingAction:IAction
    {
        private bool m_isFinished;
        private long m_tick;
        private Player m_player;
        private int m_turnIndex;

        public WaitPlayerAttackingAction(int delay, Player player,int turnIndex)
        {
            m_tick = GameMgr.GetTickCount() + delay;
            m_player = player;
            m_turnIndex = turnIndex;
            player.EndAttacking += player_EndAttacking;
        }

        void player_EndAttacking(Player player)
        {
            player.EndAttacking -= player_EndAttacking;
            m_isFinished = true;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (m_tick < tick && !m_isFinished)
            {
                m_isFinished = true;
                if (game.TurnIndex == m_turnIndex && m_player.IsAttacking)
                {
                    m_player.StopAttacking();
                    game.CheckState(0);
                }
            }
        }

        public bool IsFinish()
        {
            return m_isFinished;
        }
    }
}
