using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;

namespace Game.Server.Rooms
{
    public class ExitWaitRoomAction : IAction
    {
        GamePlayer m_player;

        public ExitWaitRoomAction(GamePlayer player)
        {
            m_player = player;
        }

        public void Execute()
        {
            //if (m_player.CurrentRoom != null)
            //    m_player.CurrentRoom.RemovePlayer(m_player);

            BaseWaitingRoom room = RoomMgr.WaitingRoom;

            room.RemovePlayer(m_player);
            //if (room.AddPlayer(m_player))
            //{
            //    BaseRoom[] list = RoomMgr.Rooms;
            //    for (int i = 0; i < list.Length; i++)
            //    {
            //        if (!list[i].IsEmpty)
            //        {
            //            m_player.Out.SendUpdateRoomList(list[i]);
            //        }
            //    }

            //    GamePlayer[] players = room.GetPlayersSafe();
            //    foreach (GamePlayer p in players)
            //    {
            //        if (p != m_player)
            //        {
            //            m_player.Out.SendSceneAddPlayer(p);
            //        }
            //    }
            //}
        }

    }
}
