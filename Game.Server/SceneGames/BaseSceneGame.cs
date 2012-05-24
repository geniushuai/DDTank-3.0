using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using System.Threading;
using log4net;
using Game.Base.Packets;
using System.Reflection;
using System.Collections;
using Game.Server.Packets;
using Game.Server.Packets.Client;
using Game.Server.Managers;
using Bussiness;

namespace Game.Server.SceneGames
{
    public class BaseSceneGame
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Sysnc the players dictionay
        /// </summary>
        protected ReaderWriterLock _locker;

        private bool _isChangeStyle;
        public bool IsChangeStyle
        {
            get
            {
                return _isChangeStyle;
            }
            set
            {
                _isChangeStyle = value;
            }
        }

        private bool _isHost;
        public bool IsHost
        {
            get
            {
                return _isHost;
            }
            set
            {
                _isHost = value;
            }
        }

        //protected object _syncStop = new object();

        private GamePlayer[] _userList;

        private bool[] _openState;

        private int _mapIndex;

        private IGameProcessor _processor;

        private TankData _data = new TankData();

        private int _count;

        private int _id;

        protected string _name;

        protected string _pwd;

        protected GamePlayer _player;

        private eGameState _gameState;

        private Timer _timer;

        public bool IsTakeOut = false;

        private int _classChangeMode;

        public int ClassChangeMode
        {
            get { return _classChangeMode; }
            set
            {
                _classChangeMode = value;
            }
        }

        private eGameMode _gameMode;

        public eGameMode GameMode
        {
            get { return _gameMode; }
            set
            {
                _gameMode = value;
            }
        }

        private eRoomType _roomType;

        public eRoomType RoomType
        {
            get { return _roomType; }
            set
            {
                _roomType = value;
            }
        }

        private eGameClass _gameClass;

        public eGameClass GameClass
        {
            get { return _gameClass; }
            set
            {
                //if(_gameClass != value)
                //{
                _gameClass = value;
                if (_gameClass == eGameClass.FREE)
                {
                    ScanTime = 2;
                }
                else
                {
                    ScanTime = 0;
                }

                SendRoomType();
                SendRoomSetUp();

                ClassChangeMode = 0;

                //}

            }        
        }

        private int _consortiaID;

        public int ConsortiaID
        {
            get
            {
                return _player.PlayerCharacter.ConsortiaID;
            }
            set
            {
                _consortiaID = value;
            }

        }

        private byte _scanTime;
        public byte ScanTime
        {
            get
            {
                return _scanTime;
            }
            set
            {
                _scanTime = value;
            }
        }

