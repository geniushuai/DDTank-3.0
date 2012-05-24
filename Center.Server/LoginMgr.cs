using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;

namespace Center.Server
{
    public class LoginMgr
    {
        private static Dictionary<int, Player> m_players = new Dictionary<int, Player>();

        private static object syc_obj = new object();

        public static void CreatePlayer(Player player)
        {
            Player older = null;
            lock (syc_obj)
            {
                player.LastTime = DateTime.Now.Ticks;
                if (m_players.ContainsKey(player.Id))
                {
                    older = m_players[player.Id];
                    player.State = older.State;
                    player.CurrentServer = older.CurrentServer;
                    m_players[player.Id] = player;
                }
                else
                {

                    older = LoginMgr.GetPlayerByName(player.Name);
                    if (older != null && m_players.ContainsKey(older.Id))
                    {
                        m_players.Remove(older.Id);
                    }
                    player.State = ePlayerState.NotLogin;
                    m_players.Add(player.Id, player);
                }
            }
            if (older != null && older.CurrentServer != null)
            {
                older.CurrentServer.SendKitoffUser(older.Id);
            }
        }

        public static bool TryLoginPlayer(int id, ServerClient server)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(id))
                {
                    Player player = m_players[id];
                    if (player.CurrentServer == null)
                    {
                        player.CurrentServer = server;
                        player.State = ePlayerState.Logining;
                        return true;
                    }
                    else
                    {
                        if (player.State == ePlayerState.Play)
                        {
                            player.CurrentServer.SendKitoffUser(id);
                        }
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static void PlayerLogined(int id, ServerClient server)
        {
            lock (syc_obj)
            {
                if (!m_players.ContainsKey(id))
                    return;

                Player player = m_players[id];
                if (player != null)
                {
                    player.CurrentServer = server;
                    player.State = ePlayerState.Play;
                }
            }
        }

        public static void PlayerLoginOut(int id, ServerClient server)
        {
            lock (syc_obj)
            {
                if (!m_players.ContainsKey(id))
                    return;

                Player player = m_players[id];
                if (player != null && player.CurrentServer == server)
                {
                    player.CurrentServer = null;
                    player.State = ePlayerState.NotLogin;
                }
            }
        }

        public static Player GetPlayerByName(string name)
        {
            Player[] list = GetAllPlayer();
            if (list != null)
            {
                foreach (Player p in list)
                {
                    if (p.Name == name)
                    {
                        return p;
                    }
                }
            }
            return null;
        }

        public static Player[] GetAllPlayer()
        {
            lock (syc_obj)
            {
               return m_players.Values.ToArray();
            }
        }

        public static void RemovePlayer(int playerId)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(playerId))
                {
                    m_players.Remove(playerId);
                }
            }
        }

        public static void RemovePlayer(List<Player> players)
        {
            lock (syc_obj)
            {
                foreach (Player p in players)
                {
                    m_players.Remove(p.Id);
                }
            }
        }

        public static Player GetPlayer(int playerId)
        {
            lock (syc_obj)
            {
                if (m_players.ContainsKey(playerId))
                    return m_players[playerId];
            }
            return null;
        }

        public static ServerClient GetServerClient(int playerId)
        {
            Player player = GetPlayer(playerId);
            if (player != null)
            {
                return player.CurrentServer;
            }
            return null;
        }

        public static int GetOnlineCount()
        {
            Player[] list = GetAllPlayer();
            int count = 0;
            foreach(Player p in list)
            {
                if(p.State != ePlayerState.NotLogin)
                {
                    count ++;
                }
            }
            return count;
        }

        public static Dictionary<int, int> GetOnlineForLine()
        {
            Dictionary<int, int> lines = new Dictionary<int, int>();
            Player[] list = GetAllPlayer();
            foreach (Player p in list)
            {
                if (p.CurrentServer == null)
                    continue;

                if (lines.ContainsKey(p.CurrentServer.Info.ID))
                {
                    lines[p.CurrentServer.Info.ID]++;
                }
                else
                {
                    lines.Add(p.CurrentServer.Info.ID, 1);
                }
            }

            return lines;
        }

        public static List<Player> GetServerPlayers(ServerClient server)
        {
            List<Player> list = new List<Player>();
            Player[] players = GetAllPlayer();
            foreach (Player p in players)
            {
                if (p.CurrentServer == server)
                {
                    list.Add(p);
                }
            }
            return list;
        }

    }
}
