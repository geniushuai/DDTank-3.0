using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SqlDataProvider.Data;

namespace Phy.Object
{
    public class Box : Physics
    {
        private int _userID;

        //private int _itemID;

        private MapGoodsInfo _items;

        private int _liveCount;

        public int UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
            }
        }

        public MapGoodsInfo Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public int LiveCount
        {
            get
            {
                return _liveCount;
            }
            set
            {
                _liveCount = value;
            }
        }

        public Box(int id, MapGoodsInfo items)
            : base(id)
        {
            _userID = 0;
            _items = items;
            _rect = new Rectangle(-15, -15, 30, 30);
            _liveCount = 8;
        }
    }
}
