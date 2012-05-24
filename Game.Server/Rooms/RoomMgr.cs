using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using System.Reflection;
using System.Collections;
using Game.Server.GameObjects;
using System.Diagnostics;
using Game.Server.Managers;
using Game.Logic;
using Game.Base.Packets;
using Game.Server.Battle;

namespace Game.Server.Rooms
{
    public partial class RoomMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private static bool m_running;

        private static Queue<IAction> m_actionQueue;

        private static Thread m_thread;

        private static BaseRoom[] m_rooms;

        private static BaseWaitingRoom m_waitingRoom;

        /// <summary>
        /// 非线程安全
        /// </summary>
        public static BaseRoom[] Rooms
        {
            get
            {
                return m_rooms;
            }
        }

        public static BaseWaitingRoom WaitingRoom
        {
            get
            {
                return m_waitingRoom;
            }
        }

        public static bool Setup(int maxRoom)
        {
            maxRoom = maxRoom < 1 ? 1 : maxRoom;
            m_thread = new Thread(new ThreadStart(RoomThread));
            m_actionQueue = new Queue<IAction>();
            m_rooms = new BaseRoom[maxRoom];
            for (int i = 0; i < maxRoom; i++)
            {
                m_rooms[i] = new BaseRoom(i + 1);
            }
            m_waitingRoom = new BaseWaitingRoom();

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

        public static readonly int THREAD_INTERVAL = 40; // 40ms

        public static readonly int PICK_UP_INTERVAL = 10 * 1000; // 10 s

        public static readonly int CLEAR_ROOM_INTERVAL = 400;

        private static long m_clearTick = 0;

        private static void RoomThread()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            long balance = 0;
            m_clearTick = TickHelper.GetTickCount();
            while (m_running)
            {
                long start = TickHelper.GetTickCount();
                int count = 0;
                try
                {
                     count = ExecuteActions();
                    if (m_clearTick <= start)
                    {
                        m_clearTick += CLEAR_ROOM_INTERVAL;
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
                if (end - start > THREAD_INTERVAL * 2)
                {
                    log.WarnFormat("Room Mgr is spent too much times: {0} ms,count:{1}", end - start, count);
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

        private static int ExecuteActions()
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
                        long begin = TickHelper.GetTickCount();
                        ac.Execute();
                        long end = TickHelper.GetTickCount();
                        if (end - begin > 40)
                        {
                            log.WarnFormat("RoomMgr action spent too much times:{0},{1}ms!",ac.GetType(),end - begin);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("RoomMgr execute action error:", ex);
                    }
                }
                return actions.Length;
            }
            return 0;
        }

        public static void ClearRooms(long tick)
        {
            foreach (BaseRoom room in m_rooms)
            {
                if (room.IsUsing)
                {
                    if (room.PlayerCount == 0)
                    {
                        room.Stop();
                    }
                }
            }
        }

        public static void AddAction(IAction action)
        {
            lock (m_actionQueue)
            {
                m_actionQueue.Enqueue(action);
            }
        }

        public static void CreateRoom(GamePlayer player, string name, string password, eRoomType roomType, byte timeType)
        {
            AddAction(new CreateRoomAction(player, name, password, roomType, timeType));
        }

        public static void EnterRoom(GamePlayer player, int roomId, string pwd, int type)
        {
            AddAction(new EnterRoomAction(player, roomId, pwd, type));
        }

        public static void EnterRoom(GamePlayer player)
        {
            EnterRoom(player, -1, null, 1);
        }

        public static void ExitRoom(BaseRoom room, GamePlayer player)
        {
            AddAction(new ExitRoomAction(room, player));
        }

        public static void StartGame(BaseRoom room)
        {
            AddAction(new StartGameAction(room));
        }

        public static void UpdatePlayerState(GamePlayer player, byte state)
        {
            AddAction(new UpdatePlayerStateAction(player, player.CurrentRoom, state));
        }

        public static void UpdateRoomPos(BaseRoom room, int pos, bool isOpened)
        {
            AddAction(new UpdateRoomPosAction(room, pos, isOpened));
        }

        public static void KickPlayer(BaseRoom baseRoom, byte index)
        {
            AddAction(new KickPlayerAction(baseRoom, index));
        }

        public static void EnterWaitingRoom(GamePlayer player)
        {
            AddAction(new EnterWaitingRoomAction(player));
        }

        public static void ExitWaitingRoom(GamePlayer player)
        {
            AddAction(new ExitWaitRoomAction(player));
        }

        public static void CancelPickup(BattleServer server, BaseRoom room)
        {
            AddAction(new CancelPickupAction(server, room));
        }

        public static void UpdateRoomGameType(BaseRoom room, GSPacketIn packet, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId)
        {
            AddAction(new RoomSetupChangeAction(room, packet, roomType, timeMode, hardLevel, levelLimits, mapId));
        }

        internal static void SwitchTeam(GamePlayer gamePlayer)
        {
            AddAction(new SwitchTeamAction(gamePlayer));
        }

        public static List<BaseRoom> GetAllUsingRoom()
        {
            List<BaseRoom> list = new List<BaseRoom>();
            lock (m_rooms)
            {
                foreach (BaseRoom room in m_rooms)
                {
                    if (room.IsUsing)
                    {
                        list.Add(room);
                    }
                }
            }
            return list;
        }

        public static void StartProxyGame(BaseRoom room, ProxyGame game)
        {
            AddAction(new StartProxyGameAction(room, game));
        }

        public static void StopProxyGame(BaseRoom room)
        {
            AddAction(new StopProxyGameAction(room));
        }
    }
}
