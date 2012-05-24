using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Server.Packets;

namespace Game.Server.Rooms
{
    public class BaseWaitingRoom
    {
        private Dictionary<int,GamePlayer> m_list;

        public BaseWaitingRoom()
        {
            m_list = new Dictionary<int, GamePlayer>();
        }


        public bool AddPlayer(GamePlayer player)
        {
            bool result = false;
            lock (m_list)
            {
                if (!m_list.ContainsKey(player.PlayerId))
                {
                    m_list.Add(player.PlayerId, player);
                    result = true;
                }
            }
            if (result)
            {
                GSPacketIn pkg = player.Out.SendSceneAddPlayer(player);
                SendToALL(pkg, player);
            }

            return result;
        }

        public bool RemovePlayer(GamePlayer player)
        {
            bool result = false;
            lock (m_list)
            {
                result = m_list.Remove(player.PlayerId);
            }

            if (result)
            {
                GSPacketIn pkg = player.Out.SendSceneRemovePlayer(player);
                SendToALL(pkg, player);
            }

            return true;
        }

        public void SendUpdateRoom(BaseRoom room)
        {
            GamePlayer[] players = GetPlayersSafe();
            GSPacketIn pkg = null;

            foreach (GamePlayer p in players)
            {
                if (pkg == null)
                {
                    pkg = p.Out.SendUpdateRoomList(room);
                }
                else
                {
                    p.Out.SendTCP(pkg);
                }
            }
        }

        public void SendToALL(GSPacketIn packet)
        {
            SendToALL(packet, null);
        }

        public void SendToALL(GSPacketIn packet, GamePlayer except)
        {
            GamePlayer[] temp = null;

            lock (m_list)
            {
                temp = new GamePlayer[m_list.Count];
                m_list.Values.CopyTo(temp, 0);
            }

            if (temp != null)
            {
                foreach (GamePlayer p in temp)
                {
                    if (p != null && p != except)
                    {
                        p.Out.SendTCP(packet);
                    }
                }
            }
        }

        public GamePlayer[] GetPlayersSafe()
        {
            GamePlayer[] temp = null;

            lock (m_list)
            {
                temp = new GamePlayer[m_list.Count];
                m_list.Values.CopyTo(temp, 0);
            }

            return temp == null ? new GamePlayer[0] : temp;
        }
    }
}
