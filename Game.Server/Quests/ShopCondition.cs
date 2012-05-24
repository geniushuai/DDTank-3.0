using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 10、购买/货币类型/支付金额
    /// </summary>
    public class ShopCondition:BaseCondition
    {
        public ShopCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            player.Paid += new GamePlayer.PlayerShopEventHandle(player_Shop);
        }

  
        /// <summary>
        /// 购买
        /// </summary>
        /// <param name="price">支付货币类型</param>
        /// <param name="value">支付金额</param>
        void player_Shop(int money, int gold, int offer, int gifttoken, string payGoods)
        {
            //点卷
            if ((m_info.Para1 == -1)&&(money>0))
            {
                Value = Value - money;
            }
            //金币
            if ((m_info.Para1 == -2) && (gold > 0))
            {
                Value = Value - gold;
            }
            //功勋
            if ((m_info.Para1 == -3) && (offer > 0))
            {
                Value = Value - offer;
            }
            //礼卷
            if ((m_info.Para1 == -4) && (gifttoken > 0))
            {
                Value = Value - gifttoken;
            }
            //支付替换类型
            string[] pay= payGoods.Split(',');
            foreach (string i in pay)
            {
                if (i == m_info.Para1.ToString())
                {
                    Value = Value - 1;
                }
            }
            if (Value < 0)
            {
                Value = 0;
            }
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            player.Paid -= new GamePlayer.PlayerShopEventHandle(player_Shop);
        }
        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
