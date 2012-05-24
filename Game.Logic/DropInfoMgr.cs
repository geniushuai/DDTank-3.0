using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using System.IO;
using System.Timers;
using Game.Base.Packets;

namespace Game.Logic
{
    public class MacroDropInfo
    {
        public int SelfDropCount { set; get; }
        public int DropCount { set; get; }
        public int MaxDropCount { set; get; }

        public MacroDropInfo(int dropCount, int maxDropCount)
        {
            DropCount = dropCount;
            MaxDropCount = maxDropCount;
        }
    }

    public class DropInfoMgr
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static System.Threading.ReaderWriterLock m_lock = new System.Threading.ReaderWriterLock();

        public static Dictionary<int, MacroDropInfo> DropInfo;

        public static bool CanDrop(int templateId)
        {
            if (DropInfo == null)
            {
                return true;
            }
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (DropInfo.ContainsKey(templateId))
                {
                    MacroDropInfo mdi = DropInfo[templateId];
                    if (mdi.DropCount < mdi.MaxDropCount || mdi.SelfDropCount >= mdi.DropCount)
                    {
                        mdi.SelfDropCount++;
                        mdi.DropCount++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DropInfoMgr CanDrop", e);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            //不包含该物品，直接返回true
            return true;
        }
    }
}
