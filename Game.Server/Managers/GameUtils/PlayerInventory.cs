using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Bussiness;

namespace Game.Server.GameUtils
{
    /// <summary>
    /// 用户的背包
    /// </summary>
    public class PlayerInventory : AbstractInventory, IBag
    {
        private GamePlayer _player;
        private List<ItemInfo> _removedList;

        public PlayerInventory(GamePlayer player)
            : base(60,0)
        {
            _player = player;
            _removedList = new List<ItemInfo>();
            //LoadFromDatabase(player.PlayerCharacter.ID);
        }

        /// <summary>
        /// 从数据库中加载
        /// </summary>
        /// <param name="playerId"></param>
        public void LoadFromDatabase(int playerId)
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                //ItemInfo[] list = pb.GetUserItem(playerId);
                ItemInfo[] list = pb.GetUserBagByType(playerId, _bagType);
                foreach (ItemInfo item in list)
                {
                    _items[item.Place] = item;
                }


                for (int i = 0; i < 11; i++)
                {
                    ItemInfo item = _items[i];
                    if (item == null)
                        continue;

                    if (!_items[i].IsValidItem())
                    {
                        UpdatePlayerProperties(item, false);
                        int place = base.FindFirstEmptySlot(11);
                        if (place != -1)
                        {
                            MoveItem(item.Place, place);
                        }
                        else
                        {
                            MailInfo mail = new MailInfo();
                            mail.Annex1 = item.ItemID.ToString();
                            mail.Content = "物品已过期!";
                            mail.Gold = 0;
                            mail.IsExist = true;
                            mail.Money = 0;
                            mail.Receiver = _player.PlayerCharacter.NickName;
                            mail.ReceiverID = item.UserID;
                            mail.Sender = _player.PlayerCharacter.NickName;
                            mail.SenderID = item.UserID;
                            mail.Title = "物品已过期!";
                            if (pb.SendMail(mail))
                            {
                                item.UserID = 0;
                                RemoveItem(item);
                            }
                        }
                    }
                }
                UpdatePlayerProperties();
            }

        }

        /// <summary>
        /// 保存到数据库中
        /// </summary>
        public void SaveToDatabase()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                lock (_lock)
                {
                    for (int i = 0; i < _items.Length; i++)
                    {
                        ItemInfo item = _items[i];
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
                        pb.UpdateGoods(item);
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
        public int AddItem(ItemInfo item)
        {
            return AddItem(item, 11);
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

        public override int RemoveItem(ItemInfo item)
        {
            //物品在身上不允许删除。
            if (IsEquipSlot(item.Place))
            {
                return -1;
                //UpdatePlayerProperties(item, false);
            }
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
        public override bool MoveItem(int fromSlot, int toSlot)
        {
            ItemInfo from = GetItemAt(fromSlot);
            ItemInfo to = GetItemAt(toSlot);

            if (from == null || fromSlot == toSlot) return false;

            //此位置是否能容纳此物品
            bool isEquip = IsEquipSlot(toSlot);
            if (isEquip)
            {
                if (fromSlot < 11)
                    return false;

                //装备物品
                if (_player.CanEquip(from.Template) && CanEquipSlotContains(toSlot, from.Template) && from.IsValidItem())
                {
                    if (base.MoveItem(fromSlot, toSlot))
                    {
                        UseItem(from);
                        _player.Out.SendUpdateInventorySlot(from.Place, true, from,_bagType);
                        if (to != null)
                        {
                            _player.Out.SendUpdateInventorySlot(to.Place, true, to,_bagType);
                            UpdatePlayerProperties(to, false);
                        }
                        else
                        {
                            _player.Out.SendUpdateInventorySlot(fromSlot, false, null,_bagType);
                        }
                        UpdatePlayerProperties(from, true);
                        UpdatePlayerStyle(from, toSlot);
                        return true;
                    }
                }
            }
            else if (to != null)
            {
                UseItem(from);
                UseItem(to);
                //合并物品
                if (from.TemplateID == to.TemplateID && to.Template.MaxCount > 1)
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
                    if (fromSlot < 11)
                    {
                        UpdatePlayerProperties(from, false);
                        UpdatePlayerStyle(null, fromSlot);
                    }
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
            if (item.Count > count && !IsEquipSlot(toSlot) && _items[toSlot] == null)
            {
                ItemInfo clone = item.Clone();
                clone.ItemID = -1;
                clone.Count = count;
                //clone.Place = toSlot;
                if (AddItem(clone) != -1)
                {
                    UseItem(item);
                    UseItem(clone);
                    item.Count -= count;
                    _player.Out.SendUpdateInventorySlot(item.Place, true, item,_bagType);
                    _player.Out.SendUpdateInventorySlot(clone.Place, true, clone,_bagType);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否为身上的插槽
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool IsEquipSlot(int slot)
        {
            return slot >= 0 && slot <= 10;
        }

        /// <summary>
        /// 更新人物的属性
        /// </summary>
        /// <param name="item"></param>
        /// <param name="add"></param>
        public void UpdatePlayerProperties(ItemInfo item, bool add)
        {
            //PlayerInfo my_character = _player.PlayerCharacter;
            //if (add)
            //{
            //    my_character.Agility += item.Agility;
            //    my_character.Defence += item.Defence;
            //    my_character.Attack += item.Attack;
            //    my_character.Luck += item.Luck;
            //    //UseItem(item);
            //}
            //else
            //{
            //    my_character.Agility -= item.Agility;
            //    my_character.Defence -= item.Defence;
            //    my_character.Attack -= item.Attack;
            //    my_character.Luck -= item.Luck;
            //}

            if (item.StrengthenLevel > 4)
            {
                int level = GetEquipLevel();
                _player.ApertureEquip(level);
                //if (add || GetEquipLevel())
                //{
                //    _player.ApertureEquip( true);
                //}
                //else
                //{
                //    _player.ApertureEquip( false);
                //}
            }
        }

        public void UpdatePlayerProperties()
        {
            PlayerInfo my_character = _player.PlayerCharacter;
            int attack = 0;
            int defence = 0;
            int agility = 0;
            int lucy = 0;
            for (int i = 0; i < 11; i++)
            {
                ItemInfo item = _items[i];
                if (item == null)
                    continue;
                attack += item.Attack;
                defence += item.Defence;
                agility += item.Agility;
                lucy += item.Luck;
            }

            my_character.Attack = attack;
            my_character.Defence = defence;
            my_character.Agility = agility;
            my_character.Luck = lucy;
        }

        public int GetEquipLevel()
        {
            int level = 0;
            lock (_lock)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (_items[i] != null && _items[i].StrengthenLevel > level)
                        level = _items[i].StrengthenLevel;
                }
            }
            return level;
        }

        /// <summary>
        /// 更新人物的形象
        /// </summary>
        /// <param name="item"></param>
        public void UpdatePlayerStyle(ItemInfo item, int place)
        {
            UpdatePlayerProperties();
            _player.UpdateStyle(GetStyle());
            if (place == 6)
            {
                _player.UpdateWeapon(item);
            }
        }

        /// <summary>
        /// 构造人物的形象
        /// </summary>
        /// <returns></returns>
        private string GetStyle()
        {
            string style = _items[0] == null ? "" : _items[0].TemplateID.ToString();
            string color = _items[0] == null ? "" : _items[0].Color;
            for (int i = 1; i < 7; i++)
            {
                style += ",";
                color += ",";
                if (_items[i] != null)
                {
                    style += _items[i].TemplateID;
                    color += _items[i].Color;
                }
            }
            _player.PlayerCharacter.Style = style;
            _player.PlayerCharacter.Colors = color;
            return style;
        }

        /// <summary>
        /// 获取物品的装备位置
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemEpuipSlot(ItemTemplateInfo item)
        {
            switch (item.CategoryID)
            {
                case 8:
                    if (_items[7] == null)
                    {
                        return 7;
                    }
                    else
                    {
                        return 8;
                    }
                case 9:
                    if (_items[9] == null)
                    {
                        return 9;
                    }
                    else
                    {
                        return 10;
                    }
                default:
                    return item.CategoryID - 1;
            }
        }

        /// <summary>
        /// 装备的位置是否能装备此物品。
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
        {
            if (temp.CategoryID == 8)
            {
                return slot == 7 || slot == 8;
            }
            else if (temp.CategoryID == 9)
            {
                return slot == 9 || slot == 10;
            }
            else
            {
                return temp.CategoryID - 1 == slot;
            }
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
            return GetItemByCategoryID(11, categoryID, property);
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
            return GetItemByTemplateID(11, TemplateID);
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
            return base.GetItemCount(11, TemplateID);
        }

        /// <summary>
        /// 删除指定ID物品数量
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveItemCount(int TemplateID, int count)
        {
            lock (_lock)
            {
                if (GetItemCount(TemplateID) >= count)
                {
                    for (int i = 11; i < _items.Length; i++)
                    {
                        if (_items[i] != null && _items[i].TemplateID == TemplateID)
                        {
                            count -= _items[i].Count;
                            if (count >= _items[i].Count)
                            {
                                _items[i].IsExist = false;
                                RemoveItem(_items[i]);
                            }
                            else
                            {
                                _items[i].Count -= count;
                                RefreshItem(_items[i]);
                            }

                            if (count <= 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
