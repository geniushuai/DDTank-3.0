using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Games
{
    public class CheckGameStateAction:IAction
    {
        private long m_tick;
        private bool m_isFinished;

        public CheckGameStateAction(int delay)
        {
            m_isFinished = false;
            m_tick += GameMgr.GetTickCount() + delay;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (m_tick <= tick)
            {
                switch (game.GameState)
                {
                    case eGameState.Inited:
                        game.Prepare();
                        break;
                    case eGameState.Prepared:
                        game.StartLoading();
                        break;
                    case eGameState.Loading:
                        if (game.IsAllComplete())
                        {
                            game.StartGame();
                        }
                        break;
                    case eGameState.Playing:
                        if (game.CurrentPlayer == null || game.CurrentPlayer.IsAttacking == false)
                        {
                            if (game.CanGameOver())
                            {
                                game.GameOver();
                            }
                            else
                            {
                                game.NextTurn();
                            }
                        }
                        break;
                    case eGameState.GameOver:
                        game.Stop();
                        break;
                }
                m_isFinished = true;
            }
        }

        public bool IsFinish()
        {
            return m_isFinished;
        }
    }
}
