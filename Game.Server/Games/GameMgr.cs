using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic;
using System.Collections;
using log4net;
using System.Reflection;
using System.Threading;
using Game.Logic.Phy.Maps;
using SqlDataProvider.Data;
using Game.Server.Statics;

namespace Game.Server.Games
{
    public class GameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly long THREAD_INTERVAL = 40;

        private static List<BaseGame> m_games;

        private static Thread m_thread;

        private static bool m_running;

        private static int m_serverId;

        private static int m_boxBroadcastLevel;

        private static int m_gameId;

        public static bool Setup(int serverId, int boxBroadcastLevel)
        {
            m_thread = new Thread(new ThreadStart(GameThread));
            m_games = new List<BaseGame>();
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
            if (m_running == false)
            {
                m_running = true;
                m_thread.Start();
            }
            return true;
        }

        public static void Stop()
        {
            if (m_running)
            {
                m_running = false;
                m_thread.Join();
            }
        }

        private static readonly int CLEAR_GAME_INTERVAL = 5 * 1000;

        private static long m_clearGamesTimer;

        private static void GameThread()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            long balance = 0;
            m_clearGamesTimer = TickHelper.GetTickCount();
            while (m_running)
            {
                long start = TickHelper.GetTickCount();
                int gameCount = 0;
                try
                {
                    gameCount = UpdateGames(start);

                    if (m_clearGamesTimer <= start)
                    {
                        m_clearGamesTimer += CLEAR_GAME_INTERVAL;

                        ArrayList temp = new ArrayList();
                        foreach (BaseGame g in m_games)
                        {
                            if (g.GameState == eGameState.Stopped)
                            {
                                temp.Add(g);
                            }
                        }
                        foreach (BaseGame g in temp)
                        {
                            m_games.Remove(g);
                        }

                        ThreadPool.QueueUserWorkItem(new WaitCallback(ClearStoppedGames),temp);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Game Mgr Thread Error:", ex);
                }

                //时间补偿
                long end = TickHelper.GetTickCount();

                balance += THREAD_INTERVAL - (end - start);
                if (end - start > THREAD_INTERVAL * 2)
                {
                    log.WarnFormat("Game Mgr spent too much times: {0} ms, count:{1}",end - start, gameCount);
                }

                if (balance > 0)
                {
                    Thread.Sleep((int)balance);
                    balance = 0;
                }
                else
                {
                    if (balance < -1000)
                    {        
                        balance += 1000;
                    }
                }
            }
        }

        private static void ClearStoppedGames(object state)
        {
            ArrayList temp = state as ArrayList;
            foreach (BaseGame g in temp)
            {
                try
                {
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    log.Error("game dispose error:", ex);
                }
            }
        }

        private static int UpdateGames(long tick)
        {
            IList games = GetAllGame();
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

                return games.Count;
            }
            return 0;
        }

        public static List<BaseGame> GetAllGame()
        {
            List<BaseGame> list = new List<BaseGame>();
            lock (m_games)
            {
                list.AddRange(m_games);
            }

            return list;
        }

        public static BaseGame StartPVPGame(int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            try
            {
                int index = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
                Map map = MapMgr.CloneMap(index);

                if (map != null)
                {
                    PVPGame game = new PVPGame(m_gameId++, roomId, red, blue, map, roomType, gameType, timeType);
                    game.GameOverLog += new BaseGame.GameOverLogEventHandle(LogMgr.LogFightAdd);
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

        public static BaseGame StartPVEGame(int roomId, List<IGamePlayer> players, int copyId, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int levelLimits)
        {
            try
            {
                PveInfo info = null;

                if (copyId == 0)
                {
                    info = PveInfoMgr.GetPveInfoByType(roomType, levelLimits);
                }
                else
                {
                    info = PveInfoMgr.GetPveInfoById(copyId);
                }
                if (info != null)
                {
                    PVEGame game = new PVEGame(m_gameId++, roomId, info, players, null, roomType, gameType, timeType, hardLevel);
                    game.GameOverLog += new BaseGame.GameOverLogEventHandle(LogMgr.LogFightAdd);
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
