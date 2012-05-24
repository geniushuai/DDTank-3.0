using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Bussiness;

namespace Center.Server
{
    [DataContract]
    public class ServerData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public int MustLevel { get; set; }

        [DataMember]
        public int LowestLevel { get; set; }

        [DataMember]
        public int Online { get; set; }
    }

    // NOTE: If you change the class name "CenterService" here, you must also update the reference to "CenterService" in App.config.
    public class CenterService : ICenterService
    {       
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<ServerData> GetServerList()
        {
            ServerInfo[] sl = ServerMgr.Servers;

            List<ServerData> list = new List<ServerData>();
            foreach (ServerInfo s in sl)
            {
                ServerData d = new ServerData();
                d.Id = s.ID;
                d.Name = s.Name;
                d.Ip = s.IP;
                d.Port = s.Port;
                d.State = s.State;
                d.MustLevel = s.MustLevel;
                d.LowestLevel = s.LowestLevel;
                d.Online = s.Online;

                list.Add(d);

            }
            return list;
        }

        #region ManageSystem

        public bool ChargeMoney(int userID,string chargeID)
        {
            ServerClient client = LoginMgr.GetServerClient(userID);
            if (client != null)
            {
                client.SendChargeMoney(userID, chargeID);
                return true;
            }
            return false;
        }

        public bool SystemNotice(string msg)
        {
            try
            {
                CenterServer.Instance.SendSystemNotice(msg);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool KitoffUser(int playerID,string msg)
        {
            try
            {
                ServerClient client = LoginMgr.GetServerClient(playerID);
                if (client != null)
                {
                    msg = string.IsNullOrEmpty(msg) ? "You are kicking out by GM!" : msg;
                    client.SendKitoffUser(playerID, msg);
                    LoginMgr.RemovePlayer(playerID);
                    return true;
                }
            }
            catch{}
            return false;
        }

        public bool ReLoadServerList()
        {
            return ServerMgr.ReLoadServerList();
        }

        public bool MailNotice(int playerID)
        {
            try
            {
                ServerClient client = LoginMgr.GetServerClient(playerID);
                if (client != null)
                {
                    GSPacketIn pkgMsg = new GSPacketIn((byte)ePackageType.MAIL_RESPONSE);
                    pkgMsg.WriteInt(playerID);
                    pkgMsg.WriteInt((int)eMailRespose.Receiver);
                    client.SendTCP(pkgMsg);
                    return true;
                }
            }
            catch{}
            return false; 
        }

        public bool AASUpdateState(bool state)
        {
            try
            {
                return CenterServer.Instance.SendAAS(state);
            }
            catch { }
            return false;
        }

        public int AASGetState()
        {
            try
            {
                return CenterServer.Instance.ASSState ? 1 : 0;
            }
            catch { }
            return 2;
        }

        public int ExperienceRateUpdate(int serverId)
        {
            try
            {
                return CenterServer.Instance.RateUpdate(serverId);
            }
            catch { }
            return 2;
        }

        public int NoticeServerUpdate(int serverId,int type)
        {
            try
            {
                return CenterServer.Instance.NoticeServerUpdate(serverId,type);
            }
            catch { }
            return 2;
        }

        public bool UpdateConfigState(int type, bool state)
        {
            try
            {
                return CenterServer.Instance.SendConfigState(type,state);
            }
            catch { }
            return false;
        }

        public int GetConfigState(int type)
        {
            try
            {
                switch (type)
                {
                    case 1:
                        return CenterServer.Instance.ASSState ? 1 : 0;
                    case 2:
                        return CenterServer.Instance.DailyAwardState ? 1 : 0;
                }
            }
            catch { }
            return 2;
        }

        public bool Reload(string type)
        {
            try
            {
                return CenterServer.Instance.SendReload(type);
            }
            catch { }
            return false;
        }

        #endregion

        #region Login

        public bool ActivePlayer(bool isActive)
        {
            try
            {
                if (isActive)
                {                    
                    Statics.LogMgr.AddRegCount();
                    return true;
                }
            }
            catch
            { }
            return false;
        }

        public bool CreatePlayer(int id, string name, string password, bool isFirst)
        {
            try
            {
                Player player = new Player();
                player.Id = id;
                player.Name = name;
                player.Password = password;
                player.IsFirst = isFirst;
                LoginMgr.CreatePlayer(player);
                return true;
            }
            catch
            { }
            return false;
        }

        public bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst)
        {
            try
            {
                //Player player = LoginMgr.GetPlayerByName(name);
                //if (player != null && player.Password == password)
                //{
                //    userID = player.Id;
                //    isFirst = player.IsFirst;
                //    return true;
                //}
                Player[] list = LoginMgr.GetAllPlayer();
                if (list != null)
                {
                    foreach (Player p in list)
                    {
                        if (p.Name == name && p.Password == password)
                        {
                            userID = p.Id;
                            isFirst = p.IsFirst;
                            return true;
                        }
                    }
                }
            }
            catch
            { }
            return false;
        }

        #endregion

        private static ServiceHost host;

        public static bool Start()
        {
            try
            {
                host = new ServiceHost(typeof(CenterService));
                host.Open();
                log.Info("Center Service started!");
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Start center server failed:{0}", ex);
                return false;
            }
        }

        public static void Stop()
        {
            try
            {
                if (host != null)
                {
                    host.Close();
                    host = null;
                }
            }
            catch { }
        }
    }
}
