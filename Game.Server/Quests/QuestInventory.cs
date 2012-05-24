using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using System.Threading;
using System.Collections;
using log4net;
using System.Reflection;
using Bussiness;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Statics;
using Game.Server.Buffer;
using Game.Server.Packets;

namespace Game.Server.Quests
{
    public class QuestInventory
    {
        #region 定义变量

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private object m_lock;

        /// <summary>
        /// 有效的用户任务记录
        /// </summary>
        protected List<BaseQuest> m_list;

        /// <summary>
        /// 无效的用户任务记录
        /// </summary>
        protected ArrayList m_clearList;

        private GamePlayer m_player;

        private byte[] m_states;
        
        private System.Text.UnicodeEncoding m_converter;

        #endregion

        #region 构造任务管理
        /// <summary>
        /// 构造任务管理
        /// </summary>
        /// <param name="player"></param>
        public QuestInventory(GamePlayer player)
        {
            m_converter = new System.Text.UnicodeEncoding();
            m_player = player;            
            m_lock = new object();
            m_list = new List<BaseQuest>();
            m_clearList = new ArrayList();
        }
        #endregion

        #region 初始化用户任务记录
        /// <summary>
        /// 初始化用户任务记录 [操作：加载有效用户任务列表；加载监听事件]
        /// </summary>
        /// <param name="playerId"></param>
        public void LoadFromDatabase(int playerId)
        {
            lock (m_lock)
            {
                m_states = m_player.PlayerCharacter.QuestSite.Count() ==0 ? InitQuest() : m_player.PlayerCharacter.QuestSite;
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    QuestDataInfo[] datas = db.GetUserQuest(playerId);
                    BeginChanges();
                    foreach (QuestDataInfo dt in datas)
                    {
                        QuestInfo info = QuestMgr.GetSingleQuest(dt.QuestID);
                        if (info != null)
                        {
                            AddQuest(new BaseQuest(info, dt));
                        }
                    }
                    CommitChanges();
                }
                if (m_list != null)
                { 
                }
            }
        }
        #endregion

