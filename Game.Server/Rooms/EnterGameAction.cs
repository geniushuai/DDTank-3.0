using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Packets;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Rooms
{
    class EnterGameAction : IAction
    {
        private GamePlayer m_player;

        private int m_roomId;

        private String m_pwd;

        private int m_type;

        public EnterGameAction(GamePlayer player, int roomId, string pwd, int type)
        {
            m_player = player;

            m_roomId = roomId;

            m_pwd = pwd;

            m_type = type;
        }

        public void Execute()
        {
            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayer(m_player);
            }

            BaseRoom[] rooms = RoomMgr.Rooms;
            BaseRoom rm = null;

            if (m_roomId == -1)
            {
                return;
            }
            else
            {
                rm = rooms[m_roomId - 1];
            }

            if (rm.NeedPassword == false || rm.Password == m_pwd)
            {
                RoomMgr.WaitingRoom.RemovePlayer(m_player);

                m_player.Out.SendRoomLoginResult(true);
                m_player.Out.SendRoomCreate(rm);

                if (rm.Game != null && rm.Game is PVEGame)
                {
                    PVEGame pve = rm.Game as PVEGame;
                    Player fp = new Player(m_player, pve.PhysicalId++, pve, 1);
                    pve.AddPlayer(m_player, fp);
       
                }

                rm.AddPlayer(m_player);

                RoomMgr.WaitingRoom.SendUpdateRoom(rm);
            }
            else
            {
                m_player.Out.SendMessage(eMessageType.ERROR, "房间密码错误");
                m_player.Out.SendRoomLoginResult(false);
            }
        }

        

    }
}
