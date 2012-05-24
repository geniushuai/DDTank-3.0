using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic;
using SqlDataProvider.Data;

namespace Game.Server.Quests
{
    /// <summary>
    /// 22、击杀玩家若干人次/游戏模式/数量
    /// </summary>
    public class GameKillByGameCondition : BaseCondition
    {
        public GameKillByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value) { }

        /// <summary>
        /// 添加击杀玩家若干人次 Para1:战斗类型  Para2:击敌数量
        /// </summary>
        /// <param name="player"></param>
        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.AfterKillingLiving += new Game.Server.GameObjects.GamePlayer.PlayerGameKillEventHandel(player_AfterKillingLiving);
        }

        public override void RemoveTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            player.AfterKillingLiving -= new Game.Server.GameObjects.GamePlayer.PlayerGameKillEventHandel(player_AfterKillingLiving);
        }

        void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            if ((!isLiving)&&(type==1))
            {
                
                switch (game.GameType)
                {
                    case eGameType.Free:
                        if (((m_info.Para1 == 0) || (m_info.Para1 == -1))&&(Value>0))
                            Value = Value - 1;
                        break;
                    case eGameType.Guild:
                        if (((m_info.Para1 == 1) || (m_info.Para1 == -1))&&(Value>0))
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
