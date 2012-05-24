using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using Bussiness;
using System.Threading;
using System.Collections;

namespace Game.Server.Managers
{
    public enum eRateType
    {
        //经验倍率
        Experience_Rate = 0,

        //财富倍率
        Riches_Rate = 1,

        //功勋倍率
        Offer_Rate = 2,
    }

    public class RateMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static System.Threading.ReaderWriterLock m_lock = new ReaderWriterLock();

        private static ArrayList m_RateInfos = new ArrayList();

        public static bool Init(GameServerConfig config)
        {
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    m_RateInfos = db.GetRate(config.ServerID);
                }
                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("RateMgr", e);
                return false;
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

        }

        public static bool ReLoad()
        {
            return Init(GameServer.Instance.Configuration);
        }

        /// <summary>
        /// 获取游戏倍率
        /// </summary>
        /// <param name="eType">倍率类型</param>
        /// <returns></returns>
        public static float GetRate(eRateType eType)
        {
            float rate = 1;
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                RateInfo _RateInfo = GetRateInfoWithType((int)eType);
                if (_RateInfo == null)
                {
                    return rate;
                }
                if (_RateInfo.Rate == 0)
                {
                    return 1;
                }
                                
                if (IsValid(_RateInfo))
                {
                    rate = _RateInfo.Rate;
                }
            }
            catch { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }

            return rate;
        }

        private static RateInfo GetRateInfoWithType(int type)
        {
            foreach (RateInfo ri in m_RateInfos)
            {
                if (ri.Type == type)
                    return ri;
            }

            return null;
        }

        private static bool IsValid(RateInfo _RateInfo)
        {
            if (_RateInfo.BeginDay == null || _RateInfo.EndDay == null)
                return false;

            if (_RateInfo.BeginDay.Year > DateTime.Now.Year || DateTime.Now.Year > _RateInfo.EndDay.Year)
                return false;

            if (_RateInfo.BeginDay.DayOfYear > DateTime.Now.DayOfYear || DateTime.Now.DayOfYear > _RateInfo.EndDay.DayOfYear)
                return false;

            if (_RateInfo.BeginTime.TimeOfDay > DateTime.Now.TimeOfDay || DateTime.Now.TimeOfDay > _RateInfo.EndTime.TimeOfDay)
                return false;

            return true;
        }

    }
}
