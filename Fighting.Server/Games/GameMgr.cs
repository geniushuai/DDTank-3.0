using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Collections;
using System.Threading;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Fighting.Server.Rooms;
using Game.Logic.Phy.Object;
using Fighting.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Fighting.Server.Games
{
    public class GameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly long THREAD_INTERVAL = 40;

        private static Dictionary<int, BaseGame> m_games;

        private static Thread m_thread;

        private static bool m_running;

        private static int m_serverId;

        private static int m_boxBroadcastLevel;

        private static int m_gameId;

        public static bool Setup(int serverId, int boxBroadcastLevel)
        {
            m_thread = new Thread(new ThreadStart(GameThread));
            m_games = new Dictionary<int, BaseGame>();
            m_serverId = serverId;
            m_boxBroadcastLevel = boxBroadcastLevel;
            m_gameId = 0;

            return true;
        }

        public static int BoxBroadcastLevel
        {
            get { return m_boxBroadcastLevel; }
        }

        public static void Start()
        {
            if (m_running == false)
            {
                m_running = true;
                m_thread.Start();
            }
        }

        public static void Stop()
        {
            if (m_running)
            {
                m_running = false;
                m_thread.Join();
            }
        }


        private static void GameThread()
        {
            long balance = 0;
            m_clearGamesTimer = TickHelper.GetTickCount();
            while (m_running)
            {
                long start = TickHelper.GetTickCount();
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
                long end = TickHelper.GetTickCount();

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
            IList games = GetGames();
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
            if (m_clearGamesTimer <= tick)
            {
                m_clearGamesTimer += CLEAR_GAME_INTERVAL;

                ArrayList temp = new ArrayList();
                lock (m_games)
                {
                    foreach (BaseGame g in m_games.Values)
                    {
                        if (g.GameState == eGameState.Stopped)
                        {
                            temp.Add(g);
                        }
                    }

                    foreach (BaseGame g in temp)
                    {
                        m_games.Remove(g.Id);
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
            }
        }

        public static List<BaseGame> GetGames()
        {
            List<BaseGame> temp = new List<BaseGame>();
            lock (m_games)
            {
                temp.AddRange(m_games.Values);
            }

            return temp;
        }

        public static BaseGame FindGame(int id)
        {
            lock (m_games)
            {
                if (m_games.ContainsKey(id))
                {
                    return m_games[id];
                }
            }
            return null;
        }

        public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            try
            {
                int index = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
                Map map = MapMgr.CloneMap(index);

                if (map != null)
                {
                    PVPGame game = new PVPGame(m_gameId++, 0, red, blue, map, roomType, gameType, timeType);

                    lock (m_games)
                    {
                        m_games.Add(game.Id, game);
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

        public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            try
            {
                int index = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
                Map map = MapMgr.CloneMap(index);

                if (map != null)
                {
                    BattleGame game = new BattleGame(m_gameId++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType);

                    lock (m_games)
                    {
                        m_games.Add(game.Id, game);
                    }
                    game.Prepare();
                    SendStartMessage(game);
                    return game;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("Create battle game error:", e);
                return null;
            }
        }

        public static void SendStartMessage(BattleGame game)
        {
            Game.Base.Packets.GSPacketIn pkg = new Game.Base.Packets.GSPacketIn((byte)ePackageType.GAME_CHAT);
            pkg.WriteInt(2);
            if (game.GameType == eGameType.Free)
            {
                foreach (Player p in game.GetAllFightPlayers())
                {
                    (p.PlayerDetail as ProxyPlayer).Rate = 1;
                    GSPacketIn pkg1 = SendBufferList(p, (p.PlayerDetail as ProxyPlayer).Buffers);
                    game.SendToAll(pkg1);
                }
                pkg.WriteString("撮合成功！您所在的小队开始了自由战");
            }
            else
            {
                pkg.WriteString("撮合成功！您所在的小队开始了公会战");
            }
            game.SendToAll(pkg, null);
        }

        public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.BUFF_OBTAIN, player.Id);
            pkg.WriteInt(infos.Count);
            foreach (BufferInfo info in infos)
            {
                pkg.WriteInt(info.Type);
                pkg.WriteBoolean(info.IsExist);
                pkg.WriteDateTime(info.BeginDate);
                pkg.WriteInt(info.ValidDate);
                pkg.WriteInt(info.Value);
            }
            return pkg;
        }

    }
}
