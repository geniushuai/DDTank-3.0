using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    
    public class RequestItemsCondition:BaseCondition
    {
        public RequestItemsCondition(QuestConditionInfo info, int value) : base(info, value) { }
        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
          //  player.GameOver += new GamePlayer.PlayerGameEventHandle(player_RequestItem);
        }

        void player_RequestItem(Game.Logic.AbstractGame game, List<MapGoodsInfo> questItems)
        { 

        }

        public override void RemoveTrigger(GamePlayer player)
        {
            //player.GameOver -= new GamePlayer.PlayerGameEventHandle(player_RequestItem);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            return m_value >= m_info.Para1;
        }

        
    }
}
