using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using System.IO;
using System.Drawing;
using Game.Logic.Phy.Maps;
using Bussiness.Managers;

namespace Game.Logic
{
    public class MapMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, MapPoint> _maps;

        private static Dictionary<int, Map> _mapInfos;

        private static Dictionary<int, List<int>> _serverMap;

        private static ThreadSafeRandom random;

        private static System.Threading.ReaderWriterLock m_lock;

        #region reload

        public static bool ReLoadMap()
        {
            try
            {
                Dictionary<int, MapPoint> tempMaps = new Dictionary<int, MapPoint>();
                Dictionary<int, Map> tempMapInfos = new Dictionary<int, Map>();

                if (LoadMap(tempMaps, tempMapInfos))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _maps = tempMaps;
                        _mapInfos = tempMapInfos;
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
                    log.Error("ReLoadMap", e);
            }

            return false;
        }

 

        
        public static bool ReLoadMapServer()
        {
            try
            {
                Dictionary<int, List<int>> tempServerMap = new Dictionary<int, List<int>>();
                if (InitServerMap(tempServerMap))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _serverMap = tempServerMap;
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
                    log.Error("ReLoadMapWeek", e);
            }

            return false;
        }

 
        #endregion

        public static bool Init()
        {
            try
            {
                random = new ThreadSafeRandom();
                m_lock = new System.Threading.ReaderWriterLock();

                _maps = new Dictionary<int, MapPoint>();
                _mapInfos = new Dictionary<int, Map>();
                if (!LoadMap(_maps, _mapInfos))
                    return false; 

                _serverMap = new Dictionary<int, List<int>>();
                if (!InitServerMap(_serverMap))
                    return false;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("MapMgr", e);
                return false;
            }
            return true;
        }

