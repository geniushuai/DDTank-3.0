using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Spells;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System.Threading;
using Bussiness;
using Phy.Maps;
using Phy.Object;
using System.Drawing;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.SceneGames
{
    public class TankData
    {
        public object _syncStop = new object();
        public ThreadSafeRandom rand = new ThreadSafeRandom();
        private int _mapIndex = 0;
        private Map _currentMap;
        private GamePlayer _currentIndex = null;
        private GamePlayer _currentFire = null;
        private BallInfo _currentBall;
        private int _currentWind = 0;
        private int _nextWind = 0;
        public bool IsFirst = true;
        public int BlastX = -1;
        public int BlastY = -1;
        public int BlastID = 0;
        public int ArkCount = 8;
        public int RedArkCount = 0;
        public int Arks;
        public ItemInfo CurrentPorp = null;
        public ISpellHandler CurrentSpell = null;
        public Dictionary<GamePlayer, Player> Players = new Dictionary<GamePlayer, Player>();

        public List<Box> _fallItemID = new List<Box>();
        public List<Point> TempPoint = new List<Point>();
        public List<Box> TempBox = new List<Box>();

        public Dictionary<int, Balance> persons = new Dictionary<int, Balance>();
        public int TotalPerson;
        public int TotalLevel;

        public int AttackUp = 1;
        public double AddWound = 0;
        public double AddMultiple = 1;
        public double Modulus = 1;
        private int _addAttack = 1;
        private int _addBall = 1;
        private int _bombs;
        private int _fireBombs;
        private int _blastBoms;

        public int PhyID;
        public bool Isforce = false;
        public bool BreachDefence = false;
        public int IsDead;
        public int LastDead;
        public bool FireLogin;
        public int TurnNum;

        public int TotalDelay;
        public int TotalHeathPoint;

        public List<GamePlayer> FlagPlayer;
        public bool[] Cards = new bool[8];

        public DateTime GameStart;
        public int CostGold;
        public int CostMoney;
        public string UserIDs;
        public string TeamA;
        public string TeamB;
        public int PropCount;
        public int GoldCount;
        public int StartedGameClass;
        public int ConsortiaID1;
        public int ConsortiaID2;
        public bool killFlag;
        public eMapType MapType;
        public bool IsChangeStyle;
        public string FightName;
        public int ChangeTeam;

        private DateTime _turnEndTime;
        private int _turnNumFlag;
        public void SetRunTime(int time)
        {
            if (GameServer.Instance.Configuration.FastLimit)
            {
                if (_turnNumFlag != TurnNum || DateTime.Now.AddSeconds(time).CompareTo(_turnEndTime) > 0)
                {
                    _turnEndTime = DateTime.Now.AddSeconds(time);
                    _turnNumFlag = TurnNum;
                }
            }
        }

        public bool IsFastSpeed()
        {
            if (GameServer.Instance.Configuration.FastLimit)
            {
                if (_turnNumFlag != TurnNum && _turnEndTime.CompareTo(DateTime.Now) > 0)
                {
                    return true;
                }

            }
            return false;     
        }

        //private PlayerData[] playerDatas = new PlayerData[8];
        public TankData()
        {
            FlagPlayer = new List<GamePlayer>();

            Reset();

            //for (int i = 0; i < playerDatas.Length; i++)
            //{
            //    playerDatas[i] = new PlayerData(null);
            //}
        }
        //public PlayerData GetPlayerData(GamePlayer player)
        //{
        //    playerDatas[player.CurrentGameIndex].Reset();
        //    playerDatas[player.CurrentGameIndex].Player = player;
        //    return playerDatas[player.CurrentGameIndex];
        //}

        public GamePlayer[] GetAllPlayers()
        {
            List<GamePlayer> temp = new List<GamePlayer>();
            foreach (KeyValuePair<GamePlayer, Player> p in Players)
            {
                if (p.Value.State != TankGameState.LOSE)
                {
                    temp.Add(p.Key);
                }
            }

            return temp.ToArray();
        }


        public int Count
        {
            get
            {
                int count = 0;
                foreach (Player p in Players.Values)
                {
                    if (p.State != TankGameState.LOSE)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public int GetFinishPlayerCount()
        {
            int Count = 0;
            GamePlayer[] player = GetAllPlayers();
            foreach (GamePlayer p in player)
            {
                if (p.CurrentGameState == ePlayerGameState.FINISH)
                {
                    Count++;
                }
            }
            return Count;
        }

        public int GetPlayFinishCount()
        {
            int Count = 0;
            GamePlayer[] player = GetAllPlayers();
            foreach (GamePlayer p in player)
            {
                if (Players[p].TurnNum == TurnNum)
                {
                    Count++;
                }
            }
            return Count;
        }

        public int MapIndex
        {
            get
            {
                return _mapIndex;
            }
            set
            {
                _mapIndex = value;
                _currentMap = Managers.MapMgr.CloneMap(_mapIndex);
            }
        }

        public Map CurrentMap
        {
            get
            {
                return _currentMap;
            }
            set
            {
                _currentMap = value;
            }
        }

        public GamePlayer CurrentIndex
        {
            set
            {
                TurnNum++;
                IsDead = -1;
                CurrentSpell = null;
                CurrentPorp = null;
                _currentIndex = value;
                AttackUp = 1;
                AddWound = 0;
                AddMultiple = 1;
                Modulus = 1;
                _fireBombs = 0;
                _blastBoms = 0;
                _addAttack = 1;
                _addBall = 1;
                BreachDefence = false;
                TotalDelay = 0;
                TotalHeathPoint = 0;
                BlastID = 0;
                TempPoint.Clear();

                Players[_currentIndex].BoutNum++;
                Players[_currentIndex].Energy = 240;
                foreach (Player p in Players.Values)
                {
                    p.NoHoleTurn = false;
                    if (p.State == TankGameState.DEAD)
                    {
                        p.Energy = 240;
                    }
                }
                _currentBall = value.Ball1;
            }
            get
            {
                return _currentIndex;
            }
        }

        public void SpendTime(int sec)
        {
            //int time = (int)((DateTime.Now.Ticks - StartTmie) / 1000 / 1000 / 10);
            int time = Math.Abs(sec);
            TotalDelay += (time > _currentIndex.CurrentGame.GetTime ? _currentIndex.CurrentGame.GetTime : time) * 20;
        }

        public BallInfo CurrentBall
        {
            get
            {
                if (_currentBall == null)
                    return _currentFire.Ball1;
                return _currentBall;
            }
        }

        public void SetCurrentBall(BallInfo info, bool stunt)
        {
            _currentBall = info;
            GSPacketIn pkg = _currentIndex.Out.SendCurrentBall(stunt);
            _currentIndex.CurrentGame.SendToPlayerExceptSelf(pkg, _currentIndex);
        }

        public void Reset()
        {
            //MapType = 0;
            TurnNum = 0;
            _turnNumFlag = 0;
            FireLogin = false;
            _bombs = 0;
            PhyID = 0;
            Arks = 0;
            _currentIndex = null;
            _currentFire = null;
            _currentWind = 0;
            IsFirst = true;
            BlastX = -1;
            BlastY = -1;
            CurrentSpell = null;
            CurrentPorp = null;
            TempBox.Clear();
            StartedGameClass = 1;
            //ConsortiaID1 = 0;
            //ConsortiaID2 = 0;
            killFlag = false;
        }

        public void StartReset(BaseSceneGame game)
        {
            //ConsortiaID1 = 0;
            //ConsortiaID2 = 0;
            //Players.Clear();
            _currentIndex = null;
            _currentFire = null;
            GameStart = DateTime.Now;
            CostGold = 0;
            CostMoney = 0;
            UserIDs = "";
            TeamA = string.Empty;
            TeamB = string.Empty;
            PropCount = 0;
            GoldCount = 0;

            ArkCount = 8;
            RedArkCount = 0;
            _fallItemID.Clear();
            FlagPlayer.Clear();
            persons.Clear();
            TotalPerson = 0;
            TotalLevel = 0;
            TotalHeathPoint = 0;
            IsChangeStyle = false;
            FightName = string.Empty;

            for (int i = 0; i < Cards.Length; i++)
            {
                Cards[i] = false;
            }

            foreach (KeyValuePair<GamePlayer, Player> p in Players)
            {
                //if (game.RoomType == eRoomType.PAIRUP)
                //    p.Key.PlayerCharacter.Total++;

                //p.Key.PlayerCharacter.CheckCount++;
                p.Value.Reset();
                p.Value.IsTakeOut = false;
                p.Key.TempInventory.Clear();

                if (!persons.ContainsKey(p.Key.CurrentTeamIndex))
                {
                    //Balance b = new Balance();
                    persons.Add(p.Key.CurrentTeamIndex, new Balance());
                }

                persons[p.Key.CurrentTeamIndex].TotalLevel += p.Key.PlayerCharacter.Grade;
                persons[p.Key.CurrentTeamIndex].TeamPerson++;
                TotalPerson++;
                TotalLevel += p.Key.PlayerCharacter.Grade;

                UserIDs += UserIDs == "" ? p.Key.PlayerCharacter.ID.ToString() : "," + p.Key.PlayerCharacter.ID.ToString();
                if (p.Key.CurrentTeamIndex == 1)
                    TeamA += TeamA == "" ? p.Key.PlayerCharacter.ID.ToString() : "," + p.Key.PlayerCharacter.ID.ToString();
                else
                    TeamB += TeamB == "" ? p.Key.PlayerCharacter.ID.ToString() : "," + p.Key.PlayerCharacter.ID.ToString();
            }
        }

        public GamePlayer CurrentFire
        {
            get
            {
                return _currentFire;
            }
            set
            {
                //if (CurrentBall == CurrentIndex.Ball2)
                //    AddBall = 1;


                if (value != null)
                {
                    if (Players[value].IsCaptain && (_currentBall == value.Ball1 || _currentBall == value.Ball2))
                    {
                        AddAttack = 1;
                    }
                    _bombs = _addBall * _addAttack * CurrentBall.Amount;
                }
                _currentFire = value;
            }
        }

        public bool Bombs
        {
            get
            {
                return _bombs <= 0;
            }
        }

        public int AddFallGoods(Point p, MapGoodsInfo itemID)
        {
            Box box = new Box(_fallItemID.Count, itemID);
            box.SetXY(p);
            _currentMap.AddPhysical(box);
            _fallItemID.Add(box);
            TempBox.Add(box);
            Arks++;
            return 0;
        }

        public MapGoodsInfo GetFallItemsID(int index, GamePlayer player)
        {
            if (_fallItemID.Count <= index || index < 0)
            {
                GameServer.log.Error("GetFallItemsID,_fallItemID.Count:" + _fallItemID.Count + ",index:" + index + ",nickname:" + player.PlayerCharacter.NickName);
                return null;
            }

            try
            {
                double space = Physics.PointToLine(Players[player].EndX, Players[player].EndY, Players[player].X, Players[player].Y, _fallItemID[index].X, _fallItemID[index].Y);
                if (space > 60)
                {
                    GameServer.log.Error("Soul is pick ark error,distance:" + space.ToString());
                    return null;

                }
            }
            catch (Exception ex)
            {
                GameServer.log.Error("ArkID:" + index + ",ark count:" + _fallItemID.Count.ToString());
                GameServer.log.Error("Ark error", ex);
            }
            return GetFallItemsID(index);
        }

        public MapGoodsInfo GetFallItemsID(int index, int userID)
        {
            if (_fallItemID.Count <= index || index < 0)
            {
                GameServer.log.Error("GetFallItemsID,_fallItemID.Count:" + _fallItemID.Count + ",index:" + index + ",userID:" + userID);
                return null;
            }
            if (_fallItemID[index].UserID == userID)
            {
                return GetFallItemsID(index);
            }
            return null;
        }

        public MapGoodsInfo GetFallItemsID(int index)
        {
            if (_fallItemID.Count > index)
            {
                MapGoodsInfo isType = _fallItemID[index].Items;
                _fallItemID[index].Items = null;
                Arks--;
                return isType;
            }
            return null;
        }

        public void LeavingRedArkCount()
        {
            int count = 0;
            //foreach (int i in _fallItemID)
            //{
            //    if (i != 0)
            //        count++;
            //}
            RedArkCount = count;
        }

        public bool TakeOutArk()
        {
            if (ArkCount == 8)
                LeavingRedArkCount();

            bool isRed = RedArkCount > rand.Next(ArkCount);
            if (isRed)
                RedArkCount--;
            ArkCount--;

            return isRed;
        }

        public Point[] GetArkPoint(int count)
        {
            int r = 0;
            Point temp;
            Point[] list = TempPoint.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                r = rand.Next(list.Length);
                temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
            return list;
        }

        //暴击率=幸运*0.045%
        public bool IsForce()
        {
            //Isforce = (_currentFire.PlayerCharacter.Luck + 50) > rand.Next(1000);
            Isforce = _currentFire.PlayerCharacter.Luck * 45 > rand.Next(100000);
            return Isforce;
        }

        //暴击伤害=1.5+幸运*0.05%
        public int AddForceWound(int wound)
        {
            if (IsForce())
            {
                //return (100 + rand.Next(50, 101)) * wound / 100;
                return (15000 + _currentFire.PlayerCharacter.Luck * 5) * wound / 10000;
            }
            return wound;
        }

        public int AddAttack
        {
            get
            {
                return _addAttack;
            }
            set
            {
                if (value > 0)
                    _addAttack += value;
                else
                    _addAttack = 1;

                GSPacketIn pkg = CurrentIndex.Out.SendAddAttck(CurrentIndex);
                CurrentIndex.CurrentGame.SendToPlayerExceptSelf(pkg, CurrentIndex);
            }
        }

        public int AddBall
        {
            get
            {
                return _addBall * CurrentBall.Amount;
            }
            set
            {
                _addBall = value;
                GSPacketIn pkg = CurrentIndex.Out.SendAddBall(CurrentIndex);
                CurrentIndex.CurrentGame.SendToPlayerExceptSelf(pkg, CurrentIndex);
            }
        }

        public double BallPower
        {
            get
            {
                double power = 1;
                if (_addBall == 3)
                    power = 0.5;
                ////if (_fireBombs > 0)
                ////    power = power * 0.7;
                //if (_addAttack > 1)
                //    power = power * 0.7;
                _fireBombs++;
                return power;
            }
        }

        public int CurrentWind
        {
            get
            {
                return _currentWind;
            }
            set
            {
                _currentWind = value;
                CurrentMap.wind = value;
            }
        }

        public int GetNextWind()
        {
            if (_currentWind > _nextWind)
            {
                //_currentWind = _currentWind - rand.Next(11);
                CurrentWind = _currentWind - rand.Next(11);
                if (_currentWind <= _nextWind)
                {
                    _nextWind = rand.Next(-40, 40);
                }
            }
            else
            {
                CurrentWind = _currentWind + rand.Next(11);
                if (_currentWind >= _nextWind)
                {
                    _nextWind = rand.Next(-40, 40);
                }
            }
            return _currentWind;
        }

        public bool FireBombs
        {
            get
            {
                return _fireBombs == _blastBoms;
            }
        }

        public void AddFireBombs(byte count)
        {
            _fireBombs += count;
        }

        public bool ReduceFireBombs
        {
            get
            {
                //if (_fireBombs <= _blastBoms)
                //    return false;

                //_blastBoms++;
                //_bombs--;
                //return _fireBombs >= _blastBoms;
                if (_bombs <= 0)
                    return false;

                _bombs--;
                return true;
            }
        }

        public void KillPerson(GamePlayer killPlayer)
        {
            if (killPlayer.CurrentTeamIndex == CurrentIndex.CurrentTeamIndex || CurrentIndex.CurrentGame == null)
                return;

            persons[CurrentIndex.CurrentTeamIndex].TatolKill++;
            Players[CurrentIndex].TotalKill++;

            int state = ConsortiaMgr.KillPlayer(CurrentIndex, killPlayer, Players, CurrentIndex.CurrentGame.RoomType, CurrentIndex.CurrentGame.GameClass);
            //int state = ConsortiaMgr.FindConsortiaAlly(CurrentIndex.PlayerCharacter.ConsortiaID,killPlayer.PlayerCharacter.ConsortiaID);

            CurrentIndex.QuestInventory.CheckKillPlayer(MapIndex, (int)CurrentIndex.CurrentGame.GameMode,
                CurrentIndex.CurrentGame.ScanTime, Players[CurrentIndex].IsCaptain, killPlayer.PlayerCharacter.Grade,
                persons[CurrentIndex.CurrentTeamIndex].TeamPerson, TotalPerson - persons[CurrentIndex.CurrentTeamIndex].TeamPerson, state, (int)CurrentIndex.CurrentGame.RoomType);


            //GameServer.Instance.LoginServer.SendConsortiaOffer(killPlayer.PlayerCharacter.ConsortiaID
        }

        public byte GetCards()
        {
            for (byte i = 0; i < Cards.Length; i++)
            {
                if (!Cards[i])
                    return i;
            }
            return 0;
        }

        public int GetDeadCount()
        {
            int count = 0;
            foreach (Player p in Players.Values)
            {
                if (p.State == TankGameState.DEAD)
                    count++;
            }
            return count;
        }

        public double GetBiasLenght(int x, int y)
        {
            return Math.Sqrt((double)(x * x + y * y));
        }

        public void CostInfo(ItemInfo item)
        {
            CostGold += item.Template.Gold;
            CostMoney += item.Template.Money;
        }

        public void InsertGameInfo(BaseSceneGame game, int winTeam)
        {
            CountBussiness.InsertGameInfo(GameStart, MapIndex, CostMoney, CostGold, UserIDs);
            Statics.StatMgr.LogGame(Players.Count, MapIndex, GameStart, PropCount, GoldCount);
            if (IsChangeStyle)
            {
                using (RecordBussiness db = new RecordBussiness())
                {
                    db.InsertFightRecord(GameStart, ChangeTeam, TeamA, TeamB, MapIndex, (int)game.RoomType, FightName, winTeam);
                }
            }
        }

        public int GetRandomGold(eRoomType eType)
        {
            bool flag = eType == eRoomType.PAIRUP;

            int gold = 1000;
            int rate = rand.Next(0, 100);
            if (rate < 1)
            {
                gold = flag ? 5000 : 1000;
            }
            else if (rate < 6)
            {
                gold = flag ? 2000 : 500;
            }
            else
            {
                //300 350 400 450 500 550 600 650 另外10%几率出现1000金币，5%几率出现2000金币，1%几率出现5000金币
                gold = flag ? rand.Next(6, 14) * 50 : rand.Next(5, 16) * 10;
            }

            return gold;
        }

        public List<int> DestroyBox()
        {
            List<int> tempBoxID = new List<int>();
            foreach (Box b in _fallItemID)
            {
                if (b.Items == null)
                    continue;

                b.LiveCount--;

                if (b.LiveCount == 0)
                {
                    b.Items = null;
                    tempBoxID.Add(b.Id);
                    b.Die();
                    Arks--;
                }
            }

            return tempBoxID;
        }

    }

    //E等级差=取整(对方平均等级 - 己方平均等级)
    //P赢 =  （P 单 * 总消灭人数 / 己方队员数）* (1 + 消灭人数 * 10% + E等级差 * 10%) 
    //P输 =  P 单 * (1 - 消灭人数 * 10% - 等级差 * 10%) 
    public class Balance
    {
        public Balance()
        {
            TatolKill = 0;
            TeamPerson = 0;
            TotalLevel = 0;
            TotalKillHealth = 0;
        }

        public int TatolKill;
        public int TeamPerson;
        public int TotalLevel;
        public int TotalKillHealth;

        public int AveLevel
        {
            get
            {
                return TotalLevel / TeamPerson;
            }
        }
    }

}
