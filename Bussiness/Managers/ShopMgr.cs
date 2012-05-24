using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using System.Threading;
using System.Collections;

namespace Bussiness.Managers
{
    public static class ShopMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int,ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();

        private static System.Threading.ReaderWriterLock m_lock = new System.Threading.ReaderWriterLock();

        /// <summary>
        /// 初始化商品信息
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            return ReLoad();
        }

        /// <summary>
        /// 重新加载商品数据
        /// </summary>
        /// <returns></returns>
        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ShopItemInfo> tempShop = LoadFromDatabase();
                if (tempShop.Count > 0)
                {
                    Interlocked.Exchange(ref m_shop, tempShop);
                }
                return true;
            }
            catch (Exception e)
            {
                log.Error("ShopInfoMgr", e);
            }
            return false;
        }

        #region 从数据库中加载Shop信息
        private static Dictionary<int,ShopItemInfo> LoadFromDatabase()
        {
            Dictionary<int, ShopItemInfo> list = new Dictionary<int, ShopItemInfo>();
            using (ProduceBussiness db = new ProduceBussiness())
            {
                ShopItemInfo[] infos = db.GetALllShop();
   
                foreach (ShopItemInfo info in infos)
                {  
                    if (!list.ContainsKey(info.ID))
                    {
                        list.Add(info.ID, info);
                    }
                }
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 获取一个商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ShopItemInfo GetShopItemInfoById(int ID)
        {
            if (m_shop.ContainsKey(ID))
            {
                return m_shop[ID];

            }
            return null;
        }

        ///// <summary>
        /////查找商品
        ///// </summary>
        ///// <param name="shopID"></param>
        ///// <param name="itemID"></param>
        ///// <returns></returns>
        //public static bool FindShop(int GoodsID)
        //{
        //    if (m_shop.ContainsKey(GoodsID))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        /// <summary>
        /// 判断玩家是否可以购买此商品
        /// </summary>
        /// <param name="shopID"></param>
        /// <param name="ItemID"></param>
        /// <param name="isBinds"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
        {
            bool result = false;
            ConsortiaEquipControlInfo cecInfo = null;
            using (ConsortiaBussiness csbs = new ConsortiaBussiness())
            {

                switch (shopID)
                {
                    case 1:                            //普通
                        result = true;
                        isBinds = false;
                        break;
                    case 2:                            //战斗商城
                        result = true;
                        isBinds = false;
                        break;
                    case 3:                            //礼券
                        result = true;
                        isBinds = false;
                        break;
                    case 4:                            //牌子
                        result = true;
                        isBinds = false;
                        break;
                    case 11:                         
                        cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
                        if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
                        {
                            result = true;
                            isBinds = true;
                        }
                        break;
                    case 12:
                        cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
                        if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
                        {
                            result = true;
                            isBinds = true;
                        }
                        break;
                    case 13:
                        cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
                        if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
                        {
                            result = true;
                            isBinds = true;
                        }
                        break;
                    case 14:
                        cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
                        if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
                        {
                            result = true;
                            isBinds = true;
                        }
                        break;
                    case 15:
                        cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
                        if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
                        {
                            result = true;
                            isBinds = true;
                        }
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 通过商品ID查找模板ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int FindItemTemplateID(int id)
        {
            if (m_shop.ContainsKey(id))
            {
                return m_shop[id].TemplateID;
            }
            return 0;
        }

        /// <summary>
        /// 查找商品信息
        /// </summary>
        /// <param name="TemplatID"></param>
        /// <returns></returns>
        public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
        {
            List<ShopItemInfo> shopItem = new List<ShopItemInfo>();
            foreach (ShopItemInfo shop in m_shop.Values)
            {
                if (shop.TemplateID == TemplatID)
                {
                    shopItem.Add(shop);
                }
            } 

            return shopItem;
        }
    }
}
