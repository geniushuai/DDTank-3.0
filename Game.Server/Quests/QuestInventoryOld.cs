using System;
using System.Collections.Generic;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Bussiness;
using Game.Server.Packets;
using Game.Server.Statics;

namespace Game.Server.Quests
{
    //1 杀敌 2 战胜 3 获取 4 使用 5 升级 6 强化 7 完成 8 合成 9 战斗 10 杀敌对公会人数 11 杀中立公会人数 12 公会战对战 13 公会战获胜场数 14 客服端控制 
    //15 客服完成  16 贡献度 17 捐献 18 功勋 19 掠夺 20 公会等级 21 结婚条件  22 结婚后的拥有 23 结婚后的获取 24 拥有 25 夫妻战胜  26 公会铁匠铺的等级判定 27 公会商城的等级判定 28 公会保管箱的等级判定
    public class QuestInventoryOld
    {
        private object _lock;
        private GamePlayer _player;
        private Dictionary<int, QuestDataInfo> _currentQuest;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="player"></param>
        public QuestInventoryOld(GamePlayer player)
        {
            _player = player;
            _lock = new object();
            _currentQuest = new Dictionary<int, QuestDataInfo>();
        }
        
        /// <summary>
        /// 从数据库中加载当前玩家的的任务列表
        /// </summary>
        /// <param name="playerId">传入玩家编号</param>
        public void LoadFromDatabase(int playerId)
        {
            lock (_lock)
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    QuestDataInfo[] infos = db.GetUserQuest(playerId);

                    foreach (QuestDataInfo info in infos)
                    {
                        if (!_currentQuest.ContainsKey(info.QuestID))
                        {
                            //未开始
                            //if (info.IsExist && info.QuestInfo.TimeLimit && DateTime.Now.CompareTo(info.QuestInfo.EndDate) > 0)
                            //{
                            //    info.IsExist = false;
                            //}
                            _currentQuest.Add(info.QuestID, info);
                        }
                    }


                }
            }
            ClearConsortiaQuest();  /*清除非工会任务*/
            ClearMarryQuest();      /*清除非结婚任务*/
        }

        /// <summary>
        /// 保存到数据库中
        /// </summary>
        public void SaveToDatabase()
        {
            lock (_lock)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    foreach (QuestDataInfo info in _currentQuest.Values)
                    {
                        if (info.IsDirty)
                        {
                            pb.UpdateDbQuestDataInfo(info);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="questID">传入任务编号</param>
        /// <param name="msg">返回消息参数</param>
        /// <returns>返回结果</returns>
        public bool AddQuest(int questID, out string msg)
        {


            QuestInfo info = Bussiness.Managers.QuestMgr.GetSingleQuest(questID);

            //未开始
            //if (info == null)
            //{
            //    msg = "Game.Server.Quests.NoQuest";
            //}
            ////else if (GetQuestCount() >= 20)
            ////{
            ////    msg = "Game.Server.Quests.QuestFull";
            ////}
            ////判断上限时间限制
            //else if (info.TimeLimit && DateTime.Now.CompareTo(info.StartDate) < 0)
            //{
            //    msg = "Game.Server.Quests.NoTime";
            //}
            ////判断下限时间限制
            //else if (info.TimeLimit && DateTime.Now.CompareTo(info.EndDate) > 0)
            //{
            //    msg = "Game.Server.Quests.TimeOver";
            //}
            ////判断所需下限等级限制
            //else if (_player.PlayerCharacter.Grade < info.NeedMinLevel)
            //{
            //    msg = "Game.Server.Quests.LevelLow";
            //}
            ////判断所需上限等级限制
            //else if (_player.PlayerCharacter.Grade > info.NeedMaxLevel)
            //{
            //    msg = "Game.Server.Quests.LevelTop";
            //}
            ////判断前置任限是否完成限制
            //else if (info.PreQuestID != 0 && (GetCurrentQuest(info.PreQuestID) == null || !GetCurrentQuest(info.PreQuestID).IsComplete))
            //{
            //    msg = "Game.Server.Quests.NoFinish";
            //}
            ////判断当前任务是否已经完成
            //else if (GetCurrentQuest(questID, true) != null && !GetCurrentQuest(questID).IsComplete)
            //{
            //    msg = "Game.Server.Quests.Have";
            //}
            ////判断当任务是否可以重复接受
            //else if (GetCurrentQuest(questID, true) != null && !info.CanRepeat)
            //{
            //    msg = "Game.Server.Quests.NoRepeat";
            //}
            ////判断当前任务可以接受间隔（天）内可重复接受次数
            //else if (GetCurrentQuest(questID, true) != null && DateTime.Now.CompareTo(GetCurrentQuest(questID).CompletedDate.Date.AddDays(info.RepeatInterval)) < 0 && GetCurrentQuest(questID).RepeatFinish < 1)
            //{
            //    msg = "Game.Server.Quests.Rest";  //条件成立
            //}
            ////判断当前用户是否允许接受工会任务
            //else if (_player.PlayerCharacter.ConsortiaID == 0 && (info.Condition == 10 || info.Condition == 11 || info.Condition == 12 || info.Condition == 13))
            //{
            //    msg = "Game.Server.Quests.NoConsortia";
            //}
            ////判断当前用户是否可以接受结婚任务
            //else if ((_player.PlayerCharacter.IsMarried == false) && (info.Type == 3 || info.Type == 4))
            //{
            //    msg = "Game.Server.Quest.QuestInventory.HaveMarry";
            //}
            //else
            //{
            //    QuestDataInfo quest = GetCurrentQuest(questID);
            //    if (quest != null)
            //    {
            //        quest.RevertInfo();
            //    }
            //    else
            //    {
            //        quest = QuestDataInfo.CreateFromQuest(info, _player.PlayerCharacter.ID);
            //        lock (_lock)
            //        {
            //            _currentQuest.Add(quest.QuestID, quest);
            //        }
            //    }
            //    //msg = "成功接受任务!";
            //    msg = "";
            //    _player.Out.SendQuestUpdate(quest);
            //    return true;
            //}

            //msg = LanguageMgr.GetTranslation(msg);
            msg = "未开始";
            return false;
        }

        /// <summary>
        /// 杀敌
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="fightMode">战斗模式</param>
        /// <param name="timeMode">时间设置</param>
        /// <param name="caption">队长限制</param>
        /// <param name="killLevel">杀敌等级</param>
        /// <returns>返回结果</returns>
        public void CheckKillPlayer(int map, int fightMode, int timeMode, bool captain, int killLevel, int selfCount, int rivalCount, int relation, int roomType)
        {
            //未开始
            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        if (q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //            continue;

            //        if (q.QuestInfo.Condition != 1 && !(relation == 2 && q.QuestInfo.Condition == 10) && !(relation == 0 && q.QuestInfo.Condition == 11))
            //            continue;

            //        //if (q.QuestInfo.Condition != 1 || q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //        //    continue;
            //        if (q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //            continue;

            //        if ((q.QuestInfo.ReqMap == -1 || q.QuestInfo.ReqMap == map) &&
            //            (q.QuestInfo.ReqFightMode == -1 || q.QuestInfo.ReqFightMode == fightMode) &&
            //            (q.QuestInfo.ReqTimeMode == -1 || q.QuestInfo.ReqTimeMode == timeMode) &&
            //            (captain || !q.QuestInfo.ReqBeCaptain) &&
            //                               (q.QuestInfo.ReqRoomType == -1 || q.QuestInfo.ReqRoomType == roomType) &&
            //            (q.QuestInfo.ReqKillLevel == -1 || q.QuestInfo.ReqKillLevel <= killLevel) &&
            //            (q.QuestInfo.ReqSelfCount == -1 || q.QuestInfo.ReqSelfCount == selfCount) &&
            //            (q.QuestInfo.ReqRivalCount == -1 || q.QuestInfo.ReqRivalCount == rivalCount))
            //        {
            //            q.ConditionCount--;
            //            _player.Out.SendQuestUpdate(q);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 胜利
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="fightMode">战斗模式</param>
        /// <param name="timeMode">时间模式</param>
        /// <param name="caption">首领</param>
        /// <param name="killLevel">杀敌等级</param>
        /// <returns></returns>
        public void CheckWin(int map, int fightMode, int timeMode, bool captain, int selfCount, int rivalCount, bool isWin, bool isFightConsortia, int roomType, bool isMarry)
        {
            //未开始
            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        if (q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //            continue;

            //        if (!(isWin && q.QuestInfo.Condition == 2) && q.QuestInfo.Condition != 9 && !(isFightConsortia && q.QuestInfo.Condition == 12) && !(isFightConsortia && isWin && q.QuestInfo.Condition == 13) && !(isMarry && isWin && q.QuestInfo.Condition == 25))
            //            continue;


            //        if ((q.QuestInfo.ReqMap == -1 || q.QuestInfo.ReqMap == map) &&
            //            (q.QuestInfo.ReqFightMode == -1 || q.QuestInfo.ReqFightMode == fightMode) &&
            //            (q.QuestInfo.ReqTimeMode == -1 || q.QuestInfo.ReqTimeMode == timeMode) &&
            //            (captain || !q.QuestInfo.ReqBeCaptain) &&
            //            (q.QuestInfo.ReqRoomType == -1 || q.QuestInfo.ReqRoomType == roomType) &&
            //            (q.QuestInfo.ReqSelfCount == -1 || q.QuestInfo.ReqSelfCount == selfCount) &&
            //            (q.QuestInfo.ReqRivalCount == -1 || q.QuestInfo.ReqRivalCount == rivalCount))
            //        {
            //            q.ConditionCount--;
            //            _player.Out.SendQuestUpdate(q);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="strengthenLevel">使用物品</param>
        public void CheckUseItem(int itemID)
        {
            //未开始
            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        if (q.QuestInfo.Condition != 4 || q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //            continue;

            //        if (q.QuestInfo.ReqItemID == itemID)
            //        {
            //            q.ConditionCount--;
            //            _player.Out.SendQuestUpdate(q);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 强化
        /// </summary>
        /// <param name="strengthenLevel">强化等级</param>
        /// <param name="categoryID">编号</param>
        public void CheckStrengthen(int strengthenLevel, int categoryID)
        {
            //未开始
            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        if (q.QuestInfo.Condition != 6 || q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //            continue;

            //        if (q.QuestInfo.StrengthenLevel == strengthenLevel &&
            //            (q.QuestInfo.ReqItemID == -1 || q.QuestInfo.ReqItemID == categoryID))
            //        {
            //            q.ConditionCount--;
            //            _player.Out.SendQuestUpdate(q);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 合成
        /// </summary>
        /// <param name="itemID">物品名称</param>
        public void CheckCompose(int itemID)
        {
            //未开始
            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        if (q.QuestInfo.Condition != 8 || q.IsComplete || !q.IsExist || q.ConditionCount == 0)
            //            continue;

            //        if (q.QuestInfo.ReqItemID == -1 || q.QuestInfo.ReqItemID == itemID)
            //        {
            //            q.ConditionCount--;
            //            _player.Out.SendQuestUpdate(q);
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="questID">任务编号</param>
        /// <param name="count">数量</param>
        public void CheckClient(int questID, int count)
        {
            //未开始
            //QuestDataInfo info = GetCurrentQuest(questID, true);
            //if (info != null)
            //{
            //    if (info.QuestInfo.Condition < count)
            //        count = info.QuestInfo.Condition;
            //    info.ConditionCount = count;
            //    _player.Out.SendQuestUpdate(info);
            //}

        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        /// 
        //1 杀敌 2 战胜 3 获取 4 使用 5 升级 6 强化 7 完成 8 合成 9 战斗 10 杀敌对公会人数 11 杀中立公会人数 12 公会战对战 13 公会战获胜场数 14 客服端控制 
        //15 客服完成  16 贡献度 17 捐献 18 功勋 19 掠夺 20 公会等级 21 结婚条件  22 结婚后的拥有 23结婚后的获取 24拥有 25 夫妻战胜  26 公会铁匠铺的等级判定 27 公会商城的等级判定 28 公会保管箱的等级判定 29公会升级任务为按人数判定
        public bool FinishQuest(int questID)
        {
            //未开始
            //string RewardPropsName = string.Empty;
            //int RewardPropsNumber = 0;
            //string RewardBuffName = string.Empty;
            //int RewardBuffTime = 0;
            //string RewardMsg = "";


            //QuestDataInfo quest = GetCurrentQuest(questID, true);
            //if (quest != null && !quest.IsComplete && quest.IsExist)
            //{
            //    if (quest.QuestInfo.TimeLimit && DateTime.Now.CompareTo(quest.QuestInfo.EndDate) > 0)
            //    {
            //        return false;
            //    }

            //    switch (quest.QuestInfo.Condition)
            //    {
            //        case 1:
            //        case 2:
            //        case 4:
            //        case 6:
            //        case 8:
            //        case 9:
            //        case 10:
            //        case 11:
            //        case 12:
            //        case 13:
            //        case 14:
            //            if (quest.ConditionCount != 0)
            //                return false;
            //            break;
            //        case 3://获取                    
            //        case 23://23 结婚后的获取                    
            //            if (_player.MainBag.GetItemCount(quest.QuestInfo.ReqItemID) < quest.QuestInfo.FinishCount && _player.PropBag.GetItemCount(quest.QuestInfo.ReqItemID) < quest.QuestInfo.FinishCount && _player.MainBag.GetItemCount(quest.QuestInfo.ReqItemID, 0) < quest.QuestInfo.FinishCount)
            //            {
            //                _player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.NoCount"));
            //                return false;
            //            }
            //            break;
            //        case 24://拥有                    
            //        case 22://22 结婚后的拥有
            //            if (_player.MainBag.GetItemCount(quest.QuestInfo.ReqItemID) < quest.QuestInfo.FinishCount && _player.PropBag.GetItemCount(quest.QuestInfo.ReqItemID) < quest.QuestInfo.FinishCount && _player.MainBag.GetItemCount(quest.QuestInfo.ReqItemID, 0) < quest.QuestInfo.FinishCount)
            //            {
            //                _player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.NoCount"));
            //                return false;
            //            }
            //            break;
            //        case 5:
            //            if (_player.PlayerCharacter.Grade < quest.QuestInfo.AchieveLevel)
            //                return false;
            //            break;
            //        case 7:
            //        case 15:
            //            break;
            //        case 16:
            //            if (_player.PlayerCharacter.Riches < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 17:
            //            if (_player.PlayerCharacter.RichesOffer < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 18:
            //            if (_player.PlayerCharacter.Offer < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 19:
            //            if (_player.PlayerCharacter.RichesRob < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 20:
            //            if (_player.PlayerCharacter.ConsortiaLevel < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 21: //21 结婚条件(任务结束时以此为判断)
            //        case 25: //25 夫妻战胜
            //            if (_player.PlayerCharacter.IsMarried == false)
            //                return false;
            //            break;
            //        case 26://26 公会铁匠铺的等级判定
            //            if (_player.PlayerCharacter.SmithLevel < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 27://27 公会商城的等级判定
            //            if (_player.PlayerCharacter.ShopLevel < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 28://28 公会保管箱的等级判定
            //            if (_player.PlayerCharacter.StoreLevel < quest.QuestInfo.FinishCount)
            //                return false;
            //            break;
            //        case 29://29 公会升级任务按总人数判定                    
            //            using (ConsortiaBussiness db = new ConsortiaBussiness())
            //            {
            //                ConsortiaInfo info = db.GetConsortiaSingle(_player.PlayerCharacter.ConsortiaID);
            //                if (info.Count < quest.QuestInfo.FinishCount)
            //                {
            //                    return false;
            //                }
            //            }
            //            break;
            //        default:
            //            return false;
            //    }

            //    if (quest.QuestInfo.RewardItemID > 0)
            //    {
            //        //ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.FindItemTemplate(_player.PlayerCharacter.Sex ? quest.QuestInfo.RewardItemID : quest.QuestInfo.RewardItemIDGirl);
            //        //if (temp != null)
            //        //{
            //        //    int needGrid = 0;
            //        //    for (int len = 0; len < quest.QuestInfo.RewardItemCount; len += temp.MaxCount)
            //        //    {
            //        //        needGrid++;
            //        //    }
            //        //    IBag bag = _player.GetBag(temp.GetBagType());
            //        //    if (needGrid > bag.GetEmptyCount())
            //        //    {
            //        //        _player.Out.SendMessage(eMessageType.ERROR, _player.GetBagName(temp.GetBagType()) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull"));

            //        //        return false;
            //        //    }
            //        //}
            //    }

            //    if (quest.QuestInfo.Condition == 3 || quest.QuestInfo.Condition == 23)
            //    {
            //        //if (!_player.MainBag.RemoveItem(quest.QuestInfo.FinishCount) && !_player.PropBag.RemoveItemCount(quest.QuestInfo.ReqItemID, quest.QuestInfo.FinishCount) && !_player.CurrentInventory.RemoveItemCount(quest.QuestInfo.ReqItemID, quest.QuestInfo.FinishCount))
            //            return false;
            //    }

            //    quest.IsComplete = true;
            //    //quest.CompletedDate = DateTime.Now.Date;
            //    quest.CompletedDate = DateTime.Now;
            //    _player.Out.SendQuestUpdate(quest);

            //    if (quest.QuestInfo.RewardItemID > 0)
            //    {
            //        ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.FindItemTemplate(_player.PlayerCharacter.Sex ? quest.QuestInfo.RewardItemID : quest.QuestInfo.RewardItemIDGirl);
            //        //if (temp != null)
            //        //{
            //        //    for (int len = 0; len < quest.QuestInfo.RewardItemCount; len += temp.MaxCount)
            //        //    {
            //        //        int count = len + temp.MaxCount > quest.QuestInfo.RewardItemCount ? quest.QuestInfo.RewardItemCount - len : temp.MaxCount;
            //        //        ItemInfo item = ItemInfo.CreateFromTemplate(temp, count, (int)ItemAddType.Quest);
            //        //        if (item == null)
            //        //            continue;
            //        //        item.ValidDate = quest.QuestInfo.RewardItemValidateTime;
            //        //        item.IsBinds = true;
            //        //        //IBag bag = _player.GetBag(item.GetBagType());
            //        //        //bag.AddItem(item);                     
            //        //        RewardPropsName = item.Template.Name;
            //        //        RewardPropsNumber = quest.QuestInfo.RewardItemCount;
            //        //        _player.AddItem(item, Game.Server.Statics.ItemAddType.Quest, item.GetBagType());
            //        //    }
            //    }
            //}

            ////在这里添加buff的判断
            //if (quest.QuestInfo.BuffID > 0 && quest.QuestInfo.BuffValidDate > 0)
            //{
            //    ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.FindItemTemplate(quest.QuestInfo.BuffID);
            //    if (temp != null)
            //    {
            //        ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, (int)ItemAddType.Quest);
            //        if (item == null)
            //        {
            //        }
            //        RewardBuffName = item.Template.Name;
            //        RewardBuffTime = quest.QuestInfo.BuffValidDate;
            //        string buffMsg = string.Empty;
            //        //_player.BufferList.AddBuffer(item, out buffMsg, quest.QuestInfo.BuffValidDate * 60);
            //    }
            //}



            //if (quest.QuestInfo.RewardGold != 0)
            //{
            //    _player.AddGold(quest.QuestInfo.RewardGold, Game.Server.Statics.GoldAddType.Qeust);
            //}

            //if (quest.QuestInfo.RewardMoney != 0)
            //{
            //    _player.AddMoney(quest.QuestInfo.RewardMoney, Game.Server.Statics.MoneyAddType.Quest);

            //}

            //if (quest.QuestInfo.RewardGP != 0)
            //{
            //    _player.AddGP(quest.QuestInfo.RewardGP);

            //}

            //if (quest.QuestInfo.RewardRiches != 0 && _player.PlayerCharacter.ConsortiaID != 0)
            //{
            //    //GameServer.Instance.LoginServer.SendConsortiaOffer(_player.PlayerCharacter.ConsortiaID, 0, quest.QuestInfo.RewardRiches);
            //    int riches = quest.QuestInfo.RewardRiches;
            //    _player.AddRobRiches(riches);
            //    using (ConsortiaBussiness db = new ConsortiaBussiness())
            //    {
            //        db.ConsortiaRichAdd(_player.PlayerCharacter.ConsortiaID, ref riches);
            //    }

            //}

            //if (quest.QuestInfo.RewardOffer != 0)
            //{
            //    _player.AddOffer(quest.QuestInfo.RewardOffer);

            //}

            //RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward");


            ////奖励提示
            //if (quest.QuestInfo.RewardGP > 0)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGB1", quest.QuestInfo.RewardGP) + " ";
            //}

            //if (quest.QuestInfo.RewardMoney > 0)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardMoney", quest.QuestInfo.RewardMoney) + " ";
            //}

            //if (quest.QuestInfo.RewardGold > 0)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", quest.QuestInfo.RewardGold) + " ";
            //}

            //if (quest.QuestInfo.RewardOffer > 0)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardOffer", quest.QuestInfo.RewardOffer) + " ";
            //}

            //if (quest.QuestInfo.RewardRiches > 0)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardRiches", quest.QuestInfo.RewardRiches) + " ";
            //}

            //if (!string.IsNullOrEmpty(RewardPropsName) && RewardPropsNumber > 0)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", RewardPropsName, RewardPropsNumber) + " ";
            //}

            //if (quest.QuestInfo.BuffID > 0 && quest.QuestInfo.BuffValidDate > -1)
            //{
            //    RewardMsg = RewardMsg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardBuff", RewardBuffName, RewardBuffTime) + "";
            //}

            //_player.Out.SendMessage(eMessageType.Normal, RewardMsg);


            return true;

        }


        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        public bool RemoveQuest(int questID)
        {
            QuestDataInfo quest = GetCurrentQuest(questID, true);
            if (quest != null && !quest.IsComplete)
            {
                quest.IsExist = false;
               // _player.Out.SendQuestUpdate(quest);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 所需物品
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> GetRequestItems()
        {
            Dictionary<int, int> list = new Dictionary<int, int>();
            //未开始
            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        if (q.QuestInfo.Condition != 3 || q.IsComplete || !q.IsExist)
            //            continue;

            //        //list.Add(q.QuestInfo.ReqItemID);
            //        if (list.ContainsKey(q.QuestInfo.ReqItemID))
            //        {
            //            list[q.QuestInfo.ReqItemID] += q.QuestInfo.FinishCount;
            //        }
            //        else
            //        {
            //            list.Add(q.QuestInfo.ReqItemID, q.QuestInfo.FinishCount);
            //            int count = _player.MainBag.GetItemCount(q.QuestInfo.ReqItemID);
            //            count += _player.PropBag.GetItemCount(q.QuestInfo.ReqItemID);
            //            //count += _player.QuestInventory.RemoveQuest(q.QuestInfo.ReqItemID);
            //            count += _player.StoreBag.GetItemCount(q.QuestInfo.ReqItemID);
            //            list[q.QuestInfo.ReqItemID] -= count;
            //        }
            //    }
            //}
            return list;
        }

        /// <summary>
        /// 获取一条任务信息
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public QuestDataInfo GetCurrentQuest(int questID)
        {
            lock (_lock)
            {
                if (_currentQuest.ContainsKey(questID))
                {
                    return _currentQuest[questID];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取全部任何数据
        /// </summary>
        /// <returns></returns>
        public QuestDataInfo[] GetALlQuest()
        {
            QuestDataInfo[] list = null;
            lock (_lock)
            {
                _currentQuest.Values.CopyTo(list,0);
            }
            return list == null ? new QuestDataInfo[0] : list;
        }

        /// <summary>
        /// 获取指定状态任务
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public QuestDataInfo GetCurrentQuest(int questID, bool isExist)
        {
            lock (_lock)
            {
                if (_currentQuest.ContainsKey(questID))
                {
                    if (_currentQuest[questID].IsExist == isExist)
                        return _currentQuest[questID];
                }
            }
            return null;
        }
        /// <summary>
        /// 获取当前用户的任务数量
        /// </summary>
        /// <returns></returns>
        public int GetQuestCount()
        {
            int count = 0;
            lock (_lock)
            {
                foreach (QuestDataInfo q in _currentQuest.Values)
                {
                    if (q.IsComplete || !q.IsExist)
                        continue;
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 清除非公会用户的任务
        /// </summary>
        /// <returns></returns>
        public bool ClearConsortiaQuest()
        {
            if (_player.PlayerCharacter.ConsortiaID != 0)
                return false;

            lock (_lock)
            {
                foreach (QuestDataInfo q in _currentQuest.Values)
                {
                    //if (q.QuestInfo.Condition == 10 || q.QuestInfo.Condition == 11 || q.QuestInfo.Condition == 12 || q.QuestInfo.Condition == 13)
                    //未开始
                    //if (q.QuestInfo.Type == 2)
                    //{
                    //    if (q.IsComplete)
                    //        continue;

                    //    q.IsExist = false;
                    //    _player.Out.SendQuestUpdate(q);
                    //}

                }
            }
            return true;
        }
        /// <summary>
        /// 清除非结婚用户的任务
        /// </summary>
        /// <returns></returns>
        public bool ClearMarryQuest()
        {
            //未开始
            //if (_player.PlayerCharacter.IsMarried == true)
            //    return false;

            //lock (_lock)
            //{
            //    foreach (QuestDataInfo q in _currentQuest.Values)
            //    {
            //        //if (q.QuestInfo.Condition == 21 || q.QuestInfo.Condition == 22 || q.QuestInfo.Condition == 23 )
            //        if (q.QuestInfo.Type == 3 || q.QuestInfo.Type == 4)
            //        {
            //            if (q.IsComplete)
            //                continue;

            //            q.IsExist = false;
            //            _player.Out.SendQuestUpdate(q);
            //        }

            //    }
            //}
            return true;
        }
    }

}