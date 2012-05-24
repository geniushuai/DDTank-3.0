using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 系统的任务
    /// </summary>
    public class QuestInfo : DataObject
    {
        #region 系统的任务
        /// <summary>
        /// 任务的唯一ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 任务显示的位置，0为主线任务，1为支线任务，2为日常任务
        /// </summary>
        public int QuestID { get; set; }

        /// <summary>
        /// 在游戏中显示的任务标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 在游戏中显示的任务详细内容
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 在游戏中显示的任务目标
        /// </summary>
        public string Objective { get; set; }

        /// <summary>
        /// 接受该任务的最小等级，玩家等级小于此等级则无法接受该任务
        /// </summary>
        public int NeedMinLevel { get; set; }

        /// <summary>
        /// 接受该任务的最大等级，玩家等级大于此等级则无法接受该任务
        /// </summary>
        public int NeedMaxLevel { get; set; }

        /// <summary>
        /// 该任务的前置任务，可以有复数个，接到该任务需要完成所有前置任务
        /// </summary>
        public string PreQuestID { get; set; }

        /// <summary>
        /// 该任务的后置任务，可以有复数个，接到该任务需要完成所有前置任务
        /// </summary>
        public string NextQuestID { get; set; }

        /// <summary>
        /// 0表无1表公会2表结婚
        /// </summary>
        public int IsOther { get; set; }

        /// <summary>
        /// 是否可以重复完成，可以为1，不可以为-1
        /// </summary>
        public bool CanRepeat { get; set; }

        /// <summary>
        /// 可重复任务的时间间隔，单位为小时，以服务器时间为准定时刷新，不考虑接受任务的具体时间
        /// </summary>
        public int RepeatInterval { get; set; }

        /// <summary>
        /// 在一段时间间隔内可重复提交的任务的次数
        /// </summary>
        public int RepeatMax { get; set; }

        /// <summary>
        /// 任务奖励的经验
        /// </summary>
        public int RewardGP { get; set; }

        /// <summary>
        /// 任务奖励的金币
        /// </summary>
        public int RewardGold { get; set; }

        /// <summary>
        /// 任务奖励的礼金
        /// </summary>
        public int RewardGiftToken { get; set; }

        /// <summary>
        /// 任务奖励的功勋
        /// </summary>
        public int RewardOffer { get; set; }

        /// <summary>
        /// 任务奖励的财富
        /// </summary>
        public int RewardRiches { get; set; }

        /// <summary>
        /// 任务奖励的buff
        /// </summary>
        public int RewardBuffID { get; set; }

        /// <summary>
        /// 任务奖励buff的时间
        /// </summary>
        public int RewardBuffDate { get; set; }

        /// <summary>
        /// 任务奖励的点券
        /// </summary>
        public int RewardMoney { get; set; }

        /// <summary>
        /// 任务随机奖励机率
        /// </summary>
        public decimal Rands { get; set; }

        /// <summary>
        /// 随机奖励倍率   随机奖励倍率
        /// </summary>
        public int RandDouble { get; set; }

        /// <summary>
        /// 是否有时间限制，1为有，0为没有
        /// </summary>
        public bool TimeMode { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        #endregion Model
    }
}
