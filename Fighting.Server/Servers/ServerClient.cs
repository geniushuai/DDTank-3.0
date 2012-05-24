using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using System.Reflection;
using System.Security.Cryptography;
using Game.Base.Packets;
using Game.Logic;
using Fighting.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Fighting.Server.Rooms;
using Game.Logic.Protocol;
using Fighting.Server.Games;
using Game.Logic.Phy.Object;
using Bussiness;

namespace Fighting.Server
{
    public class ServerClient : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private RSACryptoServiceProvider m_rsa;

        private FightServer m_svr;

        private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

        protected override void OnConnect()
        {
            base.OnConnect();
            m_rsa = new RSACryptoServiceProvider();
            RSAParameters para = m_rsa.ExportParameters(false);
            SendRSAKey(para.Modulus, para.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();

            m_rsa = null;
        }

        #region 处理协议

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            switch (pkg.Code)
            {
                case (int)eFightPackageType.LOGIN:
                    HandleLogin(pkg);
                    break;
                case (int)eFightPackageType.ROOM_CREATE:
                    HandleGameRoomCreate(pkg);
                    break;
                case (int)eFightPackageType.ROOM_REMOVE:
                    HandleGameRoomCancel(pkg);
                    break;
                case (int)eFightPackageType.SEND_TO_GAME:
                    HanleSendToGame(pkg);
                    break;
                case (int)eFightPackageType.PLAYER_USE_PROP_INGAME:
                    HandlePlayerUsingProp(pkg);
                    break;
                case (int)eFightPackageType.DISCONNECT:
                    HandlePlayerExit(pkg);
                    break;
                case (int)eFightPackageType.CHAT:
                    HandlePlayerMessage(pkg);
                    break;
                case (int)eFightPackageType.SYS_NOTICE:
                    HandleSysNotice(pkg);
                    break;
                case (int)eFightPackageType.FIND_CONSORTIA_ALLY:
                    HandleConsortiaAlly(pkg);
                    break;
                default:
                    break;
            }
        }

