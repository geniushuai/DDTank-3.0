using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using log4net.Util;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;
using System.IO;

namespace Game.Logic
{
    public static class NPCInfoMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, NpcInfo> m_npcs = new Dictionary<int, NpcInfo>();

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
                Dictionary<int, NpcInfo> tempNpc = LoadFromDatabase();
                if (tempNpc !=null && tempNpc.Count > 0)
                {
                    Interlocked.Exchange(ref m_npcs, tempNpc);
                }
                return true;
            }
            catch (Exception e)
            {
                log.Error("NPCInfoMgr", e);
            }

            return false;
        }

        private static Dictionary<int, NpcInfo> LoadFromDatabase()
        {
            Dictionary<int, NpcInfo> list = new Dictionary<int, NpcInfo>();

            using (ProduceBussiness db = new ProduceBussiness())
            {
                NpcInfo[] infos = db.GetAllNPCInfo();

                foreach (NpcInfo info in infos)
                {
                    if (!list.ContainsKey(info.ID))
                    {
                        list.Add(info.ID, info);
                    }
                }
            }

            return list;
        }

        public static NpcInfo GetNpcInfoById(int id)
        {
            if (m_npcs.ContainsKey(id))
            {
                return m_npcs[id];
            }
            return null;
        }
    }
}
