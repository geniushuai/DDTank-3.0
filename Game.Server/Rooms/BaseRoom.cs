using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Packets;
using Bussiness;
using Game.Logic.Protocol;
using Game.Server.Battle;
using Game.Logic.Phy.Object;
using Game.Server.Statics;

namespace Game.Server.Rooms
{
    public class BaseRoom
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private GamePlayer[] m_places;

        private int[] m_placesState; // 0 关闭 -1 无人 playerId 有人

        private byte[] m_playerState; // 0 未准备 1 准备 2 房主

        private int m_playerCount = 0;

        private int m_placesCount = 8;

        private bool m_isUsing = false;

        private GamePlayer m_host;

        public bool IsPlaying;

        public int RoomId;

        public int GameStyle = 0;

        public string Name;

        public string Password;

        public eRoomType RoomType;

        public eGameType GameType;

        public eHardLevel HardLevel;

        //探险模式的级别段
        public int LevelLimits;

        public byte TimeMode;

        public int MapId;

        public string m_roundName;

        public GamePlayer Host
        {
            get { return m_host; }
        }


        public byte[] PlayerState
        {
            get { return m_playerState; }
            set { m_playerState = value; }
        }
        public int PlayerCount
        {
            get { return m_playerCount; }
        }

        public int PlacesCount
        {
            get { return m_placesCount; }
        }

        public int GuildId
        {
            get { return m_host.PlayerCharacter.ConsortiaID; }
        }

        public bool IsUsing
        {
            get { return m_isUsing; }
        }

        public string RoundName
        {
            get { return m_roundName; }
            set { m_roundName = value; }
        }

        public BaseRoom(int roomId)
        {
            RoomId = roomId;
            m_places = new GamePlayer[8];
            m_placesState = new int[8];
            m_playerState = new byte[8];

            Reset();
        }

        public void Start()
        {
            if (m_isUsing == false)
            {
                m_isUsing = true;
                Reset();
            }
        }

        public void Stop()
        {
            if (m_isUsing)
            {
                m_isUsing = false;
                if (m_game != null)
                {
                    m_game.GameStopped -= m_game_GameStopped;
                    m_game = null;
                    IsPlaying = false;
                }
                RoomMgr.WaitingRoom.SendUpdateRoom(this);
            }
        }

        private void Reset()
        {
            for (int i = 0; i < 8; i++)
            {
                m_places[i] = null;
                m_placesState[i] = -1;
                m_playerState[i] = 0;
            }

            m_host = null;
            IsPlaying = false;
            m_placesCount = 8;
            m_playerCount = 0;
            HardLevel = eHardLevel.Simple;

        }

