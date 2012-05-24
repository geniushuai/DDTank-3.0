using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Games;
using Game.Server.Battle;
using Game.Server.Packets;

namespace Game.Server.Rooms
{
    public class StartGameAction : IAction
    {
        BaseRoom m_room;

        public StartGameAction(BaseRoom room)
        {
            m_room = room;
        }

        public void Execute()
        {
            if (m_room.CanStart())
            {
                List<GamePlayer> players = m_room.GetPlayers();
                if (m_room.RoomType == eRoomType.Freedom)
                {
                    List<IGamePlayer> red = new List<IGamePlayer>();
                    List<IGamePlayer> blue = new List<IGamePlayer>();
                    foreach (GamePlayer p in players)
                    {
                        if (p != null)
                        {
                            if (p.CurrentRoomTeam == 1)
                            {
                                red.Add(p);
                            }
                            else
                            {
                                blue.Add(p);
                            }
                        }
                    }

                    BaseGame game = GameMgr.StartPVPGame(m_room.RoomId, red, blue, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode);
                    StartGame(game);
                }
                else if (m_room.RoomType == eRoomType.Exploration || m_room.RoomType == eRoomType.Boss || m_room.RoomType == eRoomType.Treasure)
                {
                    List<IGamePlayer> matchPlayers = new List<IGamePlayer>();
                    foreach (GamePlayer p in players)
                    {
                        if (p != null)
                        {
                            matchPlayers.Add(p);
                        }
                    }
                    //更新房间的时间类型
                    UpdatePveRoomTimeMode();
                    BaseGame game = GameMgr.StartPVEGame(m_room.RoomId, matchPlayers, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode, m_room.HardLevel, m_room.LevelLimits);
                    StartGame(game);
                }
                else if (m_room.RoomType == eRoomType.Match)
                {
                    m_room.UpdateAvgLevel();
                   // m_room.GameType = eGameType.Guild;

                    BattleServer server = BattleMgr.AddRoom(m_room);
                    if (server != null)
                    {

                        m_room.BattleServer = server;
                        m_room.IsPlaying = true;
                        m_room.SendStartPickUp();
                    }
                    else
                    {
                        GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, "没有可用的战场服务器!");
                        m_room.SendToAll(pkg, m_room.Host);
                        m_room.SendCancelPickUp();
                    }
                }

                RoomMgr.WaitingRoom.SendUpdateRoom(m_room);
            }
        }

        private void StartGame(BaseGame game)
        {
            if (game != null)
            {
                m_room.IsPlaying = true;
                m_room.StartGame(game);
            }
            else
            {
                m_room.IsPlaying = false;
                m_room.SendPlayerState();
            }
        }

        private void UpdatePveRoomTimeMode()
        {
            if (m_room.RoomType == eRoomType.Exploration || m_room.RoomType == eRoomType.Boss || m_room.RoomType == eRoomType.Treasure)
            {
                switch (m_room.HardLevel)
                {
                    case eHardLevel.Simple:
                        m_room.TimeMode = 3;
                        break;
                    case eHardLevel.Normal:
                        m_room.TimeMode = 2;
                        break;
                    case eHardLevel.Hard:
                        m_room.TimeMode = 1;
                        break;
                    case eHardLevel.Terror:
                        m_room.TimeMode = 1;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
