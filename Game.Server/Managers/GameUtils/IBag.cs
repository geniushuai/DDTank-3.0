using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.GameUtils
{
    public interface IBag
    {
        ItemInfo GetItemAt(int slot);

        int AddItem(ItemInfo item);

        int RemoveItem(ItemInfo item);

        bool MoveItem(int fromSlot, int toSlot);

        bool SplitItem(ItemInfo item, int count, int toSlot);
    }
}
