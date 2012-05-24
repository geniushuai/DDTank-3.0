using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Server.Rooms
{
    class EnterRoomAction : IAction
    {
        private GamePlayer m_player;

        private int m_roomId;

        private String m_pwd;

        private int m_type;

        public EnterRoomAction(GamePlayer player, int roomId, string pwd, int type)
        {
            m_player = player;

            m_roomId = roomId;

            m_pwd = pwd;

            m_type = type;
        }

        public void Execute()
        {
            if (m_player.IsActive == false)
                return;

            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
            }

            BaseRoom[] rooms = RoomMgr.Rooms;
            BaseRoom rm = null;
            if (m_roomId == -1)
            {
                rm = FindRandomRoom(rooms);
                if (rm == null)
                {
                    m_player.Out.SendMessage(eMessageType.ERROR, "暂时没有合适的房间,请稍后重试!");
                    m_player.Out.SendRoomLoginResult(false);
                    return;
                }
            }
            else
            {
                rm = rooms[m_roomId - 1];
            }
            if (!rm.IsUsing)
            {
                m_player.Out.SendMessage(eMessageType.Normal, "房间不存在!");
                return;
            }

            if (rm.PlayerCount == rm.PlacesCount)
            {
                m_player.Out.SendMessage(eMessageType.ERROR, "房间人数已满！");
            }
            else
            {
                if (rm.NeedPassword == false || rm.Password == m_pwd)
                {
                    if (rm.Game == null || rm.Game.CanAddPlayer())
                    {
                        if (rm.RoomType == eRoomType.Exploration && rm.LevelLimits > (int)rm.GetLevelLimit(m_player))
                        {
                            m_player.Out.SendMessage(eMessageType.ERROR, "您的等级和该房间要求等级不符！");
                            return;
                        }

                        RoomMgr.WaitingRoom.RemovePlayer(m_player);
                        m_player.Out.SendRoomLoginResult(true);
                        m_player.Out.SendRoomCreate(rm);

                        if (rm.AddPlayerUnsafe(m_player))
                        {
                            if (rm.Game != null)
                            {
                                rm.Game.AddPlayer(m_player);
                            }
                        }

                        RoomMgr.WaitingRoom.SendUpdateRoom(rm);
                        m_player.Out.SendRoomChange(rm);
                    }
                }
                else
                {
                    m_player.Out.SendMessage(eMessageType.ERROR, "房间密码错误");
                    m_player.Out.SendRoomLoginResult(false);
                }
            }
        }

        private BaseRoom FindRandomRoom(BaseRoom[] rooms)
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].PlayerCount > 0 && rooms[i].CanAddPlayer() && rooms[i].NeedPassword == false && !rooms[i].IsPlaying)
                {
                    //探险时，要判断用户的等级，才能进入到相应的房间
                    if ((int)eRoomType.Exploration != m_type)
                    {
                        if ((int)(rooms[i].RoomType) == m_type)
                            return rooms[i];
                    }
                    else
                    {
                        if ((int)(rooms[i].RoomType) == m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(m_player))
                            return rooms[i];
                    }
                }
            }
            return null;
        }


    }
}
