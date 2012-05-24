using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Logic;

namespace Game.Server.Rooms
{
    public class CreateRoomAction : IAction
    {
        private GamePlayer m_player;

        private string m_name;

        private string m_password;

        private eRoomType m_roomType;

        private byte m_timeType;

        public CreateRoomAction(GamePlayer player, String name, String password, eRoomType roomType, byte timeType)
        {
            m_player = player;
            m_name = name;
            m_password = password;
            m_roomType = roomType;
            m_timeType = timeType;
        }

        public void Execute()
        {
            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
            }
            if (m_player.IsActive == false)
                return;
            BaseRoom[] rooms = RoomMgr.Rooms;

            BaseRoom room = null;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].IsUsing == false)
                {
                    room = rooms[i];
                    break;
                }
            }

            if (room != null)
            {
                RoomMgr.WaitingRoom.RemovePlayer(m_player);


                room.Start();
                //探险默认使用普通难度等级
                if (m_roomType == eRoomType.Exploration)
                {
                    room.HardLevel = eHardLevel.Normal;
                    room.LevelLimits = (int)room.GetLevelLimit(m_player);
                }

                room.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
               

                GSPacketIn pkg = m_player.Out.SendRoomCreate(room);
                room.AddPlayerUnsafe(m_player);
                RoomMgr.WaitingRoom.SendUpdateRoom(room);
            }
        }
    }
}
