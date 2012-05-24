using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using Bussiness;
using System.Threading;

namespace Game.Logic
{
    public static class PveInfoMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, PveInfo> m_pveInfos = new Dictionary<int, PveInfo>();

        private static System.Threading.ReaderWriterLock m_lock = new System.Threading.ReaderWriterLock();

        private static ThreadSafeRandom m_rand = new ThreadSafeRandom();

        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, PveInfo> tempPve = LoadFromDatabase();
                if (tempPve.Count > 0)
                {
                    Interlocked.Exchange(ref m_pveInfos, tempPve);
                }
                return true;
            }
            catch(Exception e)
            {
                log.Error("PveInfoMgr", e);
            }

            return false;
        }

        public static Dictionary<int, PveInfo> LoadFromDatabase()
        {
            Dictionary<int, PveInfo> list = new Dictionary<int, PveInfo>();

            using(PveBussiness db = new PveBussiness())
            {
                PveInfo[] infos = db.GetAllPveInfos();

                foreach(PveInfo info in infos)
                {
                    if (!list.ContainsKey(info.ID))
                    {
                        list.Add(info.ID, info);
                    }
                }
            }

            return list;
        }

        public static PveInfo GetPveInfoById(int id)
        {
            if (m_pveInfos.ContainsKey(id))
            {
                return m_pveInfos[id];
            }
            return null;
        }

        public static PveInfo GetPveInfoByType(eRoomType roomType, int levelLimits)
        {
            if (roomType == eRoomType.Boss || roomType == eRoomType.Treasure)
            {
                foreach (PveInfo pveInfo in m_pveInfos.Values)
                {
                    if (pveInfo.Type == (int)roomType)
                    {
                        return pveInfo;
                    }
                }
            }
            else if (roomType == eRoomType.Exploration)
            {
                foreach (PveInfo pveInfo in m_pveInfos.Values)
                {
                    if ((pveInfo.Type == (int)roomType) && (pveInfo.LevelLimits == levelLimits))
                    {
                        return pveInfo;
                    }
                }
            }
            return null;
        }

    }
}
