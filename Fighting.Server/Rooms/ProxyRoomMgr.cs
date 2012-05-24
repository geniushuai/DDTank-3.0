using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Game.Logic;
using Fighting.Server.Guild;
using Fighting.Server.Games;
using Bussiness.Managers;

namespace Fighting.Server.Rooms
{
    public class ProxyRoomMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly int THREAD_INTERVAL = 40; // 40ms

        public static readonly int PICK_UP_INTERVAL = 10 * 1000; // 10 s

        public static readonly int CLEAR_ROOM_INTERVAL = 1000; // 5 mins

        private static bool m_running = false;

        private static int m_serverId = 1;

        private static Queue<IAction> m_actionQueue = new Queue<IAction>();

        private static Thread m_thread;

        private static Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

        private static int RoomIndex = 0;

        #region Setup/Start/Stop

        public static bool Setup()
        {
            m_thread = new Thread(new ThreadStart(RoomThread));

            return true;
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


        #endregion

        #region Actions RoomThead/ExecuteActions/PickUpRooms/ClearRooms

        public static void AddAction(IAction action)
        {
            lock (m_actionQueue)
            {
                m_actionQueue.Enqueue(action);
            }
        }

        private static long m_nextPickTick = 0;
        private static long m_nextClearTick = 0;

        private static void RoomThread()
        {
            long balance = 0;
            m_nextClearTick = TickHelper.GetTickCount();
            m_nextPickTick = TickHelper.GetTickCount();
            while (m_running)
            {
                long start = TickHelper.GetTickCount();
                try
                {
                    ExecuteActions();

                    if (m_nextPickTick <= start)
                    {
                        m_nextPickTick += PICK_UP_INTERVAL;
                        PickUpRooms(start);
                    }

                    if (m_nextClearTick <= start)
                    {
                        m_nextClearTick += CLEAR_ROOM_INTERVAL;
                        ClearRooms(start);
                    }
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

        private static void ExecuteActions()
        {
            IAction[] actions = null;

            lock (m_actionQueue)
            {
                if (m_actionQueue.Count > 0)
                {
                    actions = new IAction[m_actionQueue.Count];
                    m_actionQueue.CopyTo(actions, 0);
                    m_actionQueue.Clear();
                }
            }

            if (actions != null)
            {
                foreach (IAction ac in actions)
                {
                    try
                    {
                        ac.Execute();
                    }
                    catch (Exception ex)
                    {
                        log.Error("RoomMgr execute action error:", ex);
                    }
                }
            }
        }

        private static void PickUpRooms(long tick)
        {
            List<ProxyRoom> rooms = GetWaitMatchRoomUnsafe();

            foreach (ProxyRoom red in rooms)
            {
                int maxScore = int.MinValue;
                ProxyRoom matchRoom = null;
                if (red.IsPlaying)
                    break;

                if (red.GameType == eGameType.ALL)
                {
                    foreach (ProxyRoom blue in rooms)
                    {
                        if (blue.GuildId != 0 && blue.GuildId == red.GuildId)
                            continue;

                        if (blue != red && !blue.IsPlaying && blue.PlayerCount == red.PlayerCount)
                        {
                            int guildRelation = GuildMgr.FindGuildRelationShip(red.GuildId, blue.GuildId) + 1;
                            int gameType = (int)blue.GameType;
                            int level = Math.Abs(red.AvgLevel - blue.AvgLevel);
                            int Property = Math.Abs(red.FightPower - blue.FightPower);
                            int score = guildRelation * 10000 + gameType * 1000 + Property + level;

                            if (score > maxScore)
                            {
                                matchRoom = blue;
                            }
                        }
                    }
                }
                else if (red.GameType == eGameType.Guild)
                {
                    foreach (ProxyRoom blue in rooms)
                    {
                        if (blue.GuildId != 0 && blue.GuildId == red.GuildId)
                            continue;

                        if (blue != red && blue.GameType != eGameType.Free && blue.IsPlaying == false && blue.PlayerCount == red.PlayerCount)
                        {
                            int guildRelation = GuildMgr.FindGuildRelationShip(red.GuildId, blue.GuildId) + 1;
                            int gameType = (int)blue.GameType;
                            int Property = Math.Abs(red.FightPower - blue.FightPower);
                            int level = Math.Abs(red.AvgLevel - blue.AvgLevel);

                            int score = guildRelation * 10000 + gameType * 1000 + Property + level;

                            if (score > maxScore)
                            {
                                matchRoom = blue;
                            }
                        }
                    }
                }
                else
                {
                    foreach (ProxyRoom blue in rooms)
                    {
                        //TrieuLSL ngan khoong cho cung guild choi
                        //if (blue.GuildId != 0 && blue.GuildId == red.GuildId)
                        //    continue;

                        if (blue != red && blue.GameType != eGameType.Guild && blue.IsPlaying == false && blue.PlayerCount == red.PlayerCount)
                        {
                            int gameType = (int)blue.GameType;
                            int level = Math.Abs(red.AvgLevel - blue.AvgLevel);
                            int Property = Math.Abs(red.FightPower - blue.FightPower);
                            int score = gameType * 1000 + Property + level;

                            if (score > maxScore)
                            {
                                matchRoom = blue;
                            }
                        }
                    }
                }

                if (matchRoom != null)
                    StartMatchGame(red, matchRoom);
            }
        }

        private static void ClearRooms(long tick)
        {
            List<ProxyRoom> list = new List<ProxyRoom>();

            foreach (ProxyRoom rm in m_rooms.Values)
            {
                if (rm.IsPlaying == false && rm.Game != null)
                {
                    list.Add(rm);
                }
            }

            foreach (ProxyRoom rm in list)
            {
                m_rooms.Remove(rm.RoomId);
                try
                {
                    rm.Dispose();
                }
                catch (Exception ex)
                {
                    log.Error("Room dispose error:", ex);
                }
            }
        }

        private static void StartMatchGame(ProxyRoom red, ProxyRoom blue)
        {
            int mapId = MapMgr.GetMapIndex(0, (byte)eRoomType.Match, m_serverId);

            eGameType gameType = eGameType.Free;//= red.GuildId != 0 && blue.GuildId != 0 ? eGameType.Guild : eGameType.Free;
            if (red.GameType == blue.GameType)
            {
                gameType = red.GameType;
            }
            BaseGame game = GameMgr.StartBattleGame(red.GetPlayers(), red, blue.GetPlayers(), blue, mapId, eRoomType.Match, gameType, 2);
            if (game != null)
            {
                blue.StartGame(game);
                red.StartGame(game);
            }
            if (game.GameType == eGameType.Guild)
            {
                red.Client.SendConsortiaAlly(red.GetPlayers()[0].PlayerCharacter.ConsortiaID, blue.GetPlayers()[0].PlayerCharacter.ConsortiaID, game.Id);
            }
        }

        #endregion

        public static bool AddRoomUnsafe(ProxyRoom room)
        {
            if (!m_rooms.ContainsKey(room.RoomId))
            {
                m_rooms.Add(room.RoomId, room);
                return true;
            }
            return false;
        }

        public static bool RemoveRoomUnsafe(int roomId)
        {
            if (m_rooms.ContainsKey(roomId))
            {
                m_rooms.Remove(roomId);
                return true;
            }
            return false;
        }

        public static ProxyRoom GetRoomUnsafe(int roomId)
        {
            if (m_rooms.ContainsKey(roomId))
            {
                return m_rooms[roomId];
            }
            return null;
        }

        public static ProxyRoom[] GetAllRoom()
        {
            lock (m_rooms)
            {
                return GetAllRoomUnsafe();
            }
        }

        public static ProxyRoom[] GetAllRoomUnsafe()
        {
            ProxyRoom[] list = new ProxyRoom[m_rooms.Values.Count];

            m_rooms.Values.CopyTo(list, 0);

            return list;
        }

        public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
        {
            List<ProxyRoom> list = new List<ProxyRoom>();

            foreach (ProxyRoom room in m_rooms.Values)
            {
                if (room.IsPlaying == false && room.Game == null)
                {
                    list.Add(room);
                }
            }

            return list;
        }

        public static int NextRoomId()
        {
            return Interlocked.Increment(ref RoomIndex);
        }

        public static void AddRoom(ProxyRoom room)
        {
            AddAction(new AddRoomAction(room));
        }

        public static void RemoveRoom(ProxyRoom room)
        {
            AddAction(new RemoveRoomAction(room));
        }
    }
}
