using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Collections;
using System.Configuration;
using Fighting.Server;
using System.IO;
using Fighting.Server.Rooms;
using Fighting.Server.Games;
using Game.Logic;

namespace Fighting.Service.action
{
    public class ConsoleStart : IAction
    {
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
            get { return "Starts the Fighting server in console mode"; }
        }

        /// <summary>
        /// Handles the server action
        /// </summary>
        /// <param name="parameters"></param>
        public void OnAction(Hashtable parameters)
        {
            Console.WriteLine("Starting FightingServer ... please wait a moment!");

            FightServerConfig config = new FightServerConfig();
            try
            {
                config.Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }

            FightServer.CreateInstance(config);
            FightServer.Instance.Start();

            bool run = true;
            while (run)
            {
                try
                {
                    Console.Write("> ");
                    string line = Console.ReadLine();
                    string[] para = line.Split(' ');

                    switch (para[0].ToLower())
                    {
                        case "clear":
                            Console.Clear();
                            break;
                        case "list":
                            if (para.Length > 1)
                            {
                                switch (para[1])
                                {
                                    case "-client":
                                        Console.WriteLine("server client list:");
                                        Console.WriteLine("--------------------");
                                        ServerClient[] list = FightServer.Instance.GetAllClients();
                                        foreach (ServerClient client in list)
                                        {
                                            Console.WriteLine(client.ToString());
                                        }
                                        Console.WriteLine("-------------------");
                                        break;
                                    case "-room":
                                        Console.WriteLine("room list:");
                                        Console.WriteLine("-------------------------------");
                                        ProxyRoom[] rooms = ProxyRoomMgr.GetAllRoom();
                                        foreach (ProxyRoom room in rooms)
                                        {
                                            Console.WriteLine(room.ToString());
                                        }
                                        Console.WriteLine("-------------------------------");
                                        break;
                                    case "-game":
                                        Console.WriteLine("game list:");
                                        Console.WriteLine("-------------------------------");
                                        List<BaseGame> games = GameMgr.GetGames();
                                        foreach (BaseGame g in games)
                                        {
                                            Console.WriteLine(g.ToString());
                                        }
                                        Console.WriteLine("-------------------------------");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("list [-client][-room][-game]");
                                Console.WriteLine("     -client:列出所有服务器对象");
                                Console.WriteLine("     -room:列出所有房间对象");
                                Console.WriteLine("     -game:列出所有游戏对象");
                            }
                            
                            break;
                        case "exit": 
                            run = false;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.ToString());
                }
            }

            if (FightServer.Instance != null)
                FightServer.Instance.Stop();
        }

    }
}
