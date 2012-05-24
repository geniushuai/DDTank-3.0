using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
    public class ClientModifyCondition:BaseCondition
    {
        public ClientModifyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value) { }

        public override void Reset(Game.Server.GameObjects.GamePlayer player)
        {
            
            Value = 1;
        }
        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {

            return Value <= 0;
            
        }
    }
} 