using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server.Quests
{
   
    /// <summary>
    /// 4、击杀玩家若干人次/房间模式（房间模式：0为撮合，1为自由，2为探险，3为副本，4为夺宝）/数量
    /// 
    /// </summary>
    public  class GameKillByRoomCondition:BaseCondition
    {       
        public GameKillByRoomCondition(BaseQuest quest,QuestConditionInfo info, int value) : base(quest,info, value) { }

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
            if (isLiving == false)
            { 
                
            }

            //Console.WriteLine("是否活" + isLiving.ToString() + ":房间类型" + game.RoomType.ToString());
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
                    case eRoomType.Exploration:
                        if (((m_info.Para1 == 2) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eRoomType.Boss:
                        if (((m_info.Para1 == 3) || (m_info.Para1 == -1)) && (Value > 0))
                            Value = Value - 1;
                        break;
                    case eRoomType.Treasure:
                        if (((m_info.Para1 == 4) || (m_info.Para1 == -1)) && (Value > 0))
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
