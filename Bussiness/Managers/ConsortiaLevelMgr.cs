using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using Bussiness;


namespace Game.Server.Managers
{
    public class ConsortiaLevelMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel;

        private static System.Threading.ReaderWriterLock m_lock;

        private static ThreadSafeRandom rand;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ConsortiaLevelInfo> tempConsortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();

                if (Load(tempConsortiaLevel))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _consortiaLevel = tempConsortiaLevel;
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
                    log.Error("ConsortiaLevelMgr", e);
            }

            return false;
        }

        /// <summary>
        /// Initializes the StrengthenMgr. 
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
                rand = new ThreadSafeRandom();
                return Load(_consortiaLevel);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ConsortiaLevelMgr", e);
                return false;
            }

        }

        private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel)
        {
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaLevelInfo[] infos = db.GetAllConsortiaLevel();
                foreach (ConsortiaLevelInfo info in infos)
                {
                    if (!consortiaLevel.ContainsKey(info.Level))
                    {
                        consortiaLevel.Add(info.Level, info);
                    }
                }
            }

            return true;
        }

        public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_consortiaLevel.ContainsKey(level))
                    return _consortiaLevel[level];
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }


    }
}
