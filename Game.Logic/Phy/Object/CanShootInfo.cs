using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Phy.Object
{
    public class CanShootInfo
    {
        private bool m_canShoot;
        private int m_force;
        private int m_angle;

        public CanShootInfo(bool canShoot, int force, int angle)
        {
            m_canShoot = canShoot;
            m_force = force;
            m_angle = angle;
        }

        public bool CanShoot 
        { 
            get 
            { 
                return m_canShoot; 
            }
        }

        public int Force
        {
            get 
            { 
                return m_force; 
            }
        }

        public int Angle
        {
            get
            {
                return m_angle;
            }
        }
    }
}
