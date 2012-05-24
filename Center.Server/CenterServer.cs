using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;
using System.Threading;
using Game.Base.Events;
using System.Collections;
using Game.Server.Managers;
using Game.Base.Packets;
using System.Configuration;
using Bussiness.Protocol;
using Bussiness;
using Center.Server.Managers;
using System.Net;

namespace Center.Server
{
    public class CenterServer : BaseServer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CenterServerConfig _config;

        private string Edition = "21000";



        private bool _aSSState;
        public bool ASSState
        {
            get
            {
                return _aSSState;
            }
            set
            {
                _aSSState = value;
            }
        }

        private bool _dailyAwardState;
        public bool DailyAwardState
        {
            get
            {
                return _dailyAwardState;
            }
            set
            {
                _dailyAwardState = value;
            }
        }

        protected override BaseClient GetNewClient()
        {
            return new ServerClient(this);
        }

        public override bool Start()
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                GameProperties.Refresh();

                if (!InitComponent(RecompileScripts(), "Recompile Scripts"))
                    return false;

                //初始化脚本
                if (!InitComponent(StartScriptComponents(), "Script components"))
                    return false;

                //检查版本是否和数据库一致
                if (!InitComponent(GameProperties.EDITION == Edition, "Check Server Edition:" + Edition))
                    return false;
                
                //初始化监听端口
                if (!InitComponent(InitSocket(IPAddress.Parse(_config.Ip), _config.Port), "InitSocket Port:" + _config.Port))
                    return false;

                //启动服务监听
                if (!InitComponent(CenterService.Start(), "Center Service"))
                    return false;

                //加载服务器列表
                if (!InitComponent(ServerMgr.Start(), "Load serverlist"))
                    return false;

                //加载公会等级信息
                if (!InitComponent(ConsortiaLevelMgr.Init(), "Init ConsortiaLevelMgr"))
                    return false;

                //初始化宏观掉落表
                if (!InitComponent(MacroDropMgr.Init(), "Init MacroDropMgr"))
                    return false;

                //初始化语言包
                if (!InitComponent(LanguageMgr.Setup(@""), "LanguageMgr Init"))
                    return false;

                //初始化全局Timer
                if (!InitComponent(InitGlobalTimers(), "Init Global Timers"))
                    return false;

                //发布脚本已加载事件
                GameEventMgr.Notify(ScriptEvent.Loaded);

                //宏观掉落控制开始
                MacroDropMgr.Start();

                if (!InitComponent(base.Start(), "base.Start()"))
                    return false;

                //发布服务器开始事件
                GameEventMgr.Notify(GameServerEvent.Started, this);

                GC.Collect(GC.MaxGeneration);

                log.Info("GameServer is now open for connections!");


