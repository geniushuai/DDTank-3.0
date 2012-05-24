using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Bussiness;
using Game.Server.Statics;
using Game.Server.Packets;


namespace Game.Server.GameUtils
{
    /// <summary>
    /// 
    /// </summary>
    public class CommonBag : AbstractInventory
    {
        private GamePlayer _player;

        public CommonBag(GamePlayer player,int bagType,int count)
            : base(count, bagType, true)
        {
            _player = player;
            _bagType = bagType;
            _removedList = new List<ItemInfo>();
        }

        /// <summary>
        /// 从数据库中加载
        /// </summary>
        /// <param name="playerId"></param>
        public void LoadFromDatabase(int playerId)
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                ItemInfo[] list = pb.GetUserBagByType(playerId, _bagType);
                foreach (ItemInfo item in list)
                {
                    if (item.Place >= _count || item.Place < 0)
                        continue;

                    m_items[item.Place] = item;
                }              
            }

            if (_bagType == 11)
            {
                SendStoreToMail();
            }

        }

        /// <summary>
        /// 保存到数据库中
        /// </summary>
        public void SaveToDatabase()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                lock (m_lock)
                {
                    for (int i = 0; i < m_items.Length; i++)
                    {
                        ItemInfo item = m_items[i];
                        if (item != null)
                        {
                            if (item.IsDirty)
                            {
                                if (item.ItemID > 0)
                                {
                                    pb.UpdateGoods(item);
                                }
                                else
                                {
                                    pb.AddGoods(item);
                                }
                            }
                        }
                    }

                    foreach (ItemInfo item in _removedList)
                    {
                        //pb.UpdateGoods(item);
                        if (item.ItemID > 0)
                        {
                            pb.UpdateGoods(item);
                        }
                        else
                        {
                            pb.AddGoods(item);
                        }
                    }
                    _removedList.Clear();
                }
            }        
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int AddItem(ItemInfo item)
        {
            return AddItem(item, 0);
        }

        public int AddItem(int showPlace, ItemInfo item)
        {
            int place = base.AddItem(item, 0);
            if (place != -1)
            {
                item.UserID = _player.PlayerCharacter.ID;
                _player.Out.SendUpdateInventorySlot(place, true, item, _bagType, showPlace);
            }
            return place;
        }

        /// <summary>
        /// 从指定位置开始查找空位添加物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public override int AddItem(ItemInfo item, int start)
        {
            int place = base.AddItem(item, start);
            if (place != -1)
            {
                item.UserID = _player.PlayerCharacter.ID;
                _player.Out.SendUpdateInventorySlot(place, true, item,_bagType);
            }
            return place;
        }

        public override int AddItemTo(ItemInfo item, int place)
        {
            if (place < 0)
                return -1;
            place = base.AddItemTo(item, place);
            if (place != -1)
            {
                item.UserID = _player.PlayerCharacter.ID;
                _player.Out.SendUpdateInventorySlot(place, true, item, _bagType);
            }
            return place;
        }

        public override int RemoveItem(ItemInfo item)
        {
            int place = base.RemoveItem(item);
            if (place != -1)
            {
                _player.Out.SendUpdateInventorySlot(place, false, null,_bagType);
                _removedList.Add(item);
            }
            return place;
        }

        /// <summary>
        /// 移动物品
        /// </summary>
        /// <param name="fromSlot"></param>
        /// <param name="toSlot"></param>
        /// <returns></returns>
        public override bool MoveItem(int fromSlot, int toSlot,int count)
        {
            ItemInfo from = GetItemAt(fromSlot);
            ItemInfo to = GetItemAt(toSlot);

            if (from == null || fromSlot == toSlot) return false;

            if (_bagType == 11)
            {
                ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(_player.PlayerCharacter.ConsortiaID);
                if (info == null || toSlot >= info.StoreLevel * 10 || fromSlot >= info.StoreLevel * 10)
                {
                    return false;
                }
            }

            if (to != null)
            {
                //合并物品
                if (from.IsCanWrap(to))
                {
                    int total = from.Count + to.Count;
                    if (total > to.Template.MaxCount)
                    {
                        to.Count = to.Template.MaxCount;
                        from.Count = total - to.Count;
                        _player.Out.SendUpdateInventorySlot(to.Place, true, to,_bagType);
                        _player.Out.SendUpdateInventorySlot(from.Place, true, from,_bagType);
                    }
                    else
                    {
                        to.Count = total;
                        _player.Out.SendUpdateInventorySlot(to.Place, true, to,_bagType);
                        from.Count = 0;
                        from.RemoveType = (int)ItemRemoveType.Fold;
                        from.IsExist = false;
                        _player.Out.SendUpdateInventorySlot(fromSlot, false, null,_bagType);
                        RemoveItem(from);
                    }
                    return true;
                }
                //交换物品
                else
                {
                    if (base.MoveItem(fromSlot, toSlot))
                    {
                        _player.Out.SendUpdateInventorySlot(from.Place, true, from,_bagType);
                        _player.Out.SendUpdateInventorySlot(to.Place, true, to,_bagType);
                        return true;
                    }
                }
            }
            else
            {
                if (base.MoveItem(fromSlot, toSlot))
                {
                    _player.Out.SendUpdateInventorySlot(from.Place, true, from,_bagType);
                    _player.Out.SendUpdateInventorySlot(fromSlot, false, null,_bagType);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 拆分物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool SplitItem(ItemInfo item, int count, int toSlot)
        {
            if (_bagType == 11)
            {
                ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(_player.PlayerCharacter.ConsortiaID);
                if (info != null || toSlot >= info.StoreLevel * 10)
                {
                    _player.Out.SendUpdateInventorySlot(item.Place, true, item, _bagType);
                    return false;
                }
            }

            if (item.Count > count && m_items[toSlot] == null)
            {
                ItemInfo clone = item.Clone();
                clone.ItemID = -1;
                clone.Count = count;
                if (AddItemTo(clone, toSlot) != -1)
                {
                    item.Count -= count;
                    _player.Out.SendUpdateInventorySlot(item.Place, true, item, _bagType);
                    _player.Out.SendUpdateInventorySlot(clone.Place, true, clone, _bagType);
                    return true;
                }
            }
            else
            {
                _player.Out.SendUpdateInventorySlot(item.Place, true, item, _bagType);
            }

            return false;
        }

        /// <summary>
        /// 刷新格子
        /// </summary>
        /// <param name="item"></param>
        public void RefreshItem(ItemInfo item)
        {
            _player.Out.SendUpdateInventorySlot(item.Place, item != null, item,_bagType);
        }

        /// <summary>
        /// 物品类别查找
        /// </summary>
        /// <param name="categoryID"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public ItemInfo GetItemByCategoryID(int categoryID, int property)
        {
            return GetItemByCategoryID(0, categoryID, property);
        }

        public override ItemInfo GetItemByCategoryID(int start, int categoryID, int property)
        {
            return base.GetItemByCategoryID(start, categoryID, property);
        }

        /// <summary>
        /// 模板ID查找物品
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public ItemInfo GetItemByTemplateID(int TemplateID)
        {
            return GetItemByTemplateID(0, TemplateID);
        }

        /// <summary>
        /// 指模板ID查找物品定位置
        /// </summary>
        /// <param name="start"></param>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public override ItemInfo GetItemByTemplateID(int start, int TemplateID)
        {
            return base.GetItemByTemplateID(start, TemplateID);
        }

        /// <summary>
        /// 模板ID查找物品数量
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public int GetItemCount(int TemplateID)
        {
            return base.GetItemCount(0, TemplateID);
        }

        /// <summary>
        /// 删除指定ID物品数量
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveItemCount(int TemplateID, int count)
        {
            lock (m_lock)
            {
                if (GetItemCount(TemplateID) >= count)
                {
                    for (int i = 0; i < m_items.Length; i++)
                    {
                        if (m_items[i] != null && m_items[i].TemplateID == TemplateID)
                        {
                            if (count >= m_items[i].Count)
                            {
                                count -= m_items[i].Count;
                                m_items[i].IsExist = false;
                                RemoveItem(m_items[i]);

                                if (count <= 0)
                                    return true;
                            }
                            else
                            {
                                m_items[i].Count -= count;
                                RefreshItem(m_items[i]);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public int GetEmptyCount()
        {
            return base.GetEmptyCount(0);
        }

        public bool IsSolt(int slot)
        {
            return slot >= 0 && slot < _count;
        }

        public void MoveToStore(int fromSolt, int toSolt, int bagType,int maxCount)
        {
            IBag bag = _player.GetBag(bagType);

            if (bag == null)
                return;

            ItemInfo bagItem = bag.GetItemAt(toSolt);
            ItemInfo storeItem = _player.StoreBag.GetItemAt(fromSolt);

            if (bagItem == null && storeItem == null)
                return;

            if (storeItem == null && (!IsSolt(fromSolt)||fromSolt >= maxCount))
            {
                fromSolt = FindFirstEmptySlot(0);
            }

            if (bagItem == null && !bag.IsSolt(toSolt))
            {
                if (bagType == 0)
                {
                    toSolt = bag.FindFirstEmptySlot(31);
                }
                else
                {
                    toSolt = bag.FindFirstEmptySlot(0);
                }
            }

            if (storeItem != null && storeItem.GetBagType() != bagType)
            {
                fromSolt = -1;
                toSolt = -1;
            }

            //if (fromSolt >= maxCount)
            //    return;

            if (fromSolt == -1 || toSolt == -1 || fromSolt >= maxCount)
            {
                if (bagItem != null)
                    _player.Out.SendUpdateInventorySlot(bagItem.Place, true, bagItem, bagItem.BagType);

                if (storeItem != null)
                    _player.Out.SendUpdateInventorySlot(storeItem.Place, true, storeItem, storeItem.BagType);
            }
            else
            {
                if (storeItem != null)
                {
                    RemoveItem(storeItem);
                    bag.AddItemTo(storeItem, toSolt);
                }

                if (bagItem != null)
                {
                    bag.RemoveItem(bagItem);
                    AddItemTo(bagItem, fromSolt);
                }


            }

        }

        public void SendStoreToMail()
        {
            if (_player.PlayerCharacter.ConsortiaID != 0)
                return;

            bool response = false;
            int annexIndex = 0;
            MailInfo message = new MailInfo();
            StringBuilder annexRemark = new StringBuilder();
            annexRemark.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
            _player.SaveIntoDatabase();

            using (PlayerBussiness db = new PlayerBussiness())
            {
                for (int i = 0; i < _count; i++)
                {
                    ItemInfo item = m_items[i];
                    if (item == null)
                        continue;

                    RemoveItem(item);
                    response = true;
                    annexIndex++;
                    annexRemark.Append(annexIndex);
                    annexRemark.Append("、");
                    annexRemark.Append(item.Template.Name);
                    annexRemark.Append("x");
                    annexRemark.Append(item.Count);
                    annexRemark.Append(";");

                    switch (annexIndex)
                    {
                        case 1:
                            message.Annex1 = item.ItemID.ToString();
                            message.Annex1Name = item.Template.Name;
                            break;
                        case 2:
                            message.Annex2 = item.ItemID.ToString();
                            message.Annex2Name = item.Template.Name;
                            break;
                        case 3:
                            message.Annex3 = item.ItemID.ToString();
                            message.Annex3Name = item.Template.Name;
                            break;
                        case 4:
                            message.Annex4 = item.ItemID.ToString();
                            message.Annex4Name = item.Template.Name;
                            break;
                        case 5:
                            message.Annex5 = item.ItemID.ToString();
                            message.Annex5Name = item.Template.Name;
                            break;
                    }

                    if (annexIndex == 5)
                    {                       
                        annexIndex = 0;
                        message.AnnexRemark = annexRemark.ToString();
                        annexRemark.Remove(0, annexRemark.Length);
                        annexRemark.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));
                        message.Content = ""; 
                        message.Gold = 0;
                        message.Money = 0;
                        message.Receiver = _player.PlayerCharacter.NickName;
                        message.ReceiverID = _player.PlayerCharacter.ID;
                        message.Sender = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender");
                        message.SenderID = _player.PlayerCharacter.ID;
                        message.Title = message.Annex1Name; //message.AnnexRemark;// LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Title");
                        message.Type = (int)eMailType.StoreCanel;
                        db.SendMail(message);

                        message.Revert();
                    }
                }

                if (annexIndex > 0)
                {
                    message.AnnexRemark = annexRemark.ToString();
                    message.Content = "";
                    message.Gold = 0;
                    message.Money = 0;
                    message.Receiver = _player.PlayerCharacter.NickName;
                    message.ReceiverID = _player.PlayerCharacter.ID;
                    message.Sender = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender");
                    message.SenderID = _player.PlayerCharacter.ID;
                    message.Title = message.Annex1Name; //message.AnnexRemark;// LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Title");
                    message.Type = (int)eMailType.StoreCanel;
                    db.SendMail(message);
                }
            }

            _player.SaveIntoDatabase();

            if (response)
            {
                _player.Out.SendMailResponse(_player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
        }
    }
}
