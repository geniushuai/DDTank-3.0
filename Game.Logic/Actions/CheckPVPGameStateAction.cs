using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Actions
{
    public class CheckPVPGameStateAction:IAction
    {
        private long m_tick;
        private bool m_isFinished;

        public CheckPVPGameStateAction(int delay)
        {
            m_isFinished = false;
            m_tick += TickHelper.GetTickCount() + delay;
        }

        public void Execute(BaseGame game, long tick)
        {
            
            if (m_tick <= tick)
            {
                PVPGame pvp = game as PVPGame;
                if (pvp != null)
                {
                    switch (game.GameState)
                    {
                        case eGameState.Inited:
                            pvp.Prepare();
                            break;
                        case eGameState.Prepared:
                            pvp.StartLoading();
                            break;
                        case eGameState.Loading:
                            if (pvp.IsAllComplete())
                            {
                                pvp.StartGame();
                            }
                            break;
                        case eGameState.Playing:
                            if (pvp.CurrentPlayer == null || pvp.CurrentPlayer.IsAttacking == false)
                            {
                                if (pvp.CanGameOver())
                                {
                                    pvp.GameOver();
                                }
                                else
                                {
                                    pvp.NextTurn();
                                }
                            }
                            break;
                        case eGameState.GameOver:
                            pvp.Stop();
                            break;
                    }
                }
                m_isFinished = true;
            }
        }

        public bool IsFinished(long tick)
        {
            return m_isFinished;
        }
    }
}
