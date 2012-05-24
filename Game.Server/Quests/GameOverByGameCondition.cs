using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server.Quests
{
    /// <summary>
    ///  24、战斗胜利/房间模式/数量
    ///  触发条件：游戏客户端结算
    /// </summary>
    public class GameOverByGameCondition : BaseCondition
    {

        public GameOverByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value) { }

        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.GameOver += new Game.Server.GameObjects.GamePlayer.PlayerGameOverEventHandle(player_GameOver);
        }

        void player_GameOver(AbstractGame game, bool isWin, int gainXp)
        {
            if (isWin == true)
            {
                switch (game.GameType)
                {
                    case eGameType.Free:
                        if (((m_info.Para1 == 0) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eGameType.Guild:
                        if (((m_info.Para1 == 1) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eGameType.Training:
                        if (((m_info.Para1 == 2) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eGameType.ALL:
                        if (((m_info.Para1 == 4) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eGameType.Exploration:
                        if (((m_info.Para1 == 5) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eGameType.Boss:
                        if (((m_info.Para1 == 6) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eGameType.Treasure:
                        if (((m_info.Para1 == 7) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    default:
                        break;

                }
                if (Value < 0)
                {
                    Value = 0;
                }
            }
        }
        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
