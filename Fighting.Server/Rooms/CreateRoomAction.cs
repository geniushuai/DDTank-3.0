using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic;

namespace Fighting.Server.Rooms
{
    public class CreateRoomAction :IAction
    {
        private int m_orientRoomId;
        private IGamePlayer[] m_players;
        private ServerClient m_client;

        public CreateRoomAction(IGamePlayer[] players,int oriendRoomId,ServerClient client)
        {
            m_players = players;
            m_orientRoomId = oriendRoomId;
            m_client = client;
        }

        public void Execute()
        {
            ProxyRoom room = new ProxyRoom(ProxyRoomMgr.NextRoomIdUnsafe(),m_orientRoomId, m_players, m_client);
            ProxyRoomMgr.AddRoomUnsafe(room);
        }

    }
}
