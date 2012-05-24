using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using Game.Logic;
using Game.Base.Packets;
using Game.Logic.Protocol;
using Game.Logic.Phy.Object;
using Fighting.Server.GameObjects;

namespace Fighting.Server.Rooms
{
    public class ProxyRoom
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<IGamePlayer> m_players;

        private int m_roomId;

        private int m_orientRoomId;

        private ServerClient m_client;

        public int RoomId
        {
            get { return m_roomId; }
        }

        public ServerClient Client
        {
            get { return m_client; }
        }

        public ProxyRoom(int roomId, int orientRoomId, IGamePlayer[] players, ServerClient client)
        {
            m_roomId = roomId;
            m_orientRoomId = orientRoomId;
            m_players = new List<IGamePlayer>();
            m_players.AddRange(players);
            m_client = client;
           // GetBaseProperty();
        }

        public void SendToAll(GSPacketIn pkg)
        {
            SendToAll(pkg, null);
        }

        public void SendToAll(GSPacketIn pkg, IGamePlayer except)
        {
            m_client.SendToRoom(m_orientRoomId, pkg, except);
        }

        public int PlayerCount
        {
            get { return m_players.Count; }
        }

        public List<IGamePlayer> GetPlayers()
        {
            List<IGamePlayer> list = new List<IGamePlayer>();
            lock (m_players)
            {
                list.AddRange(m_players);
            }
            return list;
        }

        public bool RemovePlayer(IGamePlayer player)
        {
            bool result = false;

            lock (m_players)
            {
                if (m_players.Remove(player))
                {
                    result = true;
                }
            }
            if (PlayerCount == 0)
            {
                ProxyRoomMgr.RemoveRoom(this);
            }
            return result;
        }

        public bool IsPlaying;

        public eGameType GameType;

        public int GuildId;

        public string GuildName;

        public int AvgLevel;

        public int FightPower = 0;

        private BaseGame m_game;

        public BaseGame Game
        {
            get { return m_game; }
        }

        public void StartGame(BaseGame game)
        {
            IsPlaying = true;
            m_game = game;
            game.GameStopped += new GameEventHandle(game_GameStopped);
            m_client.SendStartGame(m_orientRoomId, game);

        }

        void game_GameStopped(AbstractGame game)
        {
            m_game.GameStopped -= game_GameStopped;
            IsPlaying = false;
            m_client.SendStopGame(m_orientRoomId, m_game.Id);
        }

        public void Dispose()
        {
            m_client.RemoveRoom(m_orientRoomId, this);
        }

        public override string ToString()
        {
            return string.Format("RoomId:{0} OriendId:{1} PlayerCount:{2},IsPlaying:{3},GuildId:{4},GuildName:{5}", m_roomId, m_orientRoomId, m_players.Count, IsPlaying, GuildId, GuildName);
        }
    }
}
