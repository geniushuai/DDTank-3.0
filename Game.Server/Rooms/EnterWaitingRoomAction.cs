using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;

namespace Game.Server.Rooms
{
    public class EnterWaitingRoomAction:IAction
    {
        GamePlayer m_player;

        public EnterWaitingRoomAction(GamePlayer player)
        {
            m_player = player;
        }

        public void Execute()
        {
            if (m_player.CurrentRoom != null)
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);

            BaseWaitingRoom room = RoomMgr.WaitingRoom;
            if (room.AddPlayer(m_player))
            {
                BaseRoom[] list = RoomMgr.Rooms;
                List<BaseRoom> tempList = new List<BaseRoom>();
                //int maxRoomInList = 7;
                for (int i = 0; i < list.Length; i++)
                {
                    if (!list[i].IsEmpty)
                    {
                        tempList.Add(list[i]);
                        //m_player.Out.SendUpdateRoomList(list[i]);
                    }
                }
                m_player.Out.SendUpdateRoomList(tempList);

                GamePlayer[] players = room.GetPlayersSafe();
                foreach (GamePlayer p in players)
                {
                    if (p != m_player)
                    {
                        m_player.Out.SendSceneAddPlayer(p);
                    }
                }
            }
           
        }
    }
}
