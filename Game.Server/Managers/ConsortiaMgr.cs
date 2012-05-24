using System;
using System.Collections.Generic;
using SqlDataProvider.Data;
using Bussiness;
using log4net;
using System.Reflection;
using System.Threading;
using Game.Server.GameObjects;
using Game.Logic;
using Game.Logic.Phy.Object;

namespace Game.Server.Managers
{
    //0中立，1同盟，2敌对,-1为同个公会或一方以上没公会
    public class ConsortiaMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, int> _ally;

        private static System.Threading.ReaderWriterLock m_lock;

        private static Dictionary<int, ConsortiaInfo> _consortia;

        #region reload

        public static bool ReLoad()
        {
            try
            {
                Dictionary<string, int> tempAlly = new Dictionary<string, int>();
                Dictionary<int, ConsortiaInfo> tempConsortia = new Dictionary<int, ConsortiaInfo>();

                if (Load(tempAlly) && LoadConsortia(tempConsortia))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _ally = tempAlly;
                        _consortia = tempConsortia;
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
                    log.Error("ConsortiaMgr", e);
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

                _ally = new Dictionary<string, int>();
                if (!Load(_ally))
                    return false;

                _consortia = new Dictionary<int, ConsortiaInfo>();
                if (!LoadConsortia(_consortia))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ConsortiaMgr", e);
                return false;
            }

        }

        private static bool Load(Dictionary<string, int> ally)
        {
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                string key;
                ConsortiaAllyInfo[] infos = db.GetConsortiaAllyAll();
                foreach (ConsortiaAllyInfo info in infos)
                {
                    if (!info.IsExist)
                        continue;

                    if (info.Consortia1ID < info.Consortia2ID)
                    {
                        key = info.Consortia1ID + "&" + info.Consortia2ID;
                    }
                    else
                    {
                        key = info.Consortia2ID + "&" + info.Consortia1ID;
                    }

                    if (!ally.ContainsKey(key))
                    {
                        ally.Add(key, info.State);
                    }
                }
            }

