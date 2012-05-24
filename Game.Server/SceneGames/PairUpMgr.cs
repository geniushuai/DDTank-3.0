using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using System.Reflection;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Phy.Object;
using Game.Server.Managers;

namespace Game.Server.SceneGames
{
    class PairUpMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, BaseSceneGame> _pairUpList;
        private static object _synclock = new object();
        private static Timer _pairUpTimer;

        public static int AddtoPairUpMgrforTimer(BaseSceneGame game)
        {
            lock (_synclock)
            {
                if (_pairUpList.ContainsKey(game.ID))
                {
                    return -1;
                }

                _pairUpList.Add(game.ID, game);
            }

            game.GameState = eGameState.PAIRUP;
            game.SendRoomInfo();
            game.SendPairUpWait();
            return 1;

        }

        public static BaseSceneGame[] GetPairUpList()
        {
            BaseSceneGame[] list = null;

            lock (_synclock)
            {
                list = _pairUpList.Values.ToArray<BaseSceneGame>();
            }

            return list == null ? new BaseSceneGame[0] : list;

        }

        public static void MatchGame()
        {
            //List<BaseSceneGame> tempList = new List<BaseSceneGame>();
            //lock (_synclock)
            //{
            //    foreach (BaseSceneGame game in _pairUpList.Values)
            //    {
            //        tempList.Add(game);
            //    }
            //}

            BaseSceneGame[] tempList = GetPairUpList();

            foreach (BaseSceneGame game in tempList)
            {
                if (game.GameState != eGameState.PAIRUP || game.pairUpState != 0)
                {
                    continue;
                }

                game.pairUpState = 2;

                BaseSceneGame matchGame1 = null;
                BaseSceneGame matchGame2 = null;
                BaseSceneGame matchGame3 = null;
                BaseSceneGame matchGame4 = null;
                BaseSceneGame matchGame5 = null;


                foreach (BaseSceneGame g in tempList)
                {
                    if (game.GameState != eGameState.PAIRUP || g.pairUpState != 0)
                    {
                        continue;
                    }

                    if(game.GameClass == eGameClass.FREE_OR_CONSORTIA)
                    {
                        if ( g.Count == game.Count && g.GameMode == game.GameMode)
                        {
                            if (g.GameClass == eGameClass.CONSORTIA)
                            {
                                if (2 == ConsortiaMgr.FindConsortiaAlly(game.ConsortiaID, g.ConsortiaID))
                                {
                                    matchGame1 = g;
                                    //matchGame1.GameClass = eGameClass.CONSORTIA;
                                    game.GameClass = eGameClass.CONSORTIA;
                                    
                                }
                                else if (0 == ConsortiaMgr.FindConsortiaAlly(game.ConsortiaID, g.ConsortiaID))
                                {
                                    matchGame2 = g;
                                    //matchGame2.GameClass = eGameClass.CONSORTIA;
                                    game.GameClass = eGameClass.CONSORTIA;
                                }
                            }
                            else if(g.GameClass == eGameClass.FREE_OR_CONSORTIA)
                            {
                                if (2 == ConsortiaMgr.FindConsortiaAlly(game.ConsortiaID, g.ConsortiaID))
                                {
                                    matchGame1 = g;
                                    //matchGame1.GameClass = eGameClass.CONSORTIA;
                                    game.GameClass = eGameClass.CONSORTIA;

                                }
                                else if (0 == ConsortiaMgr.FindConsortiaAlly(game.ConsortiaID, g.ConsortiaID))
                                {
                                    matchGame2 = g;
                                    //matchGame2.GameClass = eGameClass.CONSORTIA;
                                    game.GameClass = eGameClass.CONSORTIA;
                                }
                                else if (game.ConsortiaID != g.ConsortiaID)
                                {
                                    int level = Math.Abs(game.AverageLevel() - g.AverageLevel());
                                    //平均等级小于10级
                                    if (level < 5)
                                    {
                                        matchGame3 = g;
                                        game.GameClass = eGameClass.FREE;
                                    }
                                    else if (level < 10)
                                    {
                                        matchGame4 = g;
                                        game.GameClass = eGameClass.FREE;
                                    }
                                    else
                                    {
                                        matchGame5 = g;
                                        game.GameClass = eGameClass.FREE;
                                    }
                                }
                            }
                            else
                            {
                                if (game.ConsortiaID != g.ConsortiaID)
                                {
                                    int level = Math.Abs(game.AverageLevel() - g.AverageLevel());
                                    //平均等级小于10级
                                    if (level < 5)
                                    {
                                        matchGame3 = g;
                                        game.GameClass = eGameClass.FREE;
                                    }
                                    else if (level < 10)
                                    {
                                        matchGame4 = g;
                                        game.GameClass = eGameClass.FREE;
                                    }
                                    else
                                    {
                                        matchGame5 = g;
                                        game.GameClass = eGameClass.FREE;
                                    }
                                }
                            }
                            
                        }

                    }                        
                    else if (game.GameClass == eGameClass.CONSORTIA)
                    {
                        if (g.GameClass == eGameClass.CONSORTIA || g.GameClass == eGameClass.FREE_OR_CONSORTIA)
                        {
                            if( g.Count == game.Count && g.GameMode == game.GameMode)
                            {
                                if (2 == ConsortiaMgr.FindConsortiaAlly(game.ConsortiaID, g.ConsortiaID))
                                {
                                    matchGame1 = g;
                                    //matchGame1.GameClass = eGameClass.CONSORTIA;
                                }
                                else if (0 == ConsortiaMgr.FindConsortiaAlly(game.ConsortiaID, g.ConsortiaID))
                                {
                                    matchGame2 = g;
                                    //matchGame2.GameClass = eGameClass.CONSORTIA;
                                }
                            }
                        }
                    }
                    else if (game.GameClass == eGameClass.FREE)
                    {
                        if (g.GameClass == eGameClass.FREE)
                        {
                            if (g.Count == game.Count && g.GameMode == game.GameMode)
                            {
                                if (game.ConsortiaID != 0 || g.ConsortiaID != 0)
                                {
                                    if (game.ConsortiaID != g.ConsortiaID)
                                    {
                                        int level = Math.Abs(game.AverageLevel() - g.AverageLevel());
                                        //平均等级小于10级
                                        if (level < 5)
                                        {
                                            matchGame1 = g;
                                        }
                                        else if (level < 10)
                                        {
                                            matchGame2 = g;
                                        }
                                        else
                                        {
                                            matchGame3 = g;
                                        }
                                    }
                                }
                                else
                                {
                                    int level = Math.Abs(game.AverageLevel() - g.AverageLevel());
                                    //平均等级小于10级
                                    if (level < 5)
                                    {
                                        matchGame1 = g;
                                    }
                                    else if (level < 10)
                                    {
                                        matchGame2 = g;
                                    }
                                    else
                                    {
                                        matchGame3 = g;
                                    }
                                }
                            }
                        }
                    }

                }

                if (matchGame1 != null)
                {
                    //matchGame.StopTimer();
                    //_pairUpList.Remove(matchGame.ID);
                    matchGame1.pairUpState = 1;
                    game.pairUpState = 1;
                    if (matchGame1.GameClass == eGameClass.FREE_OR_CONSORTIA)
                    {
                        matchGame1.GameClass = game.GameClass;
                    }
                    game.MatchGame = matchGame1;
                    matchGame1.MatchGame = game;

                    PairUpGameEntry(game);
                    game.BeginPairUpLoad();
                }
                else if(matchGame2 != null)
                {
                    matchGame2.pairUpState = 1;
                    game.pairUpState = 1;
                    if (matchGame2.GameClass == eGameClass.FREE_OR_CONSORTIA)
                    {
                        matchGame2.GameClass = game.GameClass;
                    }
                    game.MatchGame = matchGame2;
                    matchGame2.MatchGame = game;

                    PairUpGameEntry(game);
                    game.BeginPairUpLoad();
                }
                else if (matchGame3 != null)
                {
                    matchGame3.pairUpState = 1;
                    game.pairUpState = 1;
                    if (matchGame3.GameClass == eGameClass.FREE_OR_CONSORTIA)
                    {
                        matchGame3.GameClass = game.GameClass;
                    }
                    game.MatchGame = matchGame3;
                    matchGame3.MatchGame = game;

                    PairUpGameEntry(game);
                    game.BeginPairUpLoad();
                }
                else if (matchGame4 != null)
                {
                    matchGame4.pairUpState = 1;
                    game.pairUpState = 1;
                    if (matchGame4.GameClass == eGameClass.FREE_OR_CONSORTIA)
                    {
                        matchGame4.GameClass = game.GameClass;
                    }
                    game.MatchGame = matchGame4;
                    matchGame4.MatchGame = game;

                    PairUpGameEntry(game);
                    game.BeginPairUpLoad();
                }
                else if (matchGame5 != null)
                {
                    matchGame5.pairUpState = 1;
                    game.pairUpState = 1;
                    if (matchGame5.GameClass == eGameClass.FREE_OR_CONSORTIA)
                    {
                        matchGame5.GameClass = game.GameClass;
                    }
                    game.MatchGame = matchGame5;
                    matchGame5.MatchGame = game;

                    PairUpGameEntry(game);
                    game.BeginPairUpLoad();
                }
                else
                {
                    game.pairUpState = 0;

                }
            }

            //List<BaseSceneGame> list = _pairUpList.Values.ToList();
            BaseSceneGame[] list = GetPairUpList();
            lock (_synclock)
            {
                foreach (BaseSceneGame game in list)
                {
                    if (game.pairUpState != 0)
                    {
                        game.pairUpState = 0;
                        _pairUpList.Remove(game.ID);
                    }
                }
            }

        
    }

