using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Game.Logic.Phy.Maps;

namespace Game.Logic.Phy.Object
{
    public class Physics
    {
        protected int m_id;

        protected Map m_map;

        protected int m_x;

        protected int m_y;

        protected Rectangle m_rect;

        protected Rectangle m_rectBomb;

        protected bool m_isLiving;

        protected bool m_isMoving;

        public Physics(int id)
        {
            m_id = id;
            m_rect = new Rectangle(-5, -5, 10, 10);
            m_rectBomb = new Rectangle(0, 0, 0, 0);
            m_isLiving = true;
        }

        public int Id
        {
            get { return m_id; }
        }

        public Rectangle Bound
        {
            get { return m_rect; }
        }

        public Rectangle Bound1
        {
            get { return m_rectBomb; }
        }
        public bool IsMoving
        {
            get { return m_isMoving; }
        }

        public bool IsLiving
        {
            get { return m_isLiving; }

            set { m_isLiving = value; }
        }

        public virtual int X
        {
            get { return m_x; }
        }

        public virtual int Y
        {
            get { return m_y; }
        }

        public virtual Point GetCollidePoint()
        {
            return new Point(X, Y);
        }

        public void SetRect(int x, int y, int width, int height)
        {
            m_rect.X = x;
            m_rect.Y = y;
            m_rect.Width = width;
            m_rect.Height = height;
        }

        public void SetRectBomb(int x, int y, int width, int height)
        {
            m_rectBomb.X = x;
            m_rectBomb.Y = y;
            m_rectBomb.Width = width;
            m_rectBomb.Height = height;
        }

        public virtual void SetXY(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        public void SetXY(Point p)
        {
            SetXY(p.X, p.Y);
        }

        public virtual void SetMap(Map map)
        {
            m_map = map;
        }

        public virtual void StartMoving()
        {
            if (m_map != null)
            {
                m_isMoving = true;
            }
        }

        public virtual void StopMoving()
        {
            m_isMoving = false;
        }

        public virtual void CollidedByObject(Physics phy) { }

        public virtual void Die()
        {
            StopMoving();
            m_isLiving = false;
        }

        public double Distance(int x, int y)
        {
            return Math.Sqrt((m_x - x) * (m_x - x) + (m_y - y) * (m_y - y));
        }

        public static int PointToLine(int x1, int y1, int x2, int y2, int px, int py)
        {
            int a = y1 - y2;
            int b = x2 - x1;
            int c = x1 * y2 - x2 * y1;
            return (int)(Math.Abs(a * px + b * py + c) / Math.Sqrt(a * a + b * b));
        }

       

        public virtual void PrepareNewTurn() { }

        #region Dispose

        public virtual void Dispose()
        {
            if (m_map != null)
                m_map.RemovePhysical(this);
        }

        #endregion
    }
}
