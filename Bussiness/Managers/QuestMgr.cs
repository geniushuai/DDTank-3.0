using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using System.Collections;
using System.Threading;

namespace Bussiness.Managers
{    
    public class QuestMgr
    {
        #region 定义变量

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, QuestInfo> m_questinfo = new Dictionary<int, QuestInfo>();        

        private static Dictionary<int, List<QuestConditionInfo>> m_questcondiction = new Dictionary<int, List<QuestConditionInfo>>();

        private static Dictionary<int, List<QuestAwardInfo>> m_questgoods = new Dictionary<int, List<QuestAwardInfo>>();
        
        #endregion

        #region 任务加载
        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            return ReLoad();
        }

        /// <summary>
        /// 重新加载任务
        /// </summary>
        /// <returns></returns>
        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, QuestInfo> tempQuestInfo = LoadQuestInfoDb();
                Dictionary<int, List<QuestConditionInfo>> tempQuestCondiction = LoadQuestCondictionDb(tempQuestInfo);
                Dictionary<int, List<QuestAwardInfo>> tempQuestGoods = LoadQuestGoodDb(tempQuestInfo);
                if (tempQuestInfo.Count > 0)
                {
                    Interlocked.Exchange(ref m_questinfo, tempQuestInfo);
                    Interlocked.Exchange(ref m_questcondiction, tempQuestCondiction);
                    Interlocked.Exchange(ref m_questgoods, tempQuestGoods);
                }
                return true;
            }
            catch (Exception e)
            {
                log.Error("QuestMgr", e);

            }

            return false;
        }
        #endregion

        #region 从数据库中获取相关的任何数据
        /// <summary>
        /// 从数据库中加载任务模板
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, QuestInfo> LoadQuestInfoDb()
        {
            Dictionary<int, QuestInfo> list = new Dictionary<int, QuestInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                QuestInfo[] infos = db.GetALlQuest();
                foreach (QuestInfo info in infos)
                {
                    if (!list.ContainsKey(info.ID))
                    {
                        list.Add(info.ID, info);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 得到所有任务的条件
        /// </summary>
        /// <param name="quests">任务</param>
        /// <returns>返回任务对应的条件</returns>
        public static Dictionary<int, List<QuestConditionInfo>> LoadQuestCondictionDb(Dictionary<int,QuestInfo> quests)
        {
            Dictionary<int, List<QuestConditionInfo>> list = new Dictionary<int, List<QuestConditionInfo>>();

            using (ProduceBussiness db = new ProduceBussiness())
            {
                QuestConditionInfo[] infos = db.GetAllQuestCondiction();
                foreach (QuestInfo quest in quests.Values)
                {
                    IEnumerable<QuestConditionInfo> temp = infos.Where(s => s.QuestID == quest.ID);
                    list.Add(quest.ID, temp.ToList());
                }
            }
            return list;
        }

        /// <summary>
        /// 得到所有任务的奖励物品
        /// </summary>
        /// <param name="quests">任务</param>
        /// <returns>返回任务的奖励物品</returns>
        public static Dictionary<int, List<QuestAwardInfo>> LoadQuestGoodDb(Dictionary<int, QuestInfo> quests)
        {
            Dictionary<int, List<QuestAwardInfo>> list = new Dictionary<int, List<QuestAwardInfo>>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                QuestAwardInfo[] infos = db.GetAllQuestGoods();
                foreach (QuestInfo quest in quests.Values)
                {
                    IEnumerable<QuestAwardInfo> temp = infos.Where(s => s.QuestID == quest.ID);
                    list.Add(quest.ID, temp.ToList());
                }
            }
            return list;
        }
        #endregion

        #region 查找任务数据

        /// <summary>
        /// 查找一条任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static QuestInfo GetSingleQuest(int id)
        {
            if (m_questinfo.ContainsKey(id))
            {
                return m_questinfo[id];
            }
            return null;
        }

        /// <summary>
        /// 查找一条任务的全部奖励物品
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<QuestAwardInfo> GetQuestGoods(QuestInfo info)
        {
            if (m_questgoods.ContainsKey(info.ID))
            {
                List<QuestAwardInfo> items = m_questgoods[info.ID];
                return items;
            }
            return null;
        }

        /// <summary>
        /// 查找一条任务的全部条件
        /// </summary>
        /// <param name="info">传入一条任务</param>
        /// <returns></returns>
        public static List<QuestConditionInfo> GetQuestCondiction(QuestInfo info)
        {
            if (m_questcondiction.ContainsKey(info.ID))
            {
                List<QuestConditionInfo> items = m_questcondiction[info.ID];
                return items;
            }
            return null;
        }
 
       
        #endregion



    }
}
