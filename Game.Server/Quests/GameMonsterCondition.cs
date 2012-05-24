using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 13、击杀怪物/怪物ID/数量
    /// 触发条件：游戏客户端战斗
    /// </summary>
    public class GameMonsterCondition:BaseCondition
    {
        public GameMonsterCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }
        public override void AddTrigger(GamePlayer player)
        {
            player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(player_AfterKillingLiving);
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(player_AfterKillingLiving);
        }

        void player_AfterKillingLiving(Game.Logic.AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            if ((type == 2 && id == m_info.Para1) && (Value > 0) && (!isLiving))
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
