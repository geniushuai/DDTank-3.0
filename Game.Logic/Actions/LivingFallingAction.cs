using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingFallingAction : BaseAction
    {
        private Living m_living;

        private int m_fallSpeed;

        private int m_toX;

        private int m_toY;

        private bool m_isSent;

        private string m_action;

        private int m_type;

        private LivingCallBack m_callback;


        public LivingFallingAction(Living living, int toX, int toY, int speed, string action, int delay, int type, LivingCallBack callback)
            : base(delay, 2000)
        {
            m_living = living;
            m_fallSpeed = speed;
            m_action = action;
            m_toX = toX;
            m_toY = toY;
            m_isSent = false;
            m_type = type;
            m_callback = callback;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            if (!m_isSent)
            {
                m_isSent = true;
                game.SendLivingFall(m_living, m_toX, m_toY, m_fallSpeed, m_action, m_type);
            }

            if (m_toY > m_living.Y + m_fallSpeed)
            {
                m_living.SetXY(m_toX, m_living.Y + m_fallSpeed);
            }
            else
            {
                m_living.SetXY(m_toX, m_toY);
                if (game.Map.IsOutMap(m_toX, m_toY))
                {
                    m_living.SyncAtTime = false;
                    m_living.Die();
                    Console.WriteLine("掉落Fall" + m_living.IsLiving.ToString());
                }
                if (m_callback != null)
                {
                    //m_living.RunLuaFunction(m_callback);
                    m_living.CallFuction(m_callback, 0);
                }
                Finish(tick);
            }
        }
    }
}
