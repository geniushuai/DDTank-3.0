using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using Bussiness;
using System.Threading;
using Game.Server.Managers;

namespace Center.Server
{
    public class ConsortiaMrg
    {
        //static int[] level = new int[] { 200, 2500, 6000, 20000, 60000, 160000, 320000, 500000, 900000, 1440000, int.MaxValue };
        //public static int GetLevel(int riches)
        //{
        //    for (int i = 0; i < level.Length; i++)
        //    {
        //        if (riches < level[i])
        //            return i - 1;
        //    }

        //    return 1;
        //}

        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //private static Dictionary<int, ConsortiaInfo> _consortia;

        //private static System.Threading.ReaderWriterLock m_lock;

        /// <summary>
        /// Initializes the StrengthenMgr. 
        /// </summary>
        /// <returns></returns>
        //public static bool Init()
        //{
        //    try
        //    {
        //        m_lock = new System.Threading.ReaderWriterLock();
        //        _consortia = new Dictionary<int, ConsortiaInfo>();
        //        return Load(_consortia);
        //    }
        //    catch (Exception e)
        //    {
        //        if (log.IsErrorEnabled)
        //            log.Error("ConsortiaMgr", e);
        //        return false;
        //    }

        //}

        //private static bool Load(Dictionary<int, ConsortiaInfo> consortia)
        //{
        //    using (ConsortiaBussiness db = new ConsortiaBussiness())
        //    {
        //        ConsortiaInfo[] infos = db.GetConsortiaAll();
        //        foreach (ConsortiaInfo info in infos)
        //        {
        //            if (!info.IsExist)
        //                continue;


        //            if (!consortia.ContainsKey(info.ConsortiaID))
        //            {
        //                consortia.Add(info.ConsortiaID, info);
        //            }
        //        }
        //    }

        //    return true;
        //}

        //public static bool CreateConsortia(int consortiaID, int offer)
        //{
        //    bool result = false;
        //    ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(1);
        //    m_lock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        if (!_consortia.ContainsKey(consortiaID))
        //        {
        //            ConsortiaInfo info = new ConsortiaInfo();
        //            info.BuildDate = DateTime.Now;
        //            info.DeductDate = DateTime.Now;
        //            info.IsExist = true;
        //            info.Level = 1;
        //            info.Honor = offer;
        //            info.Riches = levelInfo.Reward;
        //            info.ConsortiaID = consortiaID;
        //            info.MaxCount = levelInfo.Count;
        //            _consortia.Add(consortiaID, info);
        //            result = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("AddConsortia", ex);
        //    }
        //    finally
        //    {
        //        m_lock.ReleaseWriterLock();
        //    }

        //    return result;
        //}

        //public static bool DeleteConsortia(int consortiaID)
        //{
        //    bool result = false;
        //    m_lock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        if (_consortia.ContainsKey(consortiaID))
        //        {
        //            _consortia[consortiaID].IsExist = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("DeleteConsortia", ex);
        //    }
        //    finally
        //    {
        //        m_lock.ReleaseWriterLock();
        //    }

        //    return result;
        //}

        //public static bool AddConsortiaOffer(int consortiaID, int offer, int riches)
        //{
        //    bool result = false;
        //    int flagID = 0;
        //    m_lock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
        //        {
        //            _consortia[consortiaID].Honor += offer;
        //            _consortia[consortiaID].Riches += riches;
        //            if (riches < 0)
        //            {
        //                ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(_consortia[consortiaID].Level);

        //                if (_consortia[consortiaID].Riches < levelInfo.Riches)
        //                {
        //                    _consortia[consortiaID].Level--;

