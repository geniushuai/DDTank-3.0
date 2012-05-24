using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 3、使用指定道具/道具ID/数量
    ///  触发条件：战斗使用、背包使用
    /// </summary>
    public class ItemPropertyCondition:BaseCondition
    {                
        public ItemPropertyCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            player.ItemProperty += new GamePlayer.PlayerItemPropertyEventHandle(player_ItemProperty);
        }

        /// <summary>
        /// 使用指定道具
        /// </summary>
        /// <param name="game"></param>
        /// <param name="isWin"></param>
        /// <param name="gainXp"></param>
        void player_ItemProperty(int templateID)
        {
            if ((Value < m_info.Para2) && (templateID == m_info.Para1) && (Value > 0))
            {
                Value--;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.ItemProperty -= new GamePlayer.PlayerItemPropertyEventHandle(player_ItemProperty);
        }

        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            return Value <= 0;
             
        }
    }
}
