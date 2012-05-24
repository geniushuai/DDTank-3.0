using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.BaseClass;
using System.Data.SqlClient;
using System.Configuration;
using DAL;
using log4net;
using System.Reflection;
using System.Data;

namespace Bussiness
{
    public class CountBussiness
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                //return ConfigurationSettings.AppSettings["countDb"];
                return _connectionString;
            }
        }

        private static int _appID;

        public static int AppID
        {
            get
            {
                //return int.Parse(ConfigurationSettings.AppSettings["AppID"]);
                return _appID;
            }
        }

        private static int _subID;

        public static int SubID
        {
            get
            {
                //return int.Parse(ConfigurationSettings.AppSettings["SubID"]);
                return _subID;
            }
        }

        private static int _serverID;

        public static int ServerID
        {
            get
            {
                //return int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
                return _serverID;
            }
        }

        private static bool _conutRecord;

        public static bool CountRecord
        {
            get
            {
                //return bool.Parse(ConfigurationSettings.AppSettings["CountRecord"]);
                return _conutRecord;
            }
        }

        public static void SetConfig(string connectionString,int appID,int subID,int serverID,bool countRecord)
        {
            _connectionString = connectionString;
            _appID = appID;
            _subID = subID;
            _serverID = serverID;
            _conutRecord = countRecord;
        }

        public static void InsertGameInfo(DateTime begin,int mapID, int money, int gold, string users)
        {
            InsertGameInfo(AppID, SubID, ServerID, begin, DateTime.Now, users.Split(',').Length,mapID, money, gold, users);
        }

        /// <summary>
        /// 插入游戏统计信息
        /// </summary>
        /// <param name="appid">游戏编号</param>
        /// <param name="subid">分区编号</param>
        /// <param name="serverid">服务器编号</param>
        /// <param name="begin">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="usercount">游戏人数</param>
        /// <param name="money">总花费点券</param>
        /// <param name="gold">总花费金币</param>
        /// <param name="users">用户id字符串，用","分割</param>
        public static void InsertGameInfo(int appid, int subid, int serverid, DateTime begin, DateTime end, int usercount,int mapID, int money, int gold, string users)
        {
            try
            {
                if (!CountRecord)
                    return;
                SqlHelper.BeginExecuteNonQuery(ConnectionString, "SP_Insert_Count_FightInfo", appid, subid, serverid, begin, end, usercount, mapID,money, gold, users);
            }
            catch(Exception ex) 
            {
                log.Error("Insert Log Error!", ex);
            }
        }

        public static void InsertServerInfo(int usercount, int gamecount)
        {
            InsertServerInfo(AppID, SubID, ServerID, usercount, gamecount, DateTime.Now);
        }

        /// <summary>
        /// 插入服务器统计信息
        /// </summary>
        /// <param name="appid">游戏编号</param>
        /// <param name="subid">分区编号</param>
        /// <param name="serverid">服务器编号</param>
        /// <param name="usercount">用户数</param>
        /// <param name="gamecount">游戏数</param>
        /// <param name="time">时间</param>
        public static void InsertServerInfo(int appid, int subid, int serverid, int usercount, int gamecount, DateTime time)
        {
            try
            {
                if (!CountRecord)
                    return;
                SqlHelper.BeginExecuteNonQuery(ConnectionString, "SP_Insert_Count_Server", appid, subid, serverid, usercount, gamecount, time);
            }
            catch (Exception ex)
            {
                log.Error("Insert Log Error!!", ex);
            }
        }
 
 
        public static void InsertSystemPayCount(int consumerid, int money, int gold, int consumertype, int subconsumertype)
        {
            InsertSystemPayCount(AppID, SubID, consumerid, money, gold, consumertype, subconsumertype, DateTime.Now);
        }

        /// <summary>
        /// 插入系统中直接使用的点券（未生成物品）
        /// </summary>
        /// <param name="appid">游戏编号</param>
        /// <param name="subid">分区编号</param>
        /// <param name="consumerid">消费用户id（公会id 或 用户id）</param>
        /// <param name="money">点券</param>
        /// <param name="gold">金币</param>
        /// <param name="consumertype">消费主类型</param>
        /// <param name="subconsumertype">消费子类型</param>
        /// <param name="datime">消费时间</param>

        public static void InsertSystemPayCount(int appid, int subid, int consumerid, int money, int gold, int consumertype, int subconsumertype, DateTime datime)
        {
            try
            {
                if (!CountRecord)
                    return;
                SqlHelper.BeginExecuteNonQuery(ConnectionString, "SP_Insert_Count_SystemPay", appid, subid, consumerid, money, gold, consumertype, subconsumertype, datime);
            }
            catch (Exception ex)
            {
                log.Error("InsertSystemPayCount Log Error!!!", ex);
            }
        }

        public static void InsertContentCount(Dictionary<string, string> clientInfos)
        {
            try
            {
                if (!CountRecord)
                    return;
                SqlHelper.BeginExecuteNonQuery(ConnectionString, "Modify_Count_Content", clientInfos["Application_Id"], clientInfos["Cpu"],
                    clientInfos["OperSystem"],clientInfos["IP"],clientInfos["IPAddress"],clientInfos["NETCLR"],
                    clientInfos["Browser"],clientInfos["ActiveX"],clientInfos["Cookies"],clientInfos["CSS"],
                    clientInfos["Language"],clientInfos["Computer"],clientInfos["Platform"],clientInfos["Win16"],
                    clientInfos["Win32"],clientInfos["Referry"],clientInfos["Redirect"],clientInfos["TimeSpan"],
                    clientInfos["ScreenWidth"] + clientInfos["ScreenHeight"], clientInfos["Color"], clientInfos["Flash"], "Insert");
            }
            catch (Exception ex)
            {
                log.Error("Insert Log Error!!!!", ex);
            }
        }

    


    }
}
