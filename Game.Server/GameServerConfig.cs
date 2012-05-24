using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.IO;
using System.Reflection;
using Bussiness;
using log4net;
using SqlDataProvider.Data;
using Game.Base.Config;

namespace Game.Server
{
    public class GameServerConfig:BaseAppConfig
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string RootDirectory;

        public int MaxRoomCount = 1000;

        public int MaxPlayerCount = 2000;
        [ConfigProperty("Xu_Rate", "最大连接数", 1)]
        public int Xu_Rate = 2000;

        [ConfigProperty("MaxClientCount", "最大连接数", 8000)]
        public int MaxClientCount = 2000;

        [ConfigProperty("AppID", "代理商编号", 1)]
        public int AppID;

        [ConfigProperty("AreaID", "分区编号", 1)]
        public int AreaID;

        [ConfigProperty("ServerID", "服务器编号", 1)]
        public int ServerID;

        [ConfigProperty("ServerName", "频道的名称", "7Road")]
        public string ServerName;

        [ConfigProperty("IP", "频道的IP", "127.0.0.1")]
        public string Ip;

        [ConfigProperty("Port", "频道开放端口", 9200)]
        public int Port;

        [ConfigProperty("LoginServerIp", "中心服务器的IP", "192.168.0.2")]
        public string LoginServerIp;

        [ConfigProperty("LoginServerPort", "中心服务器的端口", 9202)]
        public int LoginServerPort;

        [ConfigProperty("ScriptAssemblies", "脚本编译引用库", "")]
        public string ScriptAssemblies;

        [ConfigProperty("ScriptCompilationTarget", "脚本编译目标名称", "")]
        public string ScriptCompilationTarget;

        [ConfigProperty("LogConfigFile", "日志配置文件", "logconfig.xml")]
        public string LogConfigFile;

        [ConfigProperty("LogPath", "日志路径", 1)]
        public string LogPath;

        [ConfigProperty("CountRecord", "是否记录日志", true)]
        public bool CountRecord;

        [ConfigProperty("TxtRecord", "是否记录统计信息", true)]
        public bool TxtRecord;

        [ConfigProperty("SaveRecordInterval","统计信息保存的时间间隔,分钟为单位",5)]
        public int SaveRecordInterval;

        [ConfigProperty("PrivateKey", "RSA的私钥","")]
        public string PrivateKey;

        [ConfigProperty("DBAutosaveInterval", "数据库自动保存的时间间隔,分钟为单位", 5)]
        public int DBSaveInterval;

        [ConfigProperty("PingCheckInterval", "PING检查时间间隔,分钟为单位", 5)]
        public int PingCheckInterval;

        [ConfigProperty("GameType", "游戏类型", 0)]
        public int GAME_TYPE;

        [ConfigProperty("InterName", "接口类型", "sevenroad")]
        public string INTERFACE_NAME;

        public GameServerConfig()
        {
            Load(typeof(GameServerConfig));
        }


        public void Refresh()
        {
            Load(typeof(GameServerConfig));
            GameServer.Instance.InitGlobalTimer();
        }

        protected override void Load(Type type)
        {
            if (Assembly.GetEntryAssembly() != null)
                RootDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            else
                RootDirectory = new FileInfo(Assembly.GetAssembly(typeof(GameServer)).Location).DirectoryName;

            //load app.config
            base.Load(type);

            //load server config in db
            using (ServiceBussiness sb = new ServiceBussiness())
            {
                ServerInfo info = sb.GetServiceSingle(ServerID);
                if (info == null)
                {
                    log.ErrorFormat("Can't find server config,server id {0}", ServerID);
                }
                else
                {
                    ServerName = info.Name;
                    MaxRoomCount = info.Room;
                    MaxPlayerCount = info.Total;
                }
            }
        }
    }
}
