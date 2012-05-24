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

namespace Game.Server.GameUtils
{
    /// <summary>
    /// A class for a scene
    /// </summary>
    public class Scene
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ReaderWriterLock _locker = new ReaderWriterLock();

        protected Dictionary<int, GamePlayer> _players = new Dictionary<int,GamePlayer>();

        public Scene(ServerInfo info)
        {
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
                //player.CurrentScene = this;
                if (_players.ContainsKey(player.PlayerCharacter.ID))
                {
                    _players[player.PlayerCharacter.ID] = player;
                    return true;
                }
                else
                {
                    _players.Add(player.PlayerCharacter.ID, player);
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
