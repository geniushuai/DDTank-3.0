using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Util;
using System.Reflection;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.SceneMarryRooms;

namespace Game.Server.Managers
{
    public class MarryRoomMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static ReaderWriterLock _locker = new ReaderWriterLock();

        protected static Dictionary<int,MarryRoom> _Rooms;

        protected static TankMarryLogicProcessor _processor = new TankMarryLogicProcessor();

        public static bool Init()
        {
            _Rooms = new Dictionary<int, MarryRoom>();
            CheckRoomStatus();
            return true;
        }

        private static void CheckRoomStatus()
        {
            using(PlayerBussiness db = new PlayerBussiness())
            {
                MarryRoomInfo[] roomInfos = db.GetMarryRoomInfo();

                foreach(MarryRoomInfo roomInfo in roomInfos)
                {
                    //TimeSpan usedTime = roomInfo.BreakTime - roomInfo.BeginTime;
                    //int timeLeft = roomInfo.AvailTime - usedTime.Hours;
                    
                    if(roomInfo.ServerID != GameServer.Instance.Configuration.ServerID)
                    {
                        continue;
                    }

                    TimeSpan usedTime = DateTime.Now - roomInfo.BeginTime;
                    int timeLeft = roomInfo.AvailTime * 60 - (int)usedTime.TotalMinutes;

                    if (timeLeft > 0)
                    {
                        //创建房间
                        CreateMarryRoomFromDB(roomInfo, timeLeft);
                    }
                    else
                    { 
                        db.DisposeMarryRoomInfo(roomInfo.ID);
                        GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(roomInfo.GroomID);
                        GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(roomInfo.BrideID);

                        GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(roomInfo.GroomID, false, roomInfo);
                        GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(roomInfo.BrideID, false, roomInfo);

                    }

                }
            }
        }