        private void HandlePlayerUsingProp(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                game.Resume();
                if (pkg.ReadBoolean())
                {
                    Player player = game.FindPlayer(pkg.Parameter1);
                    ItemTemplateInfo template = ItemMgr.FindItemTemplate(pkg.Parameter2);
                    if (player != null && template != null)
                    {
                        player.UseItem(template);
                    }
                }
            }
        }


        private void HandlePlayerExit(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {

                Player player = game.FindPlayer(pkg.Parameter1);
                if (player != null)
                {
                    GSPacketIn pkg1 = new GSPacketIn((byte)eFightPackageType.DISCONNECT, player.PlayerDetail.PlayerCharacter.ID);
                    game.SendToAll(pkg1);
                    game.RemovePlayer(player.PlayerDetail, false);

                    ProxyRoom room = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Red.RoomId);
                    if (room != null)
                    {

                        if (!room.RemovePlayer(player.PlayerDetail))
                        {

                            ProxyRoom room1 = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Blue.RoomId);
                            if (room1 != null)
                            {
                                room1.RemovePlayer(player.PlayerDetail);
                            }
                        }
                    }
                }

            }

        }

        public void HandleConsortiaAlly(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                game.ConsortiaAlly = pkg.ReadInt();
                game.RichesRate = pkg.ReadInt();
            }
        }

        private void HandleSysNotice(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {

                Player player = game.FindPlayer(pkg.Parameter1);
                GSPacketIn pkg1 = new GSPacketIn((byte)eFightPackageType.SYS_NOTICE);
                // int type = pkg.ReadInt();
                pkg1.WriteInt(3);
                pkg1.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
                player.PlayerDetail.SendTCP(pkg1);
                pkg1.ClearContext();
                pkg1.WriteInt(3);
                pkg1.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", player.PlayerDetail.PlayerCharacter.NickName, player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
                game.SendToAll(pkg1, player.PlayerDetail);

            }

        }

        private void HandlePlayerMessage(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {

                Player player = game.FindPlayer(pkg.ReadInt());
                bool team = pkg.ReadBoolean();
                string msg = pkg.ReadString();
                if (player != null)
                {
                    GSPacketIn pkg1 = new GSPacketIn((byte)eFightPackageType.CHAT);
                    pkg1.ClientID = player.PlayerDetail.PlayerCharacter.ID;
                    pkg1.WriteByte(5);
                    pkg1.WriteBoolean(false);
                    pkg1.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
                    pkg1.WriteString(msg);
                    if (team)
                        game.SendToTeam(pkg,player.Team);
                    else
                        game.SendToAll(pkg1);
                }

            }

        }

        /// <summary>
        /// 战斗服务器登陆
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleLogin(GSPacketIn pkg)
        {
            byte[] rgb = pkg.ReadBytes();

            string[] temp = Encoding.UTF8.GetString(m_rsa.Decrypt(rgb, false)).Split(',');

            if (temp.Length == 2)
            {
                m_rsa = null;
                int id = int.Parse(temp[0]);
                //Info = ServerMgr.GetServerInfo(id);
                Strict = false;
            }
            else
            {
                log.ErrorFormat("Error Login Packet from {0}", TcpEndpoint);
                Disconnect();
            }
        }

        public void HandleGameRoomCreate(GSPacketIn pkg)
        {
            int roomId = pkg.ReadInt();
            int gameType = pkg.ReadInt();
            int guildId = pkg.ReadInt();

            int count = pkg.ReadInt();
            int totalLevel = 0;
            IGamePlayer[] players = new IGamePlayer[count];
            for (int i = 0; i < count; i++)
            {
                PlayerInfo info = new PlayerInfo();
                info.ID = pkg.ReadInt();
                info.NickName = pkg.ReadString();
                info.Sex = pkg.ReadBoolean();
                info.Hide = pkg.ReadInt();
                info.Style = pkg.ReadString();
                info.Colors = pkg.ReadString();
                info.Skin = pkg.ReadString();
                info.Offer = pkg.ReadInt();
                info.GP = pkg.ReadInt();
                info.Grade = pkg.ReadInt();
                info.Repute = pkg.ReadInt();
                info.ConsortiaID = pkg.ReadInt();
                info.ConsortiaName = pkg.ReadString();
                info.ConsortiaLevel = pkg.ReadInt();
                info.ConsortiaRepute = pkg.ReadInt();

                info.Attack = pkg.ReadInt();
                info.Defence = pkg.ReadInt();
                info.Agility = pkg.ReadInt();
                info.Luck = pkg.ReadInt();

                double baseAttack = pkg.ReadDouble();
                double baseDefence = pkg.ReadDouble();
                double baseAgility = pkg.ReadDouble();
                double baseBlood = pkg.ReadDouble();
                int templateId = pkg.ReadInt();
                bool canUserProp = pkg.ReadBoolean();
                int secondWeapon = pkg.ReadInt();
                int strengthLevel = pkg.ReadInt();


                double gprate = pkg.ReadDouble();
                double offerrate = pkg.ReadDouble();
                double rate = pkg.ReadDouble();
                int serverid = pkg.ReadInt();

                ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId);
                ItemInfo item = null;
                if (secondWeapon != 0)
                {
                    ItemTemplateInfo secondWeaponTemp = ItemMgr.FindItemTemplate(secondWeapon);
                    item = ItemInfo.CreateFromTemplate(secondWeaponTemp, 1, 1);
                    item.StrengthenLevel = strengthLevel;
                }

                List<BufferInfo> infos = new List<BufferInfo>();

                int buffercout = pkg.ReadInt();
                for (int j = 0; j < buffercout; j++)
                {
                    BufferInfo buffinfo = new BufferInfo();
                    buffinfo.Type = pkg.ReadInt();
                    buffinfo.IsExist = pkg.ReadBoolean();
                    buffinfo.BeginDate = pkg.ReadDateTime();
                    buffinfo.ValidDate = pkg.ReadInt();
                    buffinfo.Value = pkg.ReadInt();
                    if (info != null)
                        infos.Add(buffinfo);
                }

                players[i] = new ProxyPlayer(this, info, itemTemplate, item, baseAttack, baseDefence, baseAgility, baseBlood, gprate, offerrate, rate, infos, serverid);
                players[i].CanUseProp = canUserProp;

                int ec = pkg.ReadInt();
                for (int j = 0; j < ec; j++)
                {
                    players[i].EquipEffect.Add(pkg.ReadInt());
                }
                totalLevel += info.Grade;
            }

            ProxyRoom room = new ProxyRoom(ProxyRoomMgr.NextRoomId(), roomId, players, this);
            room.GuildId = guildId;
            room.GameType = (eGameType)gameType;

            lock (m_rooms)
            {
                if (!m_rooms.ContainsKey(roomId))
                {
                    m_rooms.Add(roomId, room);
                }
                else
                {
                    room = null;
                }
            }

            if (room != null)
            {
                ProxyRoomMgr.AddRoom(room);
            }
            else
            {
                log.ErrorFormat("Room already exists:{0}", roomId);
            }
        }

        public void HandleGameRoomCancel(GSPacketIn pkg)
        {
            ProxyRoom room = null;

            lock (m_rooms)
            {
                if (m_rooms.ContainsKey(pkg.Parameter1))
                {
                    room = m_rooms[pkg.Parameter1];
                }
            }

            if (room != null)
            {
                ProxyRoomMgr.RemoveRoom(room);
            }
        }

        public void HanleSendToGame(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                GSPacketIn inner = pkg.ReadPacket();
                game.ProcessData(inner);
            }
        }

        #endregion

        #region 发送协议

        /// <summary>
        /// 发送RSA密钥
        /// </summary>
        /// <param name="m"></param>
        /// <param name="e"></param>
        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.RSAKey);
            pkg.Write(m);
            pkg.Write(e);
            SendTCP(pkg);
        }

        public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
        {
            GSPacketIn p = new GSPacketIn((byte)eFightPackageType.SEND_TO_USER, playerId);
            p.WritePacket(pkg);
            SendTCP(p);
        }

        public void SendRemoveRoom(int roomId)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.ROOM_REMOVE, roomId);
            SendTCP(pkg);
        }

        public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
        {
            GSPacketIn p = new GSPacketIn((byte)eFightPackageType.SEND_TO_ROOM, roomId);
            if (except != null)
            {
                p.Parameter1 = except.PlayerCharacter.ID;
                p.Parameter2 = except.GamePlayerId;
            }
            else
            {
                p.Parameter1 = 0;
                p.Parameter2 = 0;
            }
            p.WritePacket(pkg);
            SendTCP(p);
        }

        public void SendStartGame(int roomId, AbstractGame game)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.ROOM_START_GAME);
            pkg.Parameter1 = roomId;
            pkg.Parameter2 = game.Id;
            pkg.WriteInt((int)game.RoomType);
            pkg.WriteInt((int)game.GameType);
            pkg.WriteInt(game.TimeType);
            SendTCP(pkg);
        }

        public void SendStopGame(int roomId, int gameId)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.ROOM_STOP_GAME);
            pkg.Parameter1 = roomId;
            pkg.Parameter2 = gameId;
            SendTCP(pkg);
        }

        public void SendGamePlayerId(IGamePlayer player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.SEND_GAME_PLAYER_ID);
            pkg.Parameter1 = player.PlayerCharacter.ID;
            pkg.Parameter2 = player.GamePlayerId;
            SendTCP(pkg);
        }

        public void SendDisconnectPlayer(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.DISCONNECT_PLAYER, playerId);
            SendTCP(pkg);
        }

        public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ON_GAME_OVER, playerId);
            pkg.Parameter1 = gameId;
            pkg.WriteBoolean(isWin);
            pkg.WriteInt(gainXp);
            SendTCP(pkg);
        }

        public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_USE_PROP_INGAME, playerId);
            pkg.Parameter1 = bag;
            pkg.Parameter2 = place;
            pkg.WriteInt(templateId);
            pkg.WriteBoolean(isLiving);
            SendTCP(pkg);
        }

        public void SendPlayerAddGold(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ADD_GOLD, playerId);
            pkg.Parameter1 = value;
            SendTCP(pkg);
        }
        public void SendPlayerAddMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ADD_MONEY, playerId);
            pkg.Parameter1 = value;
            SendTCP(pkg);
        }
        public void SendPlayerAddGiftToken(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ADD_GIFTTOKEN, playerId);
            pkg.Parameter1 = value;
            SendTCP(pkg);
        }
        public void SendPlayerAddGP(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ADD_GP, playerId);
            pkg.Parameter1 = value;
            SendTCP(pkg);
        }

        public void SendPlayerRemoveGP(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_REMOVE_GP, playerId);
            pkg.Parameter1 = value;
            SendTCP(pkg);
        }

        public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ONKILLING_LIVING, playerId);
            // pkg.WriteInt(game.Id);
            pkg.WriteInt(type);
            pkg.WriteBoolean(isLiving);
            pkg.WriteInt(demage);
            SendTCP(pkg);
        }

        public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ONMISSION_OVER, playerId);
            // pkg.WriteInt(game.Id);
            pkg.WriteBoolean(isWin);
            pkg.WriteInt(MissionID);
            pkg.WriteInt(turnNum);
            SendTCP(pkg);
        }

        public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_CONSORTIAFIGHT, playerId);
            pkg.WriteInt(consortiaWin);
            pkg.WriteInt(consortiaLose);
            pkg.WriteInt(players.Count);
            for (int i = 0; i < players.Count; i++)
            {
                pkg.WriteInt(players[i].PlayerDetail.PlayerCharacter.ID);
            }
            pkg.WriteByte((byte)roomType);
            pkg.WriteByte((byte)gameClass);
            pkg.WriteInt(totalKillHealth);
            SendTCP(pkg);


        }

        public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_SEND_CONSORTIAFIGHT, playerId);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(riches);
            pkg.WriteString(msg);
            SendTCP(pkg);
        }

        public void SendPlayerRemoveGold(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_REMOVE_GOLD, playerId);
            pkg.WriteInt(value);
            SendTCP(pkg);
        }


        public void SendPlayerRemoveMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_REMOVE_MONEY, playerId);
            pkg.WriteInt(value);
            SendTCP(pkg);
        }


        public void SendPlayerRemoveOffer(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_REMOVE_OFFER, playerId);
            pkg.WriteInt(value);
            SendTCP(pkg);
        }

        public void SendPlayerAddTemplate(int playerId, ItemInfo cloneItem, eBageType bagType, int count)
        {
            if (cloneItem != null)
            {
                GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ADD_TEMPLATE1, playerId);
                pkg.WriteInt(cloneItem.TemplateID);
                pkg.WriteByte((byte)bagType);
                pkg.WriteInt(count);
                pkg.WriteInt(cloneItem.ValidDate);
                pkg.WriteBoolean(cloneItem.IsBinds);
                pkg.WriteBoolean(cloneItem.IsUsed);
                SendTCP(pkg);
            }

        }

        public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.FIND_CONSORTIA_ALLY);
            pkg.WriteInt(Consortia1);
            pkg.WriteInt(Consortia2);
            pkg.WriteInt(GameId);
            SendTCP(pkg);
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServerClient(FightServer svr)
            : base(new byte[2048], new byte[2048])
        {
            m_svr = svr;
        }

        public override string ToString()
        {
            return string.Format("Server Client: {0} IsConnected:{1}  RoomCount:{2}", 0, IsConnected, m_rooms.Count);
        }

        public void RemoveRoom(int orientId, ProxyRoom room)
        {
            bool result = false;
            lock (m_rooms)
            {
                if (m_rooms.ContainsKey(orientId) && m_rooms[orientId] == room)
                {
                    result = m_rooms.Remove(orientId);
                }
            }

            if (result)
            {
                SendRemoveRoom(orientId);
            }
        }

    }
}
