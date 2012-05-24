using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Rooms;
using log4net;
using System.Reflection;
using System.Xml.Linq;
using System.IO;

namespace Game.Server.Battle
{
    public class BattleMgr
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<BattleServer> m_list = new List<BattleServer>();

        public static bool Setup()
        {
            if (File.Exists("battle.xml"))
            {
                try
                {
                    XDocument doc = XDocument.Load("battle.xml");
                    foreach (XElement server in doc.Root.Nodes())
                    {
                        try
                        {
                            int id = int.Parse(server.Attribute("id").Value);
                            string ip = server.Attribute("ip").Value;
                            int port = int.Parse(server.Attribute("port").Value);
                            string key = server.Attribute("key").Value;

                            m_list.Add(new BattleServer(id, ip, port, key));
                            log.InfoFormat("Battle server {0}:{1} loaded...", ip, port);
                        }
                        catch (Exception ex)
                        {
                            log.Error("BattleMgr setup error:", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("BattleMgr setup error:", ex);
                }
            }

            log.InfoFormat("Total {0} battle server loaded.", m_list.Count);
            return true;
        }

        public static void Start()
        {
            foreach (BattleServer server in m_list)
            {
                try
                {
                    server.Start();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Batter server {0}:{1} can't connected!",server.Ip,server.Port);
                    log.Error(ex.Message);
                }
            }
        }


        public static BattleServer FindActiveServer()
        {
            lock (m_list)
            {
                foreach (BattleServer server in m_list)
                {
                    if (server.IsActive)
                    {
                        return server;
                    }
                }
            }

            return null;
        }

        public static BattleServer AddRoom(BaseRoom room)
        {
            BattleServer server = FindActiveServer();
            if (server != null && server.AddRoom(room))
            {
                return server;
            }
            return null;
        }
        public static List<BattleServer> GetAllBattles()
        {
            return m_list;
        }
    }
}
