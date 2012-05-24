using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using System.Data;
using Bussiness;
using System.Configuration;

namespace Center.Server.Statics
{
    public class LogMgr
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static object _syncStop = new object();

        public static int GameType
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["GameType"]);
            }
        }

        public static int ServerID
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
            }
        }

        public static int AreaID
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["AreaID"]);
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
        private static int _serverId;
        private static int _areaId;
        public static DataTable m_LogServer;
         
        private static int regCount;
        public static object _sysObj = new object();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static bool Setup()
        {
            return Setup(GameType, ServerID, AreaID);
        }

        public static bool Setup(int gametype, int serverid, int areaid)
        {
            _gameType = gametype;
            _serverId = serverid;
            _areaId = areaid;
            
            m_LogServer = new DataTable("Log_Server");
            m_LogServer.Columns.Add("ApplicationId", typeof(int));
            m_LogServer.Columns.Add("SubId", typeof(int));
            m_LogServer.Columns.Add("EnterTime", typeof(DateTime));
            m_LogServer.Columns.Add("Online", typeof(int));
            m_LogServer.Columns.Add("Reg", typeof(int));         
            return true;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public static void Reset()
        {
 
            lock (m_LogServer)
            {
                m_LogServer.Clear();
            }
 
        }

        /// <summary>
        /// 定时保存
        /// </summary>
        public static void Save()
        {
            int online = LoginMgr.GetOnlineCount();//在线人数                   
            object[] info = { _gameType, _serverId, DateTime.Now, online, RegCount };
            //lock (m_LogServer)
            //{
               // m_LogServer.Rows.Add(info);
            //}
            RegCount = 0;
           
                
            int interval = SaveRecordSecond;                
            using (ItemRecordBussiness db = new ItemRecordBussiness())                
            {                    
                db.LogServerDb(m_LogServer);                
            }            
        }

        /// <summary>
        /// 注册用户
        /// </summary>
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

        /// <summary>
        /// 添加注册用户
        /// </summary>
        public static void AddRegCount()
        {
            lock (_sysObj)
            {
                regCount++;
            }
        }

    }
}