                GameProperties.Save();
                return true;
            }
            catch (Exception e)
            {
                log.Error("Failed to start the server", e);
                return false;
            }
        }

        /// <summary>
        /// 程序异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
            if (e.IsTerminating)
                LogManager.Shutdown();
        }

        /// <summary>
        /// 初始化模板
        /// </summary>
        /// <param name="componentInitState"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected bool InitComponent(bool componentInitState, string text)
        {
            log.Info(text + ": " + componentInitState);
            if (!componentInitState)
                Stop();
            return componentInitState;
        }

        public bool RecompileScripts()
        {
            string scriptDirectory = _config.RootDirectory + Path.DirectorySeparatorChar + "scripts";
            if (!Directory.Exists(scriptDirectory))
                Directory.CreateDirectory(scriptDirectory);

            string[] parameters = _config.ScriptAssemblies.Split(',');
            return ScriptMgr.CompileScripts(false, scriptDirectory, _config.ScriptCompilationTarget, parameters);
        }

        /// <summary>
        /// 初始化脚本
        /// </summary>
        /// <returns></returns>
        protected bool StartScriptComponents()
        {
            try
            {
                ScriptMgr.InsertAssembly(typeof(CenterServer).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);

                Assembly[] scripts = ScriptMgr.Scripts;
                foreach (Assembly asm in scripts)
                {
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
                }
                log.Info("Registering global event handlers: true");
                return true;
            }
            catch (Exception e)
            {
                log.Error("StartScriptComponents", e);
                return false;
            }
        }

        #region Global Timers

        private Timer m_loginLapseTimer;

        private Timer m_saveDBTimer;

        private Timer m_saveRecordTimer;

        private Timer m_scanAuction;

        private Timer m_scanMail;

        private Timer m_scanConsortia;

        public bool InitGlobalTimers()
        {
            int interval = _config.SaveIntervalInterval * 60 * 1000;
            if (m_saveDBTimer == null)
            {
                m_saveDBTimer = new Timer(new TimerCallback(SaveTimerProc), null, interval, interval);
            }
            else
            {
                m_saveDBTimer.Change(interval, interval);
            }

            interval = _config.LoginLapseInterval * 60 * 1000;
            if (m_loginLapseTimer == null)
            {
                m_loginLapseTimer = new Timer(new TimerCallback(LoginLapseTimerProc), null, interval, interval);
            }
            else
            {
                m_loginLapseTimer.Change(interval, interval);
            }

            interval = _config.SaveRecordInterval * 60 * 1000;
            if (m_saveRecordTimer == null)
            {
                m_saveRecordTimer = new Timer(new TimerCallback(SaveRecordProc), null, interval, interval);
            }
            else
            {
                m_saveRecordTimer.Change(interval, interval);
            }

            interval = _config.ScanAuctionInterval * 60 * 1000;
            if (m_scanAuction == null)
            {
                m_scanAuction = new Timer(new TimerCallback(ScanAuctionProc), null, interval, interval);
            }
            else
            {
                m_scanAuction.Change(interval, interval);
            }

            interval = _config.ScanMailInterval * 60 * 1000;
            if (m_scanMail == null)
            {
                m_scanMail = new Timer(new TimerCallback(ScanMailProc), null, interval, interval);
            }
            else
            {
                m_scanMail.Change(interval, interval);
            }

            interval = _config.ScanConsortiaInterval * 60 * 1000;
            if (m_scanConsortia == null)
            {
                m_scanConsortia = new Timer(new TimerCallback(ScanConsortiaProc), null, interval, interval);
            }
            else
            {
                m_scanConsortia.Change(interval, interval);
            }

            return true;
        }

        public void DisposeGlobalTimers()
        {
            if (m_saveDBTimer != null)
            {
                m_saveDBTimer.Dispose();
            }

            if (m_loginLapseTimer != null)
            {
                m_loginLapseTimer.Dispose();
            }

            if (m_saveRecordTimer != null)
            {
                m_saveRecordTimer.Dispose();
            }

            if (m_scanAuction != null)
            {
                m_scanAuction.Dispose();
            }

            if (m_scanMail != null)
            {
                m_scanMail.Dispose();
            }

            if (m_scanConsortia != null)
            {
                m_scanConsortia.Dispose();
            }
        }

        protected void SaveTimerProc(object state)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving database...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                //保存服务器状态
                ServerMgr.SaveToDatabase();

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving database complete!");
                    log.Info("Saved all databases " + startTick + "ms");
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("SaveTimerProc", e1);
            }
        }

        protected void LoginLapseTimerProc(object sender)
        {
            try
            {
                Player[] list = LoginMgr.GetAllPlayer();
                long now = DateTime.Now.Ticks;
                long interval = (long)_config.LoginLapseInterval * 10 * 1000;
                foreach (Player player in list)
                {
                    if (player.State == ePlayerState.NotLogin)
                    {
                        if (player.LastTime + interval < now)
                        {
                            LoginMgr.RemovePlayer(player.Id);
                        }
                    }
                    else
                    {
                        player.LastTime = now;
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("LoginLapseTimer callback", ex);
            }
        }

        protected void SaveRecordProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                
                Statics.LogMgr.Save();

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record complete!");
                }
                if (startTick > 2 * 60 * 1000)
                {
                    log.WarnFormat("Saved all Record  in {0} ms!", startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("SaveRecordProc", e1);
            }
        }

        protected void ScanAuctionProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                string noticeUserID = "";
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    db.ScanAuction(ref noticeUserID);
                }
                string[] userIDs = noticeUserID.Split(',');

                foreach (string s in userIDs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.MAIL_RESPONSE);
                    pkg.WriteInt(int.Parse(s));
                    pkg.WriteInt((int)eMailRespose.Receiver);
                    SendToALL(pkg);
                }

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Auction complete!");
                }
                if (startTick > 2 * 60 * 1000)
                {
                    log.WarnFormat("Scan all Auction  in {0} ms", startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("ScanAuctionProc", e1);
            }
        }

        protected void ScanMailProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                string noticeUserID = "";
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    db.ScanMail(ref noticeUserID);
                }
                string[] userIDs = noticeUserID.Split(',');

                foreach (string s in userIDs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.MAIL_RESPONSE);
                    pkg.WriteInt(int.Parse(s));
                    pkg.WriteInt((int)eMailRespose.Receiver);
                    SendToALL(pkg);
                }

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Mail complete!");
                }
                if (startTick > 2 * 60 * 1000)
                {
                    log.WarnFormat("Scan all Mail in {0} ms", startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("ScanMailProc", e1);
            }
        }

        protected void ScanConsortiaProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving Record...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                string noticeID = "";
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    db.ScanConsortia(ref noticeID);
                }
                string[] noticeIDs = noticeID.Split(',');

                foreach (string s in noticeIDs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;

                    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_RESPONSE);
                    pkg.WriteByte(2);
                    pkg.WriteInt(int.Parse(s));
                    SendToALL(pkg);
                }

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {
                    log.Info("Scan Consortia complete!");
                }
                if (startTick > 2 * 60 * 1000)
                {
                    log.WarnFormat("Scan all Consortia in {0} ms", startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("ScanConsortiaProc", e1);
            }
        }

