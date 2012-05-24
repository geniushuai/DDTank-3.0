using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using log4net.Util;
using System.Threading;

namespace Bussiness.Managers
{
    public class ItemMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, ItemTemplateInfo> _items;       

        private static System.Threading.ReaderWriterLock m_lock;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ItemTemplateInfo> tempItems = new Dictionary<int, ItemTemplateInfo>();

                if (LoadItem(tempItems))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _items = tempItems;
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
                    log.Error("ReLoad", e);
            }

            return false;
        }

        /// <summary>
        /// Initializes the ItemMgr. 
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            try
            {
                m_lock = new System.Threading.ReaderWriterLock();
                _items = new Dictionary<int, ItemTemplateInfo>();                
                return LoadItem(_items);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
                return false;
            }
        }

        public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                ItemTemplateInfo[] items = db.GetAllGoods();
                foreach (ItemTemplateInfo item in items)
                {
                    //if (item.TemplateID == 11223)
                    //{
                    //    int i = 0;
                    //}
                    if (!infos.Keys.Contains(item.TemplateID))
                    {
                        infos.Add(item.TemplateID, item);
                    }
                }
            }
            return true;
        }


        public static ItemTemplateInfo FindItemTemplate(int templateId)
        {
            if (_items == null)
                Init();

            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_items.Keys.Contains(templateId))
                {
                    return _items[templateId];
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

  
        public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
        {
            if (_items == null)
                Init();

            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach(ItemTemplateInfo p in _items.Values)
                {
                    if(p.FusionType == fusionType && p.Quality == quality)
                    {
                        return p;
                    }
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }

            return null;
        }

        public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
        {
            if (_items == null)
                Init();
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (ItemTemplateInfo p in _items.Values)
                {
                    if (p.FusionType == fusionType && p.Level == level)
                    {
                        return p;
                    }
                }
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }
    }
}
