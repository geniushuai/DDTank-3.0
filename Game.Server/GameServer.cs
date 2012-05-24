using System;
using Game.Base;
using System.IO;
using log4net.Config;
using log4net;
using System.Reflection;
using System.Collections;
using System.Threading;
using Game.Base.Packets;
using log4net.Core;
using Game.Base.Config;
using Game.Server.Managers;
using System.Data.Linq;
using Game.Server.GameObjects;
using System.Net.Sockets;
using System.Net;
using Game.Server.Packets.Client;
using Game.Base.Events;
using System.Configuration;
using Game.Server.Packets;
using Bussiness.Managers;
using Bussiness;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.Rooms;
using Game.Logic;
using Game.Server.Games;
using Game.Server.Battle;
using Game.Server.Statics;

namespace Game.Server
{
    public class GameServer : BaseServer
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly string Edition = "21000";

        public static bool KeepRunning = false;

        private static GameServer m_instance = null;

        public static void CreateInstance(GameServerConfig config)
        {
            //Only one intance
            if (m_instance != null)
                return;

            // 加载配置文件
            FileInfo logConfig = new FileInfo(config.LogConfigFile);
            if (!logConfig.Exists)
            {
                ResourceUtil.ExtractResource(logConfig.Name, logConfig.FullName,Assembly.GetAssembly(typeof(GameServer)));
            }
            
            //设置日志的配置
            XmlConfigurator.ConfigureAndWatch(logConfig);
   
            m_instance = new GameServer(config);
        }

        public static GameServer Instance
        {
            get { return m_instance; }
        }

        /// <summary>
        /// 游戏服务器
        /// </summary>
        /// <param name="config"></param>
        protected GameServer(GameServerConfig config)
        {
            m_config = config;

            if (log.IsDebugEnabled)
            {
                log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
                log.Debug("Gameserver root directory is: " + Configuration.RootDirectory);
                log.Debug("Changing directory to root directory");
            }

            Directory.SetCurrentDirectory(Configuration.RootDirectory);
        }

        private bool m_isRunning;

        private GameServerConfig m_config;

        public GameServerConfig Configuration
        {
            get { return m_config; }
        }

        private LoginServerConnector _loginServer;

        public LoginServerConnector LoginServer
        {
            get {   return _loginServer;    }
        }  

        #region Packet 接收/发送缓存

        /// <summary>
        /// The size of all packet buffers.
        /// </summary>
        private const int BUF_SIZE = 2048;

        /// <summary>
        /// Holds all packet buffers.
        /// </summary>
        private Queue m_packetBufPool;

        /// <summary>
        ///  all packet buffers.
        /// </summary>
        /// <returns>success</returns>
        private bool AllocatePacketBuffers()
        {
            int count = Configuration.MaxClientCount * 3;
            m_packetBufPool = new Queue(count);
            for (int i = 0; i < count; i++)
            {
                m_packetBufPool.Enqueue(new byte[BUF_SIZE]);
            }
            if (log.IsDebugEnabled)
                log.DebugFormat("allocated packet buffers: {0}", count.ToString());
            return true;
        }

        /// <summary>
        /// Gets the count of packet buffers in the pool.
        /// </summary>
        public int PacketPoolSize
        {
            get { return m_packetBufPool.Count; }
        }

        /// <summary>
        /// Gets packet buffer from the pool.
        /// </summary>
        /// <returns>byte array that will be used as packet buffer.</returns>
        public byte[] AcquirePacketBuffer()
        {
            lock (m_packetBufPool.SyncRoot)
            {
                if (m_packetBufPool.Count > 0)
                    return (byte[])m_packetBufPool.Dequeue();
            }
            log.Warn("packet buffer pool is empty!");
            return new byte[BUF_SIZE];
        }

