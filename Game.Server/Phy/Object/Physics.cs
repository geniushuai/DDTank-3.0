using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Maps;
using System.Drawing;

namespace Phy.Object
{
    public class Physics
    {
        protected int _id;

        protected Map _map;

        protected int _x;

        protected int _y;

        protected Rectangle _rect;

        protected bool _isLiving;

        public Physics(int id)
		{
            _id = id;
            _rect = new Rectangle(0, 0, 5, 5);
            _isLiving = true;
		}

        public Rectangle Bound
        {
            get
            {
                return _rect;
            }
        }

		virtual public void setMap(Map map)
	 	{
	 		_map = map;
	 	}

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public virtual int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public virtual int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public virtual void SetXY(int x,int y)
        {
            _x = x;
            _y = y;
        }

        public virtual void SetXY(Point p)
        {
            _x = p.X;
            _y = p.Y;
        }

        public bool IsLiving
        {
            get
            {
                return _isLiving;
            }
        }

        public virtual void StartMoving()
        {

        }

        public virtual void Die()
        {
            _isLiving = false;
            if(_map != null)
                _map.RemovePhysical(this);
        }

        public int Distance(int x, int y)
        {
            return (int)Math.Sqrt((_x - x) * (_x - x) + (_y - y) * (_y - y));
        }

        public static int PointToLine(int x1, int y1, int x2, int y2, int px, int py)
        {
            int a = y1 - y2;
            int b = x2 - x1;
            int c = x1 * y2 - x2 * y1;
            return (int)(Math.Abs(a * px + b * py + c) / Math.Sqrt(a * a + b * b));
        }
    }
}
