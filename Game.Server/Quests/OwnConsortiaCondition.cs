using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Bussiness;

namespace Game.Server.Quests
{
    /// <summary>
    /// 18、公会人数/空/具体人数
    /// 触发条件：客户端当前公会有新加入用户时触发、登陆
    /// </summary>
    public class OwnConsortiaCondition:BaseCondition
    {
        public OwnConsortiaCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest,info, value) { }

        public override void AddTrigger(GamePlayer player)
        {
            player.GuildChanged += new GamePlayer.PlayerOwnConsortiaEventHandle(player_OwnConsortia);
        }

        void player_OwnConsortia()
        { 
            
        }
        public override void RemoveTrigger(GamePlayer player)
        {
            player.GuildChanged -= new GamePlayer.PlayerOwnConsortiaEventHandle(player_OwnConsortia);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            bool result=false;
            int tempComp=0;
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaInfo info = db.GetConsortiaSingle(player.PlayerCharacter.ConsortiaID);
                switch (m_info.Para1)
                { 
                    case 0:  //公会人数
                        tempComp=info.Count;                        
                        break;
                    case 1:  //公会贡献度
                        tempComp=player.PlayerCharacter.RichesOffer+player.PlayerCharacter.RichesRob;
                        break;
                    case 2:  //公会铁匠铺等级
                        tempComp=info.SmithLevel;                        
                        break;
                    case 3:  //公会商城等级
                        tempComp=info.ShopLevel;
                        break;
                    case 4:  //公会保管箱等级
                        tempComp = info.StoreLevel;
                        break;
                    default:
                        break;
                }
                if (tempComp >= m_info.Para2)
                {
                    Value = 0;
                    result=true;
                }
                return result;
            }            
            
        }

    }
}
