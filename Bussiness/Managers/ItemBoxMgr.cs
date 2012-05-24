using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class ItemBoxMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ItemBoxInfo[] m_itemBox;
        private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;        
        public static bool ReLoad()
        {
            try
            {
                ItemBoxInfo[] tempItemBox = LoadItemBoxDb();                
                Dictionary<int,List<ItemBoxInfo>> tempItemBoxs =LoadItemBoxs(tempItemBox);
                if (tempItemBox!=null)
                {
                    Interlocked.Exchange(ref m_itemBox, tempItemBox);
                    Interlocked.Exchange(ref m_itemBoxs, tempItemBoxs);                    
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ReLoad", e);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Initializes the ItemBoxMgr. 
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            return ReLoad();
        }

        /// <summary>
        /// 从数据库中加载箱子物品
        /// </summary>
        /// <returns></returns>
        public static ItemBoxInfo[] LoadItemBoxDb()
        {
            Dictionary<int, ItemBoxInfo> list = new Dictionary<int, ItemBoxInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                ItemBoxInfo[] infos = db.GetItemBoxInfos();
                return infos;    
            }
            return null;            
        }

        /// <summary>
        /// 将物品掉落按宝箱分组
        /// </summary>
        /// <param name="itemBoxs"></param>
        /// <returns></returns>
        public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
        {
            Dictionary<int, List<ItemBoxInfo>> infos = new Dictionary<int, List<ItemBoxInfo>>();
            foreach (ItemBoxInfo info in itemBoxs)
            {
                if (!infos.Keys.Contains(info.DataId))
                { 
                    IEnumerable<ItemBoxInfo> temp=itemBoxs.Where(s=>s.DataId==info.DataId);
                    infos.Add(info.DataId, temp.ToList());
                }
            }
            return infos;
        }

        /// <summary>
        /// 从数据库中加载掉落物品
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static bool LoadItemBoxs(Dictionary<int, List<ItemBoxInfo>> infos)
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                ItemBoxInfo[] items = db.GetItemBoxInfos();
                foreach (ItemBoxInfo item in items)
                {
                    if (!infos.Keys.Contains(item.DataId))
                    {
                        IEnumerable<ItemBoxInfo> temp = items.Where(s => s.DataId == item.DataId);
                        infos.Add(item.DataId, temp.ToList());
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 查找一条箱子物品
        /// </summary>
        /// <param name="DataId"></param>
        /// <returns></returns>
        public static List<ItemBoxInfo> FindItemBox(int DataId)
        {
            if (m_itemBoxs.ContainsKey(DataId))
            {
                List<ItemBoxInfo> items = m_itemBoxs[DataId];
                return items;
            }
            return null;
        }

 
        /// <summary>
        /// 生成一条箱子里面的物品
        /// </summary>
        /// <param name="DateId">传入箱子物品</param>
        /// <returns></returns>
        public static bool CreateItemBox(int DateId,List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken)
        {
            #region 生成物品
            List<ItemBoxInfo> FiltInfos = new List<ItemBoxInfo>();
            List<ItemBoxInfo> unFiltInfos = FindItemBox(DateId);
            if (unFiltInfos == null)
            {
                return false;
            }
            FiltInfos = unFiltInfos.Where(s => s.IsSelect == true).ToList();            
            int dropItemCount = 1;//设置掉落数量
            int maxRound = Bussiness.ThreadSafeRandom.NextStatic(unFiltInfos.Where(s=>s.IsSelect == false).Select(s => s.Random).Max());
            List<ItemBoxInfo> RoundInfos = unFiltInfos.Where(s => s.IsSelect == false && s.Random >= maxRound).ToList();
            int maxItems = RoundInfos.Count();
            if (maxItems >0)
            {                
                dropItemCount = dropItemCount > maxItems ? maxItems : dropItemCount;
                int[] randomArray = GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);   
                foreach(int i in randomArray)            
                {
                    ItemBoxInfo item = RoundInfos[i];
                    if (FiltInfos == null)
                        FiltInfos = new List<ItemBoxInfo>();
                    FiltInfos.Add(item);
                }
            }
            #endregion

            #region 输出物品
            foreach (ItemBoxInfo info in FiltInfos)
            { 
                if(info==null)
                    return false;
                switch(info.TemplateId)
                {
                    case -100:
                        gold+=info.ItemCount;
                        break;
                    case -200:
                        point += info.ItemCount;
                        break;
                    case -300:
                        giftToken += info.ItemCount;
                        break;
                    default:
                        ItemTemplateInfo temp=Bussiness.Managers.ItemMgr.FindItemTemplate(info.TemplateId);
                        ItemInfo item = ItemInfo.CreateFromTemplate(temp, info.ItemCount, 101);
                        if (item == null)
                            continue;
                        item.IsBinds = info.IsBind;
                        item.ValidDate = info.ItemValid;
                        item.StrengthenLevel = info.StrengthenLevel;
                        item.AttackCompose = info.AttackCompose;
                        item.DefendCompose = info.DefendCompose;
                        item.AgilityCompose = info.AgilityCompose;
                        item.LuckCompose = info.LuckCompose;
                        if (itemInfos == null)
                            itemInfos = new List<ItemInfo>();
                        itemInfos.Add(item);
                        break;
                }                
            }
            return true;
            #endregion
        }

        /// <summary>
        /// 产生一个随机数
        /// </summary>
        /// <param name="minValue">起始值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int j;
            int[] resultRound = new int[count];
            for (j = 0; j < count; j++)
            {
                int i = Bussiness.ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (resultRound[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    resultRound[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return resultRound;
        }
    }
}
