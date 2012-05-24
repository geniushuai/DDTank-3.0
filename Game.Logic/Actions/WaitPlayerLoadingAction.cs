using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Actions
{
    public class WaitPlayerLoadingAction : IAction
    {
        private long m_time;
        private bool m_isFinished;

        public WaitPlayerLoadingAction(BaseGame game, int maxTime)
        {
            m_time = TickHelper.GetTickCount() + maxTime;
            game.GameStarted += new GameEventHandle(game_GameStarted);
        }

        void game_GameStarted(AbstractGame game)
        {
            game.GameStarted -= new GameEventHandle(game_GameStarted);
            m_isFinished = true;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (!m_isFinished && tick > m_time && game.GameState == eGameState.Loading)
            {
                if (game.GameState == eGameState.Loading)
                {
                    List<Player> player = game.GetAllFightPlayers();
                    foreach (Player p in player)
                    {
                        if (p.LoadingProcess < 100)
                        {
                            game.SendPlayerRemove(p);
                            game.RemovePlayer(p.PlayerDetail, false);
                        }
                    }

                    game.CheckState(0);
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