        public static void RemovefromPairUpMgr(BaseSceneGame game)
        {
            lock (_synclock)
            {
                if (_pairUpList.ContainsKey(game.ID))
                {
                    _pairUpList.Remove(game.ID);
                }
            }
            
            game.GameState = eGameState.FREE;
                //game.StopTimer();
            game.BeginTimer(5 * 60 * 1000);
        }

        public static int PairUpGameEntry(BaseSceneGame game)
        {
            InitPairUpGame(game);

            GamePlayer[] SecondPlayers = game.MatchGame.GetAllPlayers();

            foreach (GamePlayer p in SecondPlayers)
            {
                GamePlayer[] list = game.GetAllPlayers();
                foreach (GamePlayer d in list)
                {
                    if (d != p)
                    {
                        d.Out.SendGamePlayerEnter(p, true);
                        p.Out.SendGamePlayerEnter(d, true);
                        p.Out.SendAllBuff(d);
                        d.Out.SendAllBuff(p);
                    }
                }
            }

            return 0;
          }

        public static void PairUpGameLeave(BaseSceneGame game)
        {
            GamePlayer[] Players = game.GetAllPlayers();

            foreach (GamePlayer p in Players)
            {
                //p.CurrentTeamIndex = 1;
                p.ResetTeamInfo();
            }

            game.OperateGameClass();

            game.MatchGame = null;
        }

