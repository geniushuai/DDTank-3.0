using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using System.Security.Cryptography;
using Game.Base.Packets;
using log4net;
using System.Reflection;
using Newtonsoft.Json;
using System.Net;
using SqlDataProvider.Data;
using Bussiness.Protocol;
using Bussiness;
using System.Threading;
using System.Configuration;
using Center.Server.Managers;

namespace Center.Server
{
    public class ServerClient:BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ServerInfo Info { get; set; }

        private RSACryptoServiceProvider _rsa;

        private CenterServer _svr;

        public bool NeedSyncMacroDrop = false;

        protected override void OnConnect()
        {
            base.OnConnect();
            _rsa = new RSACryptoServiceProvider();
            RSAParameters para = _rsa.ExportParameters(false);
            SendRSAKey(para.Modulus, para.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            
            _rsa = null;
            List<Player> player = LoginMgr.GetServerPlayers(this);
            LoginMgr.RemovePlayer(player);

            //通知其他服务器,该用户下线
            SendUserOffline(player);
            //_svr.SendToALL(pkg, this);

            if (Info != null)
            {
                Info.State = 1;
                Info.Online = 0;
                Info = null;
            }
        }

        #region 处理协议
        
        public override void OnRecvPacket(GSPacketIn pkg)
        {
            Console.Write("ServerClient: code: " + pkg.Code.ToString() + "\n");
            switch (pkg.Code)
            {
                case (int)ePackageType.LOGIN:
                        HandleLogin(pkg);
                    break;
                case (int)ePackageType.IP_PORT:
                    HandleIPAndPort(pkg);
                    break;
                case (int)ePackageType.ALLOW_USER_LOGIN:
                    HandleUserLogin(pkg);
                    break;
                case (int)ePackageType.USER_STATE:
                    HandleQuestUserState(pkg);
                    break;
                case (int)ePackageType.USER_ONLINE:
                    HandleUserOnline(pkg);
                    break;
                case (int)ePackageType.USER_OFFLINE:
                    HandleUserOffline(pkg);
                    break;
                case (int)ePackageType.CHAT_PERSONAL:
                    HandleChatPersonal(pkg);
                    break;
                case (int)ePackageType.B_BUGLE:
                    HandleBigBugle(pkg);
                    break;
                case (int)ePackageType.FRIEND_STATE:
                    HandleFriendState(pkg);
                    break;
                case (int)ePackageType.FRIEND_RESPONSE:
                    HandleFirendResponse(pkg);
                    break;
                case (int)ePackageType.MAIL_RESPONSE:
                    HandleMailResponse(pkg);
                    break;
                case (int)ePackageType.SYS_RELOAD:
                    HandleReload(pkg);
                    break;
                case (int)ePackageType.SCENE_CHAT:
                    HandleChatScene(pkg);
                    break;
                case (int)ePackageType.CONSORTIA_RESPONSE:
                    HandleConsortiaResponse(pkg);
                    break;
                case (int)ePackageType.CONSORTIA_OFFER:
                    HandleConsortiaOffer(pkg);
                    break;
                case (int)ePackageType.CONSORTIA_FIGHT:
                    HandleConsortiaFight(pkg);
                    break;
                case (int)ePackageType.CONSORTIA_CREATE:
                    HandleConsortiaCreate(pkg);
                    break;
                case (int)ePackageType.MACRO_DROP:
                    HandleMacroDrop(pkg);
                    break;
                case (int)ePackageType.SYS_NOTICE:
                    HandkeItemStrengthen(pkg);
                    break;
                //case (int)ePackageType.CONSORTIA_ALLY_ADD:
                //    HandleConsortiaAlly(pkg);
                //    break;
                //case (int)ePackageType.CONSORTIA_UPGRADE:
                //    HandleConsortiaUpGrade(pkg);
                //    break;
                case (int)ePackageType.PING:
                    HandlePing(pkg);
                    break;
                case (int)ePackageType.UPDATE_PLAYER_MARRIED_STATE:
                    HandleUpdatePlayerState(pkg);
                    break;
                case (int)ePackageType.MARRY_ROOM_INFO_TO_PLAYER:
                    HandleMarryRoomInfoToPlayer(pkg);
                    break;
                case (int)ePackageType.SHUTDOWN:
                    HandleShutdown(pkg);
                    break;
            }
        }

        /// <summary>
        /// 战斗服务器登陆
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleLogin(GSPacketIn pkg)
        {
            //pkg.ReadBytes(2);
            byte[] rgb = pkg.ReadBytes();
            
            string[] temp = Encoding.UTF8.GetString(_rsa.Decrypt(rgb, false)).Split(',');

            if (temp.Length == 2)
            {
                _rsa = null;
                int id = int.Parse(temp[0]);
                Info = ServerMgr.GetServerInfo(id);
                if (Info == null || Info.State != 1)
                {
                    log.ErrorFormat("Error Login Packet from {0} want to login serverid:{1}", TcpEndpoint, id);
                    Disconnect();
                }
                else
                {
                    Strict = false;
                    //SendASS(CenterServer.Instance.ASSState);
                    CenterServer.Instance.SendConfigState();
                    Info.Online = 0;
                    Info.State = 2;
                }
            }
            else
            {
                log.ErrorFormat("Error Login Packet from {0}", TcpEndpoint);
                Disconnect();
            }
        }

        public void HandleIPAndPort(GSPacketIn pkg)
        {
            //IPAddress ip = new IPAddress((long)pkg.ReadDouble());
            //int port = pkg.ReadInt();
            //if (Info != null)
            //{
            //    Info.IP = ip.ToString();
            //    Info.Port = port;
            //}
        }

        /// <summary>
        /// 用户准备登陆
        /// </summary>
        /// <param name="pkg"></param>
        private void HandleUserLogin(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();

            if (LoginMgr.TryLoginPlayer(playerid, this))
            {
                SendAllowUserLogin(playerid, true);
            }
            else
            {
                SendAllowUserLogin(playerid, false);
            }
        }

        /// <summary>
        /// 处理用户上线
        /// </summary>
        /// <param name="pkg"></param>
        private void HandleUserOnline(GSPacketIn pkg)
        {
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int playerid = pkg.ReadInt();
                int consortiaid = pkg.ReadInt();
                LoginMgr.PlayerLogined(playerid, this);
            }

            //通知其他服务器用户上线
            _svr.SendToALL(pkg,this);
        }