        public static MarryRoom[] GetAllMarryRoom()
        {
            MarryRoom[] list = null;
            _locker.AcquireReaderLock();
            try
            {
                list = new MarryRoom[_Rooms.Count];
                _Rooms.Values.CopyTo(list, 0);
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
            return list == null ? new MarryRoom[0] : list;
        }

        public static MarryRoom CreateMarryRoom(GamePlayer player, MarryRoomInfo info)
        {
            if(!player.PlayerCharacter.IsMarried)
            {
                return null;
            }
            
            MarryRoom room = null;
            DateTime beginTime = DateTime.Now;

            info.PlayerID = player.PlayerCharacter.ID;
            info.PlayerName = player.PlayerCharacter.NickName;
            if (player.PlayerCharacter.Sex == true)
            {
                info.GroomID = info.PlayerID;
                info.GroomName = info.PlayerName;
                info.BrideID = player.PlayerCharacter.SpouseID;
                info.BrideName = player.PlayerCharacter.SpouseName;
            }
            else
            {
                info.BrideID = info.PlayerID;
                info.BrideName = info.PlayerName;
                info.GroomID = player.PlayerCharacter.SpouseID;
                info.GroomName = player.PlayerCharacter.SpouseName;
            }

            info.BeginTime = beginTime;
            info.BreakTime = beginTime;

            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (db.InsertMarryRoomInfo(info))
                {
                    room = new MarryRoom(info, _processor);
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.GroomID);
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.BrideID);
                    GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.GroomID, true, info);
                    GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.BrideID, true, info);
                }
            }

            if (room != null)
            {

                _locker.AcquireWriterLock();
                try
                {
                    _Rooms.Add(room.Info.ID, room);
                }
                finally
                {
                    _locker.ReleaseWriterLock();
                }

                if (room.AddPlayer(player))
                {
                    room.BeginTimer(60 * 1000 * 60 * room.Info.AvailTime);
                    return room;
                }
            }

            return null;
        }

        public static MarryRoom CreateMarryRoomFromDB(MarryRoomInfo roomInfo,int timeLeft)
        {
            MarryRoom room = null;
            _locker.AcquireWriterLock();
            try
            {
                room = new MarryRoom(roomInfo, _processor);
                if (room != null)
                {
                    _Rooms.Add(room.Info.ID,room);

                    room.BeginTimer(60 * 1000 * timeLeft);
                    return room;
                }
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }

            return null;
        }

        public static MarryRoom GetMarryRoombyID(int id, string pwd, ref string msg)
        {
            MarryRoom room = null;
            _locker.AcquireReaderLock();
            try
            {
                if (id > 0 )
                {
                    if (_Rooms.Keys.Contains(id))
                    {
                        if (_Rooms[id].Info.Pwd != pwd)
                        {
                            msg = "Game.Server.Managers.PWDError";
                        }
                        else
                        {
                            room = _Rooms[id];
                        }
                    }
                }
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
            return room;
        }

        public static bool UpdateBreakTimeWhereServerStop()
        {
            using(PlayerBussiness db = new PlayerBussiness())
            {
               return db.UpdateBreakTimeWhereServerStop();
            }
        }

        public static void RemoveMarryRoom(MarryRoom room)
        {
            _locker.AcquireReaderLock();
            try
            {
                if(_Rooms.Keys.Contains(room.Info.ID))
                {
                    _Rooms.Remove(room.Info.ID);
                }
            }
            finally
            {
                _locker.ReleaseReaderLock();
            }
        }

        //public static void FlagPlayerRoomCreated(int playerID, bool state,MarryRoomInfo info)
        //{
            

            
        //    //int spouseID = 0;

        //    //GamePlayer player = WorldMgr.GetPlayerById(playerID);

        //    //if (player != null)
        //    //{
        //    //    player.PlayerCharacter.IsCreatedMarryRoom = state;
        //    //    if (state)
        //    //    {
        //    //        player.PlayerCharacter.SelfMarryRoomID = info.ID;
        //    //    }
        //    //    else
        //    //    {
        //    //        player.PlayerCharacter.SelfMarryRoomID = 0;
        //    //    }
        //    //    player.Out.SendMarryRoomCreated(player, state, info);
        //    //    spouseID = player.PlayerCharacter.SpouseID;
        //    //}
        //    //else
        //    //{
        //    //    using (PlayerBussiness db = new PlayerBussiness())
        //    //    {
        //    //        PlayerInfo tempPlayer = db.GetUserSingleByUserID(playerID);
        //    //        if (tempPlayer != null)
        //    //        {
        //    //            tempPlayer.IsCreatedMarryRoom = state;
        //    //            if (state)
        //    //            {
        //    //                tempPlayer.SelfMarryRoomID = info.ID;
        //    //            }
        //    //            else
        //    //            {
        //    //                tempPlayer.SelfMarryRoomID = 0;
        //    //            }

        //    //            db.UpdatePlayerMarry(tempPlayer);
        //    //            GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempPlayer.ID);

        //    //            spouseID = tempPlayer.SpouseID;
        //    //        }
        //    //    }
        //    //}

        //    //if (spouseID != 0)
        //    //{
        //    //    GamePlayer spouse = WorldMgr.GetPlayerById(spouseID);

        //    //    if (spouse != null)
        //    //    {
        //    //        spouse.PlayerCharacter.IsCreatedMarryRoom = state;
        //    //        if (state)
        //    //        {
        //    //            spouse.PlayerCharacter.SelfMarryRoomID = info.ID;
        //    //        }
        //    //        else
        //    //        {
        //    //            spouse.PlayerCharacter.SelfMarryRoomID = 0;
        //    //        }
        //    //        spouse.Out.SendMarryRoomCreated(spouse, state, info);
        //    //    }
        //    //    else
        //    //    {
        //    //        using (PlayerBussiness db = new PlayerBussiness())
        //    //        {
        //    //            PlayerInfo tempSpouse = db.GetUserSingleByUserID(spouseID);
        //    //            if (tempSpouse != null)
        //    //            {
        //    //                tempSpouse.IsCreatedMarryRoom = state;
        //    //                if (state)
        //    //                {
        //    //                    tempSpouse.SelfMarryRoomID = info.ID;
        //    //                }
        //    //                else
        //    //                {
        //    //                    tempSpouse.SelfMarryRoomID = 0;
        //    //                }

        //    //                db.UpdatePlayerMarry(tempSpouse);
        //    //                GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

    }
}
