using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;

namespace Game.Server.Rooms
{
    public class UpdateRoomPosAction:IAction
    {
        BaseRoom m_room;

        int m_pos;

        bool m_isOpened;

        public UpdateRoomPosAction(BaseRoom room, int pos,bool isOpened)
        {
            m_room = room;
            m_pos = pos;
            m_isOpened = isOpened;
        }

        public void Execute()
        {
            if (m_room.PlayerCount > 0 && m_room.UpdatePosUnsafe(m_pos, m_isOpened))
            {
                RoomMgr.WaitingRoom.SendUpdateRoom(m_room);                
            }
        }
    }
}
