using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using System.IO;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;

namespace Game.Logic
{
    public class BallConfigMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, BallConfigInfo> m_infos;

        private static Dictionary<int, Tile> m_tiles;

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, BallConfigInfo> tempBalls = LoadFromDatabase();
                //Dictionary<int, Tile> tempBallTile = LoadFromFiles(tempBalls);

                if (tempBalls.Values.Count > 0)
                {
                    Interlocked.Exchange(ref m_infos, tempBalls);
                   // Interlocked.Exchange(ref m_tiles, tempBallTile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Ball Mgr init error:", ex);
            }
            return false;
        }

        private static Dictionary<int, BallConfigInfo> LoadFromDatabase()
        {
            Dictionary<int, BallConfigInfo> list = new Dictionary<int, BallConfigInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                BallConfigInfo[] BallConfigInfos = db.GetAllBallConfig();
                foreach (BallConfigInfo b in BallConfigInfos)
                {
                    if (!list.ContainsKey(b.TemplateID))
                    {
                        list.Add(b.TemplateID, b);
                    }
                }
            }
            return list;
        }



        public static BallConfigInfo FindBall(int id)
        {

            if (m_infos.ContainsKey(id))
                return m_infos[id];
            return null;
        }

        public static Tile FindTile(int id)
        {
            if (m_tiles.ContainsKey(id))
                return m_tiles[id];
            return null;
        }

       


    }
}
