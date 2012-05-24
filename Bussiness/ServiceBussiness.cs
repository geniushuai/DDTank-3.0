using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using SqlDataProvider.BaseClass;
using System.Data;
using System.Data.SqlClient;
using DAL;
using log4net;
using System.Reflection;
using log4net.Util;
using System.Collections;

namespace Bussiness
{
    public class ServiceBussiness : BaseBussiness
    {

        public ServerInfo GetServiceSingle(int ID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
                para[0].Value = ID;
                db.GetReader(ref reader, "SP_Service_Single", para);
                while (reader.Read())
                {
                    ServerInfo info = new ServerInfo();
                    info.ID = (int)reader["ID"];
                    info.IP = reader["IP"].ToString();
                    info.Name = reader["Name"].ToString();
                    info.Online = (int)reader["Online"];
                    info.Port = (int)reader["Port"];
                    info.Remark = reader["Remark"].ToString();
                    info.Room = (int)reader["Room"];
                    info.State = (int)reader["State"];
                    info.Total = (int)reader["Total"];
                    info.RSA = reader["RSA"].ToString();
                    info .NewerServer = (bool)reader["NewerServer"];
                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return null;

        }

        public ServerInfo[] GetServiceByIP(string IP)
        {
            List<ServerInfo> infos = new List<ServerInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@IP", SqlDbType.VarChar, 50);
                para[0].Value = IP;
                db.GetReader(ref reader, "SP_Service_ListByIP", para);
                while (reader.Read())
                {
                    ServerInfo info = new ServerInfo();
                    info.ID = (int)reader["ID"];
                    info.IP = reader["IP"].ToString();
                    info.Name = reader["Name"].ToString();
                    info.Online = (int)reader["Online"];
                    info.Port = (int)reader["Port"];
                    info.Remark = reader["Remark"].ToString();
                    info.Room = (int)reader["Room"];
                    info.State = (int)reader["State"];
                    info.Total = (int)reader["Total"];
                    info.RSA = reader["RSA"].ToString();
                    infos.Add( info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos.ToArray();
        }

        public ServerInfo[] GetServerList()
        {
            List<ServerInfo> infos = new List<ServerInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Service_List");
                while (reader.Read())
                {
                    ServerInfo info = new ServerInfo();
                    info.ID = (int)reader["ID"];
                    info.IP = reader["IP"].ToString();
                    info.Name = reader["Name"].ToString();
                    info.Online = (int)reader["Online"];
                    info.Port = (int)reader["Port"];
                    info.Remark = reader["Remark"].ToString();
                    info.Room = (int)reader["Room"];
                    info.State = (int)reader["State"];
                    info.Total = (int)reader["Total"];
                    info.RSA = reader["RSA"].ToString();
                    info.MustLevel = (int)reader["MustLevel"];
                    info.LowestLevel = (int)reader["LowestLevel"];
                    infos.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos.ToArray();
        }

        public RecordInfo GetRecordInfo(DateTime date, int SaveRecordSecond)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                para[1] = new SqlParameter("@Second", SaveRecordSecond);


                db.GetReader(ref reader, "SP_Server_Record",para);
                while (reader.Read())
                {
                    RecordInfo info = new RecordInfo();
                    info.ActiveExpendBoy = (int)reader["ActiveExpendBoy"];
                    info.ActiveExpendGirl = (int)reader["ActiveExpendGirl"];
                    info.ActviePayBoy = (int)reader["ActviePayBoy"];
                    info.ActviePayGirl = (int)reader["ActviePayGirl"];
                    info.ExpendBoy = (int)reader["ExpendBoy"];
                    info.ExpendGirl = (int)reader["ExpendGirl"];
                    info.OnlineBoy = (int)reader["OnlineBoy"];
                    info.OnlineGirl = (int)reader["OnlineGirl"];
                    info.TotalBoy = (int)reader["TotalBoy"];
                    info.TotalGirl = (int)reader["TotalGirl"];
                    info.ActiveOnlineBoy = (int)reader["ActiveOnlineBoy"];
                    info.ActiveOnlineGirl = (int)reader["ActiveOnlineGirl"];
                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return null;
        }

        public bool UpdateService(ServerInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ID", info.ID);
                para[1] = new SqlParameter("@Online", info.Online);
                para[2] = new SqlParameter("@State", info.State);
                result = db.RunProcedure("SP_Service_Update", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpdateRSA(int ID, string RSA)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ID", ID);
                para[1] = new SqlParameter("@RSA", RSA);
                result = db.RunProcedure("SP_Service_UpdateRSA", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public Dictionary<string, string> GetServerConfig()
        {
            Dictionary<string, string> infos = new Dictionary<string, string>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Server_Config");
                while (reader.Read())
                {
                    if(!infos.ContainsKey(reader["Name"].ToString()))
                    {
                        infos.Add(reader["Name"].ToString(), reader["Value"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetServerConfig", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos;
        }

        public ServerProperty GetServerPropertyByKey(string key)
        {
            ServerProperty serverProperty = null;
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@Key", key);

                db.GetReader(ref reader, "SP_Server_Config_Single",para);
                while (reader.Read())
                {
                    serverProperty = new ServerProperty();
                    serverProperty.Key = reader["Name"].ToString();
                    serverProperty.Value = reader["Value"].ToString();
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetServerConfig", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return serverProperty;
        }

        public bool UpdateServerPropertyByKey(string key, string value)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@Key", key);
                para[1] = new SqlParameter("@Value", value);
                result = db.RunProcedure("SP_Server_Config_Update", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ArrayList GetRate(int serverId)
        {
            SqlDataReader reader = null;

            try
            {
                ArrayList arrryList = new ArrayList();
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ServerID", serverId);
                db.GetReader(ref reader, "SP_Rate", para);

                while (reader.Read())
                {
                    RateInfo info = new RateInfo();
                    info.ServerID = (int)reader["ServerID"];
                    info.Rate = (float)(decimal)reader["Rate"];
                    info.BeginDay = (DateTime)reader["BeginDay"];
                    info.EndDay = (DateTime)reader["EndDay"];

                    info.BeginTime = (DateTime)reader["BeginTime"];
                    info.EndTime = (DateTime)reader["EndTime"];

                    info.Type = (int)reader["Type"];
                    arrryList.Add(info);
                }
                arrryList.TrimToSize();
                return arrryList;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetRates", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return null;
        }

        public RateInfo GetRateWithType(int serverId, int type)
        {
            SqlDataReader reader = null;

            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ServerID", serverId);
                para[1] = new SqlParameter("@Type", type);
                db.GetReader(ref reader, "SP_Rate_WithType", para);
                if (reader.Read())
                {
                    RateInfo info = new RateInfo();
                    info.ServerID = (int)reader["ServerID"];
                    info.Type = type;
                    info.Rate = (float)reader["Rate"];
                    info.BeginDay = (DateTime)reader["BeginDay"];
                    info.EndDay = (DateTime)reader["EndDay"];

                    info.BeginTime = (DateTime)reader["BeginTime"];
                    info.EndTime = (DateTime)reader["EndTime"];

                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetRate type: " + type, e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return null;
        }

        public FightRateInfo[] GetFightRate(int serverId)
        {
            SqlDataReader reader = null;
            List<FightRateInfo> infos = new List<FightRateInfo>();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ServerID", serverId);

                db.GetReader(ref reader, "SP_Fight_Rate", para);
                if (reader.Read())
                {
                    FightRateInfo info = new FightRateInfo();
                    info.ID = (int)reader["ID"];
                    info.ServerID = (int)reader["ServerID"];
                    info.Rate = (int)reader["Rate"];
                    info.BeginDay = (DateTime)reader["BeginDay"];
                    info.EndDay = (DateTime)reader["EndDay"];
                    info.BeginTime = (DateTime)reader["BeginTime"];
                    info.EndTime = (DateTime)reader["EndTime"];
                    info.SelfCue = reader["SelfCue"] == null ? "" : reader["SelfCue"].ToString();
                    info.EnemyCue = reader["EnemyCue"] == null ? "" : reader["EnemyCue"].ToString();
                    info.BoyTemplateID = (int)reader["BoyTemplateID"];
                    info.GirlTemplateID = (int)reader["GirlTemplateID"];
                    info.Name = reader["Name"] == null ? "" : reader["Name"].ToString();

                    infos.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetFightRate", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos.ToArray();
        }

        public string GetGameEdition()
        {
            string edition = string.Empty;
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Server_Edition");
                while (reader.Read())
                {
                    edition = reader["value"] == null ? "" : reader["value"].ToString();
                    return edition;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return edition;
        }

    }
}
