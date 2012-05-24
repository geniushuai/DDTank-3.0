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
using SqlDataProvider.Data;

namespace Game.Server.SceneMarryRooms
{
    public class MarryRoom
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static object _syncStop = new object();

        private List<GamePlayer> _guestsList;

        private IMarryProcessor _processor;

        private int _count;

        public MarryRoomInfo Info;

        private eRoomState _roomState;

        public eRoomState RoomState
        {
            get
            {
                return _roomState;
            }
            set
            {
                if (_roomState != value)
                {
                    _roomState = value;
                    SendMarryRoomInfoUpdateToScenePlayers(this);
                }                

                //if (_roomState == eRoomState.Hymeneal)
                //{
                //    _userRemoveList.Clear();
                //}
            }
        }

        private Timer _timer;

        private Timer _timerForHymeneal;

        private List<int> _userForbid;

        private List<int> _userRemoveList;

        public int Count
        {
            get { return _count; }
        }

        //private bool _isHymeneal;
        //public bool IsHymeneal
        //{
        //    get { return _isHymeneal; }
        //    set { _isHymeneal = value; }
        //}

        public MarryRoom(MarryRoomInfo info, IMarryProcessor processor)
        {
            Info = info;

            _processor = processor;

            _guestsList = new List<GamePlayer>();

            _count = 0;

            //_isHymeneal = false;

            _roomState = eRoomState.FREE;

            _userForbid = new List<int>();

            _userRemoveList = new List<int>();

       }

        public bool AddPlayer(GamePlayer player)
        {
            lock (_syncStop)
            {
                if( player.CurrentRoom!=null || player.IsInMarryRoom)
                {
                    return false;
                }
                
                if ( _guestsList.Count > Info.MaxCount)
                {
                    player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoom.Msg1"));
                    return false;
                }

                _count++;
                _guestsList.Add(player);
                player.CurrentMarryRoom = this;
                player.MarryMap = 1;

                if(player.CurrentRoom != null)
                {
                    player.CurrentRoom.RemovePlayerUnsafe(player);
                }
            }
            return true;
        }

        public void RemovePlayer(GamePlayer player)
        {
            lock (_syncStop)
            {
                if(RoomState == eRoomState.FREE)
                {
                    _count--;

                    _guestsList.Remove(player);

                    GSPacketIn pkg = player.Out.SendPlayerLeaveMarryRoom(player);
                    //0 player.CurrentMarryRoom.SendToPlayerExceptSelf(pkg, player);
                    player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(pkg, player);
                    player.CurrentMarryRoom = null;
                    player.MarryMap = 0;
                }
                else if(RoomState == eRoomState.Hymeneal)
                {
                    _userRemoveList.Add(player.PlayerCharacter.ID);

                    _count--;

                    _guestsList.Remove(player);

                    player.CurrentMarryRoom = null;
                }

                SendMarryRoomInfoUpdateToScenePlayers(this);
            }
        }

        public void BeginTimer(int interval)
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

        protected void OnTick(object obj)
        {

            _processor.OnTick(this);
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                //_timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }
        }

        public void BeginTimerForHymeneal(int interval)
        {
            if (_timerForHymeneal == null)
            {
                _timerForHymeneal = new Timer(new TimerCallback(OnTickForHymeneal), null, interval, interval);
            }
            else
            {
                _timerForHymeneal.Change(interval, interval);
            }
        }

        protected void OnTickForHymeneal(object obj)
        {
            try
            {
                _roomState = eRoomState.FREE;

                GSPacketIn pkg = new GSPacketIn((short)ePackageType.MARRY_CMD);
                pkg.WriteByte((byte)MarryCmdType.HYMENEAL_STOP);
                //0 SendToAll(pkg);
                //0 SendToAllForScene(pkg,1);
                SendToAll(pkg);

                StopTimerForHymeneal();
                SendUserRemoveLate();

                SendMarryRoomInfoUpdateToScenePlayers(this);
            }
            catch(Exception ex)
            {
                if(log.IsErrorEnabled)
                {
                    log.Error("OnTickForHymeneal",ex);
                }
            }
        }

        public void StopTimerForHymeneal()
        {
            if (_timerForHymeneal != null)
            {
                //_timerForHymeneal.Change(Timeout.Infinite, Timeout.Infinite);
                _timerForHymeneal.Dispose();
                _timerForHymeneal = null;
            }
        }

