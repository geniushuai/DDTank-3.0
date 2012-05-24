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
    /// 8、通关副本（要求胜利）/副本ID/次数
    /// 触发条件：挂在客户端游戏结算画面
    /// </summary>
    public class GameCopyPassCondition:BaseCondition
    {
        public GameCopyPassCondition(BaseQuest quest,QuestConditionInfo info ,int value):base(quest,info,value){}

        public override void AddTrigger(GamePlayer player)
        {
            player.MissionOver += new GamePlayer.PlayerMissionOverEventHandle(player_MissionOver);
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.MissionOver -= new GamePlayer.PlayerMissionOverEventHandle(player_MissionOver);
        }

        void player_MissionOver(AbstractGame game, int missionId, bool isWin)
        {
            if ((isWin == true) && (missionId == m_info.Para1) && (Value > 0))
            {
                Value--;
            }
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
