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

namespace Game.Server.Managers
{
    public class StrengthenMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, StrengthenInfo> _strengthens;

        private static Dictionary<int, StrengthenInfo> m_Refinery_Strengthens;

        private static Dictionary<int, StrengthenGoodsInfo> Strengthens_Goods;

        private static System.Threading.ReaderWriterLock m_lock;

        private static ThreadSafeRandom rand;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, StrengthenInfo> tempStrengthens = new Dictionary<int, StrengthenInfo>();
                Dictionary<int, StrengthenInfo> tempRefineryStrengthens = new Dictionary<int, StrengthenInfo>();
                Dictionary<int, StrengthenGoodsInfo> tempStrengthenGoodsInfos = new Dictionary<int, StrengthenGoodsInfo>();
                if (LoadStrengthen(tempStrengthens, tempRefineryStrengthens))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _strengthens = tempStrengthens;
                        m_Refinery_Strengthens = tempRefineryStrengthens;
                        Strengthens_Goods = tempStrengthenGoodsInfos;
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
                    log.Error("StrengthenMgr", e);
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
                _strengthens = new Dictionary<int, StrengthenInfo>();
                m_Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
                Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
                rand = new ThreadSafeRandom();
                return LoadStrengthen(_strengthens, m_Refinery_Strengthens);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("StrengthenMgr", e);
                return false;
            }

        }

        private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                StrengthenInfo[] infos = db.GetAllStrengthen();

                StrengthenInfo[] Refineryinfos = db.GetAllRefineryStrengthen();

                StrengthenGoodsInfo[] StrengthGoodInfos = db.GetAllStrengthenGoodsInfo();

                foreach (StrengthenInfo info in infos)
                {
                    if (!strengthen.ContainsKey(info.StrengthenLevel))
                    {
                        strengthen.Add(info.StrengthenLevel, info);
                    }
                }
                foreach (StrengthenInfo info in Refineryinfos)
                {
                    if (!RefineryStrengthen.ContainsKey(info.StrengthenLevel))
                    {
                        RefineryStrengthen.Add(info.StrengthenLevel, info);
                    }
                }

                foreach (StrengthenGoodsInfo info in StrengthGoodInfos)
                {
                    if (!Strengthens_Goods.ContainsKey(info.ID))
                    {
                        Strengthens_Goods.Add(info.ID, info);
                    }
                }
            }

            return true;
        }

        public static StrengthenInfo FindStrengthenInfo(int level)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_strengthens.ContainsKey(level))
                    return _strengthens[level];
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static StrengthenInfo FindRefineryStrengthenInfo(int level)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (m_Refinery_Strengthens.ContainsKey(level))
                    return m_Refinery_Strengthens[level];
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }
        public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int TemplateId)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (int i in Strengthens_Goods.Keys)
                {
                    if (Strengthens_Goods[i].Level == level && TemplateId == Strengthens_Goods[i].CurrentEquip)
                        return Strengthens_Goods[i];

                }
                //if (Strengthens_Goods.ContainsKey(level))
                //    return Strengthens_Goods[level];
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }


        public static void InheritProperty(ItemInfo Item, ref ItemInfo item)
        {
            if (Item.Hole1 >= 0)
                item.Hole1 = Item.Hole1;
            if (Item.Hole2 >= 0)
                item.Hole2 = Item.Hole2;
            if (Item.Hole3 >= 0)
                item.Hole3 = Item.Hole3;
            if (Item.Hole4 >= 0)
                item.Hole4 = Item.Hole4;
            if (Item.Hole5 >= 0)
                item.Hole5 = Item.Hole5;
            if (Item.Hole6 >= 0)
                item.Hole6 = Item.Hole6;

            item.AttackCompose = Item.AttackCompose;
            item.DefendCompose = Item.DefendCompose;
            item.LuckCompose = Item.LuckCompose;
            item.AgilityCompose = Item.AgilityCompose;
            item.IsBinds = Item.IsBinds;
            item.ValidDate = Item.ValidDate;
        }
    }
}