        #region 保存用户任务到数据库
        /// <summary>
        /// 保存到数据库中 [操作：更新到数据库中全部有效任务记录与过期任务记录]
        /// </summary>
        public void SaveToDatabase()
        {
            lock (m_lock)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {

                    foreach (BaseQuest q in m_list)
                    {
                        q.SaveData();
                        if (q.Data.IsDirty)
                        {
                            pb.UpdateDbQuestDataInfo(q.Data);
                        }
                    }
                    foreach (BaseQuest q in m_clearList)
                    {
                        q.SaveData();
                        pb.UpdateDbQuestDataInfo(q.Data);
                    }
                    m_clearList.Clear();
                }
            }
        }
        #endregion

        #region 将用户任务记录到内存
        /// <summary>
        /// 将用户任务添加到内存，事件开始监听 
        /// </summary>
        /// <param name="quest">传入用户任务</param>
        /// <returns></returns>
        private bool AddQuest(BaseQuest quest)
        {
            lock (m_list)
            {
                m_list.Add(quest);
            }
            OnQuestsChanged(quest);
            quest.AddToPlayer(m_player);
            return true;
        } 
        #endregion

        #region 添加用户任务记录
        /// <summary>
        /// 判断当前用户是否可以接受当前任务，如果可以则添加到内存
        /// </summary>
        /// <param name="info">系统任务</param>
        /// <returns></returns>
        public bool AddQuest(QuestInfo info, out string msg)
        {
            //TrieuLSL
            //if (info == null) return false;
        
            ////if (info.ID == 15)
            //{ 

            //}
            #region 判断是否满足系统任务条件
            msg = "";
            //当前任务是否存在
            try
            {



                if (info == null)
                {
                    msg = "Game.Server.Quests.NoQuest";
                    return false;
                }
                //任务是否开始
                if (info.TimeMode && DateTime.Now.CompareTo(info.StartDate) < 0)
                {
                    msg = "Game.Server.Quests.NoTime";
                }
                //任务是否结束
                if (info.TimeMode && DateTime.Now.CompareTo(info.EndDate) > 0)
                {
                    msg = "Game.Server.Quests.TimeOver";
                }
                //任务是否未达到用户最小等级
                if (m_player.PlayerCharacter.Grade < info.NeedMinLevel)
                {
                    msg = "Game.Server.Quests.LevelLow";
                }
                //任务是否超出用户最高等级
                if (m_player.PlayerCharacter.Grade > info.NeedMaxLevel)
                {
                    msg = "Game.Server.Quests.LevelTop";
                }
                //前置任务是否完成
                if (info.PreQuestID != "0,")
                {
                    string[] tempArry = info.PreQuestID.Split(',');
                    for (int i = 0; i < tempArry.Length - 1; i++)
                    {
                        if (IsQuestFinish(Convert.ToInt32(tempArry[i])) == false)
                        {
                            msg = "Game.Server.Quests.NoFinish";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Info(e.InnerException);
            }
            //判断当前用户是否允许接受工会任务
            if ((info.IsOther == 1) && (!m_player.PlayerCharacter.IsConsortia))
            {
                msg = "Game.Server.Quest.QuestInventory.HaveMarry";
            }
            //判断当前用户是否可以接受结婚任务
            if ((info.IsOther == 2) && (!m_player.PlayerCharacter.IsMarried))
            {
                msg = "Game.Server.Quest.QuestInventory.HaveMarry";
            }
            #endregion

            #region 判断用户已开始的当前任务
            BaseQuest oldData = FindQuest(info.ID);
            //判断当前任务已经完成
            if ((oldData != null) && (oldData.Data.IsComplete))
            {
                msg = "Game.Server.Quests.Have";
            }
            //判断当任务是否可以重复接受
            if ((oldData != null) && (!oldData.Info.CanRepeat))
            {
                msg = "Game.Server.Quests.NoRepeat";
            }
            //判断当前任务可以接受间隔（天）内可重复接受次数
            if ((oldData != null) && (DateTime.Now.CompareTo(oldData.Data.CompletedDate.Date.AddDays(oldData.Info.RepeatInterval)) < 0) && (oldData.Data.RepeatFinish < 1))
            {
                msg = "Game.Server.Quests.Rest";
            }
            BaseQuest _baseQuest = m_player.QuestInventory.FindQuest(info.ID);
            if (_baseQuest != null)
            {
                msg = "Game.Server.Quests.Have";
            }
            #endregion

            #region 当前任务添加到内存
            if (msg == "")
            {
                //check is added
            
                List<QuestConditionInfo> info_condition = Bussiness.Managers.QuestMgr.GetQuestCondiction(info);
                //设置随机获取物品
                int rand = 1;
                if (Bussiness.ThreadSafeRandom.NextStatic(1000000) <= info.Rands)
                {
                    rand = info.RandDouble;
                }

                BeginChanges();

                if (oldData == null)
                {
                    oldData = new BaseQuest(info,new QuestDataInfo());
                    AddQuest(oldData);
                    oldData.Reset(m_player, rand);
                }
                else
                {
                    oldData.Reset(m_player, rand);
                    oldData.AddToPlayer(m_player);
                    OnQuestsChanged(oldData);
                }               

                CommitChanges();
                SaveToDatabase();
                return true;
            }
            else
            {
                msg = LanguageMgr.GetTranslation(msg);                
                return false;
            }
            #endregion
      
        }
        #endregion

        #region 删除一条用户任务记录
        /// <summary>
        /// 移除一条任务
        /// </summary>
        /// <param name="quest">传入用户任务</param>
        /// <returns></returns>
        public bool RemoveQuest(BaseQuest quest)
        {
            if (quest.Info.CanRepeat == false)
            {
                bool result = false;
                lock (m_list)
                {
                    if (m_list.Remove(quest))
                    {
                        m_clearList.Add(quest);
                        result = true;
                    }
                }
                if (result)
                {
                    quest.RemoveFromPlayer(m_player);
                    OnQuestsChanged(quest);
                }
                return result;
            }
            else
            {
                //quest.Data.IsComplete = true;
                
                quest.Reset(m_player,2);
                quest.Data.RepeatFinish++;
                quest.SaveData();
                OnQuestsChanged(quest);
                return true;
            }
        }
        #endregion

        #region 更新任务数据

        public void Update(BaseQuest quest)
        {
            OnQuestsChanged(quest);
        }

        #endregion

        #region 完成任务
        /// <summary>
        /// 用户领奖
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="rewardItemID"></param>
        /// <returns></returns>
        public bool Finish(BaseQuest baseQuest, int selectedItem)
        {            
            //if (baseQuest.CanCompleted(m_player) == false)
            //    return false;

            #region 定义变量
            //提示用户
            string msg = "";
            //奖励Buff
            string RewardBuffName = string.Empty;
            int RewardBuffTime = 0;
            QuestInfo qinfo = baseQuest.Info;
            QuestDataInfo qdata = baseQuest.Data;
            #endregion

            #region 从游戏中领取奖品
            m_player.BeginAllChanges();
            try
            {
                if (baseQuest.Finish(m_player))
                {
                    RemoveQuest(baseQuest);

                    //固定奖励&选择奖励
                    List<QuestAwardInfo> awards = QuestMgr.GetQuestGoods(qinfo);
                    List<ItemInfo> mainBg = new List<ItemInfo>();
                    List<ItemInfo> propBg = new List<ItemInfo>();
                    foreach (QuestAwardInfo award in awards)
                    {
                        //获取固定奖励 或者 已经选取的可选奖励
                        if (award.IsSelect == false || award.RewardItemID == selectedItem)
                        {
                            ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.FindItemTemplate(award.RewardItemID);
                            if (temp != null)
                            {
                                msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", temp.Name, award.RewardItemCount) + " ";
                                Int32 tempCount = award.RewardItemCount;
                                if (award.IsCount == true)
                                {
                                    tempCount = tempCount * qdata.RandDobule;
                                }

                                for (int len = 0; len < tempCount; len += temp.MaxCount)
                                {
                                    int count = len + temp.MaxCount > award.RewardItemCount ? award.RewardItemCount - len : temp.MaxCount;
                                    ItemInfo item = ItemInfo.CreateFromTemplate(temp, count, (int)ItemAddType.Quest);
                                    if (item == null)
                                        continue;
                                    item.ValidDate = award.RewardItemValid;
                                    item.IsBinds = true;
                                    item.StrengthenLevel = award.StrengthenLevel;//等级
                                    item.AttackCompose = award.AttackCompose;    //攻击加成
                                    item.DefendCompose = award.DefendCompose;    //防御加成
                                    item.AgilityCompose = award.AgilityCompose;  //敏捷加成
                                    item.LuckCompose = award.LuckCompose;        //幸运加成
                                    if (temp.BagType == eBageType.PropBag)
                                    {                                        
                                        propBg.Add(item);
                                    }
                                    else
                                    {
                                        mainBg.Add(item);
                                    }
                                }
                            }
                        }
                    }

                    //判断背包的空位是否足够
                    if (mainBg.Count > 0 && m_player.MainBag.GetEmptyCount() < mainBg.Count)
                    {
                        baseQuest.CancelFinish(m_player);
                        m_player.Out.SendMessage(eMessageType.ERROR, m_player.GetInventoryName(eBageType.MainBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
                        return false;
                    }
                    if (propBg.Count > 0 && m_player.PropBag.GetEmptyCount() < propBg.Count)
                    {
                        baseQuest.CancelFinish(m_player);
                        m_player.Out.SendMessage(eMessageType.ERROR, m_player.GetInventoryName(eBageType.PropBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
                        return false;
                    }

                    //把物品放入背包
                    foreach (ItemInfo item in mainBg)
                    {
                        m_player.AddTemplate(item,eBageType.MainBag,1);
                    }
                    foreach (ItemInfo item in propBg)
                    {
                        m_player.AddTemplate(item,eBageType.PropBag,1);
                    }

                    msg = LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward") + msg;

                    //发放Buff
                    if ((qinfo.RewardBuffID > 0) && (qinfo.RewardBuffDate > 0))
                    {
                        ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.FindItemTemplate(qinfo.RewardBuffID);
                        if (temp != null)
                        {
                            RewardBuffTime = qinfo.RewardBuffDate * qdata.RandDobule;
                            AbstractBuffer buffer = BufferList.CreateBufferHour(temp, RewardBuffTime);
                            buffer.Start(m_player);
                            msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardBuff", temp.Name, RewardBuffTime) + " ";
                        }

                    }

                    //奖励金币
                    if (qinfo.RewardGold != 0)
                    {
                        int rewardGold = qinfo.RewardGold * qdata.RandDobule;
                        m_player.AddGold(rewardGold);
                        msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", rewardGold) + " ";
                    }
                    //奖励点卷
                    if (qinfo.RewardMoney != 0)
                    {
                        int rewardMoney = qinfo.RewardMoney * qdata.RandDobule;
                        m_player.AddMoney(qinfo.RewardMoney * qdata.RandDobule);
                        LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Quest, m_player.PlayerCharacter.ID, rewardMoney, m_player.PlayerCharacter.Money, 0, 0, 0, "", "", "");//添加日志
                        msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardMoney", rewardMoney) + " ";
                    }
                    //奖励GP
                    if (qinfo.RewardGP != 0)
                    {
                        int rewardGp = qinfo.RewardGP * qdata.RandDobule;
                        m_player.AddGP(rewardGp);
                        msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGB1", rewardGp) + " ";
                    }
                    //有公会则奖励财富
                    if ((qinfo.RewardRiches != 0) && (m_player.PlayerCharacter.ConsortiaID != 0))
                    {
                        int riches = qinfo.RewardRiches * qdata.RandDobule;
                        m_player.AddRichesOffer(riches);
                        using (ConsortiaBussiness db = new ConsortiaBussiness())
                        {
                            db.ConsortiaRichAdd(m_player.PlayerCharacter.ConsortiaID, ref riches);
                        }
                        msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardRiches", riches) + " ";
                    }
                    //奖励功勋
                    if (qinfo.RewardOffer != 0)
                    {
                        int rewardOffer = qinfo.RewardOffer * qdata.RandDobule;
                        m_player.AddOffer(rewardOffer,false);
                        msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardOffer", rewardOffer) + " ";
                    }
                    //奖励礼劵
                    if (qinfo.RewardGiftToken != 0)
                    {
                        int rewardGiftToken = qinfo.RewardGiftToken * qdata.RandDobule;
                        m_player.AddGiftToken(rewardGiftToken);
                        msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGiftToken", rewardGiftToken + " ");
                    }
                    m_player.Out.SendMessage(eMessageType.Normal, msg);
                    SetQuestFinish(baseQuest.Info.ID);
                    m_player.PlayerCharacter.QuestSite =m_states;                    
                }
                OnQuestsChanged(baseQuest);
         
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Quest Finish：" + ex);
                return false;
            }
            finally
            {
                m_player.CommitAllChanges();
            }
            #endregion
            return true;
        }
        #endregion

        #region 返回当前玩家的一条任务记录
        /// <summary>
        /// 返回当前玩家的一条任务
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public BaseQuest FindQuest(int id)
        {
            foreach (BaseQuest info in m_list)
            {
                
                if (info.Info.ID == id)
                {
                    return info;
                }
            }
            return null;
        }
        #endregion

        #region 内存任务数据操作：BeginChanges/CommiteChanges/UpdateChanges
        /// <summary>
        /// 临时改变的任务列表
        /// </summary>
        protected List<BaseQuest> m_changedQuests = new List<BaseQuest>();

        private int m_changeCount;

        /// <summary>
        /// 单条执行：添加到临时列表；并通知更新客户端数据
        /// </summary>
        /// <param name="quest"></param>
        protected void OnQuestsChanged(BaseQuest quest)
        {
            if (m_changedQuests.Contains(quest) == false)
                m_changedQuests.Add(quest);
            if (m_changeCount <= 0 && m_changedQuests.Count > 0)
            {
                UpdateChangedQuests();
            }
        }

        /// <summary>
        /// 多条执行：开始时将临时列表记数器加1
        /// </summary>
        private void BeginChanges()
        {
            Interlocked.Increment(ref m_changeCount);
        }

        /// <summary>
        /// 多条执行：结束时将临时任务列表减1，则通知更新客户端
        /// </summary>
        private void CommitChanges()
        {
            //控制变量减一
            int changes = Interlocked.Decrement(ref m_changeCount);
            if (changes < 0)
            {
                if (log.IsErrorEnabled)
                    log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
                Thread.VolatileWrite(ref m_changeCount, 0);
            }
            if (changes <= 0 && m_changedQuests.Count > 0)
            {
                UpdateChangedQuests();
            }
        }

        /// <summary>
        /// 通知更新客户端数据，并将临时任务列表清空
        /// </summary>
        public void UpdateChangedQuests()
        {                    
            GSPacketIn pkg = m_player.Out.SendUpdateQuests(m_player, m_states, m_changedQuests.ToArray());
            m_changedQuests.Clear();

        }
        #endregion

        #region 前置任务条件
        private byte[] InitQuest()
        {
            byte[] tempByte = new byte[200];
            for (int i = 0; i < 200; i++)
            {
                tempByte[i] = 0;
            }
            return tempByte;
        }

        /// <summary>
        /// 设置一个任务完成
        /// </summary>
        /// <param name="states"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        private bool SetQuestFinish(int questId)
        {
            if (questId > m_states.Length * 8 || questId < 1)
                return false;
            questId--;
            int index = questId / 8;
            int offset = questId % 8;
            m_states[index] |= (byte)(0x01 << offset);
            Console.WriteLine(m_states[index]);
            return true;
        }

        /// <summary>
        /// 判断一个任务是否完成
        /// </summary>
        /// <param name="states"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        private bool IsQuestFinish(int questId)
        {
            if (questId == 29)
            {
                
            }             
            if (questId > m_states.Length * 8 || questId < 1)
                return false;
            questId--;
            int index = questId / 8;
            int offset = questId % 8;
            int result = m_states[index] & (0x01 << offset);
            return result != 0;
        }

        #endregion

        #region 清除结婚与公会任务
        /// <summary>
        /// 清除非公会用户的任务
        /// </summary>
        /// <returns></returns>
        public bool ClearConsortiaQuest()
        {
            return true;
        }
        /// <summary>
        /// 清除非结婚用户的任务
        /// </summary>
        /// <returns></returns>
        public bool ClearMarryQuest()
        {
            return true;
        }

        #endregion


    }
}
