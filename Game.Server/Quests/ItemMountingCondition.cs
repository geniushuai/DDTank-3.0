using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 2、用户绑定某一个装备
    /// 触发条件：游戏客户端，用户将物备置入身上。
    /// </summary>
    public class ItemMountingCondition:BaseCondition
    {

        public ItemMountingCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override bool IsCompleted(GamePlayer player)
        {
            return player.MainBag.GetItemCount(0,m_info.Para1) >= m_info.Para2;
        }
    }
}
