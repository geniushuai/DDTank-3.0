using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Phy.Object;
using SqlDataProvider.Data;
using System.Drawing;
using Phy.Maps;
using Game.Server.Managers;
using System.Collections;
using log4net;
using System.Reflection;
using Game.Server.Rooms;
using Game.Server.Packets;
using Bussiness;
using Bussiness.Managers;

namespace Game.Server.Games
{
    public enum eGameState
    {
        Inited,
        Prepared,
        Loading,
        Playing,
        GameOver,
        Stopped
    }

    public class BaseGame
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<GamePlayer, Player> m_players;

        private List<Player> m_redTeam;

        private Player m_redLeader;

        private float m_redAvgLevel;

        private List<Player> m_blueTeam;

        private Player m_blueLeader;

        private float m_blueAvgLevel;

        private Map m_map;

        private eRoomType m_roomType;

        private eTeamType m_teamType;

        private eGameType m_gameType;

        private eMapType m_mapType;

        private int m_timeType;

        private int m_turnIndex;

        private int m_wind;

        private ArrayList m_actions;

        public eGameState GameState;

        private Player m_currentPlayer;

        public int PhysicalId;

        private Random m_random;

        public int[] Cards;

       public int CurrentTurnTotalDamage = 0;

        public BaseGame(List<GamePlayer> red, List<GamePlayer> blue, Map map,eRoomType roomType,eTeamType teamType, eGameType gameType, int timeType)
        {
            m_players = new Dictionary<GamePlayer, Player>();
            m_redTeam = new List<Player>();
            m_blueTeam = new List<Player>();
            m_roomType = roomType;
            m_gameType = gameType;
            m_teamType = teamType;
            m_random = new Random();

            m_redAvgLevel = 0;
            foreach (GamePlayer player in red)
            {
                Player fp = new Player(player, this, 1);
                fp.Direction = m_random.Next(0, 1) == 0 ? 1 : -1;
                m_players.Add(player, fp);
                m_redTeam.Add(fp);
                player.CurrentGame = this;
                m_redAvgLevel += player.PlayerCharacter.Grade;

                fp.Died += new PlayerEventHandle(Player_Died);
            }
            m_redAvgLevel = m_redAvgLevel / m_redTeam.Count;

            m_blueAvgLevel = 0;
            foreach (GamePlayer player in blue)
            {
                Player fp = new Player(player, this, 2);
                fp.Direction = m_random.Next(0, 1) == 0 ? 1 : -1;
                m_players.Add(player, fp);
                m_blueTeam.Add(fp);
                player.CurrentGame = this;
                m_blueAvgLevel += player.PlayerCharacter.Grade;

                fp.Died += new PlayerEventHandle(Player_Died);
            }
            m_blueAvgLevel = m_blueAvgLevel / blue.Count;

            m_map = map;
            m_actions = new ArrayList();
            PhysicalId = 0;

            switch (m_roomType)
            {
                case eRoomType.Freedom:
                    m_mapType = eMapType.Normal;
                    break;
                case eRoomType.Match:
                    m_mapType = eMapType.PairUp;
                    break;
                default:
                    m_mapType = eMapType.Normal;
                    break;
            }
            
           
            m_timeType = timeType;
            m_tempBox = new List<Box>();
            m_tempPoints = new List<Point>();
            Cards = new int[8];
            GameState = eGameState.Inited;
        }

        private void Player_Died(Player player)
        {
            if (m_currentPlayer != null)
            {
                if (player != m_currentPlayer)
                    m_currentPlayer.TotalKill++;
            }
        }

        public eRoomType RoomType
        {
            get { return m_roomType; }
        }

        public eTeamType TeamType
        {
            get { return m_teamType; }
        }

        public eGameType GameType
        {
            get { return m_gameType; }
        }

        public Dictionary<GamePlayer, Player> Players
        {
            get { return m_players; }
        }

        public int PlayerCount
        {
            get 
            {
                lock (m_players)
                {
                    return m_players.Count;
                }
            }
        }

        public int TurnIndex
        {
            get { return m_turnIndex; }
        }

        public int Wind
        {
            get { return m_wind; }
        }

