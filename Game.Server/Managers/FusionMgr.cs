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
    public class FusionMgr
    {   
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, FusionInfo> _fusions;

        private static System.Threading.ReaderWriterLock m_lock;

        private static ThreadSafeRandom rand;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<string, FusionInfo> tempFusions = new Dictionary<string, FusionInfo>();

                if (LoadFusion(tempFusions))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _fusions = tempFusions;
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
                    log.Error("FusionMgr", e);
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
                _fusions = new Dictionary<string, FusionInfo>();
                rand = new ThreadSafeRandom();
                return LoadFusion(_fusions);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("FusionMgr", e);
                return false;
            }

        }

        private static bool LoadFusion(Dictionary<string, FusionInfo> fusion)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                FusionInfo[] infos = db.GetAllFusion();
                foreach (FusionInfo info in infos)
                {
                    List<int> list = new List<int>();
                    list.Add(info.Item1);
                    list.Add(info.Item2);
                    list.Add(info.Item3);
                    list.Add(info.Item4);
                    list.Add(info.Formula);
                    list.Sort();

                    StringBuilder items = new StringBuilder();
                    foreach (int i in list)
                    {
                        if (i == 0)
                            continue;
                        items.Append(i);
                    }

                    string key = items.ToString();

                    if (!fusion.ContainsKey(key))
                    {
                        fusion.Add(key, info);
                    }
                }
            }

            return true;
        }

        public static ItemTemplateInfo Fusion(List<ItemInfo> Items, List<ItemInfo> AppendItems, ItemInfo Formul, ref bool isBind, ref bool result)
        {
            List<int> list = new List<int>();
            int MaxLevel = 0;
            int TotalRate = 0;
            //int BindType = 0;
            ItemTemplateInfo returnItem = null;

            foreach(ItemInfo p in Items)
            {
                list.Add(p.Template.FusionType);

                if (p.Template.Level > MaxLevel)
                {
                    MaxLevel = p.Template.Level;                    
                }

                TotalRate += p.Template.FusionRate;

                if (p.IsBinds)
                {
                    isBind = true;
                }
            }
            if (Formul.IsBinds)
            {
                isBind = true;
            }

            foreach (ItemInfo p in AppendItems)
            {
                TotalRate += p.Template.FusionRate / 2;
                if (p.IsBinds)
                {
                    isBind = true;
                }
            }
            
            list.Sort();
            StringBuilder itemString = new StringBuilder();
            foreach (int i in list)
            {
                itemString.Append(i);
            }
            itemString.Append(Formul.TemplateID);
            string key = itemString.ToString();

            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_fusions.ContainsKey(key))
                {
                    FusionInfo info = _fusions[key];
                    double rateMax = 0;
                    double rateMin = 0;
                    ItemTemplateInfo temp_0 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel);
                    ItemTemplateInfo temp_1 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 1);
                    ItemTemplateInfo temp_2 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 2);
                    List<ItemTemplateInfo> temps = new List<ItemTemplateInfo>();
                    if (temp_2 != null)
                    {
                        temps.Add(temp_2);
                    }
                    if (temp_1 != null)
                    {
                        temps.Add(temp_1);
                    }
                    if (temp_0 != null)
                    {
                        temps.Add(temp_0);
                    }
                    ItemTemplateInfo tempMax = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) <= 1.1).OrderByDescending(s => (TotalRate / (double)s.FusionNeedRate)).FirstOrDefault();
                    ItemTemplateInfo tempMin = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) > 1.1).OrderBy(s => (TotalRate / (double)s.FusionNeedRate)).FirstOrDefault();
                    if ((tempMax != null) && (tempMin == null))
                    {
                        returnItem=tempMax;
                        result = true;
                    }
                    if ((tempMax != null) && (tempMin != null))
                    {
                        if (tempMax.Level - tempMin.Level == 2)
                        {
                            rateMax = 100 * TotalRate * 0.6 / (double)tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                        }
                        else
                        {
                            rateMax = 100 * TotalRate / (double)tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                        }

                        if (100 * TotalRate / (double)tempMax.FusionNeedRate > rand.Next(100))
                        {
                            returnItem = tempMax;
                            result = true;
                        }
                        else 
                        {
                            returnItem = tempMin;
                            result = true;
                        }                        
                    }
                    if ((tempMax == null) && (tempMin != null))
                    {
                        returnItem=tempMin;
                        result = true;
                    }
                    if(result)
                    {
                        foreach (ItemInfo p in Items)
                        {
                            if (p.Template.TemplateID == returnItem.TemplateID)
                            {
                                result = false;
                                break;
                            }
                        }
                    }

                    return returnItem;
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


        public static Dictionary<int, double> FusionPreview(List<ItemInfo> Items, List<ItemInfo> AppendItems, ItemInfo Formul, ref bool isBind)
        {
            List<int> list = new List<int>();            
            int MaxLevel = 0;
            int TotalRate = 0;
            Dictionary<int, double> Item_Rate = new Dictionary<int, double>();
            Item_Rate.Clear();

            foreach (ItemInfo p in Items)
            {
                list.Add(p.Template.FusionType);

                if (p.Template.Level > MaxLevel)
                {
                    MaxLevel = p.Template.Level;                    
                }

                TotalRate += p.Template.FusionRate;

                if (p.IsBinds)
                {
                    isBind = true;
                }
            }
            if (Formul.IsBinds)
            {
                isBind = true;
            }

            foreach (ItemInfo p in AppendItems)
            {
                TotalRate += p.Template.FusionRate / 2;

                if (p.IsBinds)
                {
                    isBind = true;
                }
            }

            list.Sort();
            StringBuilder itemString = new StringBuilder();
            foreach (int i in list)
            {
                itemString.Append(i);
            }
            itemString.Append(Formul.TemplateID);
            string key = itemString.ToString();

            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_fusions.ContainsKey(key))
                {
                    FusionInfo info = _fusions[key];

                    double rateMax = 0;
                    double rateMin = 0;                    
                    ItemTemplateInfo temp_0 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel);
                    ItemTemplateInfo temp_1 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 1);
                    ItemTemplateInfo temp_2 = Bussiness.Managers.ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 2);
                    List<ItemTemplateInfo> temps = new List<ItemTemplateInfo>();
                    if (temp_2 != null)
                    {                        
                        temps.Add(temp_2);                        
                    }
                    if (temp_1 != null)
                    {
                        temps.Add(temp_1);
                    }
                    if (temp_0 != null)
                    {
                        temps.Add(temp_0);
                    }
                    ItemTemplateInfo tempMax =temps.Where(s => (TotalRate / (double)s.FusionNeedRate) <= 1.1).OrderByDescending(s => (TotalRate / (double)s.FusionNeedRate)).FirstOrDefault();
                    ItemTemplateInfo tempMin = temps.Where(s => (TotalRate / (double)s.FusionNeedRate) > 1.1).OrderBy(s => (TotalRate / (double)s.FusionNeedRate)).FirstOrDefault();
                    if ((tempMax != null)&&(tempMin==null))
                    {
                        Item_Rate.Add(tempMax.TemplateID, 100);
                    }
                    if((tempMax!=null)&&(tempMin!=null))
                    {
                        if (tempMax.Level - tempMin.Level == 2)
                        {
                            rateMax = 100 * TotalRate*0.6 / (double)tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                        }
                        else
                        {
                            rateMax = 100 * TotalRate / (double)tempMax.FusionNeedRate;
                            rateMin = 100 - rateMax;
                        }
                        Item_Rate.Add(tempMax.TemplateID, rateMax);
                        Item_Rate.Add(tempMin.TemplateID, rateMin);
                    }
                    if ((tempMax == null)&&(tempMin != null))
                    {                                                    
                        Item_Rate.Add(tempMin.TemplateID, 100);                        
                    }
                    int[] templist = Item_Rate.Keys.ToArray();
                    foreach (int ID in templist)
                    {
                        foreach (ItemInfo p in Items)
                        {
                            if (ID == p.Template.TemplateID)
                            {
                                if (Item_Rate.ContainsKey(ID))
                                {
                                    Item_Rate.Remove(ID);
                                }

                            }
                        }
                    }

                    return Item_Rate;
                }
                else
                {
                    return Item_Rate;
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

    }
}
