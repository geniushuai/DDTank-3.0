using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server.Quests
{
   
    /// <summary>
    /// 4、击杀玩家若干人次/房间模式（-1不限，0撮合，1自由，）/数量
    /// 
    /// </summary>
    public  class GameKillCondition:BaseCondition
    {       
        public GameKillCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

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
            Console.WriteLine("是否活" + isLiving.ToString() + ":房间类型" + game.RoomType.ToString());
            if ((!isLiving)&&(type==1))
            {
                
                switch (game.RoomType)
                {
                    case eRoomType.Match:
                        if (((m_info.Para1 == 0) || (m_info.Para1 == -1))&&(Value>0))
                            Value = Value - 1;
                        break;
                    case eRoomType.Freedom:
                        if (((m_info.Para1 == 1) || (m_info.Para1 == -1))&&(Value>0))
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
