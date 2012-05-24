using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using System.Collections;
using System.Threading;
using Bussiness.Protocol;
using System.Data.SqlClient;


namespace Bussiness.Managers
{
    public class DropMgr
    {
        #region 定义变量
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// 掉落类型
        /// </summary>
        private static string[] m_DropTypes = Enum.GetNames(typeof(eDropType));

        /// <summary>
        /// 全部掉落条件
        /// </summary>
        private static List<DropCondiction> m_dropcondiction = new List<DropCondiction>();
                     

        /// <summary>
        /// 掉落物品
        /// </summary>
        private static Dictionary<int, List<DropItem>> m_dropitem = new Dictionary<int, List<DropItem>>();

        #endregion

        #region 掉落加载
        /// <summary>
        /// 掉落加载
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                //加载掉落条件
                List<DropCondiction> tempDropCondiction = LoadDropConditionDb();                
                Interlocked.Exchange(ref m_dropcondiction, tempDropCondiction);
                //加载掉落物品
                Dictionary<int, List<DropItem>> tempDropItem = LoadDropItemDb();
                Interlocked.Exchange(ref m_dropitem, tempDropItem);                
                return true;
            }
            catch (Exception e)
            {
                log.Error("DropMgr", e);
            }
            return false;
        }

        /// <summary>
        /// 获到全部掉落条件
        /// </summary>
        /// <returns></returns>
        public static List<DropCondiction> LoadDropConditionDb()
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                DropCondiction[] infos = db.GetAllDropCondictions();
                return infos != null ? infos.ToList() : null;                
            }            
        }



        /// <summary>
        /// 获取掉落物品
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<DropItem>> LoadDropItemDb()
        {
            Dictionary<int, List<DropItem>> list = new Dictionary<int, List<DropItem>>();

            using (ProduceBussiness db = new ProduceBussiness())
            {
                DropItem[] infos = db.GetAllDropItems();
                foreach (DropCondiction info in m_dropcondiction)
                {
                    IEnumerable<DropItem> temp = infos.Where(s => s.DropId == info.DropId);
                    list.Add(info.DropId, temp.ToList());
                }
            }
            return list;
        }

        /// <summary>
        /// 查询条件后的物品
        /// </summary>
        /// <param name="type"></param>
        /// <param name="para1"></param>
        /// <param name="para2"></param>
        /// <returns></returns>
        public static int FindCondiction(eDropType type, string para1, string para2)
        {
            int itemId = 0;
            string temppara1 = "," + para1 + ",";
            string temppara2 = "," + para2 + ",";
            foreach (DropCondiction drop in m_dropcondiction)
            {
                if ((drop.CondictionType == (int)type) && (drop.Para1.IndexOf(temppara1) !=-1) && (drop.Para2.IndexOf(temppara2) != -1))
                    return drop.DropId;
            }
            return 0;
        }

        /// <summary>
        /// 查找掉落物品
        /// </summary>
        /// <param name="dropId"></param>
        /// <returns></returns>
        public static List<DropItem> FindDropItem(int dropId)
        {
            if (m_dropitem.ContainsKey(dropId))
            {
                return m_dropitem[dropId];
            }
            return null;
        }
        #endregion

     
    }
}
