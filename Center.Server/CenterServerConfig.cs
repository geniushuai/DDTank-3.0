using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using Game.Base.Config;
using Bussiness;
using System.Reflection;
using System.IO;
using System.Configuration;
using log4net;

namespace Center.Server
{
    public class CenterServerConfig :BaseAppConfig
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string RootDirectory;

        [ConfigProperty("IP","中心服务器监听IP","127.0.0.1")]
        public string Ip;

        [ConfigProperty("Port","中心服务器监听端口",9202)]
        public int Port;

        [ConfigProperty("LogConfigFile", "日志配置文件", "logconfig.xml")]
        public string LogConfigFile;

        [ConfigProperty("ScriptAssemblies", "脚本编译引用库", "")]
        public string ScriptAssemblies;

        [ConfigProperty("ScriptCompilationTarget", "脚本编译目标名称", "")]
        public string ScriptCompilationTarget;

        [ConfigProperty("LoginLapseInterval","登陆超时时间,分钟为单位",1)]
        public int LoginLapseInterval;

        [ConfigProperty("SaveInterval","数据保存周期,分钟为单位",1)]
        public int SaveIntervalInterval;

        [ConfigProperty("SaveRecordInterval","日志保存周期,分钟为单位",1)]
        public int SaveRecordInterval;

        [ConfigProperty("ScanAuctionInterval","排名行扫描周期,分钟为单位",60)]
        public int ScanAuctionInterval;

        [ConfigProperty("ScanMailInterval","邮件扫描周期,分钟为单位",60)]
        public int ScanMailInterval;

        [ConfigProperty("ScanConsortiaInterval","工会扫描周期,以分钟为单位",60)]
        public int ScanConsortiaInterval;

        public CenterServerConfig()
        {
            Load(typeof(CenterServerConfig));
        }

        public void Refresh()
        {
            Load(typeof(CenterServerConfig));

        }

        protected override void Load(Type type)
        {
            if (Assembly.GetEntryAssembly() != null)
                RootDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            else
                RootDirectory = new FileInfo(Assembly.GetAssembly(type).Location).DirectoryName;
 
            //Load from app config
            base.Load(type);
        }

    }
}
