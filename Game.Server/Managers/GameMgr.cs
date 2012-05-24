using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Util;
using Game.Server.SceneGames;
using System.Reflection;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Server.Managers
{
    public class GameMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static ReaderWriterLock _locker = new ReaderWriterLock();

        protected static BaseSceneGame[] _games;

        protected static TankGameLogicProcessor _processor = new TankGameLogicProcessor();

        public static bool Init()
        {
            int count = WorldMgr.WaitingScene.Info.Room;
            _games = new BaseSceneGame[count];
            for (int i = 0; i < count; i++)
            {
                _games[i] = new BaseSceneGame(i, _processor);
            }

            return true;
        }

        public static BaseSceneGame[] GetAllGame()
        {
            BaseSceneGame[] list = null;
            _locker.AcquireReaderLock();
            try
            {
                list = new BaseSceneGame[_games.Length];
                _games.CopyTo(list, 0);
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
            return list == null ? new BaseSceneGame[0] : list;
        }

        public static BaseSceneGame CreateGame(byte code, GamePlayer player, string roomName, string pwd, byte roomType, byte gameMode, byte timeType)
        {
            BaseSceneGame game = null;
            _locker.AcquireWriterLock();
            try
            {
                for (int i = 1; i < _games.Length; i++)
                {
                    if (_games[i].Count == 0 && !_games[i].IsUsed)
                    {
                        game = _games[i];
                        game.IsUsed = true;
                        break;
                    }
                }

            }
            finally
            {
                _locker.ReleaseWriterLock();
            }

            if (game != null)
            {
                try
                {

                    game.Player = player;
                    game.Name = roomName;
                    game.Pwd = pwd;
                    game.MapIndex = 0;
                    game.RoomType = (eRoomType)roomType;
                    game.GameMode = (eGameMode)gameMode;
                    game.ScanTime = timeType;

                    game.Revert();

                    if (game.AddPlayer(player))
                    {
                        //game.BeginTimer(60 * 1000 * 5);
                        return game;
                    }

                }
                finally
                {

                    _locker.AcquireWriterLock();
                    try
                    {
                        game.IsUsed = false;
                    }
                    finally
                    {
                        _locker.ReleaseWriterLock();
                    }
                }
            }

            return null;

        }

        public static BaseSceneGame GetGame(int id, string pwd,int playerConsortiaID,ref string msg)
        {
            BaseSceneGame game = null;
            _locker.AcquireReaderLock();
            try
            {
                if (id >= 0 && id < _games.Length)
                {
                    //if (_games[id].GameState == eGameState.FREE && _games[id].Pwd == pwd && _games[id].Count > 0 && _games[id].Count + _games[id].CloseTotal() < 8)
                    //    game = _games[id];

                    if (_games[id].GameState != eGameState.FREE)
                    {
                        msg = "Game.Server.Managers.GameStart";
                    }
                    else if (_games[id].Pwd != pwd)
                    {
                        msg = "Game.Server.Managers.PWDError";
                    }
                    //else if (_games[id].RoomType == eRoomType.PAIRUP && _games[id].GameClass == eGameClass.CONSORTIA && _games[id].ConsortiaID != playerConsortiaID)
                    //{
                    //    msg = "Game.Server.Managers.ConsortiaError";
                    //}
                    else if (_games[id].Count > 0 && _games[id].Count + _games[id].CloseTotal() < 8)
                    {
                        game = _games[id];
                    }
                }
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
            return game;
        }

        public static BaseSceneGame GetRandomGame(eGameMode type)
        {
            BaseSceneGame game = null;
            _locker.AcquireReaderLock();
            try
            {
                int rand = ThreadSafeRandom.NextStatic(200);
                for (int i = rand; i < _games.Length + rand; i++)
                {
                    BaseSceneGame g = _games[i % _games.Length];
                    if (g.GameState == eGameState.FREE && g.Count > 0 && g.Pwd == "" && g.Count + g.CloseTotal() < 8 && type == g.GameMode
                        && g.GameClass != eGameClass.CONSORTIA)
                    {
                        game = g;
                        break;
                    }
                }


                //for (int i = 0; i < _games.Length; i++)
                //{
                //    BaseSceneGame g = _games[i];
                //    if (g.GameState == eGameState.FREE && g.Count > 0 && g.Pwd == "" && g.Count + g.CloseTotal() < 8 && type == g.GameMode 
                //        && g.GameClass != eGameClass.CONSORTIA)
                //    {
                //        game = g;
                //        break;
                //    }
                //}
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
            return game;
        }
    }
}
