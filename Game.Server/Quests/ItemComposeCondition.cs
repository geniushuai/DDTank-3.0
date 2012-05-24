using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 19、合成/合成材料道具ID/次数
    /// 触发条件：铁匠铺合成成功。
    /// </summary>
    public class ItemComposeCondition:BaseCondition
    {
        public ItemComposeCondition(BaseQuest quest,QuestConditionInfo info, int value): base(quest,info, value){}

        public override void AddTrigger(GamePlayer player)
        {
            player.ItemCompose += new GamePlayer.PlayerItemComposeEventHandle(player_ItemCompose);
        }
        void player_ItemCompose(int templateID)
        {
            if ((templateID == m_info.Para1)&&(Value>0))
            {
                Value--;
            }
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            player.ItemCompose -= new GamePlayer.PlayerItemComposeEventHandle(player_ItemCompose);
        }
        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }

    }
}
