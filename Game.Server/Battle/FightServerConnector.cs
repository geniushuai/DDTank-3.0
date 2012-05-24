using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System.Security.Cryptography;
using Game.Server.Managers;
using System.Threading;
using Game.Logic.Protocol;
using Game.Server.Rooms;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Buffer;
using Game.Server.Statics;

namespace Game.Server.Battle
{
    public class FightServerConnector : BaseConnector
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BattleServer m_server;

        private string m_key;

        #region Override Methods

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(AsynProcessPacket), pkg);
        }

        protected void AsynProcessPacket(object state)
        {
            try
            {
                GSPacketIn pkg = state as GSPacketIn;
                switch (pkg.Code)
                {
                    case (int)eFightPackageType.RSAKey:
                        HandleRSAKey(pkg);
                        break;
                    case (int)eFightPackageType.ROOM_REMOVE:
                        HandleRoomRemove(pkg);
                        break;
                    case (int)eFightPackageType.SEND_TO_ROOM:
                        HandleSendToRoom(pkg);
                        break;
                    case (int)eFightPackageType.SEND_TO_USER:
                        HandleSendToPlayer(pkg);
                        break;
                    case (int)eFightPackageType.ROOM_START_GAME:
                        HandleStartGame(pkg);
                        break;
                    case (int)eFightPackageType.ROOM_STOP_GAME:
                        HandleStopGame(pkg);
                        break;
                    case (int)eFightPackageType.SEND_GAME_PLAYER_ID:
                        HandleUpdatePlayerGameId(pkg);
                        break;
                    case (int)eFightPackageType.DISCONNECT_PLAYER:
                        HandleDisconnectPlayer(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ON_GAME_OVER:
                        HandlePlayerOnGameOver(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_USE_PROP_INGAME:
                        HandlePlayerOnUsingItem(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ADD_GOLD:
                        HandlePlayerAddGold(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ADD_MONEY:
                        HandlePlayerAddMoney(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ADD_GIFTTOKEN :
                        HandlePlayerAddGiftToken(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ADD_GP:
                        HandlePlayerAddGP(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ONKILLING_LIVING:
                        HandlePlayerOnKillingLiving(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ONMISSION_OVER:
                        HandlePlayerOnMissionOver(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_CONSORTIAFIGHT:
                        HandlePlayerConsortiaFight(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_SEND_CONSORTIAFIGHT:
                        HandlePlayerSendConsortiaFight(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_REMOVE_GOLD:
                        HandlePlayerRemoveGold(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_REMOVE_MONEY:
                        HandlePlayerRemoveMoney(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_ADD_TEMPLATE1:
                        HandlePlayerAddTemplate1(pkg);
                        break;
                    case (int)eFightPackageType.FIND_CONSORTIA_ALLY:
                        HandleFindConsortiaAlly(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_REMOVE_GP:
                        HandlePlayerRemoveGP(pkg);
                        break;
                    case (int)eFightPackageType.PLAYER_REMOVE_OFFER:
                        HandlePlayerRemoveOffer(pkg);
                        break;
                    case (int)eFightPackageType.CHAT:
                        HandlePlayerChatSend(pkg);
                        break;

                }
            }
            catch (Exception ex)
            {
                GameServer.log.Error("AsynProcessPacket", ex);
            }
        }

        private void HandlePlayerChatSend(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.SendMessage(pkg.ReadString());
            }
        }

        public void HandleFindConsortiaAlly(GSPacketIn pkg)
        {
            int state = ConsortiaMgr.FindConsortiaAlly(pkg.ReadInt(), pkg.ReadInt());
            SendFindConsortiaAlly(state, pkg.ReadInt());

        }

        private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            AbstractGame game = player.CurrentRoom.Game;
            // pkg.ReadInt();
            if (player != null)
            {
                player.OnKillingLiving(game, pkg.ReadInt(), pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
            }
        }

        private void HandlePlayerOnMissionOver(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            AbstractGame game = player.CurrentRoom.Game;

            if (player != null)
            {
                player.OnMissionOver(game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
            }
        }

        private void HandlePlayerConsortiaFight(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            Dictionary<int, Player> players = new Dictionary<int, Player>();
            int consortiaWin = pkg.ReadInt();
            int consortiaLose = pkg.ReadInt();
            int count = pkg.ReadInt();
            int offer = 0;
            for (int i = 0; i < count; i++)
            {
                GamePlayer Temp = WorldMgr.GetPlayerById(pkg.ReadInt());
                if (Temp != null)
                {
                    Player Tempplayer = new Player(Temp, 0, null, 0);
                    players.Add(i, Tempplayer);
                }
            }
            eRoomType roomtype = (eRoomType)pkg.ReadByte();
            eGameType gametype = (eGameType)pkg.ReadByte();
            int totalKillHealth = pkg.ReadInt();
            if (player != null)
            {
                offer = player.ConsortiaFight(consortiaWin, consortiaLose, players, roomtype, gametype, totalKillHealth, count);
            }

            if (offer != 0)
            {

            }

        }

        private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.SendConsortiaFight(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
            }
        }

        private void HandlePlayerRemoveGold(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.RemoveGold(pkg.ReadInt());
            }
        }

        private void HandlePlayerRemoveMoney(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.RemoveMoney(pkg.ReadInt());
            }
        }


        private void HandlePlayerRemoveOffer(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.RemoveOffer(pkg.ReadInt());
            }
        }

        private void HandlePlayerAddTemplate1(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                
                ItemTemplateInfo template = ItemMgr.FindItemTemplate(pkg.ReadInt());
                eBageType type = (eBageType)pkg.ReadByte();
                if (template != null)
                {
                    int Count = pkg.ReadInt();
                    ItemInfo item = ItemInfo.CreateFromTemplate(template, Count, (int)ItemAddType.FightGet);
                    item.Count = Count;
                    item.ValidDate = pkg.ReadInt();
                    item.IsBinds = pkg.ReadBoolean();
                    item.IsUsed = pkg.ReadBoolean();
                    player.AddTemplate(item, type, item.Count);
                }
            }
        }

        private void HandlePlayerAddGP(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.AddGP(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddMoney(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.AddMoney(pkg.Parameter1);
            }
        }
        private void HandlePlayerAddGiftToken(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.AddGiftToken(pkg.Parameter1);
            }
        }
        private void HandlePlayerRemoveGP(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.RemoveGP(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddGold(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.AddGold(pkg.Parameter1);
             
            }
        }

        private void HandlePlayerOnUsingItem(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                int templateId = pkg.ReadInt();
                bool result = player.UsePropItem(null, pkg.Parameter1, pkg.Parameter2, templateId, pkg.ReadBoolean());
                SendUsingPropInGame(player.CurrentRoom.Game.Id, player.GamePlayerId, templateId, result);
            }
        }

        private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_USE_PROP_INGAME, gameId);
            pkg.Parameter1 = playerId;
            pkg.Parameter2 = templateId;
            pkg.WriteBoolean(result);
            SendTCP(pkg);
        }

        public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.DISCONNECT, gameId);
            pkg.Parameter1 = playerId;
            SendTCP(pkg);
        }

        private void HandlePlayerOnGameOver(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null && player.CurrentRoom != null && player.CurrentRoom.Game != null)
            {
                player.OnGameOver(player.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt());
            }
        }

        private void HandleDisconnectPlayer(GSPacketIn pkg)
        {
            GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
            if (player != null)
            {
                player.Disconnect();
            }
        }

        public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.LOGIN);
            pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
            SendTCP(pkg);
        }

        protected void HandleRSAKey(GSPacketIn packet)
        {
            RSAParameters para = new RSAParameters();
           // packet.ReadBytes(2);
            para.Modulus = packet.ReadBytes(128);
            para.Exponent = packet.ReadBytes();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(para);
            SendRSALogin(rsa, m_key);
        }

        #endregion

        public FightServerConnector(BattleServer server, string ip, int port, string key)
            : base(ip, port, true, new byte[2048], new byte[2048])
        {
            m_server = server;
            m_key = key;
            Strict = true;
        }

        public void SendAddRoom(Game.Server.Rooms.BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn((int)eFightPackageType.ROOM_CREATE);
            pkg.WriteInt(room.RoomId);
            pkg.WriteInt((int)room.GameType);
            pkg.WriteInt(room.GuildId);

            List<GamePlayer> players = room.GetPlayers();
            pkg.WriteInt(players.Count);
            foreach (GamePlayer p in players)
            {
                pkg.WriteInt(p.PlayerCharacter.ID);//改为唯一ID
                pkg.WriteString(p.PlayerCharacter.NickName);
                pkg.WriteBoolean(p.PlayerCharacter.Sex);

                pkg.WriteInt(p.PlayerCharacter.Hide);
                pkg.WriteString(p.PlayerCharacter.Style);
                pkg.WriteString(p.PlayerCharacter.Colors);
                pkg.WriteString(p.PlayerCharacter.Skin);
                pkg.WriteInt(p.PlayerCharacter.Offer);
                pkg.WriteInt(p.PlayerCharacter.GP);
                pkg.WriteInt(p.PlayerCharacter.Grade);
                pkg.WriteInt(p.PlayerCharacter.Repute);
                pkg.WriteInt(p.PlayerCharacter.ConsortiaID);
                pkg.WriteString(p.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(p.PlayerCharacter.ConsortiaLevel);
                pkg.WriteInt(p.PlayerCharacter.ConsortiaRepute);

                pkg.WriteInt(p.PlayerCharacter.Attack);
                pkg.WriteInt(p.PlayerCharacter.Defence);
                pkg.WriteInt(p.PlayerCharacter.Agility);
                pkg.WriteInt(p.PlayerCharacter.Luck);
                pkg.WriteDouble(p.GetBaseAttack());
                pkg.WriteDouble(p.GetBaseDefence());
                pkg.WriteDouble(p.GetBaseAgility());
                pkg.WriteDouble(p.GetBaseBlood());
                pkg.WriteInt(p.MainWeapon.TemplateID);
                pkg.WriteBoolean(p.CanUseProp);
                if (p.SecondWeapon != null)
                {
                    pkg.WriteInt(p.SecondWeapon.TemplateID);
                    pkg.WriteInt(p.SecondWeapon.StrengthenLevel);
                }
                else
                {
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                }
                pkg.WriteDouble(RateMgr.GetRate(eRateType.Experience_Rate) * AntiAddictionMgr.GetAntiAddictionCoefficient(p.PlayerCharacter.AntiAddiction) * (p.GPAddPlus == 0 ? 1 : p.GPAddPlus));
                pkg.WriteDouble(AntiAddictionMgr.GetAntiAddictionCoefficient(p.PlayerCharacter.AntiAddiction) * (p.OfferAddPlus == 0 ? 1 : p.OfferAddPlus));
                pkg.WriteDouble(RateMgr.GetRate(eRateType.Experience_Rate));
                pkg.WriteInt(GameServer.Instance.Configuration.ServerID);


                List<AbstractBuffer> infos = p.BufferList.GetAllBuffer();
                pkg.WriteInt(infos.Count);
                foreach (AbstractBuffer bufferInfo in infos)
                {
                    BufferInfo info = bufferInfo.Info;
                    pkg.WriteInt(info.Type);
                    pkg.WriteBoolean(info.IsExist);
                    pkg.WriteDateTime(info.BeginDate);
                    pkg.WriteInt(info.ValidDate);
                    pkg.WriteInt(info.Value);
                }

                pkg.WriteInt(p.EquipEffect.Count);
                foreach (int i in p.EquipEffect)
                {
                    pkg.WriteInt(i);
                }


            }
            SendTCP(pkg);
        }

        public void SendRemoveRoom(BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn((int)eFightPackageType.ROOM_REMOVE);
            pkg.Parameter1 = room.RoomId;
            SendTCP(pkg);
        }

        public void SendToGame(int gameId, GSPacketIn pkg)
        {
            GSPacketIn wrapper = new GSPacketIn((int)eFightPackageType.SEND_TO_GAME, gameId);
            wrapper.WritePacket(pkg);
            SendTCP(wrapper);
        }

        protected void HandleRoomRemove(GSPacketIn packet)
        {
            m_server.RemoveRoomImp(packet.ClientID);
        }

        protected void HandleStartGame(GSPacketIn pkg)
        {
            ProxyGame game = new ProxyGame(pkg.Parameter2, this, (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt());
            m_server.StartGame(pkg.Parameter1, game);
        }

        protected void HandleStopGame(GSPacketIn pkg)
        {
            int roomId = pkg.Parameter1;
            int gameId = pkg.Parameter2;

            m_server.StopGame(roomId, gameId);
        }

        protected void HandleSendToRoom(GSPacketIn pkg)
        {
            int roomId = pkg.ClientID;
            GSPacketIn inner = pkg.ReadPacket();
            m_server.SendToRoom(roomId, inner, pkg.Parameter1, pkg.Parameter2);
        }

        protected void HandleSendToPlayer(GSPacketIn pkg)
        {
            int playerId = pkg.ClientID;
            try
            {
                GSPacketIn inner = pkg.ReadPacket();
                m_server.SendToUser(playerId, inner);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("pkg len:{0}", pkg.Length), ex);
                log.Error(Marshal.ToHexDump("pkg content:", pkg.Buffer, 0, pkg.Length));
            }
        }

        private void HandleUpdatePlayerGameId(GSPacketIn pkg)
        {
            m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
        }

        public void SendChatMessage(string msg, GamePlayer player, bool team)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.CHAT, player.CurrentRoom.Game.Id);

            pkg.WriteInt(player.GamePlayerId);
            pkg.WriteBoolean(team);
            pkg.WriteString(msg);

            SendTCP(pkg);

        }

        public void SendFightNotice(GamePlayer player, int GameId)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.SYS_NOTICE, GameId);
            pkg.Parameter1 = player.GamePlayerId;
            //pkg.WriteInt(type);
            //pkg.WriteString(msg);
            //pkg.WriteString(msg1);
            SendTCP(pkg);
        }

        public void SendFindConsortiaAlly(int state, int gameid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.FIND_CONSORTIA_ALLY, gameid);
            pkg.WriteInt(state);
            pkg.WriteInt((int)RateMgr.GetRate(eRateType.Riches_Rate));
            SendTCP(pkg);
        }

    }
}
