using System;
using System.Collections.Generic;
using System.Threading;
using log4net;
using System.Reflection;
using System.Collections;
using Game.Server.Managers;
using System.Diagnostics;
using Game.Logic.Phy.Maps;

namespace Game.Logic
{
    public class GameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly long THREAD_INTERVAL = 40;

        private static ArrayList m_games;

        private static Thread m_thread;

        private static bool m_running;

        private static int m_serverId;

        private static int m_boxBroadcastLevel;

        private static int m_gameId;

        public static bool Setup(int serverId,int boxBroadcastLevel)
        {
            m_thread = new Thread(new ThreadStart(GameThread));
            m_games = new ArrayList();
            m_serverId = serverId;
            m_boxBroadcastLevel = boxBroadcastLevel;
            m_gameId = 0;

            return true;
        }

        public static int BoxBroadcastLevel
        {
            get { return m_boxBroadcastLevel; }
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

        public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex,eRoomType roomType, eTeamType teamType, eGameType gameType, int timeType)
        {
            try
            {
                int index = MapMgr.GetMapIndex(mapIndex,(byte)roomType,m_serverId);
                Map map = MapMgr.CloneMap(index);

                if (map != null)
                {
                    PVPGame game = new PVPGame(m_gameId ++,red, blue, map,roomType,teamType,gameType,timeType);

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

        public static BaseGame StartPVEGame(List<IGamePlayer> player, int mapIndex, eRoomType roomType, eTeamType teamType, eGameType gameType, int timeType)
        {
            try
            {
                int index = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
                index = 1072;
                Map map = MapMgr.CloneMap(index);

                if (map != null)
                {
                    PVEGame game = new PVEGame(m_gameId ++,player, map, roomType, teamType, gameType, timeType);

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
