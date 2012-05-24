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
using Game.Server.Statics;
using Game.Server.Packets;
using Game.Server.Buffer;
using Game.Base.Packets;

namespace Game.Server.Managers
{
    public class AwardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, DailyAwardInfo> _dailyAward;

        private static bool _dailyAwardState;

        private static System.Threading.ReaderWriterLock m_lock;

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, DailyAwardInfo> tempDaily = new Dictionary<int, DailyAwardInfo>();

                if (LoadDailyAward(tempDaily))
                {
                    m_lock.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        _dailyAward = tempDaily;
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
                _dailyAward = new Dictionary<int, DailyAwardInfo>();
                _dailyAwardState = false;
                return LoadDailyAward(_dailyAward);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("AwardMgr", e);
                return false;
            }

        }

        #region DailyAward

        public static bool DailyAwardState
        {
            set
            {
                _dailyAwardState = value;
            }
            get
            {
                return _dailyAwardState;
            }
        }

        private static bool LoadDailyAward(Dictionary<int, DailyAwardInfo> awards)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                DailyAwardInfo[] infos = db.GetAllDailyAward();
                foreach (DailyAwardInfo info in infos)
                {
                    if(!awards.ContainsKey(info.ID))
                    {
                        awards.Add(info.ID, info);
                    }
                }
            }

            return true;
        }

        public static DailyAwardInfo[] GetAllAwardInfo()
        {
            DailyAwardInfo[] infos = null;
            m_lock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                infos = _dailyAward.Values.ToArray();
            }
            catch
            { }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return infos == null ? new DailyAwardInfo[0] : infos;
        }

        //0表示男女，1男2女
        //type 1表示物品，2表示金币，3表示点券，4表示经验，5表示功勋，6表示BUFF
        public static bool AddDailyAward(GamePlayer player)
        {
            if (DateTime.Now.Date == player.PlayerCharacter.LastAward.Date)
            {
                return false;
            }

            //if (player.PlayerCharacter.DayLoginCount > 0)
            //    return false;

            player.PlayerCharacter.DayLoginCount++;
            player.PlayerCharacter.LastAward = DateTime.Now;
            DailyAwardInfo[] infos = GetAllAwardInfo();
            StringBuilder msg = new StringBuilder();            
            string full = string.Empty;
            bool has = false;
            foreach (DailyAwardInfo info in infos)
            {
                if (info.Sex != 0 && (player.PlayerCharacter.Sex ? 1 : 2) != info.Sex)
                    continue;

                has = true;
                switch (info.Type)
                {
                    case 1:
                        ItemTemplateInfo itemTemplateInfo = Bussiness.Managers.ItemMgr.FindItemTemplate(info.TemplateID);
                        if (itemTemplateInfo != null)
                        {
                            int itemCount = info.Count;
                            for (int len = 0; len < itemCount; len += itemTemplateInfo.MaxCount)
                            {
                                int count = len + itemTemplateInfo.MaxCount > itemCount ? itemCount - len : itemTemplateInfo.MaxCount;
                                ItemInfo item = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, (int)ItemAddType.DailyAward);
                                item.ValidDate = info.ValidDate;
                                item.IsBinds = info.IsBinds;                                

                                //if (player.AddItem(item, Game.Server.Statics.ItemAddType.DailyAward, item.GetBagType()) == -1)
                                if (!player.AddTemplate(item,item.Template.BagType,item.Count))                                
                                {
                                    using (PlayerBussiness db = new PlayerBussiness())
                                    {
                                        item.UserID = 0;
                                        db.AddGoods(item);

                                        MailInfo message = new MailInfo();
                                        message.Annex1 = item.ItemID.ToString();
                                        message.Content = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Content", item.Template.Name);
                                        message.Gold = 0;
                                        message.Money = 0;
                                        message.Receiver = player.PlayerCharacter.NickName;
                                        message.ReceiverID = player.PlayerCharacter.ID;
                                        message.Sender = message.Receiver;
                                        message.SenderID = message.ReceiverID;
                                        message.Title = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Title", item.Template.Name);
                                        message.Type = (int)eMailType.DailyAward;
                                        db.SendMail(message);

                                        full = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Mail");
                                    }
                                }
                            }
                        }
                        break;
                    case 2:                        
                        player.AddGold(info.Count);
                        break;
                    case 3:                        
                        player.AddMoney(info.Count);
                        LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Daily, player.PlayerCharacter.ID, info.Count, player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                        break;
                    case 4:                        
                        player.AddGP(info.Count);
                        break;
                    case 5:
                        player.AddOffer(info.Count, false);
                        break;
                    case 6:
                        ItemTemplateInfo template = Bussiness.Managers.ItemMgr.FindItemTemplate(info.TemplateID);
                        if (template != null)
                        {
                            AbstractBuffer buffer = BufferList.CreateBufferHour(template, info.ValidDate);
                            buffer.Start(player);                            
                        }
                        break;
                }
            }
            

            if (has)
            {             
                //player.Out.SendMessage(eMessageType.DailyAward, full + msg.ToString());

                if (!string.IsNullOrEmpty(full))
                {
                    player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
                }
            }

            return true;
        }

        #endregion
    }
}