        public bool CanStart()
        {
            if (RoomType == eRoomType.Freedom)
            {
                int red = 0, blue = 0;
                for (int i = 0; i < 8; i++)
                {
                    if ((i % 2) == 0)
                    {
                        if (m_playerState[i] > 0)
                            red++;
                    }
                    else
                    {
                        if (m_playerState[i] > 0)
                            blue++;
                    }
                }
                return red == blue;
            }
            else
            {
                int ready = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (m_playerState[i] > 0)
                    {
                        ready++;
                    }
                }
                return ready == m_playerCount;
            }
        }

        public bool NeedPassword
        {
            get { return !String.IsNullOrEmpty(Password); }

        }

        public bool CanAddPlayer()
        {
            return m_playerCount < m_placesCount;
        }

        public bool IsEmpty
        {
            get { return m_playerCount == 0; }
        }

        public List<GamePlayer> GetPlayers()
        {
            List<GamePlayer> temp = new List<GamePlayer>();
            lock (m_places)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (m_places[i] != null)
                    {
                        temp.Add(m_places[i]);
                    }
                }
            }
            return temp;
        }

        public void SetHost(GamePlayer player)
        {
            if (m_host == player) return;
            if (m_host != null)
            {
                //清除老的房主的ready状态
                UpdatePlayerState(player, 0, false);
            }
            m_host = player;
            UpdatePlayerState(player, 2, true);
        }

        public void UpdateRoom(string name, string pwd, eRoomType roomType, byte timeMode, int mapId)
        {
            Name = name;
            Password = pwd;
            RoomType = roomType;
            TimeMode = timeMode;
            MapId = mapId;

            UpdateRoomGameType();
            if (roomType == eRoomType.Freedom)
            {
                m_placesCount = 8;
            }
            else
            {
                m_placesCount = 4;
            }
        }

        public void UpdateRoomGameType()
        {
            switch (RoomType)
            {
                case eRoomType.Boss:
                    GameType = eGameType.Boss;
                    break;
                case eRoomType.Exploration:
                    GameType = eGameType.Exploration;
                    break;
                case eRoomType.Treasure:
                    GameType = eGameType.Treasure;
                    break;
                case eRoomType.Freedom:
                    GameType = eGameType.Free;
                    break;
                case eRoomType.Match:
                    GameType = eGameType.Free;
                    break;
                default:
                    GameType = eGameType.ALL;
                    break;
            }
        }

        public void UpdatePlayerState(GamePlayer player, byte state, bool sendToClient)
        {
            m_playerState[player.CurrentRoomIndex] = state;

            if (sendToClient)
                SendPlayerState();
        }

        private int m_avgLevel = 0;

        public int AvgLevel
        {
            get { return m_avgLevel; }
        }

        public void UpdateAvgLevel()
        {
            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                if (m_places[i] != null)
                    sum += m_places[i].PlayerCharacter.Grade;
            }

            m_avgLevel = sum / m_playerCount;
        }


        #region SendToAll/SendToTeam/SendToHost

        public void SendToAll(GSPacketIn pkg)
        {
            SendToAll(pkg, null);
        }

        public void SendToAll(GSPacketIn pkg, GamePlayer except)
        {
            GamePlayer[] temp = null;

            lock (m_places)
            {
                temp = (GamePlayer[])m_places.Clone();
            }

            if (temp != null)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] != null && temp[i] != except)
                    {
                        temp[i].Out.SendTCP(pkg);
                    }
                }
            }
        }

        public void SendToTeam(GSPacketIn pkg, int team)
        {
            SendToTeam(pkg, team, null);
        }

        public void SendToTeam(GSPacketIn pkg, int team, GamePlayer except)
        {
            GamePlayer[] temp = null;

            lock (m_places)
            {
                temp = (GamePlayer[])m_places.Clone();
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != null && temp[i].CurrentRoomTeam == team && temp[i] != except)
                {
                    temp[i].Out.SendTCP(pkg);
                }
            }
        }

        public void SendToHost(GSPacketIn pkg)
        {
            GamePlayer[] temp = null;

            lock (m_places)
            {
                temp = (GamePlayer[])m_places.Clone();
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != null && temp[i] == Host)
                {
                    temp[i].Out.SendTCP(pkg);
                }
            }
        }

        #endregion

        #region Protocal

        public void SendPlayerState()
        {
            GSPacketIn pkg = m_host.Out.SendRoomUpdatePlayerStates(m_playerState);
            SendToAll(pkg, m_host);
        }

        public void SendPlaceState()
        {
            if (m_host != null)
            {
                GSPacketIn pkg = m_host.Out.SendRoomUpdatePlacesStates(m_placesState);
                SendToAll(pkg, m_host);
            }
        }

        public void SendCancelPickUp()
        {
            if (m_host != null)
            {
                GSPacketIn pkg = m_host.Out.SendRoomPairUpCancel(this);
                SendToAll(pkg, m_host);
            }
        }

        public void SendStartPickUp()
        {
            if (m_host != null)
            {
                GSPacketIn pkg = m_host.Out.SendRoomPairUpStart(this);
                SendToAll(pkg, m_host);
            }
        }

        public void SendMessage(eMessageType type, string msg)
        {
            if (m_host != null)
            {
                GSPacketIn pkg = m_host.Out.SendMessage(type, msg);
                SendToAll(pkg, m_host);
            }
        }

        #endregion

        /// <summary>
        /// 请不要直接调用,使用RoomMgr.UpdatePos
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="isOpened"></param>
        public bool UpdatePosUnsafe(int pos, bool isOpened)
        {
            if (pos < 0 || pos > 7) return false;
            int temp = isOpened ? -1 : 0;
            if (m_placesState[pos] != temp)
            {
                if (m_places[pos] != null)
                    RemovePlayerUnsafe(m_places[pos]);
                m_placesState[pos] = temp;
                SendPlaceState();
                if (isOpened)
                {
                    m_placesCount++;
                }
                else
                {
                    m_placesCount--;
                }
                return true;
            }
            return false;
        }

        public bool IsAllSameGuild()
        {
            int guildId = GuildId;
            if (guildId != 0)
            {
                List<GamePlayer> list = GetPlayers();
                if (list.Count >= 2)
                {
                    foreach (GamePlayer p in list)
                    {
                        if (p.PlayerCharacter.ConsortiaID != guildId)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;

            }
            else
            {
                return false;
            }
        }

        public void UpdateGameStyle()
        {
            if (m_host != null && RoomType == eRoomType.Match)
            {
                if (IsAllSameGuild())
                {
                    GameStyle = 1;
                    GameType = eGameType.Guild;
                }
                else
                {
                    GameStyle = 0;
                    GameType = eGameType.Free;
                }
                GSPacketIn pkg = m_host.Out.SendRoomType(m_host, this);
                SendToAll(pkg);
            }
        }

        /// <summary>
        /// 请不要直接调用,使用RoomMgr.EnterRoom
        /// </summary>
        /// <param name="player"></param>
        public bool AddPlayerUnsafe(GamePlayer player)
        {
            int index = -1;
            lock (m_places)
            {

                for (int i = 0; i < 8; i++)
                {
                    if (m_places[i] == null && m_placesState[i] == -1)
                    {
                        m_places[i] = player;
                        m_placesState[i] = player.PlayerId;
                        m_playerCount++;
                        index = i;
                        break;
                    }
                }
            }

            if (index != -1)
            {
                player.CurrentRoom = this;
                player.CurrentRoomIndex = index;
                if (RoomType == eRoomType.Freedom)
                {
                    player.CurrentRoomTeam = (index % 2) + 1;
                }
                else
                {
                    player.CurrentRoomTeam = 1;
                }

                GSPacketIn pkg = player.Out.SendRoomPlayerAdd(player);
                SendToAll(pkg, player);

                //把自己的buffer发送给房间中的所有人
                GSPacketIn bufferPkg = player.Out.SendBufferList(player, player.BufferList.GetAllBuffer());
                SendToAll(bufferPkg, player);

                List<GamePlayer> list = GetPlayers();
                foreach (GamePlayer p in list)
                {
                    if (p != player)
                    {
                        player.Out.SendRoomPlayerAdd(p);
                        //把房间中人的buffer发送给新加入的成员
                        player.Out.SendBufferList(p, p.BufferList.GetAllBuffer());
                    }
                }

                if (m_host == null)
                {
                    m_host = player;
                    UpdatePlayerState(player, 2, true);
                }
                else
                {
                    UpdatePlayerState(player, 0, true);

                }

                SendPlaceState();
                UpdateGameStyle();
            }

            return index != -1;
        }

        public bool RemovePlayerUnsafe(GamePlayer player)
        {
            return RemovePlayerUnsafe(player, false);
        }
        /// <summary>
        /// 请不要直接调用,使用RoomMgr.ExitRoom
        /// </summary>
        /// <param name="player"></param>
        public bool RemovePlayerUnsafe(GamePlayer player, bool isKick)
        {
            int index = -1;
            lock (m_places)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (m_places[i] == player)
                    {
                        m_places[i] = null;
                        m_playerState[i] = 0;
                        m_placesState[i] = -1;
                        m_playerCount--;
                        index = i;
                        break;
                    }
                }
            }

            if (index != -1)
            {
                UpdatePosUnsafe(index, true);
                player.CurrentRoom = null;
                player.TempBag.ClearBag();
                GSPacketIn pkg = player.Out.SendRoomPlayerRemove(player);
                SendToAll(pkg);

                //发送踢人信息
                if (isKick)
                {
                    player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));
                }

                //从新挑选房主
                bool isChangeHost = false;
                if (m_host == player)
                {
                    if (m_playerCount > 0)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (m_places[i] != null)
                            {
                                SetHost(m_places[i]);
                                isChangeHost = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_host = null;
                    }
                }

                //游戏中移除人物
                if (IsPlaying)
                {
                    if (m_game != null)
                    {
                        //如果关卡中，房主退出，重置新房主的ready状态
                        if (isChangeHost && m_game is PVEGame)
                        {
                            PVEGame pveGame = m_game as PVEGame;
                            foreach (Player p in pveGame.Players.Values)
                            {
                                if (p.PlayerDetail == m_host)
                                {
                                    p.Ready = false;
                                }
                            }
                        }
                        m_game.RemovePlayer(player, isKick);
                    }
                    if (BattleServer != null)
                    {
                        if (m_game != null)
                        {
                            BattleServer.Server.SendPlayerDisconnet(Game.Id, player.GamePlayerId, RoomId);
                            if (PlayerCount == 0)
                                BattleServer.RemoveRoom(this);
                        }
                        else
                        {
                            SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed"));
                            RoomMgr.AddAction(new CancelPickupAction(BattleServer, this));
                            BattleServer.RemoveRoom(this);
                            IsPlaying = false;
                        }
                    }
                }
                else
                {
                    UpdateGameStyle();
                    if (isChangeHost)
                    {
                        if (RoomType == eRoomType.Exploration)
                        {
                            HardLevel = eHardLevel.Normal;
                        }
                        else
                        {
                            HardLevel = eHardLevel.Simple;
                        }
                        foreach (GamePlayer gp in GetPlayers())
                        {
                            gp.Out.SendRoomChange(this);
                        }
                    }
                }
            }
            return index != -1;
        }

        /// <summary>
        /// 请不要直接调用,使用RoomMgr.KickPlayer
        /// </summary>
        /// <param name="pos"></param>
        public void RemovePlayerAtUnsafe(int pos)
        {
            if (pos < 0 || pos > 7) return;
            if (m_places[pos].KickProtect)
            {
                string message = LanguageMgr.GetTranslation("Game.Server.SceneGames.Protect", m_places[pos].PlayerCharacter.NickName);
                GSPacketIn pkg = new GSPacketIn((byte)0x03);
                pkg.WriteInt(0);
                pkg.WriteString(message);

                SendToHost(pkg);
                return;
            }
            if (m_places[pos] != null)
                RemovePlayerUnsafe(m_places[pos], true);
        }

        /// <summary>
        /// 请不要直接调用,使用RoomMgr.SwitchTeam
        /// </summary>
        /// <param name="m_player"></param>
        public bool SwitchTeamUnsafe(GamePlayer m_player)
        {
            //由于改变对外会影响AddPlayer,所以放在RoomMgr中统一调度
            if (RoomType == eRoomType.Match)
                return false;
            int index = -1;
            lock (m_places)
            {
                for (int i = (m_player.CurrentRoomIndex + 1) % 2; i < 8; i += 2)
                {
                    if (m_places[i] == null && m_placesState[i] == -1)
                    {
                        index = i;

                        m_places[m_player.CurrentRoomIndex] = null;
                        m_places[i] = m_player;

                        m_placesState[m_player.CurrentRoomIndex] = -1;
                        m_placesState[i] = m_player.PlayerId;

                        m_playerState[i] = m_playerState[m_player.CurrentRoomIndex];
                        m_playerState[m_player.CurrentRoomIndex] = 0;
                        break;
                    }
                }
            }

            if (index != -1)
            {
                m_player.CurrentRoomIndex = index;
                m_player.CurrentRoomTeam = (index % 2) + 1;
                GSPacketIn pkg = m_player.Out.SendRoomPlayerChangedTeam(m_player);
                SendToAll(pkg, m_player);
                SendPlaceState();
                return true;
            }

            return false;
        }

        public eLevelLimits GetLevelLimit(GamePlayer player)
        {
            if (player.PlayerCharacter.Grade <= 10)
            {
                return eLevelLimits.ZeroToTen;
            }
            else if (player.PlayerCharacter.Grade <= 20)
            {
                return eLevelLimits.ElevenToTwenty;
            }
            else
            {
                return eLevelLimits.TwentyOneToThirty;
            }
        }


        #region StartGame/ProcessData/BattleServer

        private AbstractGame m_game;

        public AbstractGame Game
        {
            get { return m_game; }
        }

        public void StartGame(AbstractGame game)
        {
            if (m_game != null)
            {
                List<GamePlayer> list = GetPlayers();
                foreach (GamePlayer player in list)
                {
                    m_game.RemovePlayer(player, false);
                }
                m_game_GameStopped(m_game);
                //log.Error("Old game didn't remove?");
            }
            m_game = game;
            IsPlaying = true;
            m_game.GameStopped += new GameEventHandle(m_game_GameStopped);
        }

        private void m_game_GameStopped(AbstractGame game)
        {
            if (game != null)
            {
                m_game.GameStopped -= m_game_GameStopped;
                m_game = null;
                IsPlaying = false;
                RoomMgr.WaitingRoom.SendUpdateRoom(this);
            }
        }

        public void ProcessData(GSPacketIn packet)
        {
            if (m_game != null)
            {
                m_game.ProcessData(packet);
            }
        }

        public BattleServer BattleServer;

        #endregion

        public override string ToString()
        {
            return string.Format("Id:{0},player:{1},game:{2},isPlaying:{3}", RoomId, PlayerCount, Game, IsPlaying);
        }
    }
}
