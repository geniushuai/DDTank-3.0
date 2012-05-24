using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{

    /// <summary>
    /// 21、通关关卡/关卡ID/回合数
    /// 触发条件：挂在客户端结算画面。
    /// </summary>
    public  class GameMissionOverCondition:BaseCondition
    {
        public GameMissionOverCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value) { }
        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.MissionTurnOver += new GamePlayer.PlayerMissionTurnOverEventHandle(player_MissionOver);
        }

        void player_MissionOver(AbstractGame game, int missionId, int turnCount)
        {
            if (((missionId == m_info.Para1) || (m_info.Para1 == -1))&&(turnCount<=m_info.Para2) && (Value > 0))
            {
                Value=0;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.MissionTurnOver -= new GamePlayer.PlayerMissionTurnOverEventHandle(player_MissionOver);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
