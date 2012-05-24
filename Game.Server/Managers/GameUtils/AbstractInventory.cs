using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Newtonsoft.Json;

namespace Game.Server.GameUtils
{
    /// <summary>
    /// 抽象的背包容器
    /// </summary>
    public abstract class AbstractInventory
    {
        protected object _lock;

        protected ItemInfo[] _items;

        private int _count;

        protected int _bagType;

        public AbstractInventory(int count,int bagType)
        {
            _count = count;
            _bagType = bagType;
            _items = new ItemInfo[count];
            _lock = new object();
        }

        /// <summary>
        /// 从Start开始查找空位添加物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public virtual int AddItem(ItemInfo item,int start)
        {
            if (item == null) return -1;
            lock (_lock)
            {
                for (int i = start; i < _count; i++)
                {
                    if (_items[i] == null)
                    {
                        _items[i] = item;
                        item.Place = i;
                        item.BagType = _bagType;
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 删除物品，物品的位置信息在Place上
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int RemoveItem(ItemInfo item)
        {
            if (item == null) return -1;
            lock (_lock)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (_items[i] != null && _items[i].Equals(item))
                    {
                        item.Place = -1;
                        _items[i] = null;
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 移动物品
        /// </summary>
        /// <param name="fromSlot"></param>
        /// <param name="toSlot"></param>
        /// <returns></returns>
        public virtual bool MoveItem(int fromSlot, int toSlot)
        {
            if (fromSlot < 0 || toSlot < 0 || fromSlot >= _count || toSlot >= _count) return false;
            lock (_lock)
            {
                ItemInfo item = _items[fromSlot];
                _items[fromSlot] = _items[toSlot];
                if(_items[fromSlot] != null)
                {
                    _items[fromSlot].Place = fromSlot;
                }
                _items[toSlot] = item;
                item.Place = toSlot;
                return true;
            }
        }

        public virtual ItemInfo GetItemAt(int slot)
        {
            if (slot < 0 || slot >= _count) return null;
            return _items[slot];
        }

        /// <summary>
        /// 查找从Start开始的第一个空位
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public int FindFirstEmptySlot(int start)
        {
            lock (_lock)
            {
                for (int i = start; i < _count; i++)
                {
                    if (_items[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 查找最后一个空位
        /// </summary>
        /// <returns></returns>
        public int FindLastEmptySlot()
        {
            lock (_lock)
            {
                for (int i = _count -1; i >= 0; i--)
                {
                    if (_items[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 清除所有物品
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                for (int i = 0; i < _count; i++)
                {
                    _items[i] = null;
                }
            }
        }

        public virtual ItemInfo GetItemByCategoryID(int start,int categoryID,int property)
        {
            lock (_lock)
            {
                for (int i = start; i < _count; i++)
                {
                    if (_items[i] != null && _items[i].Template.CategoryID == categoryID)
                    {
                        if (property != -1 && _items[i].Template.Property1 != property)
                            continue;
                        return _items[i];
                    }
                }
                return null;
            }
        }

        public virtual ItemInfo GetItemByTemplateID(int start,int TemplateID)
        {
            lock (_lock)
            {
                for (int i = start; i < _count; i++)
                {
                    if (_items[i] != null && _items[i].TemplateID == TemplateID)
                    {
                        return _items[i];
                    }
                }
                return null;
            }
        }

        public virtual int GetItemCount(int start, int TemplateID)
        {
            int count = 0;
            lock (_lock)
            {
                for (int i = start; i < _count; i++)
                {
                    if (_items[i] != null && _items[i].TemplateID == TemplateID)
                    {
                       count += _items[i].Count;
                    }
                }
            }
            return count;
        }

        public virtual JavaScriptArray GetItemsJson(JavaScriptArray array)
        {
            //lock (_lock)
            //{
            //    for (int i = 0; i < _count; i++)
            //    {
            //        if (_items[i] != null )
            //        {
            //            array.Add(ItemInfo.GetItemJson(_items[i]));
            //        }
            //    }
            //}
            //return array;
            return GetItemsJson(0, _count, array);
        }

        public virtual JavaScriptArray GetItemsJson(int start, int end, JavaScriptArray array)
        {
            lock (_lock)
            {
                for (int i = start; i < end; i++)
                {
                    if (_items[i] != null)
                    {
                        array.Add(ItemInfo.GetItemJson(_items[i]));
                    }
                }
            }
            return array;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="item"></param>
        public virtual void UseItem(ItemInfo item)
        {
            if (!item.IsBinds && item.Template.BindType == 2)
            {
                item.IsBinds = true;
            }
        }
    }
}
