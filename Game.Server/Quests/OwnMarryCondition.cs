using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 17、结婚/空/1
    /// 触发条件：求婚成功、接受求婚.
    /// </summary>
    public class OwnMarryCondition:BaseCondition
    {
        public OwnMarryCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }
        public override void AddTrigger(GamePlayer player)
        {
            //player.OwnMarry += new GamePlayer.PlayerOwnMarryEventHandle(player_OwnMarry);
        }
        
        public override void RemoveTrigger(GamePlayer player)
        {
            //player.OwnMarry -= new GamePlayer.PlayerOwnMarryEventHandle(player_OwnMarry);
        }
        public override bool IsCompleted(GamePlayer player)
        {
            if (player.PlayerCharacter.IsMarried == true)
            {
                Value = 0;
                return true;
            }
            return false;
            
        }

    }
}
