using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 12、炼化/装备类型/炼化等级
    /// 触发条件：铁匠铺炼化成功
    /// </summary>
    public class ItemMeltCondition:BaseCondition
    {
        public ItemMeltCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }
        public override void AddTrigger(GamePlayer player)
        {
            player.ItemMelt += new GamePlayer.PlayerItemMeltEventHandle(player_ItemMelt);
        }

        /// <summary>
        /// 炼化指定装备类型，默认等级为1
        /// </summary>
        /// <param name="categoryID"></param>
        void player_ItemMelt(int categoryID)
        {
            if (categoryID == m_info.Para1)
            {
                Value = 0;
            }
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            player.ItemMelt -= new GamePlayer.PlayerItemMeltEventHandle(player_ItemMelt);            
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
