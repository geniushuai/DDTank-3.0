using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using Game.Server.GameObjects;
using System.Threading;
using Game.Base.Packets;
using Game.Server.SceneGames;

namespace Game.Server.GameUtils
{
    /// <summary>
    /// A class for a scene
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Database entity for scene.
        /// </summary>
        protected ServerInfo _info;

        /// <summary>
        /// Returns the database entity for the scene.
        /// </summary>
        public ServerInfo Info
        {
            get
            {
                return _info;
            }
        }

        /// <summary>
        /// Sysnc the players dictionay
        /// </summary>
        protected ReaderWriterLock _locker;
        
        /// <summary>
        /// Holds the players in the scene.
        /// </summary>
        protected Dictionary<int, GamePlayer> _players;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        public Scene(ServerInfo info)
        {
            _info = info;
            _info.Online = 0;
            _info.State = 2;
            _players = new Dictionary<int, GamePlayer>();
            _locker = new ReaderWriterLock();
        }

        /// <summary>
        /// Adds a player into the scene.
        /// </summary>
        /// <param name="player"></param>
        public bool AddPlayer(GamePlayer player)
        {
            _locker.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_players.ContainsKey(player.PlayerCharacter.ID))
                {
                    return true;
                }
                else
                {
                    _players.Add(player.PlayerCharacter.ID, player);
                    player.CurrentScene = this;
                    //_info.Online++;
                    return true;
                }
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Removes a player from the scene.
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(GamePlayer player)
        {
            _locker.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_players.ContainsKey(player.PlayerCharacter.ID))
                    _players.Remove(player.PlayerCharacter.ID);
                //_info.Online--;
                player.CurrentScene = null;
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }

            //通知客户端、场景用有人离开
            GamePlayer[] list = GetAllPlayer();
            GSPacketIn pkg = null;
            foreach (GamePlayer p in list)
            {
                if (pkg == null)
                {
                    pkg = p.Out.SendSceneRemovePlayer(player);
                }
                else
                {
                    p.Out.SendTCP(pkg);
                }
            }
        }

        /// <summary>
        /// Returns a array contains all the players in the scene.
        /// </summary>
        /// <returns></returns>
        public GamePlayer[] GetAllPlayer()
        {
            GamePlayer[] list = null;
            _locker.AcquireReaderLock(Timeout.Infinite);
            try
            {
                list = _players.Values.ToArray();
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
            return list == null ? new GamePlayer[0] : list;
        }

        public GamePlayer GetClientFromID(int id)
        {
            try
            {
                if (_players.Keys.Contains<int>(id))
                {
                    return _players[id];
                }
            }
            finally { }
            return null;

        }


        public void SendToALL(GSPacketIn pkg)
        {
            SendToALL(pkg, null);
        }

        public void SendToALL(GSPacketIn pkg,GamePlayer except)
        {
            GamePlayer[] list = GetAllPlayer();
            foreach (GamePlayer p in list)
            {
                if (p != except)
                {
                    p.Out.SendTCP(pkg);
                }
            }
        }

       
    }
}
