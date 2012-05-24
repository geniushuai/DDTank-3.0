using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.Statics;
using System.Threading;
using log4net;
using System.Reflection;




namespace Game.Server.GameUtils
{
    /// <summary>
    /// 抽象的背包容器
    /// </summary>
    public abstract class AbstractInventory
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected object m_lock = new object();

        private int m_type;

        private int m_capalility;

        private int m_beginSlot;

        private bool m_autoStack;

        protected ItemInfo[] m_items;

        public int BeginSlot
        {
            get { return m_beginSlot; }
        }

        public int Capalility
        {
            get { return m_capalility; }
        }

        public int BagType
        {
            get { return m_type; }
        }

        public AbstractInventory(int capability, int type, int beginSlot, bool autoStack)
        {
            m_capalility = capability;
            m_type = type;
            m_beginSlot = beginSlot;
            m_autoStack = autoStack;


            m_items = new ItemInfo[capability];
        }

        #region Add/Remove/AddTemp/RemoveTemp/AddCountToStack/RemoveCountFromStack/Start

        public bool AddItem(ItemInfo item)
        {
            return AddItem(item, m_beginSlot);
        }

        public bool AddItem(ItemInfo item, int minSlot)
        {
            if (item == null) return false;

            int place = FindFirstEmptySlot(minSlot);

            return AddItemTo(item, place);
        }

        public virtual bool AddItemTo(ItemInfo item, int place)
        {
            if (item == null || place >= m_capalility || place < 0) return false;

            lock (m_lock)
            {
                if (m_items[place] != null)
                    place = -1;
                else
                {
                    m_items[place] = item;
                    item.Place = place;
                    item.BagType = m_type;
                }
            }
            if (place != -1)
                OnPlaceChanged(place);

            return place != -1;
        }

        public virtual bool TakeOutItem(ItemInfo item)
        {
            if (item == null) return false;
            int place = -1;
            lock (m_lock)
            {
                for (int i = 0; i < m_capalility; i++)
                {
                    if (m_items[i] == item)
                    {
                        place = i;
                        m_items[i] = null;

                        break;
                    }
                }
            }

            if (place != -1)
            {
                OnPlaceChanged(place);
                if (item.BagType == this.BagType)
                {
                    item.Place = -1;
                    item.BagType = -1;
                }
            }

            return place != -1;
        }

        public bool TakeOutItemAt(int place)
        {
            return TakeOutItem(GetItemAt(place));
        }

        public virtual bool RemoveItem(ItemInfo item)
        {
            if (item == null) return false;
            int place = -1;
            lock (m_lock)
            {
                for (int i = 0; i < m_capalility; i++)
                {
                    if (m_items[i] == item)
                    {
                        place = i;
                        m_items[i] = null;

                        break;
                    }
                }
            }

            if (place != -1)
            {
                OnPlaceChanged(place);
                if (item.BagType == this.BagType)
                {
                    item.Place = -1;
                    item.BagType = -1;
                }
            }

            return place != -1;
        }

        public bool RemoveItemAt(int place)
        {
            return RemoveItem(GetItemAt(place));
        }

        public virtual bool AddCountToStack(ItemInfo item, int count)
        {
            if (item == null) return false;
            if (count <= 0 || item.BagType != m_type) return false;

            if (item.Count + count > item.Template.MaxCount)
                return false;

            item.Count += count;

            OnPlaceChanged(item.Place);

            return true;
        }

        public virtual bool RemoveCountFromStack(ItemInfo item, int count)
        {
            if (item == null) return false;
            if (count <= 0 || item.BagType != m_type) return false;
            if (item.Count < count) return false;
            if (item.Count == count)
                return RemoveItem(item);
            else
                item.Count -= count;

            OnPlaceChanged(item.Place);

            return true;
        }

        public virtual bool AddTemplate(ItemInfo cloneItem, int count)
        {
            return AddTemplate(cloneItem, count, m_beginSlot, m_capalility - 1);
        }

