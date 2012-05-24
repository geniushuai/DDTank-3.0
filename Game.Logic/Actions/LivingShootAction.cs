using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using Game.Logic.Phy.Maps;

namespace Game.Logic.Actions
{
    public class LivingShootAction:BaseAction
    {
        private Living m_living;

        private int m_tx;

        private int m_ty;

        private int m_bombId;

        private int m_force;

        private int m_angle;

        private int m_bombCount;

        private int m_minTime;

        private int m_maxTime;

        private float m_Time;

       
        public LivingShootAction(Living living, int bombId, int x, int y,int force,int angle, int bombCount, int minTime, int maxTime, float time,int delay)
            :base(delay,1000)
        {
            m_living = living;
            m_bombId = bombId;
            m_tx = x;
            m_ty = y;
            m_force = force;
            m_angle = angle;
            m_bombCount = bombCount;
            m_bombId = bombId;
            m_minTime = minTime;
            m_maxTime = maxTime;
            m_Time = time;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            if (m_living is SimpleBoss)
            {
                m_living.GetShootForceAndAngle(ref m_tx, ref m_ty, m_bombId, m_minTime, m_maxTime, m_bombCount, m_Time, ref m_force, ref m_angle);
            }
            m_living.ShootImp(m_bombId,m_tx,m_ty, m_force, m_angle, m_bombCount);
            Finish(tick);
        }
    }
}
