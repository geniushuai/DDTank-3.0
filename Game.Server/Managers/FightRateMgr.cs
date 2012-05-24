using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using Bussiness;
using System.Threading;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;

namespace Game.Server.Managers
{
    public class FightRateMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static System.Threading.ReaderWriterLock m_lock;

        protected static Dictionary<int, FightRateInfo> _fightRate;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, FightRateInfo> tempfightRate = new Dictionary<int, FightRateInfo>();

                if (LoadFightRate(tempfightRate))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _fightRate = tempfightRate;
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
                    log.Error("AwardMgr", e);
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
                _fightRate = new Dictionary<int, FightRateInfo>();
                return LoadFightRate(_fightRate);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("AwardMgr", e);
                return false;
            }

        }

        private static bool LoadFightRate(Dictionary<int, FightRateInfo> fighRate)
        {
            using (ServiceBussiness db = new ServiceBussiness())
            {
                FightRateInfo[] infos = db.GetFightRate(GameServer.Instance.Configuration.ServerID);
                foreach (FightRateInfo info in infos)
                {
                    if (!fighRate.ContainsKey(info.ID))
                    {
                        fighRate.Add(info.ID, info);
                    }
                }
            }

            return true;
        }

        public static FightRateInfo[] GetAllFightRateInfo()
        {
            FightRateInfo[] infos = null;
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                infos = _fightRate.Values.ToArray();
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return infos == null ? new FightRateInfo[0] : infos;
        }

        public static bool CanChangeStyle(BaseRoom game, GSPacketIn pkg)
        {
            FightRateInfo[] infos = GetAllFightRateInfo();
            try
            {
                foreach (FightRateInfo info in infos)
                {
                    if (info.BeginDay.Year <= DateTime.Now.Year && DateTime.Now.Year <= info.EndDay.Year)
                    {
                        if (info.BeginDay.DayOfYear <= DateTime.Now.DayOfYear && DateTime.Now.DayOfYear <= info.EndDay.DayOfYear)
                        {
                            if (info.BeginTime.TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= info.EndTime.TimeOfDay)
                            {
                                if (Bussiness.ThreadSafeRandom.NextStatic(1000000) < info.Rate)
                                {
                                    //game.Data.FightName = info.Name;
                                    //GamePlayer[] selfs = game.GetAllPlayers();

                                    //pkg.WriteBoolean(true);
                                    //pkg.WriteInt(selfs.Length);
                                    //foreach (GamePlayer p in selfs)
                                    //{
                                    //    string style = string.Empty;
                                    //    string color = string.Empty;
                                    //    string skin = string.Empty;
                                    //    p.CurrentInventory.GetStyle(11, p.PlayerCharacter.Sex ? info.BoyTemplateID : info.GirlTemplateID, ref style, ref color, ref skin);

                                    //    //p.UpdateStyle(style,color,skin, p.EquipShow(3, 1, p.PlayerCharacter.Hide), true);
                                    //    pkg.WriteInt(p.PlayerCharacter.ID);
                                    //    pkg.WriteString(style);
                                    //    pkg.WriteInt(p.EquipShow(3, 1, p.PlayerCharacter.Hide));
                                    //    pkg.WriteBoolean(p.PlayerCharacter.Sex);
                                    //    pkg.WriteString(skin);
                                    //    pkg.WriteString(color);

                                    //    p.Out.SendMessage(Game.Server.Packets.eMessageType.ChatNormal, info.SelfCue);
                                    //}

                                    //GamePlayer[] enemys = game.MatchGame.GetAllPlayers();
                                    //foreach (GamePlayer p in enemys)
                                    //{
                                    //    p.Out.SendMessage(Game.Server.Packets.eMessageType.ChatNormal, info.EnemyCue);
                                    //}

                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            catch
            { }

            pkg.WriteBoolean(false);

            return false;
        }
    }
}