        public virtual bool AddTemplate(ItemInfo cloneItem, int count, int minSlot, int maxSlot)
        {
            if (cloneItem == null) return false;
            ItemTemplateInfo template = cloneItem.Template;
            if (template == null) return false;
            if (count <= 0) return false;
            if (minSlot < m_beginSlot || minSlot > m_capalility - 1) return false;
            if (maxSlot < m_beginSlot || maxSlot > m_capalility - 1) return false;
            if (minSlot > maxSlot) return false;


            lock (m_lock)
            {
                List<int> changedSlot = new List<int>();
                int itemcount = count;

                for (int i = minSlot; i <= maxSlot; i++)
                {
                    ItemInfo item = m_items[i];
                    if (item == null)
                    {
                        itemcount -= template.MaxCount;
                        changedSlot.Add(i);
                    }
                    else if (m_autoStack && cloneItem.CanStackedTo(item))
                    {
                        itemcount -= (template.MaxCount - item.Count);
                        changedSlot.Add(i);
                    }
                    if (itemcount <= 0)
                        break;
                }

                if (itemcount <= 0)
                {
                    BeginChanges();
                    try
                    {
                        itemcount = count;
                        foreach (int i in changedSlot)
                        {
                            ItemInfo item = m_items[i];
                            if (item == null)
                            {
                                item = cloneItem.Clone();

                                item.Count = itemcount < template.MaxCount ? itemcount : template.MaxCount;

                                itemcount -= item.Count;

                                AddItemTo(item, i);
                            }
                            else
                            {
                                if (item.TemplateID == template.TemplateID)
                                {
                                    int add = (item.Count + itemcount < template.MaxCount ? itemcount : template.MaxCount - item.Count);
                                    item.Count += add;
                                    itemcount -= add;

                                    OnPlaceChanged(i);
                                }
                                else
                                {
                                    log.Error("Add template erro: select slot's TemplateId not equest templateId");
                                }
                            }
                        }

                        if (itemcount != 0)
                            log.Error("Add template error: last count not equal Zero.");
                    }
                    finally
                    {
                        CommitChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual bool RemoveTemplate(int templateId, int count)
        {
            return RemoveTemplate(templateId, count, 0, m_capalility - 1);
        }

        public virtual bool RemoveTemplate(int templateId, int count, int minSlot, int maxSlot)
        {
            if (count <= 0) return false;
            if (minSlot < 0 || minSlot > m_capalility - 1) return false;
            if (maxSlot <= 0 || maxSlot > m_capalility - 1) return false;
            if (minSlot > maxSlot) return false;

            lock (m_lock)
            {

                List<int> changedSlot = new List<int>();
                int itemcount = count;

                for (int i = minSlot; i < maxSlot; i++)
                {
                    ItemInfo item = m_items[i];
                    if (item != null && item.TemplateID == templateId)
                    {
                        changedSlot.Add(i);
                        itemcount -= item.Count;
                        if (itemcount <= 0)
                            break;
                    }
                }

                if (itemcount <= 0)
                {
                    BeginChanges();
                    itemcount = count;

                    try
                    {
                        foreach (int i in changedSlot)
                        {
                            ItemInfo item = m_items[i];

                            if (item != null && item.TemplateID == templateId)
                            {
                                if (item.Count <= itemcount)
                                {
                                    RemoveItem(item);
                                    itemcount -= item.Count;
                                }
                                else
                                {
                                    int dec = item.Count - itemcount < item.Count ? itemcount : 0;
                                    item.Count -= dec;
                                    itemcount -= dec;
                                    OnPlaceChanged(i);
                                }
                            }
                        }

                        if (itemcount != 0)
                            log.Error("Remove templat error:last itemcoutj not equal Zero.");
                    }
                    finally
                    {
                        CommitChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual bool MoveItem(int fromSlot, int toSlot, int count)
        {
            if (fromSlot < 0 || toSlot < 0 || fromSlot >= m_capalility || toSlot >= m_capalility) return false;

            bool result = false;
            lock (m_lock)
            {
                if (!CombineItems(fromSlot, toSlot) && !StackItems(fromSlot, toSlot, count))
                {
                    result = ExchangeItems(fromSlot, toSlot);
                }
                else
                {
                    result = true;
                }
            }

            if (result)
            {
                BeginChanges();
                try
                {
                    OnPlaceChanged(fromSlot);
                    OnPlaceChanged(toSlot);
                }
                finally
                {
                    CommitChanges();
                }
            }

            return result;
        }
        public bool IsSolt(int slot)
        {
            return slot >= 0 && slot < m_capalility;
        }

        public void ClearBag()
        {
            BeginChanges();
            lock (m_lock)
            {
                for (int i = m_beginSlot; i < m_capalility; i++)
                {
                    if (m_items[i] != null)
                    {
                        RemoveItem(m_items[i]);
                    }
                }
            }
          
                CommitChanges();
        }


        #endregion

        #region Combine/Exchange/Stack Items

        protected virtual bool CombineItems(int fromSlot, int toSlot)
        {
            return false;
        }

        protected virtual bool StackItems(int fromSlot, int toSlot, int itemCount)
        {
            ItemInfo fromItem = m_items[fromSlot] as ItemInfo;
            ItemInfo toItem = m_items[toSlot] as ItemInfo;

            if (itemCount == 0)
            {
                if (fromItem.Count > 0)
                    itemCount = fromItem.Count;
                else
                    itemCount = 1;
            }

            if (toItem != null && toItem.TemplateID == fromItem.TemplateID && toItem.CanStackedTo(fromItem))
            {
                if (fromItem.Count + toItem.Count > fromItem.Template.MaxCount)
                {
                    fromItem.Count -= (toItem.Template.MaxCount - toItem.Count);
                    toItem.Count = toItem.Template.MaxCount;
                }
                else
                {
                    toItem.Count += itemCount;
                    RemoveItem(fromItem);
                }
                return true;
            }
            else if (toItem == null && fromItem.Count > itemCount)
            {
                ItemInfo newItem = (ItemInfo)fromItem.Clone();
                newItem.Count = itemCount;
                if (AddItemTo(newItem, toSlot))
                {
                    fromItem.Count -= itemCount;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        protected virtual bool ExchangeItems(int fromSlot, int toSlot)
        {
            ItemInfo fromItem = m_items[toSlot];
            ItemInfo toItem = m_items[fromSlot];

            m_items[fromSlot] = fromItem;
            m_items[toSlot] = toItem;

            if (fromItem != null)
                fromItem.Place = fromSlot;

            if (toItem != null)
                toItem.Place = toSlot;

            return true;
        }

        #endregion Combine/Exchange/Stack Items

        #region Find Items

        public virtual ItemInfo GetItemAt(int slot)
        {
            if (slot < 0 || slot >= m_capalility) return null;

            return m_items[slot];
        }

        /// <summary>
        /// 查找从Start开始的第一个空位
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public int FindFirstEmptySlot(int minSlot)
        {
            if (minSlot >= m_capalility) return -1;

            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_items[i] == null)
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
            lock (m_lock)
            {
                for (int i = m_capalility - 1; i >= 0; i--)
                {
                    if (m_items[i] == null)
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
        public virtual void Clear()
        {
            lock (m_lock)
            {
                for (int i = 0; i < m_capalility; i++)
                {
                    m_items[i] = null;
                }
            }
        }

        public virtual ItemInfo GetItemByCategoryID(int minSlot, int categoryID, int property)
        {
            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_items[i] != null && m_items[i].Template.CategoryID == categoryID)
                    {
                        if (property != -1 && m_items[i].Template.Property1 != property)
                            continue;
                        return m_items[i];
                    }
                }
                return null;
            }
        }

        public virtual ItemInfo GetItemByTemplateID(int minSlot, int templateId)
        {
            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_items[i] != null && m_items[i].TemplateID == templateId)
                    {
                        return m_items[i];
                    }
                }
                return null;
            }
        }

        public virtual int GetItemCount(int templateId)
        {
            return GetItemCount(m_beginSlot, templateId);
        }

        public int GetItemCount(int minSlot, int templateId)
        {
            int count = 0;
            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_items[i] != null && m_items[i].TemplateID == templateId)
                    {
                        count += m_items[i].Count;
                    }
                }
            }
            return count;
        }

        public virtual List<ItemInfo> GetItems()
        {
            return GetItems(0, m_capalility);
        }

        public virtual List<ItemInfo> GetItems(int minSlot, int maxSlot)
        {
            List<ItemInfo> list = new List<ItemInfo>();
            lock (m_lock)
            {
                for (int i = minSlot; i < maxSlot; i++)
                {
                    if (m_items[i] != null)
                    {
                        list.Add(m_items[i]);
                    }
                }
            }
            return list;
        }

        public int GetEmptyCount()
        {
            return GetEmptyCount(m_beginSlot);
        }

        public virtual int GetEmptyCount(int minSlot)
        {
            if (minSlot < 0 || minSlot > m_capalility - 1) return 0;

            int count = 0;
            lock (m_lock)
            {
                for (int i = minSlot; i < m_capalility; i++)
                {
                    if (m_items[i] == null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="item"></param>
        public virtual void UseItem(ItemInfo item)
        {
            bool changed = false;
            if (item.IsBinds == false && (item.Template.BindType == 2 || item.Template.BindType == 3))
            {
                item.IsBinds = true;
                changed = true;
            }

            if (item.IsUsed == false)
            {
                item.IsUsed = true;
                item.BeginDate = DateTime.Now;
                changed = true;
            }

            if (changed)
            {
                OnPlaceChanged(item.Place);
            }
        }

        public virtual void UpdateItem(ItemInfo item)
        {
            if (item.BagType == m_type)
            {
                if (item.Count <= 0)
                    RemoveItem(item);
                else
                    OnPlaceChanged(item.Place);
            }
        }

        #endregion

        #region BeginChanges/CommiteChanges/UpdateChanges

        protected List<int> m_changedPlaces = new List<int>();
        private int m_changeCount;

        protected void OnPlaceChanged(int place)
        {
            if (m_changedPlaces.Contains(place) == false)
                m_changedPlaces.Add(place);

            if (m_changeCount <= 0 && m_changedPlaces.Count > 0)
            {
                UpdateChangedPlaces();
            }
        }

        public void BeginChanges()
        {
            Interlocked.Increment(ref m_changeCount);
        }

        public void CommitChanges()
        {
            int changes = Interlocked.Decrement(ref m_changeCount);
            if (changes < 0)
            {
                if (log.IsErrorEnabled)
                    log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
                Thread.VolatileWrite(ref m_changeCount, 0);
            }
            if (changes <= 0 && m_changedPlaces.Count > 0)
            {
                UpdateChangedPlaces();
            }
        }

        public virtual void UpdateChangedPlaces()
        {
            m_changedPlaces.Clear();
        }




        #endregion
    }
}
