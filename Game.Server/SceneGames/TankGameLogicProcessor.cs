using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using System.Collections;
using log4net;
using Game.Server.Spells;
using System.Reflection;
using Game.Server.SceneGames.TankHandle;
using Game.Server.Managers;
using Phy.Object;
using System.Drawing;

namespace Game.Server.SceneGames
{
    [GameProcessor((byte)eGameType.TANK, "坦克游戏逻辑")]
    public class TankGameLogicProcessor : AbstractGameProcessor
    {

        public TankGameLogicProcessor()
        {
            _commandMgr = new CommandMgr();
        }

        private CommandMgr _commandMgr;
        private ThreadSafeRandom random = new ThreadSafeRandom();
        public  readonly int TIMEOUT = 1 * 60 * 1000;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override int MaxPlayerCount
        {
            get { return 8; }
        }

        public override void InitGame(BaseSceneGame game)
        {
            base.InitGame(game);

            game.Data = new TankData();

            GamePlayer[] Players = game.GetAllPlayers();

            foreach (GamePlayer p in Players)
            {
                game.Data.Players.Add(p, new Player(p));
            }
        }

        public override void OnPlayerStateChanged(BaseSceneGame game, GamePlayer player)
        {
            base.OnPlayerStateChanged(game, player);

            if (player.CurrentGameState == ePlayerGameState.FINISH && game.GameState == eGameState.LOAD)
                CanStartGame(game);
        }

        public override void OnPlayerTeamChanged(BaseSceneGame game, GamePlayer player)
        {
            base.OnPlayerTeamChanged(game, player);
        }

        public override bool OnCanStartGame(BaseSceneGame game, GamePlayer player)
        {
            base.OnCanStartGame(game, player);

            //int readyCount = game.GetReadyPlayerCount();
            //if (readyCount != game.Count - 1 || game.Count < 1)
            //    return false;

            //GamePlayer[] players = game.GetAllPlayers();
            //int rand = random.Next(players.Length);
            //List<int> list = new List<int>();
     
            //list.Clear();
            //int count = 0;
            //foreach (GamePlayer p in players)
            //{
            //    if (p.CurrentInventory.GetItemAt(6) == null)
            //    {
            //        p.CurrentGame.Player.Out.SendMessage(eMessageType.Normal, p.PlayerCharacter.NickName + LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
            //        return false;
            //    }

            //    if (p.CurrentTeamIndex != 0)
            //    {
            //        list.Add(p.CurrentTeamIndex);
            //    }
            //    else
            //    {
            //        count++;
            //    }
            //}

            //if (list.Count < 2)
            //    return false;

            //list.Sort();

            //int k = 0;
            //int t = 0;
            //for (int i = 0; i < list.Count - 1; i++)
            //{

            //    if (list[i] == list[i + 1])
            //        k++;
            //    else
            //    {
            //        if (k != t && t != 0)
            //            return false;
            //        else
            //        {
            //            t = k;
            //            k = 0;
            //        }
            //    }
            //}

            //if (t != k)
            //    return false;

            //if (count > 0 && count != t)
            //    return false;

            return true;
        }

