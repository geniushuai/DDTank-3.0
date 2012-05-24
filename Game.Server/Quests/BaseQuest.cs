using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Bussiness.Managers;

namespace Game.Server.Quests
{
    /// <summary>
    /// 用户任务+任务模板
    /// </summary>
    public class BaseQuest
    {
        private QuestInfo m_info;

        private QuestDataInfo m_data;

        private List<BaseCondition> m_list;

        private GamePlayer m_player;
 

        /// <summary>
        /// 传入用户任务数据
        /// </summary>
        /// <param name="info">一条系统任务</param>
        /// <param name="data">一条用户任务</param>
        public BaseQuest(QuestInfo info, QuestDataInfo data)
        {
            m_info = info;
            m_data = data;
            m_data.QuestID = m_info.ID;
            m_list = new List<BaseCondition>();
            List<QuestConditionInfo> list = QuestMgr.GetQuestCondiction(info);
            int index = 0;
            foreach (QuestConditionInfo ci in list)
            {
                BaseCondition cd = BaseCondition.CreateCondition(this, ci, data.GetConditionValue(index++));
                if (cd != null)
                    m_list.Add(cd);
            }
        }

        /// <summary>
        /// 系统任务
        /// </summary>
        public QuestInfo Info
        {
            get { return m_info; }
        }

        /// <summary>
        /// 用户任务
        /// </summary>
        public QuestDataInfo Data
        {
            get { return m_data; }
        }

        public BaseCondition GetConditionById(int id)
        {
            foreach (BaseCondition cd in m_list)
            {
                if (cd.Info.CondictionID == id)
                {
                    return cd;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="player"></param>
        public void AddToPlayer(GamePlayer player)
        {
            m_player = player;
            if (m_data.IsComplete == false)
            {
                AddTrigger(player);
            }
        }

        /// <summary>
        /// 移除临听
        /// </summary>
        /// <param name="player"></param>
        public void RemoveFromPlayer(GamePlayer player)
        {
            if (m_data.IsComplete == false)
            {
                RemveTrigger(player);
            }
            m_player = null;
        }

        public void Reset(GamePlayer player,int rand)
        {
            m_data.QuestID = m_info.ID;
            m_data.UserID = player.PlayerId;
            m_data.IsComplete = false;
            m_data.IsExist = true;            
            if (m_data.CompletedDate == DateTime.MinValue)
            {
                m_data.CompletedDate = new DateTime(2000, 1, 1);                
            }
            if ((DateTime.Now - m_data.CompletedDate).TotalDays >= m_info.RepeatInterval ) //新的一天则重新更新
            {
                m_data.RepeatFinish = m_info.RepeatMax;
            }
            m_data.RepeatFinish -= 1;
            m_data.RandDobule = rand;
            foreach (BaseCondition cd in m_list)
            {
                cd.Reset(player);
            }
            SaveData();
        }

        private void AddTrigger(GamePlayer player)
        {
            foreach (BaseCondition cd in m_list)
            {
                cd.AddTrigger(player);
            }
        }

        private void RemveTrigger(GamePlayer player)
        {
            foreach (BaseCondition cd in m_list)
            {
                cd.RemoveTrigger(player);
            }
        }

        public void SaveData()
        {
            int index = 0;
            foreach (BaseCondition cd in m_list)
            {
                m_data.SaveConditionValue(index++, cd.Value);
            }
        }

        public void Update()
        {
            SaveData();
            if (m_data.IsDirty && m_player != null)
            {
                m_player.QuestInventory.Update(this);
            }
        }

        /// <summary>
        /// 是否完成
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool CanCompleted(GamePlayer player)
        {
            if (m_data.IsComplete) return false;
            foreach (BaseCondition cd in m_list)
            {
                if (cd.IsCompleted(player) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private DateTime m_oldFinishDate;
        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Finish(GamePlayer player)
        {
            if (CanCompleted(player))
            {
                foreach (BaseCondition cd in m_list)
                {
                    if (cd.Finish(player) == false)
                    {
                        return false;
                    }
                }
                
                if (!this.Info.CanRepeat)
                {
                    m_data.IsComplete = true;
                    RemveTrigger(player);
                }
                m_oldFinishDate = m_data.CompletedDate;
                m_data.CompletedDate = DateTime.Now;                
                return true;
            }
            return false;
        }

        public bool CancelFinish(GamePlayer player)
        {
            m_data.IsComplete = false;
            m_data.CompletedDate = m_oldFinishDate;
            foreach (BaseCondition cd in m_list)
            {
                cd.CancelFinish(player);
            }
            return true;
        }
    }
}
