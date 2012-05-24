using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 11、熔炼成功/熔炼类型/次数
    /// 触发条件：挂在铁匠铺熔炼成功条件下。
    /// </summary>
    public class ItemFusionCondition:BaseCondition
    {
        public  ItemFusionCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            player.ItemFusion += new GamePlayer.PlayerItemFusionEventHandle(player_ItemFusion);
        }
 
        /// <summary>
        /// 熔炼类型
        /// </summary>
        /// <param name="fusionType"></param>
        void player_ItemFusion(int fusionType)
        {
            if ((fusionType == m_info.Para1)&&(Value>0))
            {
                Value--;
            }
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            player.ItemFusion -= new GamePlayer.PlayerItemFusionEventHandle(player_ItemFusion);

        }
        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
