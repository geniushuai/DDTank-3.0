using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using System.Data;
using Bussiness;
using Game.Logic;

namespace Game.Server.Statics
{
    public class LogMgr
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static object _syncStop;

        private static int _gameType;
        private static int _serverId;
        private static int _areaId;

        public static DataTable m_LogItem;
        public static DataTable m_LogMoney;
        public static DataTable m_LogFight;

        public static bool Setup(int gametype, int serverid, int areaid)
        {
            _gameType = gametype;
            _serverId = serverid;
            _areaId = areaid;
            _syncStop = new object();

            m_LogItem = new DataTable("Log_Item");
            m_LogItem.Columns.Add("ApplicationId", System.Type.GetType("System.Int32"));
            m_LogItem.Columns.Add("SubId", typeof(int));
            m_LogItem.Columns.Add("LineId", typeof(int));
            m_LogItem.Columns.Add("EnterTime", System.Type.GetType("System.DateTime"));
            m_LogItem.Columns.Add("UserId", typeof(int));
            m_LogItem.Columns.Add("Operation", typeof(int));
            m_LogItem.Columns.Add("ItemName", typeof(string));
            m_LogItem.Columns.Add("ItemID", typeof(int));
            m_LogItem.Columns.Add("AddItem", typeof(string));
            m_LogItem.Columns.Add("BeginProperty", typeof(string));
            m_LogItem.Columns.Add("EndProperty", typeof(string));
            m_LogItem.Columns.Add("Result", typeof(int));

            m_LogMoney = new DataTable("Log_Money");
            m_LogMoney.Columns.Add("ApplicationId", typeof(int));
            m_LogMoney.Columns.Add("SubId", typeof(int));
            m_LogMoney.Columns.Add("LineId", typeof(int));
            m_LogMoney.Columns.Add("MastType", typeof(int));
            m_LogMoney.Columns.Add("SonType", typeof(int));
            m_LogMoney.Columns.Add("UserId", typeof(int));
            m_LogMoney.Columns.Add("EnterTime", System.Type.GetType("System.DateTime"));
            m_LogMoney.Columns.Add("Moneys", typeof(int));
            m_LogMoney.Columns.Add("SpareMoney", typeof(int));
            m_LogMoney.Columns.Add("Gold", typeof(int));
            m_LogMoney.Columns.Add("GiftToken", typeof(int));
            m_LogMoney.Columns.Add("Offer", typeof(int));
            m_LogMoney.Columns.Add("OtherPay", typeof(string));
            m_LogMoney.Columns.Add("GoodId", typeof(string));
            m_LogMoney.Columns.Add("GoodsType", typeof(string));
 

            m_LogFight = new DataTable("Log_Fight");
            m_LogFight.Columns.Add("ApplicationId", typeof(int));
            m_LogFight.Columns.Add("SubId", typeof(int));
            m_LogFight.Columns.Add("LineId", typeof(int));
            m_LogFight.Columns.Add("RoomId", typeof(int));
            m_LogFight.Columns.Add("RoomType", typeof(int));
            m_LogFight.Columns.Add("FightType", typeof(int));
            m_LogFight.Columns.Add("ChangeTeam", typeof(int));
            m_LogFight.Columns.Add("PlayBegin", System.Type.GetType("System.DateTime"));
            m_LogFight.Columns.Add("PlayEnd", System.Type.GetType("System.DateTime"));
            m_LogFight.Columns.Add("UserCount", typeof(int));
            m_LogFight.Columns.Add("MapId", typeof(int));
            m_LogFight.Columns.Add("TeamA", typeof(string));
            m_LogFight.Columns.Add("TeamB", typeof(string));
            m_LogFight.Columns.Add("PlayResult", typeof(string));
            m_LogFight.Columns.Add("WinTeam", typeof(int));
            m_LogFight.Columns.Add("Detail", typeof(string));
            return true; 
        }

        /// <summary>
        /// 重置
        /// </summary>
        public static void Reset()
        {
            lock (m_LogItem)
            {
                m_LogItem.Clear();
            }
            lock (m_LogMoney)
            {
                m_LogMoney.Clear();
            }
 
            lock (m_LogFight)
            {
                m_LogFight.Clear();
            }            
        }

