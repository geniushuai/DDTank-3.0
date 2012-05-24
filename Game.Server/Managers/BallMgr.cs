using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using log4net.Util;
using Game.Server.GameObjects;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using System.IO;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;

namespace Game.Server.Managers
{
    public class BallMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, BallInfo> _balls;

        private static Dictionary<int, Tile> _ballTile;

        private static System.Threading.ReaderWriterLock m_lock;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, BallInfo> tempBalls = new Dictionary<int, BallInfo>();
                Dictionary<int, Tile> tempBallTile = new Dictionary<int, Tile>();

                if (LoadBall(tempBalls, tempBallTile))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _balls = tempBalls;
                        _ballTile = tempBallTile;
                        return true;
                    }
                    catch
                    { }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }

                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("BallMgr", e);
            }

            return false;
        }

        /// <summary>
        /// Initializes the BallMgr. 
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _balls = new Dictionary<int, BallInfo>();
                _ballTile = new Dictionary<int, Tile>();
                return LoadBall(_balls, _ballTile);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("BallMgr", e);
                return false;
            }

        }

        private static bool LoadBall(Dictionary<int, BallInfo> balls,Dictionary<int, Tile> ballTile)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                BallInfo[] ballInfos = db.GetAllBall();
                foreach (BallInfo b in ballInfos)
                {
                    if (!balls.ContainsKey(b.ID))
                    {
                        balls.Add(b.ID, b);

                        Tile shape = null;
                        string file = string.Format("bomb\\{0}.bomb", b.ID);
                        if (File.Exists(file))
                        {
                            shape = new Tile(file,false);
                        }

                        if (shape != null)
                        {
                            ballTile.Add(b.ID, shape);
                        }
                        else
                        {
                            if (b.ID != 1 && b.ID != 2 && b.ID != 3)
                            {
                                if (log.IsErrorEnabled)
                                    log.Error("Ball's file is not exist!");
                                return false;
                            }
                        }
                    }
                }
            }

            if (!balls.ContainsKey(0))
            {
                BallInfo temp = new BallInfo();
                temp.ID = 0;
                temp.Power = 1;
                temp.Radii = 60;
                temp.Amount = 1;
                balls.Add(0, temp);
            }

            return true;
        }

        public static BallInfo FindBall(int GoodsID)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_balls.ContainsKey(GoodsID))
                    return _balls[GoodsID];
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return _balls[0];
        }

        public static Tile FindTile(int ballID)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_ballTile.ContainsKey(ballID))
                    return _ballTile[ballID];
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
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
                default:
                    return BombType.Normal;
            }
        }
        

    }
}