        public override bool OnCanStartPairUpGame(BaseSceneGame game, GamePlayer player)
        {
            base.OnCanStartPairUpGame(game, player);

            int readyCount = game.GetReadyPlayerCount();
            if (readyCount != game.Count - 1)
                return false;

  
            GamePlayer[] players = game.GetAllPlayers();

            foreach (GamePlayer p in players)
            {

                
                if (p.CurrentInventory.GetItemAt(6) == null)
                {
                    p.CurrentGame.Player.Out.SendMessage(eMessageType.Normal, p.PlayerCharacter.NickName + LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                    return false;
                }

            }

            return true;
        }

        public override void OnTick(BaseSceneGame game)
        {
            base.OnTick(game);

            try
            {
                if (game.GameState == eGameState.LOAD)
                {
                    GamePlayer[] keys = game.Data.Players.Keys.ToArray();
                    foreach (GamePlayer p in keys)
                    {
                        if (p.CurrentGameState == ePlayerGameState.FINISH || game.GameState != eGameState.LOAD)
                            continue;

                        game.RemovePlayer(p);
                        if (game.MatchGame != null)
                        {
                            game.MatchGame.RemovePlayer(p);
                        }
                        //p.CurrentGame.RemovePlayer(p);
                        p.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickMsg1"));
                    }
                }
                else if (game.GameState == eGameState.PLAY)
                {
                    GamePlayer player = game.Data.CurrentIndex;
                    game.RemovePlayer(player);
                    if (game.MatchGame != null)
                    {
                        game.MatchGame.RemovePlayer(player);
                    }
                    player.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickMsg2"));

                }
                else if (game.GameState == eGameState.OVER)
                {
                    if (game.MatchGame != null)
                    {
                        OnShowArk(game.MatchGame, null);
                    }
                    OnShowArk(game, null);

                }
                else if (game.GameState == eGameState.FREE)
                {
                    GamePlayer player = game.Player;
                    if (player != null && player.CurrentGame != null && player.CurrentGame.ID == game.ID)
                    {
                        game.RemovePlayer(player);
                        if (game.MatchGame != null)
                        {
                            game.MatchGame.RemovePlayer(player);
                        }
                        //player.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickMsg3"));
                    }

                }
                else if (game.GameState == eGameState.PAIRUP)
                {
                    PairUpMgr.RemovefromPairUpMgr(game);

                    game.SendPairUpFailed();
                    GSPacketIn msg = game.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUpTimeOut"));
                    game.SendToPlayerExceptSelf(msg, game.Player);
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("GameNumber:" + game.ID + ", OnTick is Error: {0}", e.ToString()));
            }
        }

        private void CanStartGame(BaseSceneGame game)
        {
            if (game.Data.Count > 0)
            {
                int Count = game.Data.GetFinishPlayerCount();
                if (Count == game.Data.Count)
                {
                    game.Start();
                }
            }
        }

        public override bool CanStopGame(BaseSceneGame game, TankData data)
        {
            //if (game.GameState != eGameState.LOAD && game.GameState != eGameState.PLAY)
            //    return false;

            //List<int> leave = new List<int>();

            //foreach (KeyValuePair<GamePlayer, Player> p in data.Players)
            //{
            //    if (p.Value.Blood > 0)
            //    {
            //        if (p.Key.CurrentTeamIndex == 0)
            //            leave.Add(p.Key.CurrentTeamIndex);
            //        else if (!leave.Contains(p.Key.CurrentTeamIndex))
            //        {
            //            if (game.GameMode == eGameMode.FLAG && game.GameState != eGameState.LOAD)
            //            {
            //                if (p.Value.IsCaptain)
            //                    leave.Add(p.Key.CurrentTeamIndex);
            //            }
            //            else
            //            {
            //                leave.Add(p.Key.CurrentTeamIndex);
            //            }
            //        }
            //    }

            //    if (leave.Count > 1)
            //        return false;

            //}

            //if (leave.Count < 2)
            //{                
            //    game.Stop();
            //    return true;
            //}
            return false;
        }

        public override void OnStarting(BaseSceneGame game, Game.Base.Packets.GSPacketIn data)
        {
            //base.OnStarting(game, data);

            TankData tank = game.Data;
            tank.Reset();
            GamePlayer[] list = game.Data.GetAllPlayers();
            int i = random.Next(list.Length);
            GamePlayer player = GetLowDelayPlayer(game, list[i]);
            //List<Point> pos = new List<Point>();
            //pos.AddRange(Managers.MapMgr.GetMapPos(game.Data.MapIndex));

            MapPoint pos = Managers.MapMgr.GetMapRandomPos(game.Data.MapIndex);

            tank.CurrentWind = random.Next(-40, 40);
            data.WriteInt(player.PlayerCharacter.ID);
            data.WriteInt(tank.CurrentWind);
            data.WriteByte((byte)game.Data.Count);
            Point temp = new Point();
            int lastTeam = -1;
            foreach (GamePlayer p in list)
            {
                temp = GetRandom(pos, temp, lastTeam, p.CurrentTeamIndex);
                tank.Players[p].X = temp.X;
                tank.Players[p].Y = temp.Y;
                tank.CurrentMap.AddPhysical(tank.Players[p]);
                p.CanFly = true;
                p.Count = 2;

                data.WriteInt(p.PlayerCharacter.ID);
                data.WriteInt(tank.Players[p].X);
                data.WriteInt(tank.Players[p].Y);
                data.WriteInt(random.Next(0, 2) > 0 ? 1 : -1);
                data.WriteInt(tank.Players[p].Blood);
                data.WriteBoolean(game.Data.Players[p].IsCaptain);
                lastTeam = p.CurrentTeamIndex;
            }
            tank.CurrentIndex = player;
            game.BeginTimer(TIMEOUT);
            if (game.RoomType == eRoomType.PAIRUP && game.MatchGame != null)
            {
                game.MatchGame.StopTimer();
            }
        }

        public Point GetRandom(MapPoint mapPoint, Point temp, int lastTeam, int team)
        {
            List<Point> list = team == 1 ? mapPoint.PosX : mapPoint.PosX1;
            int rand = random.Next(list.Count);
            Point pos = list[rand];
            //if (lastTeam != -1 && team != lastTeam)
            //{
            //    int len = pos.X - temp.X;
            //    if (Math.Abs(len) < 500)
            //    {
            //        pos = list[Math.Abs(rand + list.Count / 2 - len / 100) % list.Count];
            //    }
            //}

            list.Remove(pos);
            return pos;
        }

        public override void OnStarted(BaseSceneGame game)
        {
            base.OnStarted(game);
        }

        public override void OnGameData(BaseSceneGame game, GamePlayer player, GSPacketIn packet)
        {
            TankCmdType type = (TankCmdType)packet.ReadByte();
            try
            {
                if (game.GameState == eGameState.OVER && type != TankCmdType.PICK)
                    return;

                ICommandHandler handleCommand = _commandMgr.LoadCommandHandler((int)type);
                if (handleCommand != null)
                {
                    handleCommand.HandleCommand(this, player, packet);
                }
                else
                {
                    log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
                }
            }
            catch(Exception e)
            {
                log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(),player.Client.TcpEndpoint));
            }
        }

