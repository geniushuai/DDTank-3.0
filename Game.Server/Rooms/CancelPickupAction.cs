using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Server.Battle;

namespace Game.Server.Rooms
{
    public class CancelPickupAction:IAction
    {
        private BattleServer m_server;

        private BaseRoom m_room;


        public CancelPickupAction(BattleServer server,BaseRoom room)
        {
            m_room = room;
            m_server = server;
        }

        public void Execute()
        {
            if(m_room.Game == null && m_server != null)
            {
                m_room.BattleServer = null;
                m_room.IsPlaying = false;
                m_room.SendCancelPickUp();
                RoomMgr.WaitingRoom.SendUpdateRoom(m_room);
            }
        }
    }
}
