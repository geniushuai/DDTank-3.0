using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using SqlDataProvider.Data;
using System.Threading;
using Bussiness;
using System.Reflection;

namespace Game.Logic
{
    public static class MissionInfoMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, MissionInfo> m_missionInfos = new Dictionary<int, MissionInfo>();

        private static System.Threading.ReaderWriterLock m_lock = new System.Threading.ReaderWriterLock();

        private static ThreadSafeRandom m_rand = new ThreadSafeRandom();

        public static bool Init()
        {
            return Reload();
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, MissionInfo> tempMissionInfo = LoadFromDatabase();
                if (tempMissionInfo.Count > 0)
                {
                    Interlocked.Exchange(ref m_missionInfos, tempMissionInfo);
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error("MissionInfoMgr", e);
            }

            return false;
        }

        private static Dictionary<int, MissionInfo> LoadFromDatabase()
        {
            Dictionary<int, MissionInfo> dic = new Dictionary<int, MissionInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                MissionInfo[] infos = db.GetAllMissionInfo();
                foreach (MissionInfo info in infos)
                {
                    if (!dic.ContainsKey(info.Id))
                    {
                        dic.Add(info.Id, info);
                    }
                }
            }

            return dic;
        }

        public static MissionInfo GetMissionInfo(int id)
        {
            if (m_missionInfos.ContainsKey(id))
            {
                return m_missionInfos[id];
            }
            return null;
        }
    }
}
