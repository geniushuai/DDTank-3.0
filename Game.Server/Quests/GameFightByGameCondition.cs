using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server.Quests
{
    /// <summary>
    ///  23、完成战斗（无论胜败）/战斗模式（-1不限，0撮合，1自由，2练级，3副本）/数量
    ///  触发条件：挂在游戏结算画面
    /// </summary>
    public class GameFightByGameCondition : BaseCondition
    {
        public GameFightByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value) { }

        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.GameOver += new Game.Server.GameObjects.GamePlayer.PlayerGameOverEventHandle(player_GameOver);
        }

        public override void RemoveTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.GameOver -= new Game.Server.GameObjects.GamePlayer.PlayerGameOverEventHandle(player_GameOver);
        }

        void player_GameOver(AbstractGame game, bool isWin, int gainXp)
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

        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
