#region 条件类型编号列表
//1、房间类型
//2、NPC编号
//3、关卡编号
//4、副本编号
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Bussiness.Protocol;
using System.Collections;
using Bussiness.Managers;
using Game.Logic.Phy.Object;
using log4net;
using System.Reflection;

namespace Game.Logic
{
    /// <summary>
    /// 掉落物品
    /// </summary>
    public class DropInventory
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static int roundDate = 0;

        public DropInventory()
        {

        }

        /// <summary>
        /// 1、翻牌掉落
        /// </summary>
        /// <returns></returns>
        public static bool CardDrop(eRoomType e, ref List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Cards, ((int)e).ToString(), "0");
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.Cards, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 2、宝箱掉落
        /// </summary>
        /// <returns></returns>
        public static bool BoxDrop(eRoomType e, ref  List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Box, ((int)e).ToString(), "0");
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.Box, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 3、NPC掉落
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool NPCDrop(int dropId, ref List<ItemInfo> info)
        {            
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.NPC, dropId, ref infos))
                {
                    info = infos != null ? infos : null;                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 4、Boss掉落
        /// </summary>
        /// <param name="missionId">关卡</param>
        /// <returns></returns>
        public static bool BossDrop(int missionId, ref List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Boss, missionId.ToString(), "0");
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.Boss, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 5、副本掉落
        /// </summary>
        /// <param name="copyId">副本</param>
        /// <param name="user">1：用户 2：系统</param>
        /// <returns></returns>
        public static bool CopyDrop(int copyId, int user, ref List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Copy, copyId.ToString(), user.ToString());
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.Copy, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  6、特殊掉落
        /// </summary>
        /// <param name="missionId">关卡Id</param>
        /// <param name="boxType">箱子类型</param>
        /// <returns></returns>
        public static bool SpecialDrop(int missionId, int boxType, ref List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Special, missionId.ToString(), boxType.ToString());
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.Special, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    //Console.WriteLine("掉落类型：副本掉落，条件：" + missionId.ToString() + "，掉落物品为：" + info != null ? info.Template.Name : "空物品");
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 7、PVP任务掉落
        /// </summary>
        /// <param name="e"></param>
        /// <param name="playResult"></param>
        /// <param name="info"></param>
        /// <param name="gold"></param>
        /// <param name="money"></param>
        /// <param name="giftToken"></param>
        /// <returns></returns>
        public static bool PvPQuestsDrop(eRoomType e, bool playResult, ref  List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.PvpQuests, ((int)e).ToString(), (Convert.ToInt16(playResult)).ToString());
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.PvpQuests, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 8、开炮掉落
        /// </summary>
        /// <returns></returns>
        public static bool FireDrop(eRoomType e, ref List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.Fire, ((int)e).ToString(), "0");
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.Fire, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        ///  9、PVE任务掉落
        /// </summary>
        /// <param name="copyId"></param>
        /// <param name="npcId"></param>
        /// <param name="info"></param>
        /// <param name="gold"></param>
        /// <param name="money"></param>
        /// <param name="giftToken"></param>
        /// <returns></returns>
        public static bool PvEQuestsDrop(int npcId, ref  List<ItemInfo> info)
        {
            int dropId = GetDropCondiction(eDropType.PveQuests, npcId.ToString(), "0");
            if (dropId > 0)
            {
                List<ItemInfo> infos = null;
                if (GetDropItems(eDropType.PveQuests, dropId, ref infos))
                {
                    info = infos != null ? infos : null;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 10、用户答题掉落
        /// </summary>
        /// <param name="answerId">答题编号</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool AnswerDrop(int answerId, ref List<ItemInfo> info)
        {    
            int dropId = GetDropCondiction(eDropType.Answer, answerId.ToString(), "0");
            if (dropId > 0)
            {
                if (dropId > 0)
                {
                    List<ItemInfo> infos = null;
                    if (GetDropItems(eDropType.Answer, dropId, ref infos))
                    {
                        info = infos != null ? infos : null;
                        return true;
                    }
                }
            }
            return false;            
        }

        /// <summary>
        /// 检测是否满足掉落条件
        /// </summary>
        /// <param name="type">掉落类型</param>
        /// <param name="para1">掉落参数一</param>
        /// <param name="para2">掉落参数二</param>
        /// <returns>返回物品id</returns>
        private static int GetDropCondiction(eDropType type, string para1, string para2)
        {
            try
            {
                int dropId = Bussiness.Managers.DropMgr.FindCondiction(type, para1, para2);
                return dropId;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Drop Error：" + type + " @ " + ex);
            }
            return 0;
        }



        /// <summary>
        /// 获取掉落多个物品
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool GetDropItems(eDropType type, int dropId, ref List<ItemInfo> itemInfos)
        {
            #region 定义变量
            if (dropId == 0)
                return false;
            #endregion

            try
            {
                #region 生成随机物品
                int dropItemCount = 1;
                List<DropItem> unFiltItems = Bussiness.Managers.DropMgr.FindDropItem(dropId);
                int maxRound = Bussiness.ThreadSafeRandom.NextStatic(unFiltItems.Select(s => s.Random).Max());
                List<DropItem> filtItems = unFiltItems.Where(s => s.Random >= maxRound).ToList();
                int maxItems = filtItems.Count();
                if (maxItems == 0)
                {
                    return false;
                }
                else
                {
                    dropItemCount = dropItemCount > maxItems ? maxItems : dropItemCount;
                }
                int[] randomArray = GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);
                #endregion

                #region 设置随机物品属性
                foreach (int i in randomArray)
                {
                    int itemCount = Bussiness.ThreadSafeRandom.NextStatic(filtItems[i].BeginData, filtItems[i].EndData);
                    ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.FindItemTemplate(filtItems[i].ItemId);
                    ItemInfo item = ItemInfo.CreateFromTemplate(temp, itemCount, 101);
                    if (item == null)
                        continue;
                    item.IsBinds = filtItems[i].IsBind;
                    item.ValidDate = filtItems[i].ValueDate;
                    if (itemInfos == null)
                        itemInfos = new List<ItemInfo>();
                    if (DropInfoMgr.CanDrop(temp.TemplateID))//宏观掉落
                    {
                        itemInfos.Add(item);
                    }
                }
                return true;
                #endregion
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Drop Error：" + type + " @ " + ex);
            }
            return false;
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
