using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server.Quests
{
    /// <summary>
    ///  6、战斗胜利/房间模式/数量
    ///  触发条件：游戏客户端结算
    /// </summary>
    public class GameOverCondition:BaseCondition
    {
        public GameOverCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.GameOver += new Game.Server.GameObjects.GamePlayer.PlayerGameOverEventHandle(player_GameOver);
        }

        void player_GameOver(AbstractGame game, bool isWin, int gainXp)
        {
            if (isWin == true)
            {
                switch (game.RoomType)
                {
                    case eRoomType.Match:
                        if (((m_info.Para1 == 0) || (m_info.Para1 == -1)) && (Value > 0))
                            Value--;
                        break;
                    case eRoomType.Freedom:
                        if (((m_info.Para1 == 1) || (m_info.Para1 == -1)) && (Value > 0))
                            Value--;
                        break;
                    default:
                        break;
                }           
            }
        }
        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
