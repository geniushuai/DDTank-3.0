using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using System.Timers;
using System.IO;
using log4net;
using System.Reflection;
using System.Configuration;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;

namespace Game.Server.Statics
{
    public class StatMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static object _syncStop = new object();

        public static bool TxtRecord
        {
            get
            {
                return GameServer.Instance.Configuration.TxtRecord;
            }
        }

        public static int ServerID
        {
            get
            {
                return GameServer.Instance.Configuration.ServerId;
            }
        }

        public static int GameType
        {
            get
            {
                return GameServer.Instance.Configuration.GameType;
            }
        }

        public static int AreaID
        {
            get
            {
                return GameServer.Instance.Configuration.AreaID;
            }
        }

        public static string LogPath
        {
            get
            {
                return GameServer.Instance.Configuration.LogPath;
            }
        }

        private static int _gameType;

        private static int _serverId;

        private static int _areaId;

        private static string _logPath;

        private static string _headStr;

        private static string _record;

        private static string _errorRecord;

        public static Dictionary<GoldAddType, long> GoldAdd;

        public static Dictionary<GoldRemoveType, long> GoldRemove;

        public static Dictionary<MoneyAddType, long> MoneyAdd;

        public static Dictionary<MoneyRemoveType, long> MoneyRemove;

        public static Dictionary<GifttokenAddType, long> GifttokenAdd;

        public static Dictionary<GifttokenRemoveType, long> GifttokenRemove;

        public static Dictionary<OfferAddType, long> OfferAdd;

        public static Dictionary<OfferRemoveType, long> OfferRemove;

        public static Dictionary<int, Dictionary<ItemAddType, long>> ItemAdd;

        public static Dictionary<int, Dictionary<ItemRemoveType, long>> ItemRemove;
 


        public static List<string> gameLogs;

        public static List<string> useLogs;

        public static List<string> errorPlayerLogs;

        private static List<string> goldLogCashe;

        private static List<string> moneyLogCashe;

        private static List<string> offerLogCashe;

        private static List<string> gifttokenLogCashe;

        private static List<string> itemLogCashe;

        private static List<string> gameLogCashe;

        private static List<string> UseLogCashe;

        private static List<string> ServerStateCashe;


        public static bool Setup()
        {
            return Setup(GameType, AreaID, ServerID, LogPath);
        }

        public static bool Setup(int gametype, int areaid, int serverid, string logpath)
        {
            _gameType = gametype;
            _serverId = serverid;
            _areaId = areaid;
            _logPath = logpath;
            _record = "Record";
            _errorRecord = "ErrorRecord";

            _headStr = string.Format("{0},{1},{2}", gametype, areaid, serverid);

            GoldAdd = new Dictionary<GoldAddType, long>();
            foreach (object obj in Enum.GetValues(typeof(GoldAddType)))
            {
                GoldAdd.Add((GoldAddType)obj, 0);
            }

            GoldRemove = new Dictionary<GoldRemoveType, long>();
            foreach (object obj in Enum.GetValues(typeof(GoldRemoveType)))
            {
                GoldRemove.Add((GoldRemoveType)obj, 0);
            }

            MoneyAdd = new Dictionary<MoneyAddType, long>();
            foreach (object obj in Enum.GetValues(typeof(MoneyAddType)))
            {
                MoneyAdd.Add((MoneyAddType)obj, 0);
            }

            MoneyRemove = new Dictionary<MoneyRemoveType, long>();
            foreach (object obj in Enum.GetValues(typeof(MoneyRemoveType)))
            {
                MoneyRemove.Add((MoneyRemoveType)obj, 0);
            }

            OfferAdd = new Dictionary<OfferAddType, long>();
            foreach (object obj in Enum.GetValues(typeof(OfferAddType)))
            {
                OfferAdd.Add((OfferAddType)obj, 0);
            }

            OfferRemove = new Dictionary<OfferRemoveType, long>();
            foreach (object obj in Enum.GetValues(typeof(OfferRemoveType)))
            {
                OfferRemove.Add((OfferRemoveType)obj, 0);
            }

            GifttokenAdd = new Dictionary<GifttokenAddType, long>();
            foreach (object obj in Enum.GetValues(typeof(GifttokenAddType)))
            {
                GifttokenAdd.Add((GifttokenAddType)obj, 0);
            }

            GifttokenRemove = new Dictionary<GifttokenRemoveType, long>();
            foreach (object obj in Enum.GetValues(typeof(GifttokenRemoveType)))
            {
                GifttokenRemove.Add((GifttokenRemoveType)obj, 0);
            }

            ItemAdd = new Dictionary<int, Dictionary<ItemAddType, long>>();
            ItemRemove = new Dictionary<int, Dictionary<ItemRemoveType, long>>();
            gameLogs = new List<string>();
            useLogs = new List<string>();
            errorPlayerLogs = new List<string>();

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }

