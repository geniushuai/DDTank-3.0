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
    public class BallMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, BallInfo> m_infos;

        private static Dictionary<int, Tile> m_tiles;

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, BallInfo> tempBalls = LoadFromDatabase();
                Dictionary<int, Tile> tempBallTile = LoadFromFiles(tempBalls);

                if (tempBalls.Values.Count > 0 && tempBallTile.Values.Count > 0)
                {
                    Interlocked.Exchange(ref m_infos, tempBalls);
                    Interlocked.Exchange(ref m_tiles, tempBallTile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Ball Mgr init error:", ex);
            }
            return false;
        }

        private static Dictionary<int, BallInfo> LoadFromDatabase()
        {
            Dictionary<int, BallInfo> list = new Dictionary<int, BallInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                BallInfo[] ballInfos = db.GetAllBall();
                foreach (BallInfo b in ballInfos)
                {
                    if (!list.ContainsKey(b.ID))
                    {
                        list.Add(b.ID, b);
                    }
                }
            }
            return list;
        }

        private static Dictionary<int, Tile> LoadFromFiles(Dictionary<int, BallInfo> list)
        {
            Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
            foreach (BallInfo info in list.Values)
            {
                if (info.HasTunnel)
                {
                    string file = string.Format("bomb\\{0}.bomb", info.ID);
                    Tile shape = null;
                    if (File.Exists(file))
                    {
                        shape = new Tile(file, false);
                    }
                    tiles.Add(info.ID, shape);

                    if (shape == null && info.ID != 1 && info.ID != 2 && info.ID != 3)
                    {
                        log.ErrorFormat("can't find bomb file:{0}", file);
                    }
                }
            }
            return tiles;
        }

        public static BallInfo FindBall(int id)
        {

            if (m_infos.ContainsKey(id))
                return m_infos[id];
            //else
            //{
            //    var stringBall = id + "1";
            //    id = int.Parse(stringBall);
            //    if (m_infos.ContainsKey(id))
            //        return m_infos[id];
            //}
            return null;
        }

        public static Tile FindTile(int id)
        {
            if (m_tiles.ContainsKey(id))
                return m_tiles[id];
            return null;
        }

        public static BombType GetBallType(int ballId)
        {
            switch (ballId)
            {
                case 1:
                    return BombType.FORZEN;
                case 3:
                    return BombType.TRANFORM;
                case 5:
                case 64:
                    return BombType.CURE;
                default:
                    return BombType.Normal;
            }
        }


    }
}
