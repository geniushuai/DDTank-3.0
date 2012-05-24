using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Game.Server;
using System.ServiceProcess;
using System.Reflection;

namespace Game.Service
{
    /// <summary>
	/// DOL System Service
	/// </summary>
    public class GameServerService : ServiceBase
    {
        public GameServerService()
        {
            this.ServiceName = "ROAD";
            this.AutoLog = false;
            this.CanHandlePowerEvent = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        private static bool StartServer()
        {
            //TODO parse args for -config parameter!
            FileInfo dolserver = new FileInfo(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(dolserver.DirectoryName);
            FileInfo configFile = new FileInfo("./config/serverconfig.xml");
            GameServerConfig config = new GameServerConfig();
            //if (configFile.Exists)
            //{
            //    config.LoadFromXMLFile(configFile);
            //}
            //else
            //{
            //    if (!configFile.Directory.Exists)
            //        configFile.Directory.Create();
            //    config.SaveToXMLFile(configFile);
            //}

            GameServer.CreateInstance(config);

            return GameServer.Instance.Start();
        }

        private static void StopServer()
        {
            GameServer.Instance.Stop();
        }

        protected override void OnStart(string[] args)
        {
            if (!StartServer())
                throw new ApplicationException("Failed to start server!");
        }

        protected override void OnStop()
        {
            StopServer();
        }

        protected override void OnShutdown()
        {
            StopServer();
        }

        /// <summary>
        /// Gets the DOL service from the service list
        /// </summary>
        /// <returns></returns>
        public static ServiceController GetDOLService()
        {
            foreach (ServiceController svcc in ServiceController.GetServices())
            {
                if (svcc.ServiceName.ToLower().Equals("ROAD"))
                    return svcc;
            }
            return null;
        }
    }
}