        public double GetBiasLenght(int x, int y)
        {
            return Math.Sqrt((double)(x * x + y * y));
        }

        public void SendArk(BaseSceneGame game, GamePlayer player)
        {
            int number = 0;
            int leave = 0;
            int total = game.Data.Players.Count + 2;
            List<MapGoodsInfo> list = new List<MapGoodsInfo>();
            if (game.Data.TotalHeathPoint > 0)
            {
                game.Data.Players[player].HitNum++;
                //if (game.Data.TotalHeathPoint > 300 || game.RoomType == eRoomType.PAIRUP)
                //{
                    number = random.Next(1, 3);

                    if (game.Data.Arks + number > total)
                    {
                        number = total - game.Data.Arks;
                    }
                    if (number > 0)
                    {
                        list.AddRange(Managers.MapMgr.GetRandomGoodsByNumber(game.Data.MapIndex, number,(int)game.Data.MapType));
                    }
                //}
                //else
                //{
                //    leave = random.Next(1, 3);
                //}
            }

            int dead = game.Data.GetDeadCount();
            if (dead > 1)
            {
                leave += random.Next(dead);
            }
            if (game.Data.Arks + number + leave > total)
            {
                leave = total - game.Data.Arks - number;
            }

            if (leave > 0)
            {
                Managers.MapMgr.GetRandomFightPropByCount(game.Data.MapIndex, leave, list);
            }

            if (list.Count < 1)
                return;

            //MapGoodsInfo[] list = Managers.MapMgr.GetRandomGoodsByNumber(game.Data.MapIndex, number);
            Point[] point = game.Data.GetArkPoint(list.Count);
            int count = list.Count > point.Length ? point.Length : list.Count;
            for (int i = 0; i < count; i++)
            {
                //if (game.Data.TotalHeathPoint <= 0 && (list[i].GoodsID < 10000 || list[i].GoodsID > 10999))
                //    continue;
                game.Data.AddFallGoods(point[i], list[i]);
            }

        }

