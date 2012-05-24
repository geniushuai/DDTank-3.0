using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using System.Reflection;
using Game.Base.Packets;
using log4net.Config;
using System.IO;
using System.Threading;
using Game.Base.Events;
using Game.Server.Managers;
using Fighting.Server.Rooms;
using Fighting.Server.Games;
using Game.Logic;
using Bussiness.Managers;
using Bussiness;

namespace Fighting.Server
{
    public class FightServer : BaseServer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static bool KeepRunning = false;

        private FightServerConfig m_config;

        protected override BaseClient GetNewClient()
        {
            return new ServerClient(this);
        }

        private bool m_running;

        public override bool Start()
        {
            if (m_running)
                return false;
            try
            {
                m_running = true;

                Thread.CurrentThread.Priority = ThreadPriority.Normal;

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                //初始化监听端口
                if (!InitComponent(InitSocket(m_config.Ip, m_config.Port), "InitSocket Port:" + m_config.Port))
                    return false;

                //初始化脚本
                if (!InitComponent(StartScriptComponents(), "Script components"))
                    return false;

                if (!InitComponent(ProxyRoomMgr.Setup(), "RoomMgr.Setup"))
                    return false;

                if (!InitComponent(GameMgr.Setup(0, 4), "GameMgr.Setup"))
                    return false;

                if (!InitComponent(MapMgr.Init(), "MapMgr Init"))
                    return false;

                if (!InitComponent(ItemMgr.Init(), "ItemMgr Init"))
                    return false;

                if (!InitComponent(PropItemMgr.Init(), "ItemMgr Init"))
                    return false;

                if (!InitComponent(BallMgr.Init(), "BallMgr Init"))
                    return false;

                if (!InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
                    return false;
                if (!InitComponent(DropMgr.Init(), "DropMgr Init"))
                    return false;

                if (!InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
                    return false;

                if (!InitComponent(LanguageMgr.Setup(@""), "LanguageMgr Init"))
                    return false;

                //发布脚本已加载事件
                GameEventMgr.Notify(ScriptEvent.Loaded);

                if (!InitComponent(base.Start(), "base.Start()"))
                    return false;

                ProxyRoomMgr.Start();
                GameMgr.Start();

                //发布服务器开始事件
                GameEventMgr.Notify(GameServerEvent.Started, this);

                GC.Collect(GC.MaxGeneration);

                log.Info("GameServer is now open for connections!");

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

        /// <summary>
        /// 初始化脚本
        /// </summary>
        /// <returns></returns>
        protected bool StartScriptComponents()
        {
            try
            {
                ScriptMgr.InsertAssembly(typeof(FightServer).Assembly);
                ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
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

        public override void Stop()
        {
            if (m_running == false)
                return;
            try
            {
                m_running = false;
                GameMgr.Stop();
                ProxyRoomMgr.Stop();
            }
            catch (Exception ex)
            {
                log.Error("Server stopp error:", ex);
            }
            finally
            {
                base.Stop();
            }
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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        private FightServer(FightServerConfig config)
        {
            m_config = config;
        }

        /// <summary>
        /// 单件
        /// </summary>
        private static FightServer m_instance;

        public static FightServer Instance
        {
            get
            {
                return m_instance;
            }
        }

        public static void CreateInstance(FightServerConfig config)
        {
            //Only one intance
            if (m_instance != null)
                return;

            FileInfo logConfig = new FileInfo(config.LogConfigFile);
            if (!logConfig.Exists)
            {
                ResourceUtil.ExtractResource(logConfig.Name, logConfig.FullName, Assembly.GetAssembly(typeof(FightServer)));
            }
            //Configure and watch the config file
            XmlConfigurator.ConfigureAndWatch(logConfig);
            //Create the instance
            m_instance = new FightServer(config);
        }

    }
}
