using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using Bussiness;
using System.Threading;

namespace Game.Server.Managers
{
    public class ExperienceRateMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static System.Threading.ReaderWriterLock m_lock;

        protected static ExperienceRateInfo _RateInfo;

        public static bool Init()
        {
            try
            {
                //_RateInfo = new ExperienceRateInfo();
                //_RateInfo.Rate = 1;
                m_lock = new System.Threading.ReaderWriterLock();

                using (ServiceBussiness db = new ServiceBussiness())
                {
                    _RateInfo = db.GetExperienceRate(WorldMgr.ServerID);
                }

                if (_RateInfo == null)
                {
                    _RateInfo = new ExperienceRateInfo();
                    _RateInfo.Rate = -1;
                }

                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExperienceRateMgr", e);
                return false;
            }

        }

        public static bool ReLoad()
        {
            try
            {
                m_lock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    using (ServiceBussiness db = new ServiceBussiness())
                    {
                        _RateInfo = db.GetExperienceRate(WorldMgr.ServerID);
                    }

                    if (_RateInfo == null)
                    {
                        _RateInfo = new ExperienceRateInfo();
                        _RateInfo.Rate = -1;
                    }

                    return true;
                }
                catch{ }
                finally
                {
                    m_lock.ReleaseWriterLock();
                }                
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ExperienceRateMgr", e);
            }

            return false;
        }

        public static ExperienceRateInfo Rate
        {
            set
            {
                _RateInfo.Rate = value.Rate;
                _RateInfo.BeginDay = value.BeginDay;
                _RateInfo.EndDay = value.EndDay;
                _RateInfo.BeginTime = value.BeginTime;
                _RateInfo.EndTime = value.EndTime;
            }
        }

        public static int GetGPRate()
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            int rate = 1;
            try
            {
                if (_RateInfo.Rate == -1)
                {
                    return 1;
                }
                                
                if (_RateInfo.BeginDay != null && _RateInfo.EndDay != null)
                {
                    if (_RateInfo.BeginDay.Year <= DateTime.Now.Year && DateTime.Now.Year <= _RateInfo.EndDay.Year)
                    {
                        if (_RateInfo.BeginDay.DayOfYear <= DateTime.Now.DayOfYear && DateTime.Now.DayOfYear <= _RateInfo.EndDay.DayOfYear)
                        {
                            if (_RateInfo.BeginTime.TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= _RateInfo.EndTime.TimeOfDay)
                            {
                                rate = _RateInfo.Rate;
                            }
                        }

                    }
                }
            }
            catch { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }

            return rate;
        }

    }
}
