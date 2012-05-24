using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    ///  21、使用登录器/空/1
    ///  触发条件：用户登陆时判断
    /// </summary>
    public class OwnLoginCondition:BaseCondition
    {
        public OwnLoginCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }
        public override void AddTrigger(GamePlayer player)
        {
            player.OwnLogin += new GamePlayer.PlayerOwnLoginEventHandle(player_OwnLogin);
        }
        void player_OwnLogin()
        {
            Value = 0;
        }

        public override void RemoveTrigger(GamePlayer player)
        {
            player.OwnLogin -= new GamePlayer.PlayerOwnLoginEventHandle(player_OwnLogin);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return Value <= 0;
        }
    }
}