        private bool _isUsed;

        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            set
            {
                _isUsed = value;
            }
        }

        public int GetTime
        {
            get
            {
                switch (_scanTime)
                {
                    case 0:
                        return 5;
                    case 1:
                        return 7;
                    default:
                        return 10;
                }
            }
        }

        public bool[] OpenState
        {
            get { return _openState; }
        }

        public int MapIndex
        {
            get { return _mapIndex; }
            set { _mapIndex = value; }
        }

        public int ID
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Pwd
        {
            get
            {
                return _pwd;
            }
            set
            {
                _pwd = value;
            }
        }

        public int pairUpState;

        public int listType;

        public GamePlayer Player
        {
            get { return _player; }
            set 
            {
                if (value != null && GameState == eGameState.FREE)
                {
                    BeginTimer(60 * 1000 * 5);
                }
                _player = value; 
            }
        }

        public int DeductOffer(GamePlayer lose)
        {
            
            if (_gameState == eGameState.PLAY &&_data.Players[lose].State != TankGameState.DEAD)
            {
               
                //扣除的经验值公式：经验扣除值=12*玩家当前等级
                int GP = LevelMgr.GetGP(Player.PlayerCharacter.Grade);
                lose.SetGP(-LevelMgr.ReduceGP(lose.PlayerCharacter.Grade, lose.PlayerCharacter.GP));
                if (_roomType == eRoomType.PAIRUP)
                {
                    int offer = _gameClass == eGameClass.CONSORTIA ? 15 : 5;
                    if (lose.PlayerCharacter.Offer < offer)
                        _data.Players[lose].Offer = -lose.PlayerCharacter.Offer;
                    else
                        _data.Players[lose].Offer = -offer;

                    return offer;
                }
            }

            return -1;
        }

        public eGameState GameState
        {
            get { return _gameState; }
            set
            {
                _gameState = value;
                if (value == eGameState.FREE)
                {
                    SendRoomInfo();
                }
            }
        }

        public TankData Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        private BaseSceneGame _matchGame;
        public BaseSceneGame MatchGame
        {
            get
            {
                return _matchGame;
            }
            set
            {
                _matchGame = value;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public void Revert()
        {
            //_mapIndex = 0;
            //_teamType = eTeamType.FREE;

            bool isNew = Managers.WorldMgr.WaitingScene.Info.NewerServer ;
            //if (Managers.WorldMgr.WaitingScene.Info.NewerServer == 0)
            //{
            if (RoomType == eRoomType.PAIRUP)
            {
                for (int i = 0; i < _openState.Length; i++)
                {
                    if (isNew)
                    {
                        if (i < _openState.Length / 4)
                        {

                            _openState[i] = true;
                        }
                        else
                        {
                            _openState[i] = false;
                        }
                    }
                    else
                    {
                        if (i < _openState.Length / 2)
                        {

                            _openState[i] = true;
                        }
                        else
                        {
                            _openState[i] = false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < _openState.Length; i++)
                {
                    if (i >= 4 && isNew)
                    {
                        _openState[i] = false;
                    }
                    else
                    {
                        _openState[i] = true;
                    }
                }
            }


            _gameState = eGameState.FREE;
            _gameClass = eGameClass.FREE;
            //Data = null;
            _matchGame = null;
            _consortiaID = -1;
            pairUpState = 0;
            listType = 0;
            _classChangeMode = 0;
        }

        public int CloseTotal()
        {
            int total = 0;
            for (int i = 0; i < _openState.Length; i++)
            {
                if (!_openState[i])
                    total++;
            }
            return total;
        }

        public BaseSceneGame(int id, IGameProcessor processor)
        {
            _id = id;

            _processor = processor;

            _userList = new GamePlayer[_processor.MaxPlayerCount];

            _openState = new bool[_processor.MaxPlayerCount];

            _locker = new ReaderWriterLock();

            _count = 0;

            _gameState = eGameState.FREE;

            _roomType = eRoomType.FREE;

            Revert();
        }

        public bool AddPlayer(GamePlayer player)
        {
            int index = -1;
            //_locker.AcquireWriterLock(Timeout.Infinite);
            //try
            //{
            lock (Data._syncStop)
            {
                if (_count == _userList.Length || player.IsInGame)
                {
                    return false;
                }

                for (byte i = 0; i < _userList.Length; i++)
                {
                    if (_userList[i] == null && _openState[i])
                    {
                        _count++;
                        _userList[i] = player;
                        player.CurrentGame = this;
                        player.CurrentGameIndex = i;
                        index = i;
                        break;
                    }
                }
            }

            if (index != -1)
            {
                OperateGameClass();
                //SendRoomType();
                player.Out.SendTCP(player.Out.SendRoomInfo(_player, this));
                SendRoomInfo();
                _processor.OnAddedPlayer(this, player);
                return true;
            }
            else
            {
                log.Error(string.Format("EnterGame failed: count {0},list: {1}", _count, _userList));
                return false;
            }
            
            //}
            //finally
            //{
            //    _locker.ReleaseWriterLock();
            //}

        }

        public void RemovePlayer(GamePlayer player)
        {
            _processor.OnRemovingPlayer(this, player);
            lock (Data._syncStop)
            {
                if (_userList[player.CurrentGameIndex] == player)
                {
                    _userList[player.CurrentGameIndex] = null;
                    _count--;

                    int offer = DeductOffer(player);
                    GSPacketIn pkg = player.Out.SendGamePlayerLeave(player, offer, this);
                    SendToPlayerExceptSelf(pkg, player);

                    if (player == _player)
                    {
                        if (Count > 0)
                        {
                            for (byte i = 0; i < _userList.Length; i++)
                            {
                                if (_userList[i] != null)
                                {
                                    Player = _userList[i];
                                    GSPacketIn msgHost = player.Out.SendRoomHost(_player);
                                    SendToRoomPlayer(msgHost);
                                    break;
                                }
                            }
                        }
                    }


                    if (GameState == eGameState.PAIRUP)
                    {
                        SendPairUpCancel();
                        PairUpMgr.RemovefromPairUpMgr(this);
                        GSPacketIn msg = _player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed"));
                        SendToPlayerExceptSelf(msg, _player);
                    }

                    OperateGameClass();
                    SendRoomInfo();
                    _processor.OnRemovedPlayer(this, player);


                    player.CurrentGame = null;
                }


            }

            //if (_isChangeStyle)
            //{
            //    player.UpdateStyle();
            //}
        }

        public void BeginTimer(int interval)
        {
            if (_matchGame == null || _isHost || !_matchGame.IsHost)
            {
                if (_timer == null)
                {
                    _timer = new Timer(new TimerCallback(OnTick), null, interval, interval);
                }
                else
                {
                    _timer.Change(interval, interval);
                }
            }
            else
            {
                _matchGame.BeginTimer(interval);
            }
        }

        protected void OnTick(object obj)
        {

            _processor.OnTick(this);
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                //_timer.Dispose();
                //_timer = null;
            }
        }

        public bool CheckConsortiaSame()
        {
            if(Count < 2)
            {
                return false;
            }
            
            int tempID = _player.PlayerCharacter.ConsortiaID;
            GamePlayer[] players = GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if(p.PlayerCharacter.DutyLevel > 4)
                {
                    return false;
                }

                if(p.PlayerCharacter.ConsortiaID != 0 && tempID == p.PlayerCharacter.ConsortiaID && p.PlayerCharacter.ConsortiaLevel > 2)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public int AverageLevel()
        {
            int totalLevel = 0;
            GamePlayer[] players = GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                totalLevel += p.PlayerCharacter.Grade;
            }

            return totalLevel / Count;
        }

        public GamePlayer GetPlayerByIndex(byte index)
        {
            return _userList[index];
        }

        public GamePlayer[] GetAllPlayers()
        {
            GamePlayer[] list = null;
            //_locker.AcquireReaderLock(Timeout.Infinite);
            //try
            //{
            lock (Data._syncStop)
            {
                list = new GamePlayer[_count];
                int j = 0;
                for (int i = 0; i < _userList.Length; i++)
                {
                    if (_userList[i] != null)
                    {
                        list[j++] = _userList[i];
                    }
                }
            }
            //}
            //finally
            //{
            //    _locker.ReleaseReaderLock();
            //}
            return list;
        }

        public int GetReadyPlayerCount()
        {
            int readyCount = 0;
            GamePlayer[] player = GetAllPlayers();
            foreach (GamePlayer p in player)
            {
                if (p == _player)
                    continue;
                if (p.CurrentGameState == ePlayerGameState.READY)
                {
                    readyCount++;
                }
            }
            return readyCount;
        }

        public void SendToRoomPlayer(GSPacketIn packet) 
        {
            GamePlayer[] player;

            player = GetAllPlayers();

            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void SendToRoomPlayer(string msg)
        {
            GamePlayer[] player;

            player = GetAllPlayers();

            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    p.Out.SendMessage(eMessageType.ChatNormal, msg);
                }
            }
        }

        public void SendToAll(GSPacketIn packet)
        {
            SendToAll(packet,null,false);
        }

        public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
        {
            //SendToPlayer(packet);
            GamePlayer[] player;

            if (GameState == eGameState.FREE || GameState == eGameState.PAIRUP)
            {
                player = GetAllPlayers();
            }
            else
            {
                player = Data.GetAllPlayers();
            }

            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    if (isChat && p.IsBlackFriend(self.PlayerCharacter.ID))
                        continue;

                    p.Out.SendTCP(packet);
                }
            }
        }

        public void SendToTeam(GSPacketIn packet,GamePlayer self,bool isChat)
        {
            GamePlayer[] player;

            if (GameState == eGameState.FREE || GameState == eGameState.PAIRUP)
            {
                player = GetAllPlayers();
            }
            else
            {
                player = Data.GetAllPlayers();
            }


            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    if (isChat && p.IsBlackFriend(self.PlayerCharacter.ID))
                        continue;

                    if (p.CurrentTeamIndex == self.CurrentTeamIndex)
                        p.Out.SendTCP(packet);
                }
            }
        }

        public void SendToPlayerExceptSelf(GSPacketIn packet,GamePlayer self)
        {
            GamePlayer[] player;

            if (GameState == eGameState.FREE || GameState == eGameState.PAIRUP)
            {
                player = GetAllPlayers();
            }
            else
            {
                player = Data.GetAllPlayers();
            }

            
            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    if (p != self)
                        p.Out.SendTCP(packet);
                }
            }
        }

        public void SendToScenePlayer(GSPacketIn packet)
        {
            WorldMgr.WaitingScene.SendToALL(packet);
        }

        public void Start()
        {
            lock (Data._syncStop)
            {
                if (GameMode == eGameMode.FLAG)
                    SetCaptain();

                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_START);

                _gameState = eGameState.PLAY;
                //IsTakeOut = false;

                if (RoomType == eRoomType.PAIRUP && MatchGame != null)
                {
                    MatchGame.GameState = eGameState.PLAY;
                    //MatchGame.IsTakeOut = false;
                }

                _processor.OnStarting(this, pkg);

                SendToAll(pkg);

                _processor.OnStarted(this);
            }
        }

        public void Stop()
        {
            lock (Data._syncStop)
            {
                if (GameState != eGameState.LOAD && GameState != eGameState.PLAY)
                    return;

                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_OVER);
                _processor.OnStopping(this, pkg);

                SendToAll(pkg);

                _gameState = eGameState.OVER;

                if (RoomType == eRoomType.PAIRUP && MatchGame != null)
                {
                    MatchGame.GameState = eGameState.OVER;
                }

                _data.CurrentMap = null;

                //_data._fallItemID.Clear();
                _data.FlagPlayer.Clear();

                foreach (GamePlayer player in _data.GetAllPlayers())
                {
                    //player.UpdateStyle();
                    player.OnGameStop();
                    //player.Out.SendCheckCode();
                }

                _processor.OnStopped(this);

                //if(!_data.killFlag)
                //{
                //    if (MatchGame != null)
                //    {
                //        ShowArk(MatchGame, null);
                //    }
                //    ShowArk(this, null);
                //}

                if (MatchGame != null && !MatchGame.IsTakeOut)
                {
                    ShowArk(MatchGame, null);
                }
                if (!IsTakeOut)
                {
                    ShowArk(this, null);
                }

            }
        }

        public bool CanStartGame(GamePlayer player,GSPacketIn data)
        {
            lock (Data._syncStop)
            {
                if (player == _player && _gameState == eGameState.FREE)
                {
                    if (_processor.OnCanStartGame(this, player))
                    {
                        _processor.InitGame(this);
                        
                        BeginTimer(70 * 1000);
                        Data.StartReset(this);
                        //StopTimer();
                        _gameState = eGameState.LOAD;
                        _isChangeStyle = false;
                        SendRoomInfo();

                        IsTakeOut = false;
                        Data.MapType = eMapType.Normal;
                        Data.MapIndex = Managers.MapMgr.GetMapIndex(MapIndex, (byte)eMapType.Normal);
                        GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_LOAD);
                        pkg.WriteInt(Data.MapIndex);
                        pkg.WriteInt(Data.StartedGameClass);
                        pkg.WriteBoolean(false);
                        SendToAll(pkg);
                        return true;
                    }
                }
                return false;
            }
        }

        public bool CanStartPairUpGame(GamePlayer player, GSPacketIn data)
        {
            lock (Data._syncStop)
            {
                if (player == _player && _gameState == eGameState.FREE)
                {
                    if (_processor.OnCanStartPairUpGame(this, player))
                    {
                        _isChangeStyle = false;
                        IsTakeOut = false;
                        return true;
                    }
                }
                return false;
            }
        }

        public void BeginPairUpLoad()
        {
            IsHost = true;
            BeginTimer(70 * 1000);
            Data.StartReset(this);
            //StopTimer();
            _gameState = eGameState.LOAD;
            SendRoomInfo();
            if (RoomType == eRoomType.PAIRUP && MatchGame != null)
            {
                //MatchGame.BeginTimer(70 * 1000);
                MatchGame.StopTimer();
                MatchGame.GameState = eGameState.LOAD;
                MatchGame.IsHost = false;
            }

            Data.MapIndex = Managers.MapMgr.GetMapIndex(MapIndex,(byte)eMapType.PairUp);
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_LOAD);
            pkg.WriteInt(Data.MapIndex);
            pkg.WriteInt(Data.StartedGameClass);

            _isChangeStyle = Managers.FightRateMgr.CanChangeStyle(this,pkg);
            Data.IsChangeStyle = _isChangeStyle;
            Data.ChangeTeam = _player.CurrentTeamIndex;
            Data.MapType = eMapType.PairUp;

            SendToAll(pkg);
        }

        public void ProcessData(GamePlayer player, GSPacketIn data)
        {
            lock (Data._syncStop)
            {
                //if (_gameState != eGameState.FREE)
                if (_gameState == eGameState.PLAY || _gameState == eGameState.LOAD || _gameState == eGameState.OVER)
                {
                    _processor.OnGameData(this, player, data);
                }
            }
        }

        public void ReturnPacket(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClientID = player.PlayerCharacter.ID;
            SendToAll(pkg);
        }

        internal void OnPlayerStateChange(GamePlayer player)
        {
            GamePlayer[] list;
            if (player.CurrentGameState == ePlayerGameState.FINISH && Data != null)
            {
                list = Data.GetAllPlayers();
            }
            else
            {
                list = GetAllPlayers();
            }
            
            GSPacketIn pkg = null;

            foreach (GamePlayer p in list)
            {
                if (pkg == null)
                {
                    pkg = p.Out.SendGamePlayerChangedState(player);
                }
                else
                {
                    p.Out.SendTCP(pkg);
                }
            }
            _processor.OnPlayerStateChanged(this, player);
        }

        internal void OnPlayerTeamChange(GamePlayer player)
        {

            GamePlayer[] list = GetAllPlayers();
            GSPacketIn pkg = null;

            foreach (GamePlayer p in list)
            {
                if (pkg == null)
                {
                    pkg = p.Out.SendGamePlayerChangedTeam(player);
                }
                else
                {
                    p.Out.SendTCP(pkg);
                }
            }
            _processor.OnPlayerTeamChanged(this, player);
        }

        public bool ChangeTeam(GamePlayer player)
        {
            //_locker.AcquireWriterLock(Timeout.Infinite);

            //try
            //{
            lock (Data._syncStop)
            {
                for (byte i = (byte)(player.CurrentTeamIndex % 2); i < _userList.Length; i += 2)
                {
                    if (_userList[i] == null && _openState[i])
                    {
                        _userList[player.CurrentGameIndex] = null;
                        _userList[i] = player;
                        player.CurrentGameIndex = i;
                        return true;
                    }
                }
            }
            //}
            //finally
            //{
            //    _locker.ReleaseWriterLock();
            //}
            return false;
        }

        public bool KickPlayerIndex(GamePlayer player, byte index)
        {
            GamePlayer kickPlayer = GetPlayerByIndex(index);
            if (kickPlayer != null && kickPlayer != player)
            {
                if (kickPlayer.BuffInventory.IsKickProtect())
                {
                    player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Game.Server.SceneGames.Protect"));
                    return false;
                }

                RemovePlayer(kickPlayer);
                kickPlayer.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));

                GSPacketIn pkg = player.Out.SendMessage(eMessageType.ChatERROR, kickPlayer.PlayerCharacter.NickName+ "  " + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2"));
                player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
                return true;
            }

            SendRoomInfo();
            return false;
        }

        public void SendRoomInfo()
        {
            GSPacketIn msgRoom = _player.Out.SendRoomInfo(_player, this);
            SendToScenePlayer(msgRoom);
        }

        public void SendRoomType()
        {
            GSPacketIn msgRoomType = _player.Out.SendRoomType(_player, this);
            SendToPlayerExceptSelf(msgRoomType, _player);
            //SendToScenePlayer(msgRoomType);
        }

        public void SendRoomSetUp()
        {
            GSPacketIn msgRoomSetUp = _player.Out.SendRoomSetUp(_player, this);
            SendToPlayerExceptSelf(msgRoomSetUp, _player);
            //SendToScenePlayer(msgRoomType);
        }

        public void SendPairUpCancel()
        {
            GSPacketIn msg = _player.Out.SendPairUpCancel(_player, this);
            SendToPlayerExceptSelf(msg, _player);
            //SendToScenePlayer(msgRoomType);
        }

        public void ShowArk(BaseSceneGame game, GamePlayer player)
        {
            _processor.OnShowArk(game, player);
        }

        public void SetCaptain()
        {
            List<int> list = new List<int>();
            ThreadSafeRandom random = new ThreadSafeRandom();
            GamePlayer[] players = Data.FlagPlayer.ToArray();
            int rand = random.Next(players.Length);
            for (int i = rand; i < players.Length + rand; i++)
            {
                if (Data.Players[players[i % players.Length]].State == TankGameState.LOSE)
                    continue;

                if (players[i % players.Length].CurrentTeamIndex != 0)
                {
                    if (!list.Contains(players[i % players.Length].CurrentTeamIndex))
                    {
                        list.Add(players[i % players.Length].CurrentTeamIndex);
                        Data.Players[players[i % players.Length]].IsCaptain = true;
                    }
                    else
                    {
                        Data.Players[players[i % players.Length]].IsCaptain = false;
                    }
                }
                else
                {
                    list.Add(0);
                    Data.Players[players[i % players.Length]].IsCaptain = true;
                }
            }

            players = Data.Players.Keys.ToArray();
            rand = random.Next(players.Length);
            for (int i = rand; i < players.Length + rand; i++)
            {
                if (Data.Players[players[i % players.Length]].State == TankGameState.LOSE)
                    continue;

                if (!list.Contains(players[i % players.Length].CurrentTeamIndex))
                {
                    list.Add(players[i % players.Length].CurrentTeamIndex);
                    Data.Players[players[i % players.Length]].IsCaptain = true;
                }
            }


        }

        public void SendPlayFinish( GamePlayer player)
        {
            _processor.SendPlayFinish(this, player);
        }

        public bool CanStopGame()
        {
            return _processor.CanStopGame(this, Data);
        }

        public void SendPairUpWait()
        {
            _processor.SendPairUpWait(this);
        }

        public void SendPairUpFailed()
        {
            _processor.SendPairUpFailed(this);
        }

        public void OperateGameClass()
        {
            if(RoomType == eRoomType.PAIRUP)
            {
                if(GameState == eGameState.FREE || GameState == eGameState.PAIRUP)
                {
                    if (CheckConsortiaSame())
                    {
                        GameClass = eGameClass.CONSORTIA;
                    }
                    else
                    {
                        GameClass = eGameClass.FREE;
                    }
                }
            }
        }

    }
}
