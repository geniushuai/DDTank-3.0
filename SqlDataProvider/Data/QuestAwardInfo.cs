using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class QuestAwardInfo
    {
        #region 任务物品  
        /// <summary>
        /// 任务编号
        /// </summary>
        public int QuestID{get;set;}

        /// <summary>
        /// 奖励物品名称
        /// </summary>
        public int RewardItemID { get; set; }

        /// <summary>
        /// 0表必选取 1表多选
        /// </summary>
        public bool IsSelect { get; set; }

        /// <summary>
        /// 有效期（天）
        /// </summary>
        public int RewardItemValid { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int RewardItemCount { get; set; }

        /// <summary>
        /// 强化等级
        /// </summary>
        public int StrengthenLevel { get; set; }

        /// <summary>
        /// 攻击加成
        /// </summary>
        public int AttackCompose { get; set; }

        /// <summary>
        /// 防御加成
        /// </summary>
        public int DefendCompose { get; set; }

        /// <summary>
        /// 敏捷加成
        /// </summary>
        public int AgilityCompose { get; set; }

        /// <summary>
        /// 幸运加成
        /// </summary>
        public int LuckCompose { get; set; }

        /// <summary>
        /// 是否受奖励倍率影响
        /// </summary>
        public bool IsCount { get; set; }

        #endregion Model
    }
}