        /// <summary>
        /// 定时保存
        /// </summary>
        public static void Save()
        {
            if (_syncStop != null) //是否已经初始化
            {
                lock (_syncStop)
                {
                    using (ItemRecordBussiness db = new ItemRecordBussiness())
                    {
                        SaveLogItem(db);
                        SaveLogMoney(db);
                        SaveLogFight(db);
                    }
                }
            }
        }

        public static void SaveLogItem(ItemRecordBussiness db)
        {
            
            lock (m_LogItem)
            {
                db.LogItemDb(m_LogItem);
            }
            
        }
        public static void SaveLogMoney(ItemRecordBussiness db)
        {
            lock (m_LogMoney)
            {
                db.LogMoneyDb(m_LogMoney);
            }
        }
 
        public static void SaveLogFight(ItemRecordBussiness db)
        {
            lock (m_LogFight)
            {
                db.LogFightDb(m_LogFight);
            }
        }

        /// <summary>
        /// 添加铁匠铺操作：强化、合成、熔炼、镶嵌
        /// </summary>
        /// <param name="logItemInfo"></param>
        public static void LogItemAdd(int userId, LogItemType itemType, string beginProperty, ItemInfo item, string AddItem, int result)
        {
            try
            {
                string endProperty = "";
                if (item != null)
                    endProperty = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", item.StrengthenLevel, item.Attack, item.Defence, item.Agility, item.Luck, item.AttackCompose, item.DefendCompose, item.AgilityCompose, item.LuckCompose);

                object[] info = { _gameType, _serverId, _areaId, DateTime.Now, userId, (int)itemType, item == null ? "" : item.Template.Name, item == null ? 0 : item.ItemID, AddItem, beginProperty, endProperty, result };
                lock (m_LogItem)
                {
                    m_LogItem.Rows.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("LogMgr Error：ItemAdd @ " + e);
            }
            
        }

        /// <summary>
        /// 添加用户消费记录：
        /// </summary>
        /// <param name="logMoneyInfo"></param>
        public static void LogMoneyAdd(LogMoneyType masterType, LogMoneyType sonType, int userId, int moneys, int SpareMoney, int gold, int giftToken, int offer, string otherPay, string goodId, string goodsType)
        {
            try
            {
                if (moneys != 0 && moneys <= SpareMoney)
                {
                    switch (sonType)
                    {
                        case LogMoneyType.Game_PaymentTakeCard:
                        case LogMoneyType.Game_TryAgain:
                        case LogMoneyType.Auction_Update:
                        case LogMoneyType.Shop_Card:
                        case LogMoneyType.Consortia_Rich:
                        case LogMoneyType.Marry_Unmarry:
                        case LogMoneyType.Game_Boos:
                        case LogMoneyType.Shop_Present:
                        case LogMoneyType.Item_Move:
                        case LogMoneyType.Mail_Pay:
                        case LogMoneyType.Marry_Spark:
                        case LogMoneyType.Marry_Room:
                        case LogMoneyType.Shop_Buy:
                        case LogMoneyType.Item_Color:
                        case LogMoneyType.Shop_Continue:
                        case LogMoneyType.Mail_Send:
                        case LogMoneyType.Marry_RoomAdd:
                        case LogMoneyType.Marry_Hymeneal:
                        case LogMoneyType.Marry_Gift:
                        case LogMoneyType.Marry_Flower:
                            moneys = moneys * (-1);
                            break;
                        default:
                            break;
                    }
                    object[] info = { _gameType, _serverId, _areaId, masterType, sonType, userId, DateTime.Now, moneys, SpareMoney, gold, giftToken, offer, otherPay, goodId, goodsType };
                    lock (m_LogMoney)
                    {
                        m_LogMoney.Rows.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("LogMgr Error：LogMoney @ " + e);
            }            
        }

 

        /// <summary>
        /// 添加战斗记录
        /// </summary>
        /// <param name="logFightInfo"></param>
        public static void LogFightAdd(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA,string teamB, string playResult,int winTeam,string BossWar)
        {
            try
            {
                object[] info = { _gameType, _serverId, _areaId, roomId, (int)roomType, (int)fightType, changeTeam, playBegin, playEnd, userCount, mapId, teamA, teamB, playResult, winTeam, BossWar };

                lock (m_LogFight)
                {
                    m_LogFight.Rows.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("LogMgr Error：Fight @ " + e);
            }
        }
    }
}
