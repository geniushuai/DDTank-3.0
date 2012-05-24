using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Base;
using Game.Server.Packets.Client;
using System.Collections;
using log4net;
using System.Threading;
using System.Net.Sockets;
using Game.Server.ChatServer;
using System.Security.Cryptography;
using Game.Server.Packets;
using Game.Server.Managers;
using Game.Server.GameObjects;
using System.Net;
using Bussiness.Protocol;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;
using SqlDataProvider.Data;
using Game.Logic;

namespace Game.Server
{
    public class LoginServerConnector : BaseConnector
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int m_serverId;
        private string m_loginKey;

        #region 重载函数
        protected override void OnDisconnect()
        {
            //log.Error("LoginServerConnector is OnDisconnect!");
            base.OnDisconnect();
        }
        #endregion

        #region 接受处理

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
                    case (int)eChatServerPacket.RSAKey:
                        HandleRSAKey(pkg);
                        break;
                    case (int)eChatServerPacket.KITOFF_USER:
                        HandleKitoffPlayer(pkg);
                        break;
                    case (int)eChatServerPacket.ALLOW_USER_LOGIN:
                        HandleAllowUserLogin(pkg);
                        break;
                    case (int)eChatServerPacket.USER_OFFLINE:
                        HandleUserOffline(pkg);
                        break;
                    case (int)eChatServerPacket.USER_ONLINE:
                        HandleUserOnline(pkg);
                        break;
                    case (int)eChatServerPacket.CHAT_PERSONAL:
                        HandleChatPersonal(pkg);
                        break;
                    case (int)eChatServerPacket.B_BUGLE:
                        HandleBigBugle(pkg);
                        break;
                    case (int)eChatServerPacket.FRIEND_STATE:
                        HandleFriendState(pkg);
                        break;
                    case (int)eChatServerPacket.FRIEND_RESPONSE:
                        HandleFirendResponse(pkg);
                        break;
                    case (int)eChatServerPacket.CHARGE_MONEY:
                        HandleChargeMoney(pkg);
                        break;
                    case (int)eChatServerPacket.SYS_NOTICE:
                        HandleSystemNotice(pkg);
                        break;
                    case (int)eChatServerPacket.MAIL_RESPONSE:
                        HandleMailResponse(pkg);
                        break;
                    case (int)eChatServerPacket.SYS_RELOAD:
                        HandleReload(pkg);
                        break;
                    case (int)eChatServerPacket.SCENE_CHAT:
                        HandleChatConsortia(pkg);
                        break;
                    case (int)eChatServerPacket.CONSORTIA_RESPONSE:
                        HandleConsortiaResponse(pkg);
                        break;
                    //case (int)eChatServerPacket.CONSORTIA_ALLY_ADD:
                    //    HandleConsortiaAlly(pkg);
                    //    break;
                    case (int)eChatServerPacket.CONSORTIA_FIGHT:
                        HandleConsortiaFight(pkg);
                        break;
                    case (int)eChatServerPacket.UPDATE_ASS:
                        HandleASSState(pkg);
                        break;
                    case (int)eChatServerPacket.SYS_MESS:
                        HandleSysMess(pkg);
                        break;
                    case (int)eChatServerPacket.CONSORTIA_CREATE:
                        HandleConsortiaCreate(pkg);
                        break;
                    case (int)eChatServerPacket.RATE:
                        HandleRate(pkg);
                        break;
                    case (int)eChatServerPacket.MACRO_DROP:
                        HandleMacroDrop(pkg);
                        break;
                    case (int)eChatServerPacket.UPDATE_CONFIG_STATE:
                        HandleConfigState(pkg);
                        break;
                    case (int)eChatServerPacket.UPDATE_PLAYER_MARRIED_STATE:
                        HandleUpdatePlayerMarriedState(pkg);
                        break;
                    case (int)eChatServerPacket.MARRY_ROOM_INFO_TO_PLAYER:
                        HandleMarryRoomInfoToPlayer(pkg);
                        break;
                    case (int)eChatServerPacket.SHUTDOWN:
                        HandleShutdown(pkg);
                        break;

                }
            }
            catch (Exception ex)
            {
                GameServer.log.Error("AsynProcessPacket", ex);
            }
        }

        protected void HandleRSAKey(GSPacketIn packet)
        {
            
            RSAParameters para = new RSAParameters();
           // packet.ReadBytes(2);
            para.Modulus = packet.ReadBytes(128);
            para.Exponent = packet.ReadBytes();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(para);
            SendRSALogin(rsa, m_loginKey);
            SendListenIPPort(IPAddress.Parse(GameServer.Instance.Configuration.Ip), GameServer.Instance.Configuration.Port);
        }

        /// <summary>
        /// 踢用户下线
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleKitoffPlayer(Object stateInfo)
        {
            try
            {
                GSPacketIn packet = (GSPacketIn)stateInfo;
                int playerid = packet.ReadInt();
                GamePlayer client = WorldMgr.GetPlayerById(playerid);
                if (client != null)
                {
                    string msg = packet.ReadString();
                    client.Out.SendKitoff(msg);
                    client.Client.Disconnect();
                }
                else
                {
                    //异常补发
                    SendUserOffline(playerid, 0);
                }

            }
            catch (Exception e)
            {
                GameServer.log.Error("HandleKitoffPlayer", e);
            }
        }

        /// <summary>
        /// 允许用户登陆
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleAllowUserLogin(Object stateInfo)
        {
            try
            {
                GSPacketIn packet = (GSPacketIn)stateInfo;
                int playerid = packet.ReadInt();

                if (packet.ReadBoolean())
                {
                    GamePlayer player = LoginMgr.LoginClient(playerid);
                    if (player != null)
                    {
                        if (player.Login())
                        {
                            SendUserOnline(playerid, player.PlayerCharacter.ConsortiaID);
                            WorldMgr.OnPlayerOnline(playerid, player.PlayerCharacter.ConsortiaID);
                        }
                        else
                        {
                            player.Client.Disconnect();
                            SendUserOffline(playerid, 0);
                        }
                    }
                    else
                    {
                        SendUserOffline(playerid, 0);
                    }
                }
            }
            catch (Exception e)
            {
                GameServer.log.Error("HandleAllowUserLogin", e);
            }
        }

        /// <summary>
        /// 用户下线处理
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleUserOffline(GSPacketIn packet)
        {
            int count = packet.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int playerid = packet.ReadInt();
                int consortiaID = packet.ReadInt();

                if (LoginMgr.ContainsUser(playerid))
                {
                    SendAllowUserLogin(playerid);
                }

                WorldMgr.OnPlayerOffline(playerid, consortiaID);
            }
        }

        /// <summary>
        /// 用户上线处理
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleUserOnline(GSPacketIn packet)
        {
            int count = packet.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int playerid = packet.ReadInt();
                int consortiaID = packet.ReadInt();

                LoginMgr.ClearLoginPlayer(playerid);

                GamePlayer player = WorldMgr.GetPlayerById(playerid);
                if (player != null)
                {
                    GameServer.log.Error("Player hang in server!!!");
                    player.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
                    player.Client.Disconnect();
                }

                WorldMgr.OnPlayerOnline(playerid, consortiaID);
            }
        }

        /// <summary>
        /// 私聊
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleChatPersonal(GSPacketIn packet)
        {
            int playerID = packet.ReadInt();
            GamePlayer client = WorldMgr.GetPlayerById(playerID);
            if (client != null && !client.IsBlackFriend(packet.ClientID))
            {
                client.Out.SendTCP(packet);
            }
        }

        /// <summary>
        /// 处理大喇叭
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleBigBugle(GSPacketIn packet)
        {
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                p.Out.SendTCP(packet);
            }
        }

        /// <summary>
        /// 改变好友状态
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleFriendState(GSPacketIn pkg)
        {
            WorldMgr.ChangePlayerState(pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
        }

        /// <summary>
        /// 添加好友响应
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleFirendResponse(GSPacketIn packet)
        {
            int playerID = packet.ReadInt();
            GamePlayer client = WorldMgr.GetPlayerById(playerID);
            if (client != null)
            {
                client.Out.SendTCP(packet);
            }
        }

        public void HandleMailResponse(GSPacketIn packet)
        {
            int playerID = packet.ReadInt();
            GamePlayer client = WorldMgr.GetPlayerById(playerID);
            if (client != null)
            {
                client.Out.SendTCP(packet);
            }
        }

        public void HandleReload(GSPacketIn packet)
        {
            eReloadType type = (eReloadType)packet.ReadInt();
            bool result = false;
            switch (type)
            {
                case eReloadType.ball:
                    result = BallMgr.ReLoad();
                    break;
                
                case eReloadType.fusion:
                    result = FusionMgr.ReLoad();
                    break;
                case eReloadType.item:
                    result = ItemMgr.ReLoad();
                    break;
                case eReloadType.map:
                    result = MapMgr.ReLoadMap();
                    break;
                case eReloadType.mapserver:
                    result = MapMgr.ReLoadMapServer();
                    break;                
                case eReloadType.quest:
                    result = QuestMgr.ReLoad();
                    break;                
                case eReloadType.server:
                    GameServer.Instance.Configuration.Refresh();
                    break;
                case eReloadType.rate:
                    result = RateMgr.ReLoad();
                    break;
                case eReloadType.fight:
                    result = FightRateMgr.ReLoad();
                    break;
                case eReloadType.dailyaward:
                    result = AwardMgr.ReLoad();
                    break;
                case eReloadType.language:
                    result = LanguageMgr.Reload("");
                    break;
                default:
                    break;
                case eReloadType.shop:
                    result = ShopMgr.ReLoad();

                    break;
                case eReloadType.consortia:
                    result = ConsortiaMgr.ReLoad();
                    break;
                //case eReloadType.prop:
                //    result = PropItemMgr.Reload();
                //    break;
            }
            packet.WriteInt(GameServer.Instance.Configuration.ServerID);
            packet.WriteBoolean(result);
            SendTCP(packet);
        }

        public void HandleChargeMoney(GSPacketIn packet)
        {
            int playerID = packet.ClientID;
            GamePlayer client = WorldMgr.GetPlayerById(playerID);
            if (client != null)
            {
                client.ChargeToUser();
            }
        }

        public void HandleSystemNotice(GSPacketIn packet)
        {
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                p.Out.SendTCP(packet);
            }
        }

        public void HandleASSState(GSPacketIn packet)
        {
            bool state = packet.ReadBoolean();
            AntiAddictionMgr.SetASSState(state);
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                p.Out.SendAASControl(state, p.IsAASInfo, p.IsMinor);
            }
        }

        public void HandleConfigState(GSPacketIn packet)
        {
            bool aSSState = packet.ReadBoolean();
            bool dailyState = packet.ReadBoolean();

            AwardMgr.DailyAwardState = dailyState;
            AntiAddictionMgr.SetASSState(aSSState);
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                p.Out.SendAASControl(aSSState, p.IsAASInfo, p.IsMinor);
            }
        }

        public void HandleSysMess(GSPacketIn packet)
        {
            int type = packet.ReadInt();
            switch (type)
            {
                case 1://玩家不在线
                    int playerID = packet.ReadInt();
                    string nickname = packet.ReadString().Replace("\0", "");
                    GamePlayer client = WorldMgr.GetPlayerById(playerID);
                    if (client != null)
                    {
                        client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg1", nickname));
                    }
                    break;
            }
        }

        /// <summary>
        /// 公会聊天
        /// </summary>
        /// <param name="packet"></param>
        protected void HandleChatConsortia(GSPacketIn packet)
        {
            byte channel = packet.ReadByte();
            bool team = packet.ReadBoolean();
            string nick = packet.ReadString();
            string msg = packet.ReadString();
            int id = packet.ReadInt();
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == id)
                    p.Out.SendTCP(packet);
            }
        }

        protected void HandleConsortiaResponse(GSPacketIn packet)
        {
            switch (packet.ReadByte())
            {
                case 1:
                    HandleConsortiaUserPass(packet);
                    break;
                case 2:
                    HandleConsortiaDelete(packet);
                    break;
                case 3:
                    HandleConsortiaUserDelete(packet);
                    break;
                case 4:
                    HandleConsortiaUserInvite(packet);
                    break;
                case 5:
                    HandleConsortiaBanChat(packet);
                    break;
                case 6:
                    HandleConsortiaUpGrade(packet);
                    break;
                case 7:
                    HandleConsortiaAlly(packet);
                    break;
                case 8:
                    HandleConsortiaDuty(packet);
                    break;
                case 9:
                    HandleConsortiaRichesOffer(packet);
                    break;
                case 10:
                    HandleConsortiaShopUpGrade(packet);
                    break;
                case 11:
                    HandleConsortiaSmithUpGrade(packet);
                    break;
                case 12:
                    HandleConsortiaStoreUpGrade(packet);
                    break;
            }
        }

        public void HandleConsortiaFight(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            int riches = packet.ReadInt();
            string msg = packet.ReadString();
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.Out.SendMessage(eMessageType.ChatNormal, msg);
                }
            }
        }

        public void HandleConsortiaCreate(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            int offer = packet.ReadInt();
            ConsortiaMgr.AddConsortia(consortiaID);
           // FightConsortiaMgr.AddConsortia(consortiaID);
            //GamePlayer[] players = WorldMgr.GetAllPlayers();
            //foreach (GamePlayer p in players)
            //{
            //    if (p.PlayerCharacter.ConsortiaID == consortiaID)
            //    {
            //        p.Out.SendMessage(eMessageType.ChatNormal, msg);
            //    }
            //}

            //GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_CREATE);
            //pkg.WriteInt(consortiaID);
            //pkg.WriteInt(offer);
        }

        public void HandleConsortiaUserPass(GSPacketIn packet)
        {
            int cid = packet.ReadInt();
            bool isInvite = packet.ReadBoolean();
            int consortiaID = packet.ReadInt();
            string consortiaName = packet.ReadString();
            int id = packet.ReadInt();
            string userName = packet.ReadString();
            int inviteUserID = packet.ReadInt();
            string inviteUserName = packet.ReadString();
            int dutyID = packet.ReadInt();
            string dutyName = packet.ReadString();
            int offer = packet.ReadInt();
            int richesOffer = packet.ReadInt();
            int richesRob = packet.ReadInt();
            DateTime lastDate = packet.ReadDateTime();
            int grade = packet.ReadInt();
            int level = packet.ReadInt();
            int state = packet.ReadInt();
            bool sex = packet.ReadBoolean();
            int right = packet.ReadInt();
            int win = packet.ReadInt();
            int total = packet.ReadInt();
            int escape = packet.ReadInt();
            int consortiaRepute = packet.ReadInt();

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ID == id)
                {
                    p.BeginChanges();

                    p.PlayerCharacter.ConsortiaID = consortiaID;
                    p.PlayerCharacter.ConsortiaName = consortiaName;
                    p.PlayerCharacter.DutyName = dutyName;
                    p.PlayerCharacter.DutyLevel = level;
                    p.PlayerCharacter.Right = right;
                    p.PlayerCharacter.ConsortiaRepute = consortiaRepute;
                    ConsortiaInfo consotia = ConsortiaMgr.FindConsortiaInfo(consortiaID);
                    if (consotia != null)
                        p.PlayerCharacter.ConsortiaLevel = consotia.Level;

                    p.CommitChanges();
                }

                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaDelete(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID != consortiaID)
                    continue;

                p.ClearConsortia();
                p.AddRobRiches(-p.PlayerCharacter.RichesRob);

                p.Out.SendTCP(packet);
            }

        }

        public void HandleConsortiaUserDelete(GSPacketIn packet)
        {
            int id = packet.ReadInt();
            int consortiaID = packet.ReadInt();
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID || p.PlayerCharacter.ID == id)
                {
                    if (p.PlayerCharacter.ID == id)
                    {
                        p.ClearConsortia();
                    }
                    p.Out.SendTCP(packet);
                }
            }

        }

        public void HandleConsortiaUserInvite(GSPacketIn packet)
        {
            int id = packet.ReadInt();
            int playerid = packet.ReadInt();
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ID == playerid)
                {
                    p.Out.SendTCP(packet);
                    return;
                }
            }
        }

        public void HandleConsortiaBanChat(GSPacketIn packet)
        {
            bool isBan = packet.ReadBoolean();
            int playerid = packet.ReadInt();
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ID == playerid)
                {
                    p.PlayerCharacter.IsBanChat = isBan;
                    p.Out.SendTCP(packet);
                    return;
                }
            }
        }

        public void HandleConsortiaUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            string consortiaName = packet.ReadString();
            int consortiaLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaUpGrade(consortiaID, consortiaLevel);

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.PlayerCharacter.ConsortiaLevel = consortiaLevel;
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaStoreUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            string consortiaName = packet.ReadString();
            int storeLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaStoreUpGrade(consortiaID, storeLevel);

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.PlayerCharacter.StoreLevel = storeLevel;
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaShopUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            string consortiaName = packet.ReadString();
            int shopLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaShopUpGrade(consortiaID, shopLevel);

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.PlayerCharacter.ShopLevel = shopLevel;
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaSmithUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            string consortiaName = packet.ReadString();
            int smithLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaSmithUpGrade(consortiaID, smithLevel);

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.PlayerCharacter.SmithLevel = smithLevel;
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaAlly(GSPacketIn packet)
        {
            int consortiaID1 = packet.ReadInt();
            int consortiaID2 = packet.ReadInt();
            int state = packet.ReadInt();
            ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID1 || p.PlayerCharacter.ConsortiaID == consortiaID2)
                {
                    p.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaDuty(GSPacketIn packet)
        {
            int updateType = packet.ReadByte();
            int consortiaID = packet.ReadInt();
            int playerID = packet.ReadInt();
            string playerName = packet.ReadString();
            int dutyLevel = packet.ReadInt();
            string dutyName = packet.ReadString();
            int right = packet.ReadInt();

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    if (updateType == 2 && p.PlayerCharacter.DutyLevel == dutyLevel)
                    {
                        p.PlayerCharacter.DutyName = dutyName;
                    }
                    else if (p.PlayerCharacter.ID == playerID && (updateType == 5 || updateType == 6 || updateType == 7 || updateType == 8 || updateType == 9))
                    {
                        p.PlayerCharacter.DutyLevel = dutyLevel;
                        p.PlayerCharacter.DutyName = dutyName;
                        p.PlayerCharacter.Right = right;

                    }

                    p.Out.SendTCP(packet);
                }
            }

        }


        public void HandleRate(GSPacketIn packet)
        {
            RateMgr.ReLoad();
        }


        public void HandleConsortiaRichesOffer(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            //int playerID = packet.ReadInt();
            //string playerName = packet.ReadString();
            //int riches = packet.ReadInt();

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    p.Out.SendTCP(packet);
                }
            }

        }

        public void HandleUpdatePlayerMarriedState(GSPacketIn packet)
        {
            int playerId = packet.ReadInt();

            GamePlayer player = WorldMgr.GetPlayerById(playerId);
            if (player != null)
            {
                player.LoadMarryProp();

                player.LoadMarryMessage();
                player.QuestInventory.ClearMarryQuest();
            }
        }

        public void HandleMarryRoomInfoToPlayer(GSPacketIn packet)
        {
            int playerId = packet.ReadInt();

            GamePlayer player = WorldMgr.GetPlayerById(playerId);
            if (player != null)
            {
                packet.Code = (short)Game.Server.Packets.ePackageType.MARRY_ROOM_STATE;
                packet.ClientID = playerId;

                player.Out.SendTCP(packet);
            }
        }

        public void HandleShutdown(GSPacketIn pkg)
        {
            GameServer.Instance.Shutdown();
        }

        public void HandleMacroDrop(GSPacketIn pkg)
        {
            Dictionary<int, MacroDropInfo> temp = new Dictionary<int, MacroDropInfo>();
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int templateId = pkg.ReadInt();
                int dropcount = pkg.ReadInt();
                int maxCount = pkg.ReadInt();
                MacroDropInfo mdi = new MacroDropInfo(dropcount, maxCount);
                temp.Add(templateId, mdi);
            }
            MacroDropMgr.UpdateDropInfo(temp);
        }

        //private void ShutDownCallBack(object state)
        //{
        //    _count--;
        //    Console.WriteLine(string.Format("Server will shutdown after {0} mins!", _count));
        //    GameClient[] list = GameServer.Instance.GetAllClients();
        //    foreach (GameClient c in list)
        //    {
        //        if (c.Out != null)
        //        {
        //            c.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _count, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
        //        }
        //    }
        //    if (_count == 0)
        //    {
        //        _timer.Dispose();
        //        _timer = null;
        //        GameServer.Instance.Stop();
        //        Console.WriteLine("Server has stopped!");
        //    }
        //}

        #endregion

        #region 发送协议

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="name"></param>
        public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.LOGIN);
            pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
            SendTCP(pkg);
        }

        /// <summary>
        /// 发送监听端口和IP
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SendListenIPPort(IPAddress ip, int port)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.IP_PORT);
            pkg.Write(ip.GetAddressBytes());
            pkg.WriteInt(port);

            SendTCP(pkg);
        }

        public void SendPingCenter()
        {
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            int playerCount = players == null ? 0 : players.Length;

            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.PING);
            pkg.WriteInt(playerCount);

            SendTCP(pkg);
        }


        /// <summary>
        /// 用户批量上线
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public GSPacketIn SendUserOnline(Dictionary<int, int> users)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.USER_ONLINE);
            pkg.WriteInt(users.Count);
            foreach (KeyValuePair<int, int> i in users)
            {
                pkg.WriteInt(i.Key);
                pkg.WriteInt(i.Value);
            }
            SendTCP(pkg);
            return pkg;
        }

        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public GSPacketIn SendUserOnline(int playerid, int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.USER_ONLINE);
            pkg.WriteInt(1);
            pkg.WriteInt(playerid);
            pkg.WriteInt(consortiaID);
            SendTCP(pkg);
            return pkg;
        }

        /// <summary>
        /// 用户批量下线
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        //public GSPacketIn SendUserOffline(Dictionary<int, int> users)
        //{
        //    GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.USER_OFFLINE);
        //    pkg.WriteInt(users.Count);
        //    foreach (KeyValuePair<int,int> i in users)
        //    {
        //        pkg.WriteInt(i.Key);
        //        pkg.WriteInt(i.Value);
        //    }
        //    SendTCP(pkg);
        //    return pkg;
        //}

        /// <summary>
        /// 用户下线
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public GSPacketIn SendUserOffline(int playerid, int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.USER_OFFLINE);
            pkg.WriteInt(1);
            pkg.WriteInt(playerid);
            pkg.WriteInt(consortiaID);
            SendTCP(pkg);
            return pkg;
        }

        /// <summary>
        /// 是否允许用户上线
        /// </summary>
        /// <param name="playerid"></param>
        public void SendAllowUserLogin(int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.ALLOW_USER_LOGIN);
            pkg.WriteInt(playerid);
            SendTCP(pkg);
        }

        public void SendMailResponse(int playerid)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.MAIL_RESPONSE);
            pkg.WriteInt(playerid);
            SendTCP(pkg);
        }

        public void SendConsortiaUserPass(int playerid, string playerName, ConsortiaUserInfo info, bool isInvite, int consortiaRepute, string loginName,int fightpower)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE, playerid);
            pkg.WriteByte(1);
            pkg.WriteInt(info.ID);
            pkg.WriteBoolean(isInvite);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.UserID);
            pkg.WriteString(info.UserName);
            pkg.WriteInt(playerid);
            pkg.WriteString(playerName);
            pkg.WriteInt(info.DutyID);
            pkg.WriteString(info.DutyName);
            pkg.WriteInt(info.Offer);
            pkg.WriteInt(info.RichesOffer);
            pkg.WriteInt(info.RichesRob);
            pkg.WriteDateTime(info.LastDate);
            pkg.WriteInt(info.Grade);
            pkg.WriteInt(info.Level);
            pkg.WriteInt(info.State);
            pkg.WriteBoolean(info.Sex);
            pkg.WriteInt(info.Right);
            pkg.WriteInt(info.Win);
            pkg.WriteInt(info.Total);
            pkg.WriteInt(info.Escape);
            pkg.WriteInt(consortiaRepute);
            pkg.WriteString(loginName);
            pkg.WriteInt(fightpower);
            pkg.WriteInt(500);
            //New
            pkg.WriteString("Honor");
            SendTCP(pkg);
        }

        public void SendConsortiaDelete(int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(2);
            pkg.WriteInt(consortiaID);
            SendTCP(pkg);
        }

        public void SendConsortiaUserDelete(int playerid, int consortiaID, bool isKick, string nickName, string kickName)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(3);
            pkg.WriteInt(playerid);
            pkg.WriteInt(consortiaID);
            pkg.WriteBoolean(isKick);
            pkg.WriteString(nickName);
            pkg.WriteString(kickName);
            SendTCP(pkg);
        }

        public void SendConsortiaInvite(int ID, int playerid, string playerName, int inviteID, string intviteName, string consortiaName, int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(4);
            pkg.WriteInt(ID);
            pkg.WriteInt(playerid);
            pkg.WriteString(playerName);
            pkg.WriteInt(inviteID);
            pkg.WriteString(intviteName);
            pkg.WriteInt(consortiaID);
            pkg.WriteString(consortiaName);

            SendTCP(pkg);
        }

        public void SendConsortiaBanChat(int playerid, string playerName, int handleID, string handleName, bool isBan)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(5);
            pkg.WriteBoolean(isBan);
            pkg.WriteInt(playerid);
            pkg.WriteString(playerName);
            pkg.WriteInt(handleID);
            pkg.WriteString(handleName);

            SendTCP(pkg);
        }

        public void SendConsortiaFight(int consortiaID, int riches, string msg)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_FIGHT);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(riches);
            pkg.WriteString(msg);
            SendTCP(pkg);
        }

        public void SendConsortiaOffer(int consortiaID, int offer, int riches)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_OFFER);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(offer);
            pkg.WriteInt(riches);
            SendTCP(pkg);
        }

        public void SendConsortiaCreate(int consortiaID, int offer, string consotiaName)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_CREATE);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(offer);
            pkg.WriteString(consotiaName);
            SendTCP(pkg);
        }

        public void SendConsortiaUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(6);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.Level);

            SendTCP(pkg);
        }

        public void SendConsortiaShopUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(10);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.ShopLevel);

            SendTCP(pkg);
        }

        public void SendConsortiaSmithUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(11);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.SmithLevel);

            SendTCP(pkg);
        }

        public void SendConsortiaStoreUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(12);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.StoreLevel);

            SendTCP(pkg);
        }


        public void SendConsortiaAlly(int consortiaID1, int consortiaID2, int state)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(7);
            pkg.WriteInt(consortiaID1);
            pkg.WriteInt(consortiaID2);
            pkg.WriteInt(state);

            SendTCP(pkg);

            ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
        }

        public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID)
        {
            SendConsortiaDuty(info, updateType, consortiaID, 0, "", 0, "");
        }

        //updateType：1.添加责务，2.更改责务，3.职务升级，4.职务降级，5.个人改变,6.个人升级,7.个人降级,8.会长转让,9.升会长。
        public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID, int playerID, string playerName, int handleID, string handleName)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(8);
            pkg.WriteByte((byte)updateType);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(playerID);
            pkg.WriteString(playerName);
            pkg.WriteInt(info.Level);
            pkg.WriteString(info.DutyName);
            pkg.WriteInt(info.Right);
            pkg.WriteInt(handleID);
            pkg.WriteString(handleName);

            SendTCP(pkg);
        }

        public void SendConsortiaRichesOffer(int consortiaID, int playerID, string playerName, int riches)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.CONSORTIA_RESPONSE);
            pkg.WriteByte(9);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(playerID);
            pkg.WriteString(playerName);
            pkg.WriteInt(riches);

            SendTCP(pkg);
        }

        public void SendUpdatePlayerMarriedStates(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.UPDATE_PLAYER_MARRIED_STATE);
            pkg.WriteInt(playerId);

            SendTCP(pkg);
        }

        public void SendMarryRoomInfoToPlayer(int playerId, bool state, MarryRoomInfo info)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.MARRY_ROOM_INFO_TO_PLAYER);
            pkg.WriteInt(playerId);
            pkg.WriteBoolean(state);
            if (state)
            {
                pkg.WriteInt(info.ID);
                pkg.WriteString(info.Name);
                pkg.WriteInt(info.MapIndex);
                pkg.WriteInt(info.AvailTime);
                pkg.WriteInt(info.PlayerID);
                pkg.WriteInt(info.GroomID);
                pkg.WriteInt(info.BrideID);
                pkg.WriteDateTime(info.BeginTime);
                pkg.WriteBoolean(info.IsGunsaluteUsed);
            }
            SendTCP(pkg);
        }

        public void SendShutdown(bool isStoping)
        {
            GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.SHUTDOWN);
            pkg.WriteInt(m_serverId);
            pkg.WriteBoolean(isStoping);
            SendTCP(pkg);
        }

        /// <summary>
        /// 转发包
        /// </summary>
        /// <param name="packet"></param>
        public void SendPacket(GSPacketIn packet)
        {
            SendTCP(packet);
        }


        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="loginKey"></param>
        public LoginServerConnector(string ip, int port, int serverid, string name, byte[] readBuffer, byte[] sendBuffer)
            : base(ip, port, true, readBuffer, sendBuffer)
        {
            m_serverId = serverid;
            m_loginKey = string.Format("{0},{1}", serverid, name);
            Strict = true;
        }

        #endregion
    }
}