        public Map Map
        {
            get { return m_map; }
        }

        public Player CurrentPlayer
        {
            get { return m_currentPlayer; }
        }

        public bool HasPlayer
        {
            get { return m_players.Count > 0; }
        }

        public Random Random
        {
            get { return m_random; }
        }

        public void UpdateWind(int wind, bool sendToClient)
        {
            if (m_wind != wind)
            {
                m_wind = wind;

                if (sendToClient)
                {
                    GamePlayer player = m_redTeam[0].PlayerDetail;
                    GSPacketIn pkg = player.Out.SendGameUpdateWind(m_wind);
                    SendToAll(pkg, player);
                }
            }
        }

        public void Prepare()
        {
            if (GameState == eGameState.Inited)
            {
                SendCreateGame();

                if (m_teamType == eTeamType.Leader)
                {
                    SendOpenSelectLeaderWindow(5);
                    WaitTime(6 * 1000);
                }
                else
                {
                    CheckState(0);
                }

                GameState = eGameState.Prepared;
            }
        }

        public void StartLoading()
        {
            if (GameState == eGameState.Prepared)
            {
                GameState = eGameState.Loading;
                ClearWaitTimer();

                SendStartLoading(60);
                WaitTime(61 * 1000);
            }
        }

        public void StartGame()
        {
            if (GameState == eGameState.Loading)
            {
                GameState = eGameState.Playing;
                ClearWaitTimer();

                if (m_teamType == eTeamType.Leader)
                {
                    m_redLeader = SelectLeader(m_redTeam);
                    m_redLeader.SetCaptain(true);
                    m_blueLeader = SelectLeader(m_blueTeam);
                    m_blueLeader.SetCaptain(true);
                }

                Player[] list = GetAllFightPlayersSafe();

                MapPoint mapPos = MapMgr.GetMapRandomPos(m_map.Info.ID);
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_START);
                pkg.WriteInt(list.Length);
                foreach (Player p in list)
                {
                    Point pos = GetPlayerPoint(mapPos,p.Team);
                    p.SetXY(pos);
                    m_map.AddPhysical(p);
                    p.StartMoving();

                    p.StartGame();

                    pkg.WriteInt(p.Id);
                    pkg.WriteInt(p.X);
                    pkg.WriteInt(p.Y);
                    pkg.WriteInt(p.Direction);
                    pkg.WriteInt(p.Blood);
                    pkg.WriteBoolean(p.IsCaptain);
                }

                SendToAll(pkg);

                WaitTime(list.Length * 1000);

                OnGameStarted();
            }
        }

        public void NextTurn()
        {
            if (GameState == eGameState.Playing)
            {
                ClearWaitTimer();
                CheckBox();

                m_turnIndex++;
                m_wind = GetNextWind();

                List<Box> newBoxes = CreateBox();

                Player[] players = GetAllFightPlayersSafe();
                foreach (Player p in players)
                {
                    p.BeginNextTurn();
                }

                m_currentPlayer = FindLowDelayPlayer(players, m_random.Next(0, players.Length));

                m_currentPlayer.StartAttacking();

                GSPacketIn pkg = m_currentPlayer.PlayerDetail.Out.SendGameNextTurn(m_currentPlayer, this, newBoxes);
                SendToAll(pkg, m_currentPlayer.PlayerDetail);

                AddAction(new WaitPlayerAttackingAction((m_timeType + 15) * 1000, m_currentPlayer, m_turnIndex));

                OnBeginNewTurn();
            }
        }
    
        public void GameOver()
        {
            if (GameState == eGameState.Playing)
            {
                GameState = eGameState.GameOver;
                ClearWaitTimer();

                Player[] players = GetAllFightPlayersSafe();
                int winTeam = -1;
                if (m_teamType == eTeamType.Leader)
                {
                    if (m_blueLeader.IsLiving)
                        winTeam = 2;
                    if (m_redLeader.IsLiving)
                        winTeam = 1;
                }
                else
                {
                    foreach (Player p in players)
                    {
                        if (p.IsLiving)
                        {
                            winTeam = p.Team;
                            break;
                        }
                    }
                }

                if (winTeam == -1 && m_currentPlayer != null)
                    winTeam = m_currentPlayer.Team;

                int riches = CalculateGuildMatchResult(players, winTeam);

                bool canBlueTakeOut = false;
                bool canRedTakeOut = false;

                foreach (Player p in players)
                {
                    if (p.TotalHurt > 0)
                    {
                        if (p.Team == 1)
                            canRedTakeOut = true;
                        else
                            canBlueTakeOut = true;
                    }
                }

                ////P赢=[2+伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[0.9+(游戏开始时对方玩家人数-1)*30%]
                ////P输=[伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[0.9+(游戏开始时对方玩家人数-1)*30%]

                GSPacketIn pkg = new GSPacketIn((short)ePackageType.GAME_OVER);

                pkg.WriteInt(PlayerCount);
                foreach (Player p in players)
                {
                    float againstTeamLevel = p.Team == 1 ? m_blueAvgLevel : m_redAvgLevel;
                    float againstTeamCount = p.Team == 1 ? m_blueTeam.Count : m_redTeam.Count;
                    float disLevel = Math.Abs(againstTeamLevel - p.PlayerDetail.PlayerCharacter.Grade);
                    float winPlus = p.Team == winTeam ? 2 : 0;
                    int gp = 0;
                    int totalShoot = p.TotalShootCount == 0 ? 1 : p.TotalShootCount;
                    
                    if( m_roomType == eRoomType.Match || disLevel < 5)
                    {
                        gp = (int)Math.Ceiling(( winPlus + p.TotalHurt * 0.001 + p.TotalKill * 0.5 + (p.TotalHitTargetCount / totalShoot) * 2) * againstTeamLevel * (0.9 + (againstTeamCount - 1) * 0.3));
                    }
                    else
                    {
                        gp = 1;
                    }

                    p.GainGP = p.PlayerDetail.AddGP(gp);
                    p.CanTakeOut = p.Team == 1 ? canRedTakeOut : canBlueTakeOut;

                    pkg.WriteInt(p.Id);
                    pkg.WriteBoolean(p.Team == winTeam);
                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.GP);
                    pkg.WriteInt(p.TotalKill);
                    pkg.WriteInt(p.TotalHurt);
                    pkg.WriteInt(p.TotalHitTargetCount);
                    pkg.WriteInt(p.TotalShootCount);
                    pkg.WriteInt(p.GainGP);
                    pkg.WriteInt(p.GainOffer);
                    pkg.WriteBoolean(p.CanTakeOut);
                }

                pkg.WriteInt(riches);
                pkg.WriteInt(m_redTeam.Count);

                SendToAll(pkg);

                foreach (Player p in players)
                {
                    p.PlayerDetail.OnGameOver(this, p.Team == winTeam, p.GainGP);
                }

                WaitTime(15 * 1000);
                OnGameOverred();
            }
        }

        public void Stop()
        {
            if (GameState == eGameState.GameOver)
            {
                GameState = eGameState.Stopped;

                Player[] players = GetAllFightPlayersSafe();
                foreach (Player p in players)
                {
                    if (p.IsActive && p.HasTakeCard == false && p.CanTakeOut == true)
                    {
                        TakeCard(p);
                    }
                }

                lock (m_players)
                {
                    m_players.Clear();
                }

                OnGameStopped();
            }
 
        }

        private int CalculateGuildMatchResult(Player[] players, int winTeam)
        {
            if (m_gameType == eGameType.Guild)
            {
                //StringBuilder winStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
                //StringBuilder loseStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));

                //GamePlayer winPlayer = null;
                //GamePlayer losePlayer = null;

                //foreach (Player p in players)
                //{
                //    if (p.Team == winTeam)
                //    {
                //        winStr.Append(string.Format("[{0}]", p.PlayerDetail.PlayerCharacter.NickName));
                //        winPlayer = p.PlayerDetail;
                //    }
                //    else
                //    {
                //        loseStr.Append(string.Format("{0}", p.PlayerDetail.PlayerCharacter.NickName));
                //        losePlayer = p.PlayerDetail;
                //    }
                //}
                
                //winStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1") + losePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2"));
                //loseStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3") + winPlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4"));
                //int riches = ConsortiaMgr.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID,winPlayer.PlayerCharacter.ConsortiaID,game.Data.Players, game.RoomType, game.GameClass);
                //GameServer.Instance.LoginServer.SendConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, riches, winStr);
                //GameServer.Instance.LoginServer.SendConsortiaFight(losePlayer.PlayerCharacter.ConsortiaID, -riches, loseStr);
                return 1;
            }
            return 0;
        }

        public int GetTurnWaitTime()
        {
            return 7;
        }

        public void RemovePlayer(GamePlayer gamePlayer)
        {
            Player player = null;
            lock(m_players)
            {
                if (m_players.ContainsKey(gamePlayer))
                {
                    player = m_players[gamePlayer];
                    m_players.Remove(gamePlayer);
                }
            }
            if (player != null)
            {
                AddAction(new RemovePlayerAction(player));
            }
        }

        private Player SelectLeader(List<Player> list)
        {
            ArrayList temp = new ArrayList();
            foreach (Player p in list)
            {
                if (p.WannaLeader)
                {
                    temp.Add(p);
                }
            }
            if (temp.Count > 0)
            {
                return (Player)temp[m_random.Next(0, temp.Count)];
            }
            else
            {
                return list[m_random.Next(0, list.Count)];
            }

        }

        private Point GetPlayerPoint(MapPoint mapPos,int team)
        {
            List<Point> list = team == 1 ? mapPos.PosX : mapPos.PosX1;
            int rand = m_random.Next(list.Count);
            Point pos = list[rand];
            list.Remove(pos);
            return pos;
        }

        private int GetNextWind()
        {
            return 0;
        }

        private Player FindLowDelayPlayer(Player[] players, int start)
        {
            Player player = players[start];
            int delay = player.Delay;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Delay < delay && players[i].IsLiving)
                {
                    delay = players[i].Delay;
                    player = players[i];
                }
            }
            return player;
        }

        public bool IsAllComplete()
        {
            Player[] list = GetAllFightPlayersSafe();
            foreach (Player p in list)
            {
                if (p.LoadingProcess < 100)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanGameOver()
        {
            if (m_teamType == eTeamType.Leader)
            {
                return m_redLeader.IsLiving == false || m_blueLeader.IsLiving == false;
            }
            else
            {
                bool red = true;
                bool blue = true;
                foreach (Player p in m_redTeam)
                {
                    if (p.IsLiving)
                    {
                        red = false;
                        break;
                    }
                }

                foreach (Player p in m_blueTeam)
                {
                    if (p.IsLiving)
                    {
                        blue = false;
                        break;
                    }
                }
                return red || blue;
            }
        }

        public bool TakeCard(Player player)
        {
            int index = 0;

            for(int i = 0; i < Cards.Length; i ++)
            {
                if(Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }

            return TakeCard(player, index);
        }

        public bool TakeCard(Player player,int index)
        {
            if (player.CanTakeOut == false)
                return false;

            if (player.IsActive == false || index < 0 || index > 7 || player.HasTakeCard || Cards[index] > 0)
                return false;

            MapGoodsInfo info = MapMgr.GetRandomAward(m_map.Info.ID, m_map.Info.Type);

            bool isItem = false;
            int value = 100;
            if (info != null)
            {
                if (info.GoodsID > 0)
                {
                    ItemTemplateInfo temp = ItemMgr.GetSingleGoods(info.GoodsID);
                    if (temp != null)
                    {
                        isItem = true;
                        value = info.GoodsID;
                        player.PlayerDetail.TempInventory.AddItemTemplate(temp, info);
                    }
                }
                else if (info.GoodsID == -1)
                {
                    value = info.Value;
                }
            }

            if (isItem == false)
            {
                value = player.PlayerDetail.AddGold(value);
            }

            player.HasTakeCard = true;
            Cards[index] = 1;

            GSPacketIn pkg = player.PlayerDetail.Out.SendGamePlayerTakeCard(player,index,isItem, value);
            SendToAll(pkg, player.PlayerDetail);

            return true;
        }

        public void CheckState(int delay)
        {
            AddAction(new CheckGameStateAction(delay));
        }

        public void ProcessData(GamePlayer gamePlayer, GSPacketIn packet)
        {
            if (m_players.ContainsKey(gamePlayer))
            {
                Player player = m_players[gamePlayer];
                AddAction(new ProcessPacketAction(player, packet));
            }
        }

        public int GetGhostCount()
        {
            int count = 0;
            foreach (Player p in m_players.Values)
            {
                if (p.IsActive && p.IsLiving == false)
                {
                    count++;
                }
            }
            return count;
        }
        
        #region Send/Protocal

        private void SendCreateGame()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CREATE);

            pkg.WriteInt(m_map.Info.ID);
            pkg.WriteInt((byte)m_roomType);
            pkg.WriteInt((byte)m_teamType);
            pkg.WriteInt((byte)m_gameType);
            pkg.WriteInt(m_timeType);

            Player[] players = GetAllFightPlayersSafe();
            pkg.WriteInt(players.Length);
            foreach (Player p in players)
            {
                GamePlayer gp = p.PlayerDetail;
                pkg.WriteInt(p.Id);
                pkg.WriteString(gp.PlayerCharacter.NickName);
                pkg.WriteBoolean(gp.PlayerCharacter.Sex);
                pkg.WriteInt(gp.PlayerCharacter.Hide);
                pkg.WriteString(gp.PlayerCharacter.Style);
                pkg.WriteString(gp.PlayerCharacter.Colors);
                pkg.WriteString(gp.PlayerCharacter.Skin);
                pkg.WriteInt(gp.PlayerCharacter.Grade);
                pkg.WriteInt(gp.PlayerCharacter.Repute);
                pkg.WriteInt(gp.CurrentWeapon().TemplateID);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaID);
                pkg.WriteString(gp.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaLevel);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaRepute);

                pkg.WriteInt(p.Team);
            }

            SendToAll(pkg);
        }

        private void SendOpenSelectLeaderWindow(int maxTime)
        {
            GSPacketIn pkg = new GSPacketIn((int)ePackageType.GAME_CAPTAIN_AFFIRM);
            pkg.WriteInt(maxTime);
            SendToAll(pkg);
        }

        private void SendStartLoading(int maxTime)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_LOAD);
            SendToAll(pkg);
        }

        #endregion

        #region Actions/Update

        

        public void AddAction(IAction action)
        {
            lock (m_actions)
            {
                m_actions.Add(action);
            }
        }

        public void AddAction(ArrayList actions)
        {
            lock (m_actions)
            {
                m_actions.AddRange(actions);
            }
        }

        private long m_waitTimer = 0;
        public void ClearWaitTimer()
        {
            m_waitTimer = 0;
        }
        public void WaitTime(int delay)
        {
            m_waitTimer = Math.Max(m_waitTimer, GameMgr.GetTickCount() + delay);
        }

        public void Update(long tick)
        {
            ArrayList temp;

            lock (m_actions)
            {
                temp = (ArrayList)m_actions.Clone();
                m_actions.Clear();
            }

            if (temp != null && GameState != eGameState.Stopped)
            {
                if (temp.Count > 0)
                {
                    ArrayList left = new ArrayList();
                    foreach (IAction action in temp)
                    {
                        try
                        {
                            action.Execute(this, tick);
                            if (action.IsFinish() == false)
                            {
                                left.Add(action);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Map update error:", ex);
                        }
                    }
                    AddAction(left);
                }
                else if(m_waitTimer < tick)
                {
                    CheckState(0);
                }
            }
        }
        #endregion

        #region SendToAll/SendToTeam GetAllPlayer

        public Player[] GetAllFightPlayersSafe()
        {
            Player[] temp = null;
            lock (m_players)
            {
                temp = new Player[m_players.Values.Count];
                m_players.Values.CopyTo(temp, 0);
            }

            return temp == null ? new Player[0] : temp;
        }

        public void SendToAll(GSPacketIn pkg)
        {
            SendToAll(pkg, null);
        }

        public void SendToAll(GSPacketIn pkg, GamePlayer except)
        {
            Player[] temp = GetAllFightPlayersSafe();
            foreach (Player p in temp)
            {
                if (p.IsActive && p.PlayerDetail != except)
                {
                    p.PlayerDetail.Out.SendTCP(pkg);
                }
            }
        }

        public void SendToTeam(GSPacketIn pkg, int team)
        {
            SendToTeam(pkg, team, null);
        }

        public void SendToTeam(GSPacketIn pkg, int team, GamePlayer except)
        {
            Player[] temp = GetAllFightPlayersSafe();
            foreach (Player p in temp)
            {
                if (p.IsActive && p.PlayerDetail != except && p.Team == team)
                {
                    p.PlayerDetail.Out.SendTCP(pkg);
                }
            }
        }
        #endregion

        #region Box/Temp Point

        private List<Box> m_tempBox;
        private List<Point> m_tempPoints;
        private int m_tempBoxId;

        public void AddTempPoint(int x, int y)
        {
            m_tempPoints.Add(new Point(x, y));
        }

        public Box AddBox(MapGoodsInfo item,Point pos)
        {
            Box box = new Box(m_tempBoxId ++, item);
            box.SetXY(pos);
            m_map.AddPhysical(box);
            m_tempBox.Add(box);
            return box;
        }

        public void CheckBox()
        {
            List<Box> temp = new List<Box>();
            foreach (Box b in m_tempBox)
            {
                if (b.IsLiving == false)
                {
                    temp.Add(b);
                }
            }

            foreach (Box b in temp)
            {
                m_tempBox.Remove(b);
            }
        }

        public List<Box> CreateBox()
        {
            int total = m_players.Count + 2;

            int itemCount = 0;
            List<MapGoodsInfo> list = new List<MapGoodsInfo>();
            if(CurrentTurnTotalDamage > 0)
            {
                itemCount = m_random.Next(1, 3);
                if(m_tempBox.Count + itemCount > total)
                {
                    itemCount = total - m_tempBox.Count;
                }
                if(itemCount > 0)
                {
                    list.AddRange(MapMgr.GetRandomGoodsByNumber(m_map.Info.ID,itemCount,(int)m_mapType));
                }

            }

            int ghostCount = GetGhostCount();
            int propCount = 0;
            if (ghostCount > 0)
            {
                propCount = m_random.Next(ghostCount);
            }
            if (m_tempBox.Count + itemCount + propCount > total)
            {
                propCount = total - m_tempBox.Count - itemCount;
            }
            if (propCount > 0)
            {
                MapMgr.GetRandomFightPropByCount(m_map.Info.ID, propCount, list);
            }

            List<Box> box = new List<Box>();
            if(list.Count > 0)
            {
                for(int i =0; i < m_tempPoints.Count; i ++)
                {
                    int index = m_random.Next(m_tempPoints.Count);
                    Point p = m_tempPoints[index];
                    m_tempPoints[index] = m_tempPoints[i];
                    m_tempPoints[i] = p;
                }
                int count = Math.Max(list.Count,m_tempPoints.Count);
                for (int i = 0; i < count; i++)
                {
                    box.Add(AddBox(list[i], m_tempPoints[i]));
                }
            }

            m_tempPoints.Clear();
            return box;
        }

        #endregion

        #region Statics Number/Method

        public int TotalCostMoney;
        public int TotalCostGold;

        public void AfterUseItem(ItemInfo item)
        {
            TotalCostGold += item.Template.Gold;
            TotalCostMoney += item.Template.Money;
        }
        #endregion

        #region Events

        public event GameEventHandle GameStarted;
        public event GameEventHandle GameOverred;
        public event GameEventHandle GameStopped;
        public event GameEventHandle BeginNewTurn;

        protected void OnGameStarted()
        {
            if (GameStarted != null) GameStarted(this);
        }

        protected void OnGameOverred()
        {
            if (GameOverred != null) GameOverred(this);
        }

        protected void OnGameStopped()
        {
            if (GameStopped != null) GameStopped(this);
        }

        protected void OnBeginNewTurn()
        {
            if (BeginNewTurn != null) BeginNewTurn(this);
        }

        #endregion
    }

    public delegate void GameEventHandle(BaseGame game);
}