        public static void InitPairUpGame(BaseSceneGame game)
        {
            TankData baseData = new TankData();

            GamePlayer[] firstPlayers = game.GetAllPlayers();

            foreach (GamePlayer p in firstPlayers)
            {
                p.CurrentTeamIndex = 1;
                baseData.Players.Add(p, new Player(p));
            }

            GamePlayer[] secondPlayers = game.MatchGame.GetAllPlayers();

            foreach (GamePlayer p in secondPlayers)
            {
                p.CurrentTeamIndex = 2;
                baseData.Players.Add(p, new Player(p));
            }

            if(game.GameClass == eGameClass.CONSORTIA)
            {
                baseData.ConsortiaID1 = game.ConsortiaID;
                baseData.ConsortiaID2 = game.MatchGame.ConsortiaID;
            }

            baseData.StartedGameClass = (int)game.GameClass;

            game.Data = baseData;

            game.MatchGame.Data = baseData;
        }

        public static bool Init()
        {
            try
            {
                //lock(_synclock)
                //{
                    _pairUpList = new Dictionary<int, BaseSceneGame>();
                    _pairUpTimer = new Timer(new TimerCallback(PairUpTimerProc), null, 30 * 1000, 30 * 1000);
                  
                    return true;
                //}
 
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("PairUpMgr", e);
                return false;
            }

        }

        private static void PairUpTimerProc(object sender)
        {
            try 
            {
                 MatchGame();
            }
            catch(Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("PairUpMgr", e);
            }
            
        }

    }
}
