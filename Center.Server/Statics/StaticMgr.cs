using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Bussiness;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;

namespace Center.Server.Statics
{
    public class StaticMgr
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool TxtRecord
        {
            get
            {
                return bool.Parse(ConfigurationSettings.AppSettings["TxtRecord"]);
            }
        }

        public static int ServerID
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
            }
        }

        public static int GameType
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["GameType"]);
            }
        }

        public static int AreaID
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["AreaID"]);
            }
        }

        public static string LogPath
        {
            get
            {
                return ConfigurationSettings.AppSettings["LogPath"];
            }
        }

        public static int SaveRecordSecond
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["SaveRecordInterval"]) * 60;
            }
        }

        private static int _gameType;

        private static int _areaId;

        private static int _serverId;

        private static string _logPath;

        private static string _headStr;

        private static List<string> _saveLogCache;

        private static List<string> _onlineLogCache;

        private static int regCount;

        public static object _sysObj = new object();

        public static bool Setup()
        {
            return Setup(GameType,AreaID,ServerID,LogPath);
        }

        public static void AddRegCount() 
        {
            lock (_sysObj)
            {
                regCount++;
            }
        }

        public static int RegCount
        {
            get
            {
                lock (_sysObj)
                {
                    return regCount;
                }
            }
            set
            {
                lock (_sysObj)
                {
                    regCount = value;
                }
            }
        }
        

        public static bool Setup(int type, int areaid, int serverid, string logpath)
        {
         
            _gameType = type;
            _areaId = areaid;
            _serverId = serverid;
            _logPath = logpath;
            _headStr = string.Format("{0},{1},{2}", type, areaid, serverid);
            dt = DateTime.Now;

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }

            _saveLogCache = new List<string>();
            _onlineLogCache = new List<string>();

            return true;
        }

        private static DateTime dt;
        public static void Save()
        {
            
            if (!TxtRecord)
                return;
            //DateTime dt = DateTime.Now;
            int interval = SaveRecordSecond;
            //dt = dt.AddMinutes(5);
            dt = dt.AddSeconds(interval);

            //int totalM = 0  ; //注册人数(男)
            //int totalF = 0; //注册人数(女)
            //int onlineM = 0; //在线人数(男)
            //int onlineF = 0; //在线人数(女)
            //int paymenM = 0; //付费用户(男)
            //int paymenF = 0; //付费用户(女)
            //int activePaymenM = 0; //活跃付费用户(男)
            //int activePaymenF = 0; //活跃付费用户(女)
            //int activeCustomerM = 0; //活跃消费用户(男)
            //int activeCustomerF = 0; //活跃消费用户(女)

//            新的User日志格式

//#格式:【用户记录】游戏类型,分区,服务器ID,[时间],注册人数(男),注册人数(女)，在线人数(男)，在线人数(女)，登陆用户数(男), 登陆用户数(女),活跃充值用户(男)，活跃充值用户(女)，活跃消费用户(男),活跃消费用户(女）,活跃在线用户(男)、活跃在线用户(女)

            using (ServiceBussiness db = new ServiceBussiness())
            {
                try
                {
                    RecordInfo info = db.GetRecordInfo(dt, SaveRecordSecond);
                    int online = LoginMgr.GetOnlineCount();

                    //writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", dt, totalM, totalF, onlineM, onlineF, paymenM, paymenF, activePaymenM, activePaymenF, activeCustomerM, activeCustomerF));
                    if (info == null)
                    {
                        info = new RecordInfo();
                    }
                    _saveLogCache.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}", _headStr, dt, info.TotalBoy, info.TotalGirl, online, 0,
                        info.ExpendBoy, info.ExpendGirl, info.ActviePayBoy, info.ActviePayGirl, info.ActiveExpendBoy, info.ActiveExpendGirl, info.ActiveOnlineBoy, info.ActiveOnlineGirl));

                    string file = string.Format("{0}\\user-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                    using (FileStream fs = File.Open(file, FileMode.Append))
                    {
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            while (_saveLogCache.Count != 0)
                            {
                                writer.WriteLine(_saveLogCache[0]);
                                _saveLogCache.RemoveAt(0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled)
                        log.Error("Save log error", ex);
                }

                try
                {
                    //1,1,2,2009-1-10 0:02:06,0,
                    //游戏类型、代理商、频道、日期、在线用户、注册用户
                    Dictionary<int, int> lines = LoginMgr.GetOnlineForLine();

                    int online = LoginMgr.GetOnlineCount();
                    _onlineLogCache.Add(string.Format("{0},{1},{2},{3},{4},{5}", _gameType, _areaId, 0, dt, online, RegCount));
                    RegCount = 0;

                    string file = string.Format("{0}\\online-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
                    using (FileStream fs = File.Open(file, FileMode.Append))
                    {
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            while (_onlineLogCache.Count != 0)
                            {
                                writer.WriteLine(_onlineLogCache[0]);
                                _onlineLogCache.RemoveAt(0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled)
                        log.Error("OnlineForLine log error", ex);
                }

            }
            //#格式:【用户记录】游戏类型,分区,时间,付费方式(ebank、SMS、Post),男人数,女人数,男付费金额,女付费金额
            //using (PlayerBussiness db = new PlayerBussiness())
            //{
            //    ChargeRecordInfo[] infos = db.GetChargeRecordInfo(dt, SaveRecordSecond);
            //    string file = string.Format("{0}\\pay-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", _logPath, _gameType, _areaId, _serverId, dt);
            //    using (FileStream fs = File.Open(file, FileMode.Append))
            //    {
            //        using (StreamWriter writer = new StreamWriter(fs))
            //        {
            //            foreach (ChargeRecordInfo info in infos)
            //            {
            //                writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
            //                    _headStr, dt, info.PayWay, info.TotalBoy, info.TotalGirl, info.BoyTotalPay, info.GirlTotalPay));
            //            }
            //        }
            //    }
            //}
        }
    }
}