        /// <summary>
        /// Releases previously acquired packet buffer.
        /// </summary>
        /// <param name="buf">The released buf</param>
        public void ReleasePacketBuffer(byte[] buf)
        {
            if (buf == null || GC.GetGeneration(buf) < GC.MaxGeneration)
                return;
            lock (m_packetBufPool.SyncRoot)
            {
                m_packetBufPool.Enqueue(buf);
            }
        }

        #endregion

        #region Client

        protected override BaseClient GetNewClient()
        {
            return new GameClient(this,AcquirePacketBuffer(),AcquirePacketBuffer());
        }

        public new GameClient[] GetAllClients()
        {
            GameClient[] list = null;

            lock (_clients.SyncRoot)
            {
                list = new GameClient[_clients.Count];
                _clients.Keys.CopyTo(list, 0);
            }
            return list;
        }

        #endregion

        #region Start

        private bool m_debugMenory = false;

        public override bool Start()
        {
            if (m_isRunning)
                return false;
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Thread.CurrentThread.Priority = ThreadPriority.Normal;

                GameProperties.Refresh();
               

                if (!InitComponent(RecompileScripts(), "Recompile Scripts"))
                        return false;

                if (!InitComponent(StartScriptComponents(), "Script components"))
                    return false;

                if (!InitComponent((GameProperties.EDITION == Edition), "Edition:" + Edition))
                    return false;

                if (!InitComponent(InitSocket(IPAddress.Parse(Configuration.Ip),Configuration.Port), "InitSocket Port:" + Configuration.Port))
                    return false;
                
                if (!InitComponent(AllocatePacketBuffers(), "AllocatePacketBuffers()"))
                    return false;

                if (!InitComponent(LogMgr.Setup(Configuration.GAME_TYPE, Configuration.ServerID, Configuration.AreaID), "LogMgr Init"))
                    return false;

                if (!InitComponent(WorldMgr.Init(), "WorldMgr Init"))
                    return false;

                if (!InitComponent(MapMgr.Init(), "MapMgr Init"))
                    return false;

                if (!InitComponent(ItemMgr.Init(), "ItemMgr Init"))
                    return false;

                if (!InitComponent(ItemBoxMgr.Init(), "ItemBox Init"))
                    return false;

                if (!InitComponent(BallMgr.Init(), "BallMgr Init"))
                    return false;
                if (!InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
                    return false;

                if (!InitComponent(FusionMgr.Init(), "FusionMgr Init"))
                    return false;

                if (!InitComponent(AwardMgr.Init(), "AwardMgr Init"))
                    return false;

                if (!InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
                    return false;

                if (!InitComponent(MissionInfoMgr.Init(), "MissionInfoMgr Init"))
                    return false;

                if (!InitComponent(PveInfoMgr.Init(), "PveInfoMgr Init"))
                    return false;

                if (!InitComponent(DropMgr.Init(), "Drop Init"))
                    return false;

                if (!InitComponent(FightRateMgr.Init(), "FightRateMgr Init"))
                    return false;

                if (!InitComponent(ConsortiaLevelMgr.Init(), "ConsortiaLevelMgr Init"))
                    return false;

                if (!InitComponent(RefineryMgr.Init(), "RefineryMgr Init"))
                    return false;

                if (!InitComponent(StrengthenMgr.Init(), "StrengthenMgr Init"))
                    return false;

                if (!InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
                    return false;

                if (!InitComponent(ShopMgr.Init(), "ShopMgr Init"))
                    return false;

                if (!InitComponent(QuestMgr.Init(), "QuestMgr Init"))
                    return false;

                if(!InitComponent(RoomMgr.Setup(Configuration.MaxRoomCount),"RoomMgr.Setup"))
                    return false;

                if (!InitComponent(GameMgr.Setup(Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION), "GameMgr.Start()"))
                    return false;

                if (!InitComponent(ConsortiaMgr.Init(), "ConsortiaMgr Init"))
                    return false;

                if (!InitComponent(LanguageMgr.Setup(@""), "LanguageMgr Init"))
                    return false;

				if (!InitComponent(RateMgr.Init(Configuration), "ExperienceRateMgr Init"))
                    return false;

                if (!InitComponent(MacroDropMgr.Init(), "MacroDropMgr Init"))
                    return false;

                if (!InitComponent(BattleMgr.Setup(), "BattleMgr Setup"))
                    return false;

                if (!InitComponent(InitGlobalTimer(), "Init Global Timers"))
                    return false;

                if (!InitComponent(MarryRoomMgr.Init(), "MarryRoomMgr Init"))
                    return false;
                if(!InitComponent(LogMgr.Setup(1,4,4),"LogMgr Setup"))
                    return false;
                GameEventMgr.Notify(ScriptEvent.Loaded);

                if (!InitComponent(InitLoginServer(), "Login To CenterServer"))
                    return false;
                
                RoomMgr.Start();
                GameMgr.Start();
                BattleMgr.Start();
                MacroDropMgr.Start();

                if (!InitComponent(base.Start(), "base.Start()"))
                    return false;

               

                GameEventMgr.Notify(GameServerEvent.Started, this);

                GC.Collect(GC.MaxGeneration);

                if (log.IsInfoEnabled)
                    log.Info("GameServer is now open for connections!");

                m_isRunning = true;
                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to start the server", e);

                return false;
            }
        }

        private bool InitLoginServer()
        {
            _loginServer = new LoginServerConnector(m_config.LoginServerIp, m_config.LoginServerPort,m_config.ServerID,m_config.ServerName, AcquirePacketBuffer(),AcquirePacketBuffer());
            _loginServer.Disconnected += new ClientEventHandle(loginServer_Disconnected);
            return _loginServer.Connect();
        }

        private static int m_tryCount = 4;
        private void loginServer_Disconnected(BaseClient client)
        {
            bool running = m_isRunning;
            Stop();
            if (running && m_tryCount > 0)
            {
                m_tryCount--;
                log.Error("Center Server Disconnect! Stopping Server");
                log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", m_tryCount);
                Thread.Sleep(1000);
                if (Start())
                {
                    log.Error("Restart the game server success!");
                }
            }
            else
            {
                if (m_tryCount == 0)
                {
                    log.ErrorFormat("Restart the game server failed after {0} times.", 4);
                    log.Error("Server Stopped!");
                }
                LogManager.Shutdown();
            }
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
                if (e.IsTerminating)
                    Stop();
            }
            catch
            {
                try
                {
                    using (FileStream fs = new System.IO.FileStream(@"c:\testme.log", FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                        {
                            w.WriteLine(e.ExceptionObject);
                        }
                    }
                }
                catch { }
            }
        }

        private static bool m_compiled = false;
        public bool RecompileScripts()
        {
            if (!m_compiled)
            {
                string scriptDirectory = Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
                if (!Directory.Exists(scriptDirectory))
                    Directory.CreateDirectory(scriptDirectory);

                string[] parameters = Configuration.ScriptAssemblies.Split(',');
                m_compiled = ScriptMgr.CompileScripts(false, scriptDirectory, Configuration.ScriptCompilationTarget, parameters);
            }
            return m_compiled;
        }

        protected bool StartScriptComponents()
        {
            try
            {
                //---------------------------------------------------------------
                //Create the server rules
                if (log.IsInfoEnabled)
                    log.Info("Server rules: true");

                ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
                //---------------------------------------------------------------
                //Register all event handlers
                ArrayList scripts = new ArrayList(ScriptMgr.Scripts);
                foreach (Assembly asm in scripts)
                {
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
                }
                if (log.IsInfoEnabled)
                    log.Info("Registering global event handlers: true");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("StartScriptComponents", e);
                return false;
            }
            //---------------------------------------------------------------
            return true;
        }

        protected bool InitComponent(bool componentInitState, string text)
        {
            if (m_debugMenory)
                log.Debug("Start Memory " + text + ": " + GC.GetTotalMemory(false) / 1024 / 1024);
            if (log.IsInfoEnabled)
                log.Info(text + ": " + componentInitState);
            if (!componentInitState)
                Stop();
            if (m_debugMenory)
                log.Debug("Finish Memory " + text + ": " + GC.GetTotalMemory(false) / 1024 / 1024);
            return componentInitState;
        }

       
        #endregion

        #region Stop

        /// <summary>
        /// Stops the server, disconnects all clients, and writes the database to disk
        /// </summary>
        public override void Stop()
        {
            if (m_isRunning)
            {
                m_isRunning = false;

                //记录礼堂使用情况
                if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
                {
                    Console.WriteLine("Update BreakTime failed");
                }

                RoomMgr.Stop();
                GameMgr.Stop();

                if (_loginServer != null)
                {
                    _loginServer.Disconnected -= new ClientEventHandle(loginServer_Disconnected);
                    _loginServer.Disconnect();
                }

                //ping check timer
                if (m_pingCheckTimer != null)
                {
                    m_pingCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_pingCheckTimer.Dispose();
                    m_pingCheckTimer = null;
                }
                
                //Stop the World Save timer
                if (m_saveDbTimer != null)
                {
                    m_saveDbTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_saveDbTimer.Dispose();
                    m_saveDbTimer = null;
                }

                if (m_saveRecordTimer != null)
                {
                    m_saveRecordTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_saveRecordTimer.Dispose();
                    m_saveRecordTimer = null;
                    SaveRecordProc(null);
                }

                if (m_buffScanTimer != null)
                {
                    m_buffScanTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_buffScanTimer.Dispose();
                    m_buffScanTimer = null;
                }

                SaveTimerProc(null);

                //Stop the base server
                base.Stop();

                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                log.Info("Server Stopped!");
                //LogManager.Shutdown();


                Console.WriteLine("Server Stopped!");
            }
        }

        
        private Timer _shutdownTimer;
        public void Shutdown()
        {
            GameServer.Instance.LoginServer.SendShutdown(true);
            _shutdownTimer = new Timer(new TimerCallback(ShutDownCallBack), null, 0, 60 * 1000);
        }

        private int _shutdownCount = 6;
        private void ShutDownCallBack(object state)
        {
            try
            {
                _shutdownCount--;
                Console.WriteLine(string.Format("Server will shutdown after {0} mins!", _shutdownCount));
                GameClient[] list = GameServer.Instance.GetAllClients();
                foreach (GameClient c in list)
                {
                    if (c.Out != null)
                    {
                        c.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
                    }
                }
                if (_shutdownCount == 0)
                {
                    Console.WriteLine("Server has stopped!");
                    GameServer.Instance.LoginServer.SendShutdown(false);
                    _shutdownTimer.Dispose();
                    _shutdownTimer = null;
                    GameServer.Instance.Stop();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        #endregion

        #region Global Timers

        protected Timer m_saveDbTimer;

        protected Timer m_pingCheckTimer;

        protected Timer m_saveRecordTimer;

        protected Timer m_buffScanTimer;

        public bool InitGlobalTimer()
        {
            int interval = Configuration.DBSaveInterval * 60 * 1000;
            if (m_saveDbTimer == null)
            {
                m_saveDbTimer = new Timer(new TimerCallback(SaveTimerProc), null, interval, interval);
            }
            else
            {
                m_saveDbTimer.Change(interval, interval);
            }

            interval = Configuration.PingCheckInterval * 60 * 1000;
            if (m_pingCheckTimer == null)
            {
                m_pingCheckTimer = new Timer(new TimerCallback(PingCheck), null, interval, interval);
            }
            else
            {
                m_pingCheckTimer.Change(interval, interval);
            }

            interval = Configuration.SaveRecordInterval * 60 * 1000;
            if (m_saveRecordTimer == null)
            {
                m_saveRecordTimer = new Timer(new TimerCallback(SaveRecordProc), null, interval, interval);
            }
            else
            {
                m_saveRecordTimer.Change(interval, interval);
            }

            interval = 60 * 1000;
            if (m_buffScanTimer == null)
            {
                m_buffScanTimer = new Timer(new TimerCallback(BuffScanTimerProc), null, interval, interval);
            }
            else
            {
                m_buffScanTimer.Change(interval,interval);
            }
            return true;
        }

        /// <summary>
        /// 定时检测客户端状态
        /// </summary>
        /// <param name="sender"></param>
        protected void PingCheck(object sender)
        {
            try
            {
                log.Info("Begin ping check....");
                long interval = (long)Configuration.PingCheckInterval * 60 * 1000 * 1000 * 10;

                GameClient[] list = GetAllClients();
                if (list != null)
                {
                    //注意清理断开连接的客户端，以及 连接上长时间不发送登陆包的客户端。
                    foreach (GameClient client in list)
                    {
                        if (client.IsConnected)
                        {
                            if (client.Player != null)
                            {
                                client.Out.SendPingTime(client.Player);

                                if (AntiAddictionMgr.ISASSon && AntiAddictionMgr.count == 0)
                                {
                                    if (client.Player.PlayerCharacter.IsFirst == 0 && (DateTime.Now - client.Player.PlayerCharacter.AntiDate).TotalMinutes >= 30)
                                        client.Player.Out.SendAASState(true);
                                    AntiAddictionMgr.count++;

                                }
                            }
                            else if (client.PingTime + interval < DateTime.Now.Ticks)
                            {
                                client.Disconnect();
                            }                           
                        }
                        else
                        {
                            client.Disconnect();
                        }
                    }
                }

                log.Info("End ping check....");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("PingCheck callback", e);
            }

            try
            {
                log.Info("Begin ping center check....");

                GameServer.Instance.LoginServer.SendPingCenter();

                log.Info("End ping center check....");
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("PingCheck center callback", e);
            }
        }

        /// <summary>
        /// 定时保存数据到数据库
        /// </summary>
        /// <param name="sender"></param>
        protected void SaveTimerProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Saving database...");
                    log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                int saveCount = 0;

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
               
                //保存人物
                GamePlayer[] list = WorldMgr.GetAllPlayers();
                foreach (GamePlayer p in list)
                {
                    p.SaveIntoDatabase();
                    saveCount++;
                }

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {    
                    log.Info("Saving database complete!");
                    log.Info("Saved all databases and " + saveCount + " players in " + startTick + "ms");
                }
                if (startTick > 2 * 60 * 1000)
                {
                    log.WarnFormat("Saved all databases and {0} players in {1} ms",saveCount,startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("SaveTimerProc", e1);
            }
            finally
            {
                GameEventMgr.Notify(GameServerEvent.WorldSave);
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
                    log.WarnFormat("Saved all Record  in {0} ms",  startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("SaveRecordProc", e1);
            }
        }

        protected void BuffScanTimerProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                if (log.IsInfoEnabled)
                {
                    log.Info("Buff Scaning ...");
                    log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                }
                int saveCount = 0;

                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                //保存人物
                GamePlayer[] list = WorldMgr.GetAllPlayers();
                foreach (GamePlayer p in list)
                {
                    if (p.BufferList != null)
                    {
                        p.BufferList.Update();
                        saveCount++;
                    }
                }

                Thread.CurrentThread.Priority = oldprio;

                startTick = Environment.TickCount - startTick;
                if (log.IsInfoEnabled)
                {
                    log.Info("Buff Scan complete!");
                    log.Info("Buff all " + saveCount + " players in " + startTick + "ms");
                }
                if (startTick > 2 * 60 * 1000)
                {
                    log.WarnFormat("Scan all Buff and {0} players in {1} ms", saveCount, startTick);
                }
            }
            catch (Exception e1)
            {
                if (log.IsErrorEnabled)
                    log.Error("BuffScanTimerProc", e1);
            }

        }

        #endregion

    }
}
