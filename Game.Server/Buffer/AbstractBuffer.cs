using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class AbstractBuffer
    {
        protected BufferInfo m_info;
        protected GamePlayer m_player;

        public AbstractBuffer(BufferInfo info)
        {
            m_info = info;
        }

        public BufferInfo Info
        {
            get { return m_info; }
        }

        public virtual void Start(GamePlayer player)
        {
            m_info.UserID = player.PlayerId;
            m_info.IsExist = true;
            m_player = player;
            m_player.BufferList.AddBuffer(this);
        }

        public virtual void Restore(GamePlayer player)
        {
            Start(player);
        }

        public virtual void Stop()
        {
            m_info.IsExist = false;
            m_player.BufferList.RemoveBuffer(this);
            m_player = null;
        }

        public bool Check()
        {
            if (m_info.BeginDate.AddMinutes(m_info.ValidDate) < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
