using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fighting.Server.Rooms
{
    public class CancelRoomAction:IAction
    {
        private int m_roomId;
        
        public CancelRoomAction(int roomId)
        {
            m_roomId = roomId;
        }

        public void Execute()
        {
            ProxyRoom room = ProxyRoomMgr.GetRoom(m_roomId);
            if (room != null)
            {
                bool result = ProxyRoomMgr.RemoveRoom(m_roomId);
                room.SendCancelPickUp(result);
            }
        }
    }
}
