using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections;
using System.Timers;
using System.Threading;
using Game.Server.GameObjects;
using System.Reflection;
using System.Collections.Specialized;
using Game.Server.GameUtils;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;
using System.Security.Cryptography;
using System.Configuration;

namespace Game.Server.Managers
{
    public sealed class WorldMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

        private static Dictionary<int, GamePlayer> m_players = new Dictionary<int,GamePlayer>();

        public static Scene _marryScene;

        public static Scene MarryScene
        {
            get {   return _marryScene; }
        }

        private static RSACryptoServiceProvider m_rsa;

        public static RSACryptoServiceProvider RsaCryptor
        {
            get {   return m_rsa;   }
        }

        public static bool Init()
        {
            bool result = false;
            try
            {
                m_rsa = new RSACryptoServiceProvider();
                m_rsa.FromXmlString(GameServer.Instance.Configuration.PrivateKey);
                m_players.Clear();
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    ServerInfo info = db.GetServiceSingle(GameServer.Instance.Configuration.ServerID);
                    if (info != null)
                    {
                        _marryScene = new Scene(info);
                        result = true;
                    }                    
                }                
            }
            catch (Exception e)
            {
                log.Error("WordMgr Init", e);                
            }
            return result;
        }

        public static bool AddPlayer(int playerId, GamePlayer player)
        {
            m_clientLocker.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    return false;
                }
                else
                {
                    m_players.Add(playerId, player);
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            return true;
        }

        public static bool RemovePlayer(int playerId)
        {
            m_clientLocker.AcquireWriterLock(Timeout.Infinite);
            GamePlayer player = null;
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    player = m_players[playerId];
                    m_players.Remove(playerId);
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            if (player != null)
            {
                GameServer.Instance.LoginServer.SendUserOffline(playerId, player.PlayerCharacter.ConsortiaID);
                return true;
            }
            return false;
        }

        public static GamePlayer GetPlayerById(int playerId)
        {
            GamePlayer player = null;
            m_clientLocker.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    player = m_players[playerId];
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return player;
        }

        public static GamePlayer GetClientByPlayerNickName(string nickName)
        {
            GamePlayer[] list = GetAllPlayers();
            foreach (GamePlayer client in list)
            {
                if (client.PlayerCharacter.NickName == nickName)
                    return client;
            }
            return null;
        }

        public static GamePlayer[] GetAllPlayers()
        {
            List<GamePlayer> list = new List<GamePlayer>();

            m_clientLocker.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (GamePlayer p in m_players.Values)
                {
                    if (p == null || p.PlayerCharacter == null)
                        continue;

                    list.Add(p);
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return list.ToArray();
        }

        public static GamePlayer[] GetAllPlayersNoGame()
        {
            List<GamePlayer> list = new List<GamePlayer>(); ;

            m_clientLocker.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (GamePlayer p in GetAllPlayers())
                {
                    if (p.CurrentRoom == null)
                        list.Add(p);
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }

            return list.ToArray();
        }

        public static string GetPlayerStringByPlayerNickName(string nickName)
        {
            GamePlayer[] list = GetAllPlayers();
            foreach (GamePlayer client in list)
            {
                if (client.PlayerCharacter.NickName == nickName)
                    return client.ToString();
            }
            return nickName + " is not online!";
        }
        public static string DisconnectPlayerByName(string nickName)
        {
            GamePlayer[] list = GetAllPlayers();
            foreach (GamePlayer client in list)
            {
                if (client.PlayerCharacter.NickName == nickName)
                {
                    client.Disconnect();
                    return "OK";
                }
            }
            return nickName + " is not online!";
        }

        /// <summary>
        /// 用户下线,遍历所有用户,有用户是其好友，则通知其状态改变
        /// </summary>
        /// <param name="playerid"></param>
        public static void OnPlayerOffline(int playerid, int consortiaID)
        {
            ChangePlayerState(playerid, false, consortiaID);
        }

        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="playerid"></param>
        public static void OnPlayerOnline(int playerid, int consortiaID)
        {
            ChangePlayerState(playerid, true, consortiaID);
        }

        public static void ChangePlayerState(int playerID, bool state, int consortiaID)
        {
            GSPacketIn pkg = null;
            GamePlayer[] list = GetAllPlayers();
            foreach (GamePlayer client in list)
            {
                if ((client.Friends != null && client.Friends.ContainsKey(playerID) && client.Friends[playerID] == 0) || (client.PlayerCharacter.ConsortiaID != 0 && client.PlayerCharacter.ConsortiaID == consortiaID))
                {
                    if (pkg == null)
                    {
                        pkg = client.Out.SendFriendState(playerID, state);
                    }
                    else
                    {
                        client.Out.SendTCP(pkg);
                    }
                }
            }
        }
    }
}
