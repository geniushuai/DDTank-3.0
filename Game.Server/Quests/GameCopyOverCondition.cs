using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Game.Logic;

namespace Game.Server.Quests
{
    /// <summary>
    /// 7、完成副本（无论胜败）/副本ID/次数
    /// 触发条件：挂在客户端结算画面。
    /// </summary>
    public class GameCopyOverCondition:BaseCondition
    {
        public GameCopyOverCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }
        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.MissionOver += new GamePlayer.PlayerMissionOverEventHandle(player_MissionOver);
        }

        void player_MissionOver(AbstractGame game, int missionId, bool isWin)
        {
            if (((missionId == m_info.Para1) || (m_info.Para1 == -1)) && (Value > 0))
            {
                Value--;
            }
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.MissionOver -= new GamePlayer.PlayerMissionOverEventHandle(player_MissionOver);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }

    }
}
