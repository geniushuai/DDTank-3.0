using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object
{
    public class Box :PhysicalObj
    {
        private int _userID;

        private int _liveCount;

        private ItemInfo m_item;

        public int UserID
        {
            get {   return _userID;     }
            set {   _userID = value;    }
        }

        public int LiveCount
        {
            get {   return _liveCount;  }
            set {   _liveCount = value; }
        }

        public ItemInfo Item
        {
            get { return m_item; }
        }

        public Box(int id,string model,ItemInfo item)
            : base(id,"",model,"",1,1)
        {
            _userID = 0;
            m_rect = new Rectangle(-15, -15, 30, 30);
            m_item = item;
        }

        public override int Type
        {
            get {   return  1;   }
        }

        public override void CollidedByObject(Physics phy)
        {
            if (phy is SimpleBomb)
            {
                SimpleBomb bomb = phy as SimpleBomb;
                bomb.Owner.PickBox(this);
            }
        }
    }
}
