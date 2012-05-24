using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
    public class FinishGameCondition:BaseCondition
    {
        /// <summary>
        /// 构造完成一场战斗条件<justin>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="value"></param>
        public FinishGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            player.GameOver += new GamePlayer.PlayerGameOverEventHandle(player_GameOver);            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="isWin"></param>
        /// <param name="gainXp"></param>
        void player_GameOver(Game.Logic.AbstractGame game, bool isWin, int gainXp)
        {
            if (Value < m_info.Para1)
            {
                Value ++;                
            }
            
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(player_GameOver);
        }

        public override bool IsCompleted(Game.Server.GameObjects.GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
