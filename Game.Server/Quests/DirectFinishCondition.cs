using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    public class DirectFinishCondition:BaseCondition
    {
        /// <summary>
        ///  16、直接完成/空/1
        ///  触发条件：由客户端发出该任务。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="value"></param>
        public DirectFinishCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override bool IsCompleted(GamePlayer player)
        {
            return true;
        }
    }
}
