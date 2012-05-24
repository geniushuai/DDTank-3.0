using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Bussiness.Managers;
using Game.Server.Statics;
using Game.Server.GameUtils;
using Game.Logic;

namespace Game.Server.Quests
{
    /// <summary>
    /// 15、上缴道具（完成任务道具消失）/道具ID/数量
    /// 触发条件：提取邮件、战斗完成物品结算
    /// </summary>
    public class TurnPropertyCondition : BaseCondition
    {
        private BaseQuest m_quest;
        private GamePlayer m_player;
        public TurnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value)
            : base(quest, info, value)
        {
            m_quest = quest;
        }

        public override void AddTrigger(GamePlayer player)
        {
            m_player = player;
            player.GameKillDrop += new GamePlayer.GameKillDropEventHandel(QuestDropItem);            
            base.AddTrigger(player);
        }

        public override bool IsCompleted(GamePlayer player)
        {
            bool result = false;
            if (player.GetItemCount(m_info.Para1) >= m_info.Para2)
            {
                Value = 0;
                result =true;
            }
            return result;
        }

        public override bool Finish(GamePlayer player)
        {
            return player.RemoveTemplate(m_info.Para1, m_info.Para2);
        }
        
        public override void RemoveTrigger(GamePlayer player)
        {
            player.GameKillDrop -= new GamePlayer.GameKillDropEventHandel(QuestDropItem);
            base.RemoveTrigger(player);
        }

        public override bool CancelFinish(GamePlayer player)
        { 
            ItemTemplateInfo template = ItemMgr.FindItemTemplate(m_info.Para1);
            if (template != null)
            {
                ItemInfo item = ItemInfo.CreateFromTemplate(template, m_info.Para2, (int)ItemAddType.TurnProperty);
                return player.AddTemplate(item, eBageType.TempBag, m_info.Para2);
            }
            else
            {
                return false;
            }                        
        }
        private void QuestDropItem(AbstractGame game,int copyId,int npcId,bool playResult)
        {
            if (m_player.GetItemCount(m_info.Para1) < m_info.Para2)
            {
                List<ItemInfo> infos = null;
                int golds=0, moneys=0, gifttokens=0;
                if (game is PVEGame)
                {
                    DropInventory.PvEQuestsDrop(npcId, ref infos);
                }
                if (game is PVPGame)
                {
                    DropInventory.PvPQuestsDrop(game.RoomType, playResult, ref infos);
                }
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {
                        ItemInfo.FindSpecialItemInfo(info, ref golds,ref moneys,ref gifttokens);
                        if (info != null)
                        {
                            m_player.TempBag.AddTemplate(info, info.Count);
                        }
                    }                    
                    m_player.AddGold(golds);
                    m_player.AddGiftToken(gifttokens);
                    m_player.AddMoney(moneys);
                    LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Drop, m_player.PlayerCharacter.ID, moneys, m_player.PlayerCharacter.Money, 0, 0, 0, "", "", "");//添加日志
                }
            }
        }

    }
}