            if (!Directory.Exists(_record))
            {
                Directory.CreateDirectory(_record);
            }
            if (!Directory.Exists(_errorRecord))
            {
                Directory.CreateDirectory(_errorRecord);
            }

            goldLogCashe = new List<string>();
            moneyLogCashe = new List<string>();
            offerLogCashe = new List<string>();
            gifttokenLogCashe = new List<string>();
            itemLogCashe = new List<string>();
            gameLogCashe = new List<string>();
            UseLogCashe = new List<string>();
            ServerStateCashe = new List<string>();

            return true;
        }

        public static void Reset()
        {
            lock (GoldAdd)
            {
                foreach (object obj in Enum.GetValues(typeof(GoldAddType)))
                {
                    GoldAdd[(GoldAddType)obj] = 0;
                }
            }

            lock (GoldRemove)
            {
                foreach (object obj in Enum.GetValues(typeof(GoldRemoveType)))
                {
                    GoldRemove[(GoldRemoveType)obj] = 0;
                }
            }

            lock (MoneyAdd)
            {
                foreach (object obj in Enum.GetValues(typeof(MoneyAddType)))
                {
                    MoneyAdd[(MoneyAddType)obj] = 0;
                }
            }


            lock (MoneyRemove)
            {
                foreach (object obj in Enum.GetValues(typeof(MoneyRemoveType)))
                {
                    MoneyRemove[(MoneyRemoveType)obj] = 0;
                }
            }

            lock (OfferAdd)
            {
                foreach (object obj in Enum.GetValues(typeof(OfferAddType)))
                {
                    OfferAdd[(OfferAddType)obj] = 0;
                }
            }

            lock (OfferRemove)
            {
                foreach (object obj in Enum.GetValues(typeof(OfferRemoveType)))
                {
                    OfferRemove[(OfferRemoveType)obj] = 0;
                }
            }

            lock (GifttokenAdd)
            {
                foreach (object obj in Enum.GetValues(typeof(GifttokenAddType)))
                {
                    GifttokenAdd[(GifttokenAddType)obj] = 0;
                }
            }

            lock (GifttokenRemove)
            {
                foreach (object obj in Enum.GetValues(typeof(GifttokenAddType)))
                {
                    GifttokenRemove[(GifttokenRemoveType)obj] = 0;
                }
            }


            lock (ItemAdd)
            {
                foreach (KeyValuePair<int, Dictionary<ItemAddType, long>> kp in ItemAdd)
                {
                    foreach (object obj in Enum.GetValues(typeof(ItemAddType)))
                    {
                        kp.Value[(ItemAddType)obj] = 0;
                    }
                }
            }

            lock (ItemRemove)
            {
                foreach (KeyValuePair<int, Dictionary<ItemRemoveType, long>> kp in ItemRemove)
                {
                    foreach (object obj in Enum.GetValues(typeof(ItemRemoveType)))
                    {
                        kp.Value[(ItemRemoveType)obj] = 0;
                    }
                }
            }

            lock (gameLogs)
            {
                gameLogs.Clear();
            }
        }

        public static void Save()
        {
            DateTime dt = DateTime.Now;

            lock (_syncStop)
            {
                SaveGoldLog(dt);
                SaveMoneyLog(dt);                
                SaveItemLog(dt);
                SaveGameLog(dt);
            }

            Reset();

            SaveUseLog();
            SaveServerState();
            SaveErrorPlayerLog();
        }

        private static void SaveGoldLog(DateTime dt)
        {
            try
            {
                lock (GoldAdd)
                {
                    foreach (GoldAddType type in GoldAdd.Keys)
                    {
                        if (GoldAdd[type] == 0)
                            continue;
                        goldLogCashe.Add(string.Format("{0},add,{1},{2},{3}", _headStr, dt, type, GoldAdd[type]));
                    }
                }
                lock (GoldRemove)
                {
                    foreach (GoldRemoveType type in GoldRemove.Keys)
                    {
                        if (GoldRemove[type] == 0)
                            continue;
                        goldLogCashe.Add(string.Format("{0},remove,{1},{2},{3}", _headStr, dt, type, GoldRemove[type]));
                    }
                }

                //gold log
                //FileStream fs;
                string file = string.Format("{0}\\gold-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);

                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (goldLogCashe.Count != 0)
                        {
                            writer.WriteLine(goldLogCashe[0]);
                            goldLogCashe.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }
        }

        private static void SaveMoneyLog(DateTime dt)
        {
            try
            {
                lock (MoneyAdd)
                {
                    foreach (MoneyAddType type in MoneyAdd.Keys)
                    {
                        if (MoneyAdd[type] == 0)
                            continue;
                        moneyLogCashe.Add(string.Format("{0},add,{1},{2},{3}", _headStr, dt, type, MoneyAdd[type]));
                    }
                }
                lock (MoneyRemove)
                {
                    foreach (MoneyRemoveType type in MoneyRemove.Keys)
                    {
                        if (MoneyRemove[type] == 0)
                            continue;
                        moneyLogCashe.Add(string.Format("{0},remove,{1},{2},{3}", _headStr, dt, type, MoneyRemove[type]));
                    }
                }

                //money log
                string file = string.Format("{0}\\money-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (moneyLogCashe.Count != 0)
                        {
                            writer.WriteLine(moneyLogCashe[0]);
                            moneyLogCashe.RemoveAt(0);
                        }
                        //writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }
        }

        private static void SaveOfferLog(DateTime dt)
        {
            try
            {
                lock (OfferAdd)
                {
                    foreach (OfferAddType type in OfferAdd.Keys)
                    {
                        if (OfferAdd[type] == 0)
                            continue;
                        offerLogCashe.Add(string.Format("{0},add,{1},{2},{3}", _headStr, dt, type, OfferAdd[type]));
                    }
                }
                lock (OfferRemove)
                {
                    foreach (OfferRemoveType type in OfferRemove.Keys)
                    {
                        if (OfferRemove[type] == 0)
                            continue;
                        offerLogCashe.Add(string.Format("{0},remove,{1},{2},{3}", _headStr, dt, type, OfferRemove[type]));
                    }
                }

                //offer log
                string file = string.Format("{0}\\offer-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (offerLogCashe.Count != 0)
                        {
                            writer.WriteLine(offerLogCashe[0]);
                            offerLogCashe.RemoveAt(0);
                        }
                        //writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }
        }

        private static void SaveGifttokenLog(DateTime dt)
        {
            try
            {
                lock (GifttokenAdd)
                {
                    foreach (GifttokenAddType type in GifttokenAdd.Keys)
                    {
                        if (GifttokenAdd[type] == 0)
                            continue;
                        
                        gifttokenLogCashe.Add(string.Format("{0},add,{1},{2},{3}", _headStr, dt, type,GifttokenAdd[type]));
                    }
                }
                lock (GifttokenRemove)
                {
                    foreach (GifttokenRemoveType type in GifttokenRemove.Keys)
                    {
                        if (GifttokenRemove[type] == 0)
                            continue;
                        gifttokenLogCashe.Add(string.Format("{0},remove,{1},{2},{3}", _headStr, dt, type, GifttokenRemove[type]));
                    }
                }

                //Gifttoken log
                string file = string.Format("{0}\\gifttokenr-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (gifttokenLogCashe.Count != 0)
                        {
                            writer.WriteLine(gifttokenLogCashe[0]);
                            gifttokenLogCashe.RemoveAt(0);
                        }
                        //writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }
        }


        private static void SaveItemLog(DateTime dt)
        {
            try
            {
                lock (ItemAdd)
                {
                    foreach (KeyValuePair<int, Dictionary<ItemAddType, long>> kp in ItemAdd)
                    {
                        foreach (KeyValuePair<ItemAddType, long> p in kp.Value)
                        {
                            if (p.Value == 0)
                                continue;
                            itemLogCashe.Add(string.Format("{0},add,{1},{2},{3},{4}", _headStr, dt, kp.Key, p.Key, p.Value));
                        }
                    }
                }
                lock (ItemRemove)
                {
                    foreach (KeyValuePair<int, Dictionary<ItemRemoveType, long>> kp in ItemRemove)
                    {
                        foreach (KeyValuePair<ItemRemoveType, long> p in kp.Value)
                        {
                            if (p.Value == 0)
                                continue;
                            itemLogCashe.Add(string.Format("{0},remove,{1},{2},{3},{4}", _headStr, dt, kp.Key, p.Key, p.Value));
                        }
                    }
                }

                //item log
                string file = string.Format("{0}\\items-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (itemLogCashe.Count != 0)
                        {
                            writer.WriteLine(itemLogCashe[0]);
                            itemLogCashe.RemoveAt(0);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }
        }

        private static void SaveGameLog(DateTime dt)
        {
            try
            {
                lock (gameLogs)
                {
                    foreach (string s in gameLogs)
                    {
                        gameLogCashe.Add(s);
                    }
                }

                //game log
                string file = string.Format("{0}\\game-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                //using (FileStream fs = File.Open(file, FileMode.OpenOrCreate))
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (gameLogCashe.Count != 0)
                        {
                            writer.WriteLine(gameLogCashe[0]);
                            gameLogCashe.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }
        }

        public static void SaveUseLog()
        {
            DateTime dt = DateTime.Now;

            lock (useLogs)
            {
                foreach (string s in useLogs)
                {
                    UseLogCashe.Add(s);
                }
                useLogs.Clear();
            }

            //use log
            try
            {
                string file = string.Format("{0}\\use-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _record, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (UseLogCashe.Count != 0)
                        {
                            writer.WriteLine(UseLogCashe[0]);
                            UseLogCashe.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }


            //lock (useLogs)
            //{
            //    useLogs.Clear();
            //}
        }

        public static void SaveErrorPlayerLog()
        {
            DateTime dt = DateTime.Now;
            try
            {
                string file = string.Format("{0}\\ErrorPlayer-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _errorRecord, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        lock (errorPlayerLogs)
                        {
                            foreach (string s in errorPlayerLogs)
                            {
                                writer.WriteLine(s);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("SaveErrorPlayerLog log error", ex);
            }


            lock (errorPlayerLogs)
            {
                errorPlayerLogs.Clear();
            }
        }

        public static void SaveServerState()
        {
            DateTime dt = DateTime.Now;

            GameClient[] list = GameServer.Instance.GetAllClients();
            int clientCount = list == null ? 0 : list.Length;

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            int playerCount = players == null ? 0 : players.Length;

            List<BaseRoom> rooms = RoomMgr.GetAllUsingRoom();
            int roomCount = 0;
            int gameCount = 0;
            foreach (BaseRoom r in rooms)
            {
                if (!r.IsEmpty)
                {
                    roomCount++;
                    if (r.IsPlaying)
                    {
                        gameCount++;
                    }
                }
            }
         
            long memoryCount = GC.GetTotalMemory(false);

            ServerStateCashe.Add(string.Format("{0},{1},{2},{3},{4},{5},{6}", _headStr, dt, clientCount, playerCount, roomCount, gameCount, memoryCount));

            try
            {
                string file = string.Format("{0}\\serverstate-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                using (FileStream fs = File.Open(file, FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        while (ServerStateCashe.Count != 0)
                        {
                            writer.WriteLine(ServerStateCashe[0]);
                            ServerStateCashe.RemoveAt(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save log error", ex);
            }

        }

        public static void LogGoldAdd(GoldAddType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (GoldAdd)
            {
                try
                {
                    GoldAdd[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogGoldRemove(GoldRemoveType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (GoldRemove)
            {
                try
                {
                    GoldRemove[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogMoneyAdd(MoneyAddType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (MoneyAdd)
            {
                try
                {
                    MoneyAdd[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogMoneyRemove(MoneyRemoveType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (MoneyRemove)
            {
                try
                {
                    MoneyRemove[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogGifttokenAdd(GifttokenAddType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (GifttokenAdd)
            {
                try
                {
                    GifttokenAdd[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogGifttokenRemove(GifttokenRemoveType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (GifttokenRemove)
            {
                try
                {
                    GifttokenRemove[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogOfferAdd(OfferAddType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (OfferAdd)
            {
                try
                {
                    OfferAdd[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogOfferRemove(OfferRemoveType type, long count)
        {
            if (!TxtRecord)
                return;

            if (count == 0)
                return;

            lock (OfferRemove)
            {
                try
                {
                    OfferRemove[type] += count;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
        public static void LogItemAdd(int itemid, ItemAddType type, long count)
        {
            if (!TxtRecord)
                return;
            lock (ItemAdd)
            {
                try
                {
                    if (ItemAdd.ContainsKey(itemid))
                    {
                        ItemAdd[itemid][type] += count;
                    }
                    else
                    {
                        Dictionary<ItemAddType, long> temp = new Dictionary<ItemAddType, long>();
                        foreach (object obj in Enum.GetValues(typeof(ItemAddType)))
                        {
                            temp.Add((ItemAddType)obj, 0);
                        }
                        ItemAdd[itemid] = temp;
                        temp[type] += count;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogItemRemove(int itemid, ItemRemoveType type, long count)
        {
            if (!TxtRecord)
                return;
            lock (ItemRemove)
            {
                try
                {
                    if (ItemRemove.ContainsKey(itemid))
                    {
                        ItemRemove[itemid][type] += count;
                    }
                    else
                    {
                        Dictionary<ItemRemoveType, long> temp = new Dictionary<ItemRemoveType, long>();
                        foreach (object obj in Enum.GetValues(typeof(ItemRemoveType)))
                        {
                            temp.Add((ItemRemoveType)obj, 0);
                        }
                        ItemRemove[itemid] = temp;
                        temp[type] += count;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        //#格式:【游戏记录】游戏类型,分区,服务器ID,[时间],玩家人数,地图ID,消耗时间，使用道具数，产生金币数
        public static void LogGame(int usercount, int mapid, DateTime time, int useprop, int generategold)
        {
            if (!TxtRecord)
                return;
            lock (gameLogs)
            {
                try
                {
                    gameLogs.Add(string.Format("{0},{1},{2},{3},{4},{5},{6}", _headStr, DateTime.Now, usercount, mapid, DateTime.Now - time, useprop, generategold));
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogUse(int userid, string userName, string nickName, ItemRemoveType type, string itemIDs)
        {
            if (!TxtRecord)
                return;
            lock (useLogs)
            {
                try
                {
                    useLogs.Add(string.Format("{0},{1},{2},{3},{4},{5},{6}", _headStr, DateTime.Now, userid, userName, nickName, type, itemIDs));
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static void LogErrorPlayer(int userid, string userName, string nickName, ItemRemoveType type, string remark)
        {
            if (!TxtRecord)
                return;
            lock (errorPlayerLogs)
            {
                try
                {
                    errorPlayerLogs.Add(string.Format("{0},{1},{2},{3},{4},{5},{6}", _headStr, DateTime.Now, userid, userName, nickName, type, remark));
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
    }
}
