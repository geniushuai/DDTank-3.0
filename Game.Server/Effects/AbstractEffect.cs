using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Phy.Object;

namespace Game.Logic.Effects
{
    public abstract class AbstractEffect
    {
        protected Player m_player;

        public virtual void Start(Player player)
        {
            m_player.EffectList.Add(this);
        }

        public virtual void Stop() 
        {
            m_player.EffectList.Remove(this);
        }
    }
}
