using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Rooms;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Battle
{
    public class BattleServer
    {
        private int m_serverId;

        private FightServerConnector m_server;

        public FightServerConnector Server
        {
            get { return m_server; }
        }

        private Dictionary<int, BaseRoom> m_rooms;

        private string m_ip;

        private int m_port;

        public BattleServer(int serverId, string ip, int port, string loginKey)
        {
            m_serverId = serverId;
            m_ip = ip;
            m_port = port;
            m_server = new FightServerConnector(this, ip, port, loginKey);
            m_rooms = new Dictionary<int, BaseRoom>();
            m_server.Disconnected += new Game.Base.ClientEventHandle(m_server_Disconnected);
            m_server.Connected += new Game.Base.ClientEventHandle(m_server_Connected);
        }

        public bool IsActive
        {
            get { return m_server.IsConnected; }
        }

        public string Ip
        {
            get { return m_ip; }
        }

        public int Port
        {
            get { return m_port; }
        }

        public void Start()
        {
            m_server.Connect();
        }

        void m_server_Connected(Game.Base.BaseClient client)
        {

        }

        void m_server_Disconnected(Game.Base.BaseClient client)
        {

        }

        public BaseRoom FindRoom(int roomId)
        {
            BaseRoom room = null;

            lock (m_rooms)
            {
                if (m_rooms.ContainsKey(roomId))
                {
                    room = m_rooms[roomId];
                }
            }

            return room;
        }


        public bool AddRoom(Game.Server.Rooms.BaseRoom room)
        {
            bool result = false;

            lock (m_rooms)
            {
                if (!m_rooms.ContainsKey(room.RoomId))
                {
                    m_rooms.Add(room.RoomId, room);
                    result = true;
                }
            }

            if (result)
            {
                m_server.SendAddRoom(room);
            }
            return result;
        }

        public bool RemoveRoom(BaseRoom room)
        {
            bool result = false;

            lock (m_rooms)
            {
                result = m_rooms.ContainsKey(room.RoomId);
            }

            if (result)
            {
                m_server.SendRemoveRoom(room);
            }

            return result;
        }

        public void RemoveRoomImp(int roomId)
        {
            BaseRoom room = null;

            lock (m_rooms)
            {
                if (m_rooms.ContainsKey(roomId))
                {
                    room = m_rooms[roomId];
                    m_rooms.Remove(roomId);
                }
            }

            if (room != null)
            {
                if (room.IsPlaying && room.Game == null)
                {
                    RoomMgr.CancelPickup(this, room);
                }
                else
                {
                    RoomMgr.StopProxyGame(room);
                }
            }
        }

        public void StartGame(int roomId, ProxyGame game)
        {
            BaseRoom room = FindRoom(roomId);

            if (room != null)
            {
                RoomMgr.StartProxyGame(room, game);
            }
        }

        public void StopGame(int roomId, int gameId)
        {
            BaseRoom room = FindRoom(roomId);
            if (room != null)
            {
                RoomMgr.StopProxyGame(room);
                lock (m_rooms)
                {
                    m_rooms.Remove(roomId);
                }
            }
        }

        public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
        {
            BaseRoom room = FindRoom(roomId);
            if (room != null)
            {

                if (exceptId != 0)
                {
                    GamePlayer player = WorldMgr.GetPlayerById(exceptId);
                    if (player != null)
                    {
                        if (player.GamePlayerId == exceptGameId)
                        {
                            room.SendToAll(pkg, player);
                        }
                        else
                        {
                            room.SendToAll(pkg);
                        }
                    }
                }
                else
                {
                    room.SendToAll(pkg);
                }
            }
        }

        public void SendToUser(int playerid, GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(playerid);
            if (player != null)
            {
                player.SendTCP(pkg);
            }
        }

        public void UpdatePlayerGameId(int playerid, int gamePlayerId)
        {
            GamePlayer player = WorldMgr.GetPlayerById(playerid);
            if (player != null)
            {
                player.GamePlayerId = gamePlayerId;
            }
        }

        public override string ToString()
        {
            return string.Format("ServerID:{0},Ip:{1},Port:{2},IsConnected:{3},RoomCount:{4}", m_serverId, m_server.RemoteEP.Address, m_server.RemoteEP.Port, m_server.IsConnected, m_rooms.Count);
        }
    }
}
