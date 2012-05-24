using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{

    /// <summary>
    /// 9、强化/装备类型/强化等级
    /// 触发条件：铁匠铺中强化成功
    /// </summary>
    public class ItemStrengthenCondition:BaseCondition
    {
        public ItemStrengthenCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.ItemStrengthen += new GamePlayer.PlayerItemStrengthenEventHandle(player_ItemStrengthen);
        }
        void player_ItemStrengthen(int categoryID,int level)
        {
            if ((m_info.Para1 == categoryID)&&(m_info.Para2<=level))
            {
                Value = 0;
            }
        }
        public override void RemoveTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.ItemStrengthen -= new GamePlayer.PlayerItemStrengthenEventHandle(player_ItemStrengthen);
        }

        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            ///TrieuLSL
            return true;
        }
    }
}
