using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 20、邮寄/物品id/数量
    /// 触发条件：邮件成功时。
    /// </summary>
    public class OwnMailCondition:BaseCondition
    {
        public OwnMailCondition(BaseQuest quest,QuestConditionInfo info,int value):base(quest,info,value){}

        public override void AddTrigger(GamePlayer player)
        {
            player.OwnMail += new GamePlayer.PlayerOwnMailEventHandle(player_OwnMail);
        }
        void player_OwnMail(int templateID,int count)
        {
            if ((templateID == m_info.Para1)&&(Value>0))
            {
                Value -= count;
            }
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            player.OwnMail -= new GamePlayer.PlayerOwnMailEventHandle(player_OwnMail);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }

    }
}