        /// <summary>
        /// 处理用户下线
        /// </summary>
        /// <param name="pkg"></param>
        private void HandleUserOffline(GSPacketIn pkg)
        {
            List<int> users = new List<int>();
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int playerid = pkg.ReadInt();
                int consortiaid = pkg.ReadInt();
                //判断用户是否在此服务器上,是则成功删除,否则忽略
                LoginMgr.PlayerLoginOut(playerid, this);
            }
            _svr.SendToALL(pkg);
        }


        /// <summary>
        /// 用户的私有消息
        /// </summary>
        /// <param name="pkg"></param>
        private void HandleUserPrivateMsg(GSPacketIn pkg,int playerid)
        {
            ServerClient client = LoginMgr.GetServerClient(playerid);
            if (client != null)
            {
                client.SendTCP(pkg);
            }
            //else
            //{
            //    int id = pkg.ClientID;
            //    string nickName = pkg.ReadString();
            //    GSPacketIn packet = new GSPacketIn((byte)ePackageType.SYS_MESS);
            //    packet.WriteInt(1);
            //    packet.WriteInt(id);
            //    packet.WriteString(nickName);
            //}
        }

        /// <summary>
        /// 用户的共有消息
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleUserPublicMsg(GSPacketIn pkg)
        {
            _svr.SendToALL(pkg, this);
        }

        /// <summary>
        /// 查询用户状态
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleQuestUserState(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();

            ServerClient client = LoginMgr.GetServerClient(playerid);
            if (client == null)
            {
                SendUserState(playerid, false);
            }
            else
            {
                SendUserState(playerid, true);
            }
        }

        public void HandlePing(GSPacketIn pkg)
        {
            //int playerid = pkg.ReadInt();
            Info.Online = pkg.ReadInt();
            Info.State = ServerMgr.GetState(Info.Online, Info.Total);
        }

        /// <summary>
        /// 私聊
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleChatPersonal(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            ServerClient client = LoginMgr.GetServerClient(playerid);
            if (client != null)
            {
                client.SendTCP(pkg);
            }
            else
            {
                int id = pkg.ClientID;
                string nickName = pkg.ReadString();
                GSPacketIn packet = new GSPacketIn((byte)ePackageType.SYS_MESS);
                packet.WriteInt(1);
                packet.WriteInt(id);
                packet.WriteString(nickName);
                SendTCP(packet);
            }
        }

        /// <summary>
        /// 处理大喇叭
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleBigBugle(GSPacketIn pkg)
        {
            _svr.SendToALL(pkg,this);
        }

        /// <summary>
        /// 改变好友状态
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleFriendState(GSPacketIn pkg)
        {
            _svr.SendToALL(pkg, this);
        }

        public void HandleFirendResponse(GSPacketIn pkg)
        {
            _svr.SendToALL(pkg, this);
        }

        public void HandleMailResponse(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            HandleUserPrivateMsg(pkg, playerid);
        }

        public void HandleReload(GSPacketIn pkg)
        {
            eReloadType type = (eReloadType)pkg.ReadInt();
            int serverID = pkg.ReadInt();
            bool result = pkg.ReadBoolean();
            Console.WriteLine(serverID + " " + type.ToString() + " is reload " + (result ? "succeed!" : "fail"));
        }

        public void HandleChatScene(GSPacketIn pkg)
        {
            byte channel = pkg.ReadByte();
            switch (channel)
            {
                case 3:
                    HandleChatConsortia(pkg);
                    break;
            }
        }

        /// <summary>
        /// 公会聊天
        /// </summary>
        /// <param name="pkg"></param>
        public void HandleChatConsortia(GSPacketIn pkg)
        {
            _svr.SendToALL(pkg, this);
        }

        public void HandleConsortiaResponse(GSPacketIn pkg)
        {

            //_svr.SendToALL(pkg, null);
            switch (pkg.ReadByte())
            {
                case 1://通过
                    //int consortiaID = pkg.ReadInt();
                    //string consortiaName = pkg.ReadString();
                    //int playerid = pkg.ReadInt();
                    //ServerClient client = PlayerMgr.GetServer(playerid);
                    //if (client != null)
                    //{
                    //    client.SendTCP(pkg);
                    //}
                    break;
                case 2://解散
                    //int id = pkg.ReadInt();
                    //ConsortiaMrg.DeleteConsortia(id);
                    break;
                case 3://踢出
                    //int id3 = pkg.ReadInt();
                    //ServerClient c3 = PlayerMgr.GetServer(id3);
                    //if (c3 != null)
                    //{
                    //    c3.SendTCP(pkg);
                    //}
                    break;
                case 4://邀请
                    //int inviteID = pkg.ReadInt();
                    //int id4 = pkg.ReadInt();
                    //ServerClient c4 = PlayerMgr.GetServer(id4);
                    //if (c4 != null)
                    //{
                    //    c4.SendTCP(pkg);
                    //}
                    break;
                case 5://禁言
                    //bool isBan = pkg.ReadBoolean();SP_Admin_SendAllItem
                    //int id5 = pkg.ReadInt();
                    //ServerClient c5 = PlayerMgr.GetServer(id5);
                    //if (c5 != null)
                    //{
                    //    c5.SendTCP(pkg);
                    //}
                    break;
                case 6://公会升级
                    break;
                case 7://关系改变
                    break;
                case 8://权限改变
                    break;
                case 9://mfpg
                    break;
                case 10://银行升级
                    break;
                default:

                    break;
            }
            _svr.SendToALL(pkg, null);

        }

        public void HandleConsortiaOffer(GSPacketIn pkg)
        {
            int id = pkg.ReadInt();
            int offer = pkg.ReadInt();
            int riches = pkg.ReadInt();

            //ConsortiaMrg.AddConsortiaOffer(id, offer, riches);
 
        }

        public void HandleConsortiaCreate(GSPacketIn pkg)
        {
            int id = pkg.ReadInt();
            int offer = pkg.ReadInt();

            _svr.SendToALL(pkg, null);
            //ConsortiaMrg.CreateConsortia(id, offer);

        }

        public void HandleConsortiaUpGrade(GSPacketIn pkg)
        {
            //int id = pkg.ReadInt();

            //ConsortiaMrg.ConsortiaUpGrade(id);

            _svr.SendToALL(pkg, this);

        }

        public void HandleConsortiaFight(GSPacketIn pkg)
        {
            //int consortiaID = pkg.ReadInt();
            //int riches = pkg.ReadInt();

            //ConsortiaMrg.AddConsortiaOffer(consortiaID, 0, riches);
            _svr.SendToALL(pkg);
        }

        public void HandkeItemStrengthen(GSPacketIn pkg)
        {

            _svr.SendToALL(pkg, this);
        }

        public void HandleUpdatePlayerState(GSPacketIn pkg)
        {
            int playerId = pkg.ReadInt();
            Player player = LoginMgr.GetPlayer(playerId);
            if (player != null && player.CurrentServer != null)
            {
                player.CurrentServer.SendTCP(pkg);
            }
        }

        public void HandleMarryRoomInfoToPlayer(GSPacketIn pkg)
        {
            int playerId = pkg.ReadInt();
            Player player = LoginMgr.GetPlayer(playerId);
            if (player != null && player.CurrentServer != null)
            {
                player.CurrentServer.SendTCP(pkg);
            }
        }

        public void HandleShutdown(GSPacketIn pkg)
        {
            int serverID = pkg.ReadInt();
            bool isStoping = pkg.ReadBoolean();
            if (isStoping)
            {
                Console.WriteLine(serverID + "  begin stoping !");
            }
            else
            {
                Console.WriteLine(serverID + "  is stoped !");
            }
        }

        public void HandleMacroDrop(GSPacketIn pkg)
        {
            Dictionary<int, int> temp = new Dictionary<int, int>();
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int templateId = pkg.ReadInt();
                int dropCount = pkg.ReadInt();
                temp.Add(templateId, dropCount);
            }
            MacroDropMgr.DropNotice(temp);
            NeedSyncMacroDrop = true;
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
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.RSAKey);
            pkg.Write(m);
            pkg.Write(e);
            SendTCP(pkg);
        }

        /// <summary>
        /// 允许用户登陆
        /// </summary>
        /// <param name="playerid"></param>
        public void SendAllowUserLogin(int playerid,bool allow)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.ALLOW_USER_LOGIN);
            pkg.WriteInt(playerid);
            pkg.WriteBoolean(allow);
            SendTCP(pkg);
        }

        public void SendKitoffUser(int playerid)
        {
            SendKitoffUser(playerid, LanguageMgr.GetTranslation("Center.Server.SendKitoffUser"));
        }

        public void SendKitoffUser(int playerid, string msg)
        {
             GSPacketIn pkg = new GSPacketIn((byte)ePackageType.KITOFF_USER);
            pkg.WriteInt(playerid);
            pkg.WriteString(msg);
            SendTCP(pkg);
        }

        /// <summary>
        /// 用户批量下线
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public void SendUserOffline(List<Player> users)
        {
            for (int i = 0; i < users.Count; i += 100)
            {
                int count = i + 100 > users.Count ? users.Count - i : 100;
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.USER_OFFLINE);
                pkg.WriteInt(count);
                for (int j = i; j < i + count; j++)
                {
                    pkg.WriteInt(users[j].Id);
                    pkg.WriteInt(0);
                }
                SendTCP(pkg);
                _svr.SendToALL(pkg, this);
            }

            //GSPacketIn pkg = new GSPacketIn((byte)ePackageType.USER_OFFLINE);
            //pkg.WriteInt(users.Count);
            //foreach (int i in users)
            //{
            //    pkg.WriteInt(i);
            //    pkg.WriteInt(0);
            //}
            //SendTCP(pkg);
            //_svr.SendToALL(pkg, this);
            //return pkg;
        }

        /// <summary>
        /// 发送用户状态
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        public void SendUserState(int player,bool state)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.USER_STATE,player);
            pkg.WriteBoolean(state);
            SendTCP(pkg);
        }

        /// <summary>
        /// 通知有用户充值
        /// </summary>
        /// <param name="player"></param>
        public void SendChargeMoney(int player, string chargeID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.CHARGE_MONEY, player);
            pkg.WriteString(chargeID);
            SendTCP(pkg);
        }

        public void SendASS(bool state)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.UPDATE_ASS);
            pkg.WriteBoolean(state);
            SendTCP(pkg);
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServerClient(CenterServer svr)
            : base(new byte[2048], new byte[2048])
        {
            _svr = svr;
        }
     
    }
}