        public override void SendPlayFinish(BaseSceneGame game, GamePlayer player)
        {
            //game.BeginTimer(TIMEOUT);

            //List<int> BoxIDs = game.Data.DestroyBox();
            //if (BoxIDs.Count != 0)
            //{
            //    SendBoxDisappear(BoxIDs, game);
            //}

            //if (game.Data.IsDead != -1)
            //{
            //    if (CanStopGame(game, game.Data))
            //        return;
            //}

            //if (player.CanFly == false)
            //{
            //    if ((--player.Count) == 0)
            //    {
            //        player.CanFly = true;
            //        player.Count = 2;
            //    }
            //}
            
            //player.OnEndFitting();
            //game.Data.CurrentFire = null;
            ////game.Data.Players[player].Delay = (int)(game.Data.TotalDelay * player.BaseAgility);
            //game.Data.Players[player].Delay = game.Data.TotalDelay ;
            //GamePlayer nextPlayer = GetLowDelayPlayer(game, player);
            //if (nextPlayer != null)
            //{
            //    game.Data.CurrentIndex = nextPlayer;
            //    GSPacketIn pkg = nextPlayer.Out.SendPlayerTurn(nextPlayer,game);
            //    game.SendToPlayerExceptSelf(pkg, nextPlayer);
            //    GSPacketIn pkgWind = nextPlayer.Out.SendPlayerWind(nextPlayer);
            //    game.SendToPlayerExceptSelf(pkgWind, nextPlayer);
            //}
        }

        public void SendNextFire(BaseSceneGame game, GamePlayer player)
        {
            GSPacketIn pkgMsg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkgMsg.WriteByte((byte)TankCmdType.PLAYFINISH);
            pkgMsg.WriteInt(game.Data.TurnNum);
            player.CurrentGame.SendToAll(pkgMsg);
        }

        public void SendBoxDisappear(List<int> BoxID, BaseSceneGame game)
        {
            GSPacketIn pkgMsg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkgMsg.WriteByte((byte)TankCmdType.DISAPPEAR);
            pkgMsg.WriteInt(BoxID.Count);
            foreach(int id in BoxID)
            {
                pkgMsg.WriteInt(id);
            }
            
            game.SendToAll(pkgMsg);
        }

        public override void SendPairUpWait(BaseSceneGame game)
        {
            GSPacketIn pkgMsg = new GSPacketIn((byte)ePackageType.GAME_PAIRUP_WAIT);

            game.SendToAll(pkgMsg);
        }

        public override void SendPairUpFailed(BaseSceneGame game)
        {
            GSPacketIn pkgMsg = new GSPacketIn((byte)ePackageType.GAME_PAIRUP_FAILED);

            game.SendToAll(pkgMsg);
        }

        private GamePlayer GetLowDelayPlayer(BaseSceneGame game, GamePlayer player)
        {
            Player temp = game.Data.Players[player];
            Player first = temp;
            foreach (Player p in game.Data.Players.Values)
            {
                if (p.Blood > 0 && temp.Delay >= p.Delay && first != p)
                {
                    temp = p;
                }
            }

            temp.PlayerDetail.OnBeginFitting();
            //temp.Delay = (int)(560 * temp.PlayerDetail.BaseAgility);
            //temp.Delay = 1000 - player.PlayerCharacter.Agility / 2;
            temp.Delay = 1600 - temp.PlayerDetail.PlayerCharacter.Agility / 2;
            if (temp.IsFrost > 0 || temp.Blood <= 0)
            {
                //玩家被冰冻后，单回合delay计算为1600+500+武器普通炸弹delay-敏捷/2
                //好吧，反正也就差个几十delay，统一用100吧
                //temp.Delay = 1600 - player.PlayerCharacter.Agility / 2;
                temp.Delay = 600;
                return GetLowDelayPlayer(game, temp.PlayerDetail);
            }

            return temp.PlayerDetail;
        }

