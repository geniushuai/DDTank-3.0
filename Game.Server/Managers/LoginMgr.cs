using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Bussiness;

namespace Game.Server.Managers
{

    public class LoginMgr
    {
        private static Dictionary<int, GameClient> _players = new Dictionary<int, GameClient>();

        private static object _locker = new object();

        public static void Add(int player, GameClient server)
        {
            GameClient temp = null;
            lock (_locker)
            {
                if (_players.ContainsKey(player))
                {
                    GameClient client = _players[player];
                    if (client != null)
                    {
                        temp = client;
                    }
                    _players[player] = server;
                }
                else
                {
                    _players.Add(player, server);
                }
            }

            if (temp != null)
            {
                temp.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
                temp.Disconnect();
            }
        }

        public static void Remove(int player)
        {
            lock (_locker)
            {
                if (_players.ContainsKey(player))
                {
                    _players.Remove(player);
                }
            }
        }

        public static GamePlayer LoginClient(int playerId)
        {
            GameClient client = null;
            lock (_locker)
            {
                if (_players.ContainsKey(playerId))
                {
                    client = _players[playerId];
                    _players.Remove(playerId);
                }
            }
            if (client != null)
            {
                return client.Player;
            }
            return null;
        }

        public static void ClearLoginPlayer(int playerId)
        {
            GameClient client = null;
            lock (_locker)
            {
                if (_players.ContainsKey(playerId))
                {
                    client = _players[playerId];
                    _players.Remove(playerId);
                }
            }
            if (client != null)
            {
                client.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
                client.Disconnect();
            }
        }

        public static void ClearLoginPlayer(int playerId, GameClient client)
        {
            lock (_locker)
            {
                if (_players.ContainsKey(playerId) && _players[playerId] == client)
                {
                    _players.Remove(playerId);
                }
            }
        }

        public static bool ContainsUser(int playerId)
        {
            lock (_locker)
            {
                if (_players.ContainsKey(playerId) && _players[playerId].IsConnected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public static bool ContainsUser(string account)
        {
            lock (_locker)
            {
                foreach (GameClient client in _players.Values)
                {
                    if (client != null && client.Player != null && client.Player.Account == account)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
