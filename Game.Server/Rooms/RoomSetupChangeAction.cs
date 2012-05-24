using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic;
using Game.Base.Packets;

namespace Game.Server.Rooms
{
    class RoomSetupChangeAction : IAction
    {
        private BaseRoom m_room;

        private eRoomType m_roomType;

        private byte m_timeMode;

        private eHardLevel m_hardLevel;

        private int m_mapId;

        private int m_levelLimits;

        private GSPacketIn m_packet;

        public RoomSetupChangeAction(BaseRoom room, GSPacketIn packet, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId)
        {
            m_room = room;
            m_roomType = roomType;
            m_timeMode = timeMode;
            m_hardLevel = hardLevel;
            m_levelLimits = levelLimits;
            m_mapId = mapId;
            m_packet = packet;
        }

        public void Execute()
        {
            m_room.RoomType = m_roomType;
            m_room.TimeMode = m_timeMode;
            m_room.HardLevel = m_hardLevel;
            m_room.LevelLimits = m_levelLimits;
            m_room.MapId = m_mapId;
            m_room.UpdateRoomGameType();
            m_room.SendToAll(m_packet);
            RoomMgr.WaitingRoom.SendUpdateRoom(m_room);
        }
    }
}
