using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server.Quests
{
    /// <summary>
    ///  5、完成战斗（无论胜败）/房间模式（-1不限，0撮合，1自由，2练级，3副本）/数量
    ///  触发条件：挂在游戏结算画面
    /// </summary>
    public class GameFightCondition:BaseCondition
    {
        public GameFightCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

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
            switch (game.RoomType)
            {
                case eRoomType.Match:   //撮合战
                    if (((m_info.Para1 == 0) || (m_info.Para1 == -1)) && (Value > 0))
                        Value--;
                    break;
                case eRoomType.Freedom://自由战
                    if (((m_info.Para1 == 1) || (m_info.Para1 == -1)) && (Value > 0))
                        Value--;
                    break;                
                default:                    
                    break;
            }     
        }

        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
