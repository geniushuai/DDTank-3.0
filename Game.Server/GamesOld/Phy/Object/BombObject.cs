using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Maps;
using Phy.Maths;
using Phy.Actions;
using System.Drawing;
using Phy.Object;
using Game.Server;
using Game.Server.Effects;

namespace Phy.Object
{
    public class BombObject : Physics
    {
        private float m_mass;

        private float m_gravityFactor;

        private float m_windFactor;

        private float m_airResitFactor;

        private EulerVector m_vx;

        private EulerVector m_vy;

        public BombObject(int id,float mass, float gravityFactor, float windFactor, float airResitFactor)
            : base(id)
        {
            m_mass = mass;
            m_gravityFactor = gravityFactor;
            m_windFactor = windFactor;
            m_airResitFactor = airResitFactor;

            m_vx = new EulerVector(0, 0, 0);
            m_vy = new EulerVector(0, 0, 0);

            m_rect = new Rectangle(-3,-3,6,6);
        }

        public void setSpeedXY(int vx,int vy)
        {
            m_vx.x1 = vx;
            m_vy.x1 = vy;
        }

        public override void SetXY(int x, int y)
        {
            base.SetXY(x, y);
            m_vx.x0 = x;
            m_vy.x0 = y;
        }

        public float vX
        {
            get { return m_vx.x1; }
        }

        public float vY
        {
            get { return m_vy.x1; }
        }

        private float m_arf = 0; //空气阻力
        private float m_gf = 0;  //重力
        private float m_wf = 0;  //风力
        public override void SetMap(Map map)
        {
            base.SetMap(map);
            UpdateAGW();
        }

        protected void UpdateForceFactor(float air, float gravity, float wind)
        {
            m_airResitFactor = air;
            m_gravityFactor = gravity;
            m_windFactor = wind;
            UpdateAGW();
        }

        private void UpdateAGW()
        {
            if (m_map != null)
            {
                m_arf = m_map.airResistance * m_airResitFactor;
                m_gf = m_map.gravity * m_gravityFactor * m_mass;
                m_wf = m_map.wind * m_windFactor;
            }
        }

        protected Point CompleteNextMovePoint(float dt)
        {
            m_vx.ComputeOneEulerStep(m_mass, m_arf, m_wf, dt);
            m_vy.ComputeOneEulerStep(m_mass, m_arf, m_gf, dt);
            return new Point((int)m_vx.x0, (int)m_vy.x0);
        }

        public void MoveTo(int px, int py)
        {
            if (px != m_x || py != m_y)
            {
                int dx = px - m_x;
                int dy = py - m_y;
                int count = 0;
                int dt = 1;
                bool useX = true;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    useX = true;
                    count = Math.Abs(dx);
                    dt = dx / count;
                }
                else
                {
                    useX = false;
                    count = Math.Abs(dy);
                    dt = dy / count;
                }

                
                Point dest = new Point(m_x,m_y);
                for (int i = 1; i <= count; i += 3)
                {
                    if (useX)
                    {
                        dest = GetNextPointByX(m_x, px, m_y, py, m_x + i * dt);
                    }
                    else
                    {
                        dest = GetNextPointByY(m_x, px, m_y, py, m_y + i * dt);
                    }

                    Rectangle rect = m_rect;
                    rect.Offset(dest.X, dest.Y);
                    Physics[] list = m_map.FindPhysicalObjects(rect,this);
                    if (list.Length > 0)
                    {
                        base.SetXY(dest.X, dest.Y);
                        CollideObjects(list);
                    }
                    else if (!m_map.IsRectangleEmpty(rect))
                    {
                        base.SetXY(dest.X, dest.Y);
                        CollideGround();
                    }
                    else if (m_map.IsOutMap(dest.X, dest.Y))
                    {
                        base.SetXY(dest.X, dest.Y);
                        FlyoutMap();
                    }
                    if (!m_isLiving)
                    {
                        return;
                    }
                   
                }
                base.SetXY(px, py);
            }
        }

        protected virtual void CollideObjects(Physics[] list) 
        {
 
        }

        protected virtual void CollideGround() 
        {
            if (m_isMoving)
            {
                StopMoving();
            }
        }

        protected virtual void FlyoutMap()
        {
            if (m_isLiving)
            {
                Die();
            }
        }

        private Point GetNextPointByX(int x1,int x2,int y1,int y2, int x)
        {
            if (x2 == x1)
                return new Point(x, y1);
            else
                return new Point(x, (int)((x - x1) * (y2 - y1) / (x2 - x1) + y1));
        }

        private Point GetNextPointByY(int x1,int x2,int y1,int y2, int y)
        {
            if (y2 == y1)
                return new Point(x1, y);
            else
                return new Point((int)((y - y1) * (x2 - x1) / (y2 - y1) + x1), y);
        }

    }
}
