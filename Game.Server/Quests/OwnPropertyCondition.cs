using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 14、拥有道具（完成任务道具不消失）/道具ID/数量
    /// 触发条件：提取邮件、战斗完成物品结算
    /// </summary>
    public class OwnPropertyCondition:BaseCondition
    {
        public OwnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            //player.OwnProperty += new GamePlayer.PlayerOwnPropertyEventHandle(player_OwnProperty);
        }
        void player_OwnProperty()
        { 
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            //player.OwnProperty -= new GamePlayer.PlayerOwnPropertyEventHandle(player_OwnProperty);
        }
        public override bool IsCompleted(GamePlayer player)
        {            
            if (player.GetItemCount(m_info.Para1) >= m_info.Para2)
            {
                Value = 0;
                return true;
            }
            return false;
        }

    }
}