#endregion

        public override void Stop()
        {
            DisposeGlobalTimers();
            SaveTimerProc(null);
            SaveRecordProc(null);

            CenterService.Stop();

            base.Stop();
        }

        public new ServerClient[] GetAllClients()
        {
            ServerClient[] list = null;

            lock (_clients.SyncRoot)
            {
                list = new ServerClient[_clients.Count];
                _clients.Keys.CopyTo(list, 0);
            }
            return list;
        }

        public void SendToALL(GSPacketIn pkg)
        {
            SendToALL(pkg, null);
        }

        public void SendToALL(GSPacketIn pkg, ServerClient except)
        {
            ServerClient[] list = GetAllClients();
            if (list != null)
            {
                foreach (ServerClient client in list)
                {
                    if (client != except)
                    {
                        client.SendTCP(pkg);
                    }
                }
            }
        }

        public void SendConsortiaDelete(int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CONSORTIA_RESPONSE);
            pkg.WriteByte(5);
            pkg.WriteInt(consortiaID);
            SendToALL(pkg);
        }

        public void SendSystemNotice(string msg)
        {
            //0后台系统公告，1其它公告
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
            pkg.WriteInt(0);
            pkg.WriteString(msg);
            SendToALL(pkg, null);
        }

        public bool SendAAS(bool state)
        {
            if (StaticFunction.UpdateConfig("Center.Service.exe.config", "AAS", state.ToString()))
            {
                ASSState = state;
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.UPDATE_ASS);
                pkg.WriteBoolean(state);
                SendToALL(pkg);
                return true;
            }
            return false;
        }

        public bool SendConfigState(int type, bool state)
        {
            string config = string.Empty;
            switch (type)
            {
                case 1:
                    config = "AAS";
                    break;
                case 2:
                    config = "DailyAwardState";
                    break;
                default:
                    return false;
            }

            //TODO:修改数据库里面的配置

            if (StaticFunction.UpdateConfig("Center.Service.exe.config", config, state.ToString()))
            {
                switch (type)
                {
                    case 1:
                        ASSState = state;
                        break;
                    case 2:
                        DailyAwardState = state;
                        break;
                }
                SendConfigState();
                return true;
            }

            return false;
        }

        public void SendConfigState()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.UPDATE_CONFIG_STATE);
            pkg.WriteBoolean(ASSState);
            pkg.WriteBoolean(DailyAwardState);
            SendToALL(pkg);
        }

        public int RateUpdate(int serverId)
        {
            ServerClient[] list = GetAllClients();
            if (list != null)
            {
                foreach (ServerClient client in list)
                {
                    if (client.Info.ID == serverId)
                    {
                        GSPacketIn pkg = new GSPacketIn((byte)ePackageType.Rate);
                        pkg.WriteInt(serverId);
                        client.SendTCP(pkg);
                        return 0;
                    }
                }
            }

            return 1;
        }

        public int NoticeServerUpdate(int serverId, int type)
        {
            ServerClient[] list = GetAllClients();
            if (list != null)
            {
                foreach (ServerClient client in list)
                {
                    if (client.Info.ID == serverId)
                    {
                        GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_RELOAD);
                        pkg.WriteInt(type);
                        client.SendTCP(pkg);
                        return 0;
                    }
                }
            }

            return 1;
        }

        public bool SendReload(eReloadType type)
        {
            return SendReload(type.ToString());
        }

        public bool SendReload(string str)
        {
            try
            {
                eReloadType type = (eReloadType)Enum.Parse(typeof(eReloadType), str, true);
                switch (type)
                {
                    case eReloadType.server:
                        _config.Refresh();
                        InitGlobalTimers();
                        LoadConfig();
                        ServerMgr.ReLoadServerList();
                        SendConfigState();
                        break;
                    default:
                        break;
                }

                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_RELOAD);
                pkg.WriteInt((int)type);
                SendToALL(pkg, null);
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("请检查是否存在此指令!" + ex.ToString());
                log.Error("Order is not Exist!", ex);
            }
            return false;
        }

        public void SendShutdown()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SHUTDOWN);
            SendToALL(pkg);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public CenterServer(CenterServerConfig config)
        {
            _config = config;
            LoadConfig();
        }

        public void LoadConfig()
        {
            _aSSState = bool.Parse(ConfigurationSettings.AppSettings["AAS"]);
            _dailyAwardState = bool.Parse(ConfigurationSettings.AppSettings["DailyAwardState"]);
        }

        /// <summary>
        /// 单件
        /// </summary>
        private static CenterServer _instance;

        public static CenterServer Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void CreateInstance(CenterServerConfig config)
        {
            //Only one intance
            if (Instance != null)
                return;

            FileInfo logConfig = new FileInfo(config.LogConfigFile);
            if (!logConfig.Exists)
            {
                ResourceUtil.ExtractResource(logConfig.Name, logConfig.FullName, Assembly.GetAssembly(typeof(CenterServer)));
            }
            //Configure and watch the config file
            XmlConfigurator.ConfigureAndWatch(logConfig);
            //Create the instance
            _instance = new CenterServer(config);
        }

    }
}