            return true;
        }

        private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia)
        {
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaInfo[] infos = db.GetConsortiaAll();
                foreach (ConsortiaInfo info in infos)
                {
                    if (!info.IsExist)
                        continue;


                    if (!consortia.ContainsKey(info.ConsortiaID))
                    {
                        consortia.Add(info.ConsortiaID, info);
                    }
                }
            }

            return true;
        }

        public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
        {
            string key;
            if (cosortiaID1 < consortiaID2)
            {
                key = cosortiaID1 + "&" + consortiaID2;
            }
            else
            {
                key = consortiaID2 + "&" + cosortiaID1;
            }

            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (!_ally.ContainsKey(key))
                {
                    _ally.Add(key, state);
                }
                else
                {
                    _ally[key] = state;
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

            return 0;
        }

        public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
        {
            bool result = false;
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].Level = consortiaLevel;
                }
                else
                {
                    ConsortiaInfo info = new ConsortiaInfo();
                    info.BuildDate = DateTime.Now;
                    info.Level = consortiaLevel;
                    info.IsExist = true;
                    _consortia.Add(consortiaID, info);
                }
            }
            catch (Exception ex)
            {
                log.Error("ConsortiaUpGrade", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

            return result;
        }

        public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
        {
            bool result = false;
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].StoreLevel = storeLevel;
                }
            }
            catch (Exception ex)
            {
                log.Error("ConsortiaUpGrade", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

            return result;
        }

        public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
        {
            bool result = false;
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].ShopLevel = shopLevel;
                }
            }
            catch (Exception ex)
            {
                log.Error("ConsortiaUpGrade", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

            return result;
        }

        public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
        {
            bool result = false;
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
                {
                    _consortia[consortiaID].SmithLevel = smithLevel;
                }
            }
            catch (Exception ex)
            {
                log.Error("ConsortiaUpGrade", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

            return result;
        }

        public static bool AddConsortia(int consortiaID)
        {
            bool result = false;
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (!_consortia.ContainsKey(consortiaID))
                {
                    ConsortiaInfo info = new ConsortiaInfo();
                    info.BuildDate = DateTime.Now;
                    info.Level = 1;
                    info.IsExist = true;
                    info.ConsortiaName = "";
                    info.ConsortiaID = consortiaID;
                    _consortia.Add(consortiaID, info);
                }
            }
            catch (Exception ex)
            {
                log.Error("ConsortiaUpGrade", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }

            return result;
        }

        #endregion

        #region query

        public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
        {
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_consortia.ContainsKey(consortiaID))
                {
                    return _consortia[consortiaID];
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

        //-1为不能打公会战，0中立战，2敌对战
        public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
        {
            if (consortiaID1 == 0 || consortiaID2 == 0 || consortiaID1 == consortiaID2)
                return -1;

            ConsortiaInfo consortia1 = FindConsortiaInfo(consortiaID1);
            ConsortiaInfo consortia2 = FindConsortiaInfo(consortiaID2);

            if (consortia1 == null || consortia2 == null || consortia1.Level < 3 || consortia2.Level < 3)
                return -1;

            return FindConsortiaAlly(consortiaID1, consortiaID2);
        }

        /// <summary>
        /// 查询公会关系
        /// </summary>
        /// <param name="cosortiaID1"></param>
        /// <param name="consortiaID2"></param>
        /// <returns></returns>
        public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
        {
            if (cosortiaID1 == 0 || consortiaID2 == 0 || cosortiaID1 == consortiaID2)
                return -1;

            string key;
            if (cosortiaID1 < consortiaID2)
            {
                key = cosortiaID1 + "&" + consortiaID2;
            }
            else
            {
                key = consortiaID2 + "&" + cosortiaID1;
            }

            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (_ally.ContainsKey(key))
                {
                    return _ally[key];
                }
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }

            return 0;
        }

        public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
        {
            return GetOffer(FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
        }

        private static int GetOffer(int state, eGameType gameType)
        {
            switch (gameType)
            {
                case eGameType.Free:
                    switch (state)
                    {
                        case 0:
                            return 1;
                        case 1:
                            return 0;
                        case 2:
                            return 3;
                    }
                    break;
                case eGameType.Guild:
                    switch (state)
                    {
                        case 0:
                            return 5;
                        case 1:
                            return 0;
                        case 2:
                            return 10;
                    }
                    break;
            }
            return 0;
        }

        #endregion

        public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
        {
            if (roomType != eRoomType.Match)
                return -1;

            int state = FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);

            if (state == -1)
                return state;

            int offer = GetOffer(state, gameClass);
            if (lose.PlayerCharacter.Offer < offer)
                offer = lose.PlayerCharacter.Offer;

            if (offer != 0)
            {
                players[win].GainOffer = offer;
                players[lose].GainOffer = -offer;

            }

            return state;
        }

        public static int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
        {

            if (roomType != eRoomType.Match)
                return 0;

            int playerCount = playercount / 2;



            int riches = 0;
            int state = 2;
            int rate = 1;
            int value = 3;
            if (gameClass == eGameType.Guild)
            {
                value = 10;
                rate = (int)RateMgr.GetRate(eRateType.Offer_Rate);
            }

            float richesRate = RateMgr.GetRate(eRateType.Riches_Rate);
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {


                if (gameClass == eGameType.Free)
                {
                    playerCount = 0;
                }
                else
                {
                    db.ConsortiaFight(consortiaWin, consortiaLose, playerCount, out riches, state, totalKillHealth, richesRate);
                }
                //（对方公会等级-3）*50*对方玩家人数（对方玩家人数是指进入战斗时的人数，中途退出或者掉线的玩家不改变这个数值）
                //玩家增加掠夺财富
                foreach (KeyValuePair<int, Player> p in players)
                {
                    //if (p.Value.State == TankGameState.LOSE)
                    //    continue;
                    if (p.Value == null)
                        continue;

                    if (p.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaWin)
                    {

                        p.Value.PlayerDetail.AddOffer((playerCount + value) * rate);

                        p.Value.PlayerDetail.PlayerCharacter.RichesRob += riches;
                    }
                    else if (p.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaLose)
                    {
                        p.Value.PlayerDetail.AddOffer((int)Math.Round(playerCount * 0.5) * rate);
                        p.Value.PlayerDetail.RemoveOffer(value);
                    }
                }

            }

            return riches;
        }

    }
}