        //                    if (_consortia[consortiaID].Level < 1)
        //                    {
        //                        _consortia[consortiaID].IsExist = false;
        //                        flagID = _consortia[consortiaID].ConsortiaID;
        //                    }
        //                    else
        //                    {
        //                        levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(_consortia[consortiaID].Level);
        //                        _consortia[consortiaID].MaxCount = levelInfo.Count;
        //                    }
        //                    using (ConsortiaBussiness db = new ConsortiaBussiness())
        //                    {
        //                        db.UpdateConsortia(_consortia[consortiaID]);
        //                    }
        //                }
        //            }
        //            result = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("AddConsortiaOffer", ex);
        //    }
        //    finally
        //    {
        //        m_lock.ReleaseWriterLock();
        //    }

        //    if (flagID != 0)
        //    {
        //        CenterServer.Instance.SendConsortiaDelete(flagID);
        //    }

        //    return result;


        //}

        //public static bool ConsortiaFight(int consortiaID, int riches)
        //{
        //    bool result = false;
        //    int flagID = 0;
        //    m_lock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        //if (_consortia.ContainsKey(win) && _consortia[win].IsExist && _consortia.ContainsKey(lose) && _consortia[lose].IsExist)
        //        //{
        //        //    int riches = _consortia[lose].Riches * 2 / 100;

        //        //    if (riches < 100)
        //        //        riches = 100;

        //        //    _consortia[win].Riches += riches;
        //        //    _consortia[lose].Riches -= riches;


        //        //    ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(_consortia[lose].Level);
        //        //    if (_consortia[lose].Riches < levelInfo.Riches)
        //        //    {
        //        //        _consortia[lose].Level--;

        //        //        if (_consortia[lose].Level < 1)
        //        //        {
        //        //            _consortia[lose].IsExist = false;
        //        //            flagID = _consortia[lose].ConsortiaID;
        //        //        }
        //        //        else
        //        //        {
        //        //            levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(_consortia[lose].Level);
        //        //            _consortia[lose].MaxCount = levelInfo.Count;
        //        //        }
        //        //    }

        //        //    result = true;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("AddConsortiaOffer", ex);
        //    }
        //    finally
        //    {
        //        m_lock.ReleaseWriterLock();
        //    }

        //    if (flagID != 0)
        //    {
        //        CenterServer.Instance.SendConsortiaDelete(flagID);
        //    }

        //    return result;
        //}

        //public static bool ConsortiaUpGrade(int consortiaID)
        //{
        //    bool result = false;
        //    m_lock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
        //        {
        //            ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(_consortia[consortiaID].Level + 1);
        //            if (levelInfo != null)
        //            {
        //                _consortia[consortiaID].Riches += levelInfo.Riches;
        //                _consortia[consortiaID].Level = levelInfo.Level;
        //                _consortia[consortiaID].MaxCount = levelInfo.Count;
        //                result = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("ConsortiaUpGrade", ex);
        //    }
        //    finally
        //    {
        //        m_lock.ReleaseWriterLock();
        //    }

        //    return result;
        //}

        //public static void Save()
        //{
        //    List<int> list = new List<int>();
        //    DateTime now = DateTime.Now;
        //    m_lock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        foreach (ConsortiaInfo info in _consortia.Values)
        //        {
        //            if (now.CompareTo(info.DeductDate.AddDays(7)) > 0)
        //            {
        //                ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(info.Level);
        //                info.DeductDate = DateTime.Now;
        //                info.Riches -= levelInfo.Deduct;

        //                if (info.Riches < levelInfo.Riches)
        //                {
        //                    info.Level--;
        //                    if (info.Level < 1)
        //                    {
        //                        info.IsExist = false;
        //                        list.Add(info.ConsortiaID);
        //                    }
        //                    else
        //                    {
        //                        levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(info.Level);
        //                        info.MaxCount = levelInfo.Count;
        //                    }
        //                }
        //            }

        //            using (ConsortiaBussiness db = new ConsortiaBussiness())
        //            {
        //                if (info.IsDirty)
        //                    db.UpdateConsortia(info);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Save", ex);
        //    }
        //    finally
        //    {
        //        m_lock.ReleaseWriterLock();
        //    }

        //    foreach (int i in list)
        //    {
        //        CenterServer.Instance.SendConsortiaDelete(i);
        //    }
        //}

 
    }
}
