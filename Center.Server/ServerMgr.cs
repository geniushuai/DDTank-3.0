using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;

namespace Center.Server
{
    public class ServerMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int, ServerInfo> _list = new Dictionary<int, ServerInfo>();

        private static object _syncStop = new object();

        public static bool Start()
        {
            try
            {
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    ServerInfo[] list = db.GetServerList();
                    foreach (ServerInfo s in list)
                    {
                        s.State = 1;
                        s.Online = 0;
                        _list.Add(s.ID, s);
                    }
                }
                log.Info("Load server list from db.");
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Load server list from db failed:{0}", ex);
                return false;
            }
        }

        public static bool ReLoadServerList()
        {
            try
            {
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    lock (_syncStop)
                    {
                        ServerInfo[] list = db.GetServerList();
                        foreach (ServerInfo s in list)
                        {
                            if (_list.ContainsKey(s.ID))
                            {
                                _list[s.ID].IP = s.IP;
                                _list[s.ID].Name = s.Name;
                                _list[s.ID].Port = s.Port;
                                _list[s.ID].Room = s.Room;
                                _list[s.ID].Total = s.Total;
                                _list[s.ID].MustLevel = s.MustLevel;
                                _list[s.ID].LowestLevel = s.LowestLevel;
                                _list[s.ID].Online = s.Online;
                                _list[s.ID].State = s.State;
                            }
                            else
                            {
                                s.State = 1;
                                s.Online = 0;
                                _list.Add(s.ID, s);
                            }
                        }
                    }
                }
                log.Info("ReLoad server list from db.");
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ReLoad server list from db failed:{0}", ex);
                return false;
            }
        }

        public static ServerInfo GetServerInfo(int id)
        {
            if (_list.ContainsKey(id))
            {
                return _list[id];
            }
            else
            {
                return null;
            }
        }

        public static ServerInfo[] Servers
        {
            get
            {
                return _list.Values.ToArray<ServerInfo>();
            }
        }

//        world(胡敏) 18:14:20
//把0.28的状态去掉，0.65改为0.5
        public static int GetState(int count, int total)
        {
            //if (count >= total)
            //    return 5;
            //else if (count > total * 0.65)
            //    return 4;
            //else if (count > total * 0.28)
            //    return 3;
            //else
            //    return 2;
            if (count >= total)
                return 5;
            else if (count > total * 0.5)
                return 4;
            else
                return 2;
        }

        public static void SaveToDatabase()
        {
            try
            {
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    foreach (ServerInfo info in _list.Values)
                    {
                        //if (info.State != 1)
                        //{
                        //    info.State = GetState(info.Online, info.Total);
                        //}
                        db.UpdateService(info);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Save server state", ex);
            }
        }
    }
}
