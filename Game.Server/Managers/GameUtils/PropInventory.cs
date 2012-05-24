using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
    /// <summary>
    /// 道具栏容器
    /// </summary>
    public class PropInventory:AbstractInventory
    {
        private GamePlayer _player;

        public PropInventory(GamePlayer player):base(3,-1)
        {
            _player = player;
        }

        public int AddItem(ItemInfo item)
        {
            return AddItem(item, 0);
        }

        /// <summary>
        /// 成功返回物品的位置，失败返回-1
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public override int AddItem(ItemInfo item,int start)
        {
            int place = base.AddItem(item,start);
            if (place != -1)
            {
                _player.Out.SendUpdatePropInventorySlot(place, true, item.TemplateID);
            }
            return place;
        }

        /// <summary>
        /// 通过模板添加物品,成功返回ItemInfo实例,失败返回null。
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public ItemInfo AddItemTemplate(ItemTemplateInfo temp)
        {
            ItemInfo info = ItemInfo.CreateFromTemplate(temp,1);
            //info.Count = 1;
            if (AddItem(info) != -1)
            {
                return info;
            }
            else
            {
                return null;
            }
        }

        public override int RemoveItem(ItemInfo item)
        {
            int place = base.RemoveItem(item);
            if (place != -1)
            {
                _player.Out.SendUpdatePropInventorySlot(place, false, 0);
            }
            return place;
        }
        
        /// <summary>
        /// 从指定位置删除物品，成功返回该位置的物品的ItemInfo的实例,失败返回null
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public ItemInfo RemoveItemAt(int slot)
        {
            ItemInfo item = GetItemAt(slot);
            if (item != null)
            {
                RemoveItem(item);
            }
            return item;
        }

        public ItemInfo[] Items
        {
            get
            {
                return _items;
            }
        }

    }
}
