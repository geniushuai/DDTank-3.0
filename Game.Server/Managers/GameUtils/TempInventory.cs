using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
    /// <summary>
    /// 战利品临时容器
    /// </summary>
    public class TempInventory:AbstractInventory
    {
        private GamePlayer _player;

        public TempInventory(GamePlayer player)
            : base(30,-1)
        {
            _player = player;
        }

        public int AddItem(ItemInfo item)
        {
            return AddItem(item, 0);
        }

        public override int AddItem(ItemInfo item,int start)
        {
            int place = base.AddItem(item,1);
            if (place != -1)
            {
                _player.Out.SendUpdateTempInventorySlot( place, true, item);
            }
            return place;
        }

        /// <summary>
        /// 通过模板添加物品
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int AddItemTemplate(ItemTemplateInfo temp,int count)
        {
            //ItemInfo info = new ItemInfo(temp);
            ItemInfo info = ItemInfo.CreateFromTemplate(temp, count);
            if (info == null)
                return -1;
            ItemInfo.BuyItemDate(info, 0);
            info.UserID = _player.PlayerCharacter.ID;
            return AddItem(info);
        }

        public override int RemoveItem(ItemInfo item)
        {
            int start = item.Place;
            int place = base.RemoveItem(item);
            if (place != -1)
            {
                item.Place = start;
                _player.Out.SendUpdateTempInventorySlot(place, false,null);
            }
            return place;
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
