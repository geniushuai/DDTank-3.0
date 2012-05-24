using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic;
using Fighting.Server.Rooms;
using Game.Logic.Phy.Maps;
using Game.Logic.Protocol;

namespace Fighting.Server.Games
{
    public class BattleGame : PVPGame
    {
        private ProxyRoom m_roomRed;

        private ProxyRoom m_roomBlue;

        public ProxyRoom Red
        {
            get { return m_roomRed; }
        }

        public ProxyRoom Blue
        {
            get { return m_roomBlue; }
        }

        public BattleGame(int id, List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, Map map, eRoomType roomType, eGameType gameType, int timeType)
            : base(id, roomBlue.RoomId, red, blue, map, roomType, gameType, timeType)
        {
            m_roomRed = roomRed;
            m_roomBlue = roomBlue;
        }

        public override void SendToAll(Game.Base.Packets.GSPacketIn pkg, IGamePlayer except)
        {
            if (m_roomRed != null)
            {
                m_roomRed.SendToAll(pkg, except);
            }
            if (m_roomBlue != null)
            {
                m_roomBlue.SendToAll(pkg, except);
            }
        }

        public override void SendToTeam(Game.Base.Packets.GSPacketIn pkg, int team, IGamePlayer except)
        {
            if (team == 1)
            {
                m_roomRed.SendToAll(pkg, except);
            }
            else
            {
                m_roomBlue.SendToAll(pkg, except);
            }
        }

        public override string ToString()
        {
            return new StringBuilder(base.ToString()).Append(",class=BattleGame").ToString();
        }
    }
}