        public GamePlayer[] GetAllPlayers()
        {
            lock (_syncStop)
            {
                return _guestsList.ToArray();                
            }
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

        public void SendToAll(GSPacketIn packet)
        {
            SendToAll(packet,null,false);
        }

        public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
        {
            GamePlayer[] player;
            player = GetAllPlayers();

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

        //public void SendToAllForScene(GSPacketIn packet)
        //{
        //    SendToAllForScene(packet, null);
        //}

        public void SendToAllForScene(GSPacketIn packet, int sceneID)
        {
            GamePlayer[] player;
            player = GetAllPlayers();

            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    if (p.MarryMap == sceneID)
                    {
                        p.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToPlayerExceptSelf(GSPacketIn packet,GamePlayer self)
        {
            GamePlayer[] player;
            player = GetAllPlayers();
            
            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    if (p != self)
                        p.Out.SendTCP(packet);
                }
            }
        }

        public void SendToPlayerExceptSelfForScene(GSPacketIn packet, GamePlayer self)
        {
            GamePlayer[] player;
            player = GetAllPlayers();

            if (player != null)
            {
                foreach (GamePlayer p in player)
                {
                    if (p != self)
                    { 
                        if(p.MarryMap == self.MarryMap)
                        {
                            p.Out.SendTCP(packet);
                        }
                    }
                        
                }
            }
        }

        public void SendToScenePlayer(GSPacketIn packet)
        {
            WorldMgr.MarryScene.SendToALL(packet);
        }


        public void ProcessData(GamePlayer player, GSPacketIn data)
        {
            lock (_syncStop)
            {
                _processor.OnGameData(this, player, data);
            }
        }

        public void ReturnPacket(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClientID = player.PlayerCharacter.ID;
            SendToPlayerExceptSelf(pkg, player);
            //SendToAll(pkg);
        }

        public void ReturnPacketForScene(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClientID = player.PlayerCharacter.ID;
            SendToPlayerExceptSelfForScene(pkg, player);
            //SendToAll(pkg);
        }

        public bool KickPlayerByUserID(GamePlayer player, int userID)
        {
            GamePlayer kickPlayer = GetPlayerByUserID(userID);
            if (kickPlayer != null && kickPlayer.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID
                && kickPlayer.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
            {
                RemovePlayer(kickPlayer);
                kickPlayer.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));

                GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatERROR, kickPlayer.PlayerCharacter.NickName+ "  " + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2"));
                player.CurrentMarryRoom.SendToPlayerExceptSelf(msg, player);

                return true;
            }
            return false;
        }

        public void KickAllPlayer()
        {
            GamePlayer[] players = GetAllPlayers();
            foreach(GamePlayer p in players)
            {
                RemovePlayer(p);
                p.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.TimeOver"));
            }
        }

        public GamePlayer GetPlayerByUserID(int userID)
        {
            lock (_syncStop)
            {
                foreach (GamePlayer p in _guestsList)
                {
                    if (p.PlayerCharacter.ID == userID)
                    {
                        return p;
                    }
                }
            }
            return null;
        }

        public void RoomContinuation(int time)
        { 
            TimeSpan timeLeft = DateTime.Now - Info.BeginTime;
            int newTime = Info.AvailTime * 60 - timeLeft.Minutes + time * 60;
            Info.AvailTime += time;
            //更新数据库
            using(PlayerBussiness db = new PlayerBussiness())
            {
                db.UpdateMarryRoomInfo(Info);
            }
            BeginTimer(60 * 1000 * newTime);

        }

        public void SetUserForbid(int userID)
        {
            lock(_syncStop)
            {
                _userForbid.Add(userID);
            }

            return;
        }

        public bool CheckUserForbid(int userID)
        {
            lock (_syncStop)
            {
                return _userForbid.Contains(userID);
            }
        }

        public void SendUserRemoveLate()
        {
            lock (_syncStop)
            {
                foreach(int userID in _userRemoveList)
                {
                    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.PLAYER_EXIT_MARRY_ROOM, userID);
                    //0 SendToAll(pkg);
                    SendToAllForScene(pkg,1);
                }

                _userRemoveList.Clear();
            }

            return;
        }

        public GSPacketIn SendMarryRoomInfoUpdateToScenePlayers(MarryRoom room)
        {
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.MARRY_ROOM_UPDATE);
            bool result = room != null;
            pkg.WriteBoolean(result);
            if (result)
            {
                pkg.WriteInt(room.Info.ID);
                pkg.WriteBoolean(room.Info.IsHymeneal);
                pkg.WriteString(room.Info.Name);
                pkg.WriteBoolean(room.Info.Pwd == "" ? false : true);
                pkg.WriteInt(room.Info.MapIndex);
                pkg.WriteInt(room.Info.AvailTime);
                pkg.WriteInt(room.Count);
                pkg.WriteInt(room.Info.PlayerID);
                pkg.WriteString(room.Info.PlayerName);
                pkg.WriteInt(room.Info.GroomID);
                pkg.WriteString(room.Info.GroomName);
                pkg.WriteInt(room.Info.BrideID);
                pkg.WriteString(room.Info.BrideName);
                pkg.WriteDateTime(room.Info.BeginTime);
                pkg.WriteByte((byte)room.RoomState);
                pkg.WriteString(room.Info.RoomIntroduction);
            }

            SendToScenePlayer(pkg);
            return pkg;
        }


    }
}