        public override void OnStopping(BaseSceneGame game, Game.Base.Packets.GSPacketIn pkg)
        {
            base.OnStopping(game, pkg);
            game.BeginTimer(TIMEOUT / 2);

            TankData data = game.Data;
            pkg.WriteByte((byte)data.Players.Count);

            int winTeam = -1;
            int lastTeam = -1;
            data.killFlag = false;

            foreach (KeyValuePair<GamePlayer, Player> p in data.Players)
            {
                //if(p.Value.TotalHurt > 0)
                //{
                //    data.killFlag = true;
                //}

                if (winTeam == -1 && p.Value.Blood > 0)
                {
                    if (game.GameMode == eGameMode.FLAG)
                    {
                        if (p.Value.IsCaptain)
                        {
                            winTeam = p.Key.CurrentTeamIndex;
                            break;
                        }
                        else                        
                        {
                            lastTeam = p.Key.CurrentTeamIndex;
                        }
                    }
                    else
                    {
                        winTeam = p.Key.CurrentTeamIndex;
                        break;
                    }
                }
            }

            //if (!data.killFlag)
            //{
            //    game.BeginTimer(3000);
            //}

            if (winTeam == -1 && game.Data.CurrentIndex == null)
            {
                winTeam = lastTeam;
            }

            if (winTeam == -1)
            {
                if (game.Data.CurrentFire != null)
                    winTeam = game.Data.CurrentFire.CurrentTeamIndex;
                else
                    winTeam = data.LastDead;
            }

            int GP;

            //E等级差=（对方平均等级+5）-玩家等级；取值区间（1~9）；
            //            P赢=4*[（对方平均等级+5）-玩家等级]+伤害的血量*0.2%+击杀数*2+（命中次数/玩家总的回合次数）*8；
            //P输=1*[（对方平均等级+5）-玩家等级]+伤害的血量*0.2%+击杀数*2+（命中次数/玩家总的回合次数）*8


            //            P赢=[2+伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*[（对方平均等级+10）-玩家等级]
            //P输=[伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*[（对方平均等级+10）-玩家等级]；

//            P赢=[2+伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[1+(总击杀数-1)*33%]
//P输=[伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[1+(总击杀数-1)*33%]

            //P赢=[2+伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[0.9+(游戏开始时对方玩家人数-1)*30%]
            //P输=[伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[0.9+(游戏开始时对方玩家人数-1)*30%]


            //bool flag = data.GameStart.AddMinutes(1).CompareTo(DateTime.Now) > 0;
            string winStr = LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5");
            GamePlayer winPlayer = null;
            string loseStr = LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5");
            GamePlayer losePlayer = null;

            foreach (KeyValuePair<GamePlayer, Player> p in data.Players)
            {

               
                if (game.RoomType == 0)
                {
                    p.Key.PlayerCharacter.CheckCount++;
                }
                else 
                {
                    p.Key.PlayerCharacter.CheckCount += 2;
                       
                }
                     p.Key.Out.SendCheckCode();

                if (p.Value.TotalHurt > 0)
                {
                    data.killFlag = true;
                }

                if (game.GameClass == eGameClass.CONSORTIA)
                {
                    if (p.Key.CurrentTeamIndex == winTeam)
                    {
                        winStr += " [" + p.Key.PlayerCharacter.NickName + "] ";
                        winPlayer = p.Key;
                    }
                    else
                    {
                        loseStr += " [" + p.Key.PlayerCharacter.NickName + "] ";
                        losePlayer = p.Key;
                    }
                }
            }

            int riches = 0;
            if (game.GameClass == eGameClass.CONSORTIA)
            {
                winStr += LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1") + losePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2");
                loseStr += LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3") + winPlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4");
                riches = ConsortiaMgr.ConsortiaFight(winPlayer.CurrentTeamIndex == 1 ? data.ConsortiaID1 : data.ConsortiaID2,
                    winPlayer.CurrentTeamIndex == 1 ? data.ConsortiaID2 : data.ConsortiaID1, game.Data.Players, game.RoomType, game.GameClass,data.persons[winPlayer.CurrentTeamIndex].TotalKillHealth);
                GameServer.Instance.LoginServer.SendConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, riches, winStr);
                //GameServer.Instance.LoginServer.SendConsortiaFight(losePlayer.PlayerCharacter.ConsortiaID, -riches, loseStr);

            }

            foreach (KeyValuePair<GamePlayer, Player> p in data.Players)
            {
                GP = 1;
                if (p.Value.State != TankGameState.LOSE)
                {
                    if (game.RoomType == eRoomType.PAIRUP)
                        p.Key.PlayerCharacter.Total++;

                    double level = (data.TotalLevel - data.persons[p.Key.CurrentTeamIndex].TotalLevel) / (data.TotalPerson - data.persons[p.Key.CurrentTeamIndex].TeamPerson);
                    double disLevel = level + 10 - p.Key.PlayerCharacter.Grade;

                    if (p.Key.CurrentTeamIndex == winTeam)
                    {
                        if (game.RoomType == eRoomType.PAIRUP)
                            p.Key.PlayerCharacter.Win++;

                        //GP = (int)Math.Ceiling((((data.killFlag ? 2 : 0) + (double)p.Value.TotalHurt * 0.001 + p.Value.TotalKill * 0.5 + (p.Value.HitNum / (p.Value.BoutNum == 0 ? 1 : p.Value.BoutNum)) * 2) * level * (1 + (data.persons[p.Key.CurrentTeamIndex].TatolKill - 1) * 0.33)));
                        GP = (int)Math.Ceiling((((data.killFlag ? 2 : 0) + (double)p.Value.TotalHurt * 0.001 + p.Value.TotalKill * 0.5 + (p.Value.HitNum / (p.Value.BoutNum == 0 ? 1 : p.Value.BoutNum)) * 2) * level * (0.9 + (data.persons[p.Key.CurrentTeamIndex].TeamPerson - 1) * 0.3)));

                    }
                    else
                    {
                        GP = (int)Math.Ceiling(((double)p.Value.TotalHurt * 0.001 + p.Value.TotalKill * 0.5 + (p.Value.HitNum / (p.Value.BoutNum == 0 ? 1 : p.Value.BoutNum)) * 2) * level * (0.9 + (data.persons[p.Key.CurrentTeamIndex].TeamPerson - 1) * 0.3));
                    }

                    bool isMarried = false;
                    if(p.Key.PlayerCharacter.IsMarried)
                    {
                        foreach (GamePlayer q in data.Players.Keys)
                        {
                            if (q.PlayerCharacter.ID != p.Key.PlayerCharacter.ID)
                            {
                                if (q.CurrentTeamIndex == p.Key.CurrentTeamIndex && p.Key.PlayerCharacter.SpouseID == q.PlayerCharacter.ID)
                                {
                                    GP = (int)(GP * 1.2);
                                    isMarried = true;
                                }
                            }
                        }
                    }

                    p.Key.QuestInventory.CheckWin(data.MapIndex, (int)game.GameMode, game.ScanTime, p.Value.IsCaptain,
                             data.persons[p.Key.CurrentTeamIndex].TeamPerson, data.TotalPerson - data.persons[p.Key.CurrentTeamIndex].TeamPerson, p.Key.CurrentTeamIndex == winTeam, game.GameClass == eGameClass.CONSORTIA, (int)game.RoomType, isMarried);


                    double AAS = AntiAddictionMgr.GetAntiAddictionCoefficient(p.Key.PlayerCharacter.AntiAddiction);
                    GP = (int)(GP * AAS);

					GP = (int)(GP * RateMgr.GetRate(eRateType.Experience_Rate));
                    GP *= p.Key.BuffInventory.GPMultiple();

                    if (game.RoomType != eRoomType.PAIRUP && (disLevel <= 0 || disLevel >= 20))
                    {
                        GP = 1;
                    }

                    if (AAS < 10E-6)
                    {
                        GP = 0;
                    }
                    else
                    {
                        if (GP < 1)
                            GP = 1;
                    }

                    p.Key.SetGP(GP);

                    Dictionary<int, int> requestItems = p.Key.QuestInventory.GetRequestItems();
                    List<MapGoodsInfo> questItems = Managers.MapMgr.GetQuestGoodsAll(data.MapIndex);
                    foreach (MapGoodsInfo questID in questItems)
                    {
                        if (requestItems.ContainsKey(questID.GoodsID) && requestItems[questID.GoodsID] > 0)
                        {
                            int total = random.Next(questID.Value + 1);
                            if (p.Key.CurrentTeamIndex != winTeam)
                            {
                                total = total / 2;
                            }
                            else if (total < 1)
                            {
                                total = 1;
                            }
                            if (total < 1)
                                continue;

                            int count = requestItems[questID.GoodsID] > total ? total : requestItems[questID.GoodsID];
                            ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.GetSingleGoods(questID.GoodsID);
                            p.Key.TempInventory.AddItemTemplate(temp, questID, count);
                            requestItems[questID.GoodsID] -= count;
                        }
                    }
                }

                //p.Value.IsTakeOut = true;
                pkg.WriteInt(p.Key.PlayerCharacter.ID);
                pkg.WriteBoolean(winTeam == p.Key.CurrentTeamIndex);
                pkg.WriteInt(p.Key.PlayerCharacter.Grade);
                pkg.WriteInt(p.Key.PlayerCharacter.GP);
                pkg.WriteInt((int)p.Value.TotalKill);
                pkg.WriteInt(p.Value.TotalHurt);
                pkg.WriteInt(GP);
                pkg.WriteInt((int)p.Value.HitNum);
                pkg.WriteInt((int)p.Value.BoutNum);
                pkg.WriteInt(p.Value.Offer);

                if (data.persons[p.Key.CurrentTeamIndex].TotalKillHealth > 0)
                {
                    p.Value.IsTakeOut = true;
                    if (p.Key.CurrentGame != null)
                        p.Key.CurrentGame.IsTakeOut = true;
                }
                pkg.WriteBoolean(p.Value.IsTakeOut);
            }

            pkg.WriteInt(riches);
            //pkg.WriteBoolean(data.killFlag);
            pkg.WriteInt(data.TotalPerson / 2);

            data.InsertGameInfo(game, winTeam);

            GamePlayer[] list = data.Players.Keys.ToArray();
            foreach (GamePlayer p in list)
            {
                if (data.Players[p].State == TankGameState.LOSE)
                    data.Players.Remove(p);
                else
                {
                    //data.Players[p].Reset();
                    //p.PropInventory.Clear();
                    //if (!killFlag)
                    //{
                    //    if (p.CurrentGame.GameState != eGameState.FREE)
                    //    {
                    //        p.CurrentGame.ShowArk(p.CurrentGame, p);
                    //    }
                    //}
                }
            }
        }

        public override void OnAddedPlayer(BaseSceneGame game, GamePlayer player)
        {
            base.OnAddedPlayer(game, player);
            //TankData data = game.Data;
            //data.Players.Add(player, new Player(player));
        }

        public override void OnRemovedPlayer(BaseSceneGame game, GamePlayer player)
        {
            base.OnRemovedPlayer(game, player);
            
            TankData data = game.Data;
            if(data == null)
            {
                if (game.GameState == eGameState.FREE || game.GameState == eGameState.PAIRUP || game.GameState == eGameState.OVER)
                {
                    if (game.Count == 0)
                    {
                        game.StopTimer();

                        if (game.RoomType == eRoomType.PAIRUP)
                        {
                            PairUpMgr.RemovefromPairUpMgr(game);
                        }
                        else
                        {
                            game.GameState = eGameState.FREE;
                        }
                    }
                }

                return;
            }

            if (!data.Players.ContainsKey(player))
            {
                return;
            }

            //if (game.GameState == eGameState.PLAY || game.GameState == eGameState.LOAD)
            //{
            //    int disLevel = (data.TotalLevel - data.persons[player.CurrentTeamIndex].TotalLevel) / (data.TotalPerson - game.Data.persons[player.CurrentTeamIndex].TeamPerson) - data.persons[player.CurrentTeamIndex].AveLevel;
            //    int GP = (int)(20 * (1 - data.Players[player].TotalKill * 0.1 - disLevel * 0.1) * 0.8);

            //    if (GP > 1 && player.PlayerCharacter.Grade > 10)
            //        player.SetGP(-GP);
            //}

            //data.Players[player].State = TankGameState.LOSE;
            data.Players[player].Lose();

            if (game.GameState == eGameState.LOAD && player.CurrentGameState != ePlayerGameState.FINISH)
            {
                CanStartGame(game);
            }

            if (game.GameState == eGameState.OVER)
            {
                byte index = data.GetCards();
                if (data.Players[player].IsTakeOut)
                {
                    GSPacketIn pkg = new GSPacketIn((int)ePackageType.GAME_TAKE_OUT, player.PlayerCharacter.ID);
                    pkg.WriteByte(index);
                    ThreadSafeRandom rand = new ThreadSafeRandom();
                    int gold = data.GetRandomGold(game.RoomType);
                    player.SetGold(gold, Game.Server.Statics.GoldAddType.Cards);
                    pkg.WriteByte(1);
                    pkg.WriteInt(gold);
                    player.Out.SendTCP(pkg);
                }
            }

            if (!CanStopGame(game, data) && game.GameState == eGameState.PLAY)
            {
                if (game.RoomType == eRoomType.PAIRUP)
                {
                    player.PlayerCharacter.Escape++;
                    player.PlayerCharacter.Total++;
                }

                if (player == data.CurrentIndex && game.Data.Count > 0)
                    SendPlayFinish(game, player);
                //else if (game.Data.Count > 0)
                //{
                //    int Count = game.Data.GetPlayFinishCount();
                //    if (Count == game.Data.Count)
                //        SendPlayFinish(game, player);
                //}
            }


            if (game.GameState == eGameState.FREE || game.GameState == eGameState.PAIRUP || game.GameState == eGameState.OVER)
            {
                data.Players.Remove(player);
            }

            if (game.GameState == eGameState.OVER && 8 - game.Data.ArkCount == game.Data.Count)
            {
                OnShowArk(game, player);
            }

            if (game.Data.Count == 0)
            {
                game.StopTimer();
                game.GameState = eGameState.FREE;
                game.Data.Players.Clear();

                if (game.RoomType == eRoomType.PAIRUP)
                {
                    PairUpMgr.RemovefromPairUpMgr(game);
                    if(game.MatchGame != null)
                    {
                        game.MatchGame.StopTimer();
                        game.MatchGame.GameState = eGameState.FREE;
                        PairUpMgr.RemovefromPairUpMgr(game.MatchGame);
                    }

                    PairUpMgr.PairUpGameLeave(game);
                }
                //else
                //{
                //    //game.Data = null;
                //}
                               
            }

        }

        public override void OnShowArk(BaseSceneGame game, GamePlayer player)
        {
            game.Data._fallItemID.Clear();

            game.GameState = eGameState.FREE;
            game.BeginTimer(TIMEOUT * 5);

            if (game.RoomType == eRoomType.PAIRUP)
            {
                PairUpMgr.PairUpGameLeave(game);
            }
        }

    }

}
