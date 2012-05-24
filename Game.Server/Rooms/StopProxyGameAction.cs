using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Rooms
{
    public class StopProxyGameAction : IAction
    {
        private BaseRoom m_room;

        public StopProxyGameAction(BaseRoom room)
        {
            m_room = room;
        }

        public void Execute()
        {
            if (m_room.Game != null)
                m_room.Game.Stop();
            RoomMgr.WaitingRoom.SendUpdateRoom(m_room);
        }
    }
}
