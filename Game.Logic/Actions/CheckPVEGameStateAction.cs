using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;

namespace Game.Logic.Actions
{
    public class CheckPVEGameStateAction : IAction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private long m_time;
        private bool m_isFinished;

        public CheckPVEGameStateAction(int delay)
        {
            m_time = TickHelper.GetTickCount() + delay;
            m_isFinished = false;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (m_time <= tick && game.GetWaitTimer() < tick)
            {
                PVEGame pve = game as PVEGame;
                if (pve != null)
                {
                    switch (pve.GameState)
                    {
                        case eGameState.Inited:
                            pve.Prepare();
                            break;
                        case eGameState.Prepared:
                            pve.PrepareNewSession();
                            break;
                        case eGameState.SessionPrepared:
                            if (pve.CanStartNewSession())
                            {
                                pve.StartLoading();
                            }
                            else
                            {
                                game.WaitTime(1000);
                            }
                            break;
                        case eGameState.Loading:
                            if (pve.IsAllComplete())
                            {
                                pve.StartGame();
                            }
                            else
                            {
                                game.WaitTime(1000);
                            }
                            break;
                        //TODO
                        case eGameState.GameStartMovie:
                            if (game.CurrentActionCount <= 1)
                            {
                                pve.StartGame();
                            }
                            else
                            {
                                pve.StartGameMovie();
                            }
                            break;
                        case eGameState.GameStart:
                            pve.PrepareNewGame();
                            break;
                        case eGameState.Playing:
                            if ((pve.CurrentLiving == null || pve.CurrentLiving.IsAttacking == false) && game.CurrentActionCount <= 1)
                            {
                                if (pve.CanGameOver())
                                {
                                    pve.GameOver();
                                }
                                else
                                {
                                    pve.NextTurn();
                                }
                            }
                            break;
                        case eGameState.GameOver:
                            if (pve.HasNextSession())
                            {
                                pve.PrepareNewSession();
                            }
                            else
                            {
                                pve.GameOverAllSession();
                            }
                            break;
                        case eGameState.ALLSessionStopped:
                            if (pve.PlayerCount == 0 || pve.WantTryAgain == 0)
                            {
                                pve.Stop();
                            }
                            else if (pve.WantTryAgain == 1)
                            {
                                pve.SessionId--;
                                pve.PrepareNewSession();
                            }
                            else
                            {
                                game.WaitTime(1000);
                            }
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