        public static bool LoadMap(Dictionary<int, MapPoint> maps, Dictionary<int, Map> mapInfos)
        {
            try
            {
                MapBussiness db = new MapBussiness();
                MapInfo[] query = db.GetAllMap();

                foreach (MapInfo m in query)
                {
                    if (string.IsNullOrEmpty(m.PosX) )//|| string.IsNullOrEmpty(m.PosX1))
                        continue;

                    if (!maps.Keys.Contains(m.ID))
                    {
                        string[] tmp = m.PosX.Split('|');
                        string[] tmp1 = m.PosX1.Split('|');
                        //List<Point> pos = new List<Point>();
                        MapPoint pos = new MapPoint();
                        foreach (string s in tmp)
                        {
                            if (string.IsNullOrEmpty(s.Trim()))
                                continue;

                            string[] xy = s.Split(',');
                            pos.PosX.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
                            //Point temp = new Point(int.Parse(xy[0]), int.Parse(xy[1]));
                            //pos.Add(temp);
                        }

                        foreach (string s in tmp1)
                        {
                            if (string.IsNullOrEmpty(s.Trim()))
                                continue;

                            string[] xy = s.Split(',');
                            pos.PosX1.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
                            //Point temp = new Point(int.Parse(xy[0]), int.Parse(xy[1]));
                            //pos.Add(temp);
                        }

                        maps.Add(m.ID, pos);
                    }

                    if (!mapInfos.ContainsKey(m.ID))
                    {
                        Tile force = null;
                        string file = string.Format("map\\{0}\\fore.map", m.ID);
                        if (File.Exists(file))
                        {
                            force = new Tile(file, true);
                        }

                        Tile dead = null;
                        file = string.Format("map\\{0}\\dead.map", m.ID);
                        if (File.Exists(file))
                        {
                            dead = new Tile(file, false);
                        }

                        if (force != null || dead != null)
                        {
                            mapInfos.Add(m.ID, new Map(m, force, dead));
                        }
                        else
                        {
                            if (log.IsErrorEnabled)
                                log.Error("Map's file is not exist!");
                            return false;
                        }
                    }
                }

                if (maps.Count == 0 || mapInfos.Count == 0)
                {
                    if (log.IsErrorEnabled)
                        log.Error("maps:" + maps.Count + ",mapInfos:" + mapInfos.Count);
                    return false;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("MapMgr", e);
                return false;
            }
            return true;
        }

        public static Map CloneMap(int index)
        {
            if (_mapInfos.ContainsKey(index))
            {
                return _mapInfos[index].Clone();

            }
            return null;
        }

        public static MapInfo FindMapInfo(int index)
        {
            if (_mapInfos.ContainsKey(index))
            {
                return _mapInfos[index].Info;
            }
            return null;
        }

        public static int GetMapIndex(int index, byte type,int serverId)
        {
            if (index != 0 && !_maps.Keys.Contains(index))
                index = 0;
            if (index == 0)
            {
                List<int> tempIndex = new List<int>();
                foreach (int id in _serverMap[serverId])
                {
                    MapInfo tempInfo = FindMapInfo(id);
                    if ((int)(type & tempInfo.Type) != 0)
                    {
                        tempIndex.Add(id);
                    }
                }

                if (tempIndex.Count == 0)
                {

                    int count = _serverMap[serverId].Count;
                    return _serverMap[serverId][random.Next(count)];
                }
                else
                {
                    int count = tempIndex.Count;
                    return tempIndex[random.Next(count)];
                }
            }
            return index;
        }

        public static MapPoint GetMapRandomPos(int index)
        {
            MapPoint pos = new MapPoint();
            MapPoint temp;
            if (index != 0 && !_maps.Keys.Contains(index))
                index = 0;

            if (index == 0)
            {
                int[] map = _maps.Keys.ToArray();
                temp = _maps[map[random.Next(map.Length)]];
            }
            else
            {
                temp = _maps[index];
            }

            if (random.Next(2) == 1)
            {
                pos.PosX.AddRange(temp.PosX);
                pos.PosX1.AddRange(temp.PosX1);
            }
            else
            {
                pos.PosX.AddRange(temp.PosX1);
                pos.PosX1.AddRange(temp.PosX);
            }

            return pos;
        }

        public static MapPoint GetPVEMapRandomPos(int index)
        {
            MapPoint pos = new MapPoint();
            MapPoint temp;
            if (index != 0 && !_maps.Keys.Contains(index))
                index = 0;

            if (index == 0)
            {
                int[] map = _maps.Keys.ToArray();
                temp = _maps[map[random.Next(map.Length)]];
            }
            else
            {
                temp = _maps[index];
            }
            pos.PosX.AddRange(temp.PosX);
            pos.PosX1.AddRange(temp.PosX1);

            return pos;
        }
 
  
        #region 周期

        public static int GetWeekDay
        {
            get
            {
                int day = Convert.ToInt32(DateTime.Now.DayOfWeek);
                return day == 0 ? 7 : day;
            }
        }

        public static bool InitServerMap(Dictionary<int, List<int>> servermap)
        {
            MapBussiness db = new MapBussiness();
            ServerMapInfo[] infos = db.GetAllServerMap();

            try
            {
                var temp=0;
                foreach (ServerMapInfo info in infos)
                {
                    //if (info.WeekID < 1 || info.WeekID > 7)
                    //    continue;

                    if (!servermap.Keys.Contains(info.ServerID))
                    {
                        string[] str = info.OpenMap.Split(',');
                        List<int> list = new List<int>();
                        foreach (string s in str)
                        {
                            if (string.IsNullOrEmpty(s))
                                continue;
                            if(int.TryParse(s,out temp))list.Add(temp);
                        }
                        servermap.Add(info.ServerID, list);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }

            return true;
        }

        #endregion
     }

    public class MapPoint
    {
        public MapPoint()
        {
            posX = new List<Point>();
            posX1 = new List<Point>();
        }
        private List<Point> posX;

        public List<Point> PosX
        {
            get
            {
                return posX;
            }
            set
            {
                posX = value;
            }
        }

        private List<Point> posX1;

        public List<Point> PosX1
        {
            get
            {
                return posX1;
            }
            set
            {
                posX1 = value;
            }
        }

    }
}
