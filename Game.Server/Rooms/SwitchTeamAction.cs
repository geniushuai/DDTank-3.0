using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;

namespace Game.Server.Rooms
{
    public class SwitchTeamAction:IAction
    {
        private GamePlayer m_player;

        public SwitchTeamAction(GamePlayer player)
        {
            m_player = player;
        }

        public void Execute()
        {
            BaseRoom room = m_player.CurrentRoom;
            if (room != null)
                room.SwitchTeamUnsafe(m_player);
        }
    }
}
