using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Bussiness;
using log4net;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    /// <summary>
    /// 道具管理
    /// </summary>
    public class PropItemMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ThreadSafeRandom random = new ThreadSafeRandom();

        private static System.Threading.ReaderWriterLock m_lock;

        #region AllProp

        private static Dictionary<int, ItemTemplateInfo> _allProp;

        public static bool Reload()
        {
            try
            {
                Dictionary<int, ItemTemplateInfo> tempProp = new Dictionary<int, ItemTemplateInfo>();
                if (LoadProps(tempProp))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _allProp = tempProp;
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
                    log.Error("ReloadProps", e);
            }

            return false;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _allProp = new Dictionary<int, ItemTemplateInfo>();
                return LoadProps(_allProp);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("InitProps", e);
                return false;
            }
        }

        private static bool LoadProps(Dictionary<int, ItemTemplateInfo> allProp)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                ItemTemplateInfo[] list = db.GetSingleCategory(10);
                foreach (ItemTemplateInfo p in list)
                {
                    allProp.Add(p.TemplateID, p);
                }
            }
            return true;
        }

        public static ItemTemplateInfo FindAllProp(int id)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_allProp.ContainsKey(id))
                {
                    return _allProp[id];
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }

            return null;
        }

        static int[] PropBag = new int[] { 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008 };

        public static ItemTemplateInfo FindPropBag(int id)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);

            if (!PropBag.Contains(id))
                return null;

            try
            {
                if (_allProp.ContainsKey(id))
                {
                    return _allProp[id];
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }

            return null;
        }

        public static ItemTemplateInfo GetRandomFightProp(int Map)
        {
            int id = MapMgr.GetRandomFightPropIndex(Map);
            return FindAllProp(id);
        } 

        #endregion
    }
}
