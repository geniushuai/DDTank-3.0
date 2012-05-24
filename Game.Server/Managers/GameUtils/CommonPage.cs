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
    /// 
    /// </summary>
    public class CommonBag : AbstractInventory,IBag
    {
        private GamePlayer _player;
        private List<ItemInfo> _removedList;

        public CommonBag(GamePlayer player,int bagType)
            : base(49, bagType)
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
                    _items[item.Place] = item;
                }              
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
            return AddItem(item, 0);
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

            if (to != null)
            {
                //合并物品
                if (from.TemplateID == to.TemplateID && to.Template.MaxCount > 1)
                {
                    UseItem(from);
                    UseItem(to);
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
            if (item.Count > count  && _items[toSlot] == null)
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
            lock (_lock)
            {
                if (GetItemCount(TemplateID) >= count)
                {
                    for (int i = 0; i < _items.Length; i++)
                    {
                        if (_items[i] != null && _items[i].TemplateID == TemplateID)
                        {
                            count -= _items[i].Count;
                            if (count >= 0)
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
