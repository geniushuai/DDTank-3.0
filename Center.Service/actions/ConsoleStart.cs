using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections;
using System.IO;
using System.Reflection;
using Center.Server;
using System.Configuration;
using Bussiness.Protocol;
using Game.Base;

namespace Game.Service.actions
{
    /// <summary>
    /// Handles 
    /// </summary>
    public class ConsoleStart:IAction
    {
        /// <summary>
        /// Defines a logger for this class
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public string HelpStr
        {
            get
            {
                return ConfigurationSettings.AppSettings["HelpStr"];
            }
        }

        /// <summary>
        /// Returns the name of this action
        /// </summary>
        public string Name
        {
            get { return "--start"; }
        }

        /// <summary>
        /// Returns the syntax of this action
        /// </summary>
        public string Syntax
        {
            get { return "--start [-config=./config/serverconfig.xml]"; }
        }

        /// <summary>
        /// Returns the description of this action
        /// </summary>
        public string Description
        {
            get { return "Starts the DOL server in console mode"; }
        }

        /// <summary>
        /// Starts the game server and returns the result
        /// </summary>
        /// <returns></returns>
        private static bool StartServer()
        {
            Console.WriteLine("Starting the server");
            bool start = CenterServer.Instance.Start();
            return start;
        }

        /// <summary>
        /// Handles the server action
        /// </summary>
        /// <param name="parameters"></param>
        public void OnAction(Hashtable parameters)
        {
            Console.WriteLine("Starting GameServer ... please wait a moment!");
            CenterServer.CreateInstance(new CenterServerConfig());
            StartServer();

            ConsoleClient client = new ConsoleClient();
            bool run = true;
            while (run)
            {
                try
                {
                    Console.Write("> ");
                    string line = Console.ReadLine();
                    string[] para = line.Split('&');

                    switch (para[0].ToLower())
                    {
                        case "exit": run = false; break;
                        case "notice":
                            if (para.Length < 2)
                            {
                                Console.WriteLine("公告需要公告内容,用&隔开!");
                            }
                            else
                            {
                                CenterServer.Instance.SendSystemNotice(para[1]);
                            }
                            break;
                        case "reload":
                            if (para.Length < 2)
                            {
                                Console.WriteLine("加载需要指定表,用&隔开!");
                            }
                            else
                            {
                                CenterServer.Instance.SendReload(para[1]);
                            }
                            //ServerMgr.ReLoadServerList();
                            break;
                        case "shutdown":
                            CenterServer.Instance.SendShutdown();
                            break;
                        case "help":
                            Console.WriteLine(HelpStr);
                            break;
                        case "AAS":
                            if (para.Length < 2)
                            {
                                Console.WriteLine("加载需要指定状态true or false,用&隔开!");
                            }
                            else
                            {
                                CenterServer.Instance.SendAAS(bool.Parse(para[1]));
                            }
                            break;
                        default:
                            if (line.Length <= 0) break;
                            if (line[0] == '/')
                            {
                                line = line.Remove(0, 1);
                                line = line.Insert(0, "&");
                            }

                            try
                            {
                                bool res = CommandMgr.HandleCommandNoPlvl(client, line);
                                if (!res)
                                {
                                    Console.WriteLine("Unknown command: " + line);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error:" + ex.ToString());
                }
            }

            if (CenterServer.Instance != null)
                CenterServer.Instance.Stop();
        }

        public void Reload(eReloadType type)
        {

        }
       
    }
}
