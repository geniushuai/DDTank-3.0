using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Rooms;
using Game.Server.GameObjects;
using System.Threading;
using log4net;
using System.Reflection;
using System.Collections;
using Phy.Maps;
using Game.Server.Managers;
using System.Diagnostics;

namespace Game.Server.Games
{
    public class GameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly long THREAD_INTERVAL = 40;

        private static ArrayList m_games;

        private static Thread m_thread;

        private static bool m_running;

        public static bool Setup()
        {
            m_thread = new Thread(new ThreadStart(GameThread));
            m_games = new ArrayList();
            return true;
        }

        public static bool Start()
        {
            m_running = true;
            m_thread.Start();
            return true;
        }

        public static void Stop()
        {
            m_running = false;
            m_thread.Join();
        }

        private static long StopwatchFrequencyMilliseconds = Stopwatch.Frequency / 1000;

        public static long GetTickCount()
        {
            return Stopwatch.GetTimestamp() / StopwatchFrequencyMilliseconds;
        }

        private static void GameThread()
        {
            long balance = 0;
            m_clearGamesTimer = GetTickCount();
            while (m_running)
            {
                long start = GetTickCount();
                try
                {
                    UpdateGames(start);
                    ClearStoppedGames(start);
                }
                catch (Exception ex)
                {
                    log.Error("Room Mgr Thread Error:", ex);
                }

                //时间补偿
                long end = GetTickCount();

                balance += THREAD_INTERVAL - (end - start);

                if (balance > 0)
                {
                    Thread.Sleep((int)balance);
                    balance = 0;
                }
                else
                {
                    if (balance < -1000)
                    {
                        log.WarnFormat("Room Mgr is delay {0} ms!", balance);
                        balance += 1000;
                    }
                }
            }
        }

        private static void UpdateGames(long tick)
        {
            IList games = GetGamesSafe();
            if (games != null)
            {
                foreach (BaseGame g in games)
                {
                    try
                    {
                        g.Update(tick);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Game  updated error:", ex);
                    }
                }
            }
        }

        private static readonly int CLEAR_GAME_INTERVAL = 60 * 1000;

        private static long m_clearGamesTimer;

        private static void ClearStoppedGames(long tick)
        {
            if(m_clearGamesTimer <= tick )
            {
                m_clearGamesTimer += CLEAR_GAME_INTERVAL;

                ArrayList temp = new ArrayList();
                lock (m_games)
                {
                    foreach (BaseGame g in m_games)
                    {
                        if (g.GameState == eGameState.Stopped)
                        {
                            temp.Add(g);
                        }
                    }

                    foreach (BaseGame g in temp)
                    {
                        m_games.Remove(temp);
                    }
                }
            }
        }

        public static IList GetGamesSafe()
        {
            ArrayList temp = null;
            lock (m_games)
            {
                temp = (ArrayList)m_games.Clone();
            }

            return temp;
        }

        public static BaseGame StartGame(List<GamePlayer> red, List<GamePlayer> blue, int mapIndex,eRoomType roomType, eTeamType teamType, eGameType gameType, int timeType)
        {
            try
            {
                int index = MapMgr.GetMapIndex(mapIndex,(byte)roomType);
                Map map = MapMgr.CloneMap(index);

                if (map != null)
                {
                    BaseGame game = new BaseGame(red, blue, map,roomType,teamType,gameType,timeType);

                    lock (m_games)
                    {
                        m_games.Add(game);
                    }

                    game.Prepare();
                    return game;
                }
                else 
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("Create game error:", e);
                return null;
            }

        }




    }
}
