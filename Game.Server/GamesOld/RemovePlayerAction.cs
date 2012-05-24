using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Phy.Object;

namespace Game.Server.Games
{
    public class RemovePlayerAction:IAction
    {
        private bool m_isFinished;
        private Player m_player;

        public RemovePlayerAction(Player player)
        {
            m_player = player;
            m_isFinished = false;
        }

        public void Execute(BaseGame game, long tick)
        {
            m_player.DeadLink();
            m_isFinished = true;
        }

        public bool IsFinish()
        {
            return m_isFinished;
        }
    }
}
