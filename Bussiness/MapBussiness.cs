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


namespace Bussiness
{
    public class MapBussiness : BaseBussiness
    {
        public MapInfo[] GetAllMap()
        {
            List<MapInfo> infos = new List<MapInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Maps_All");
                while (reader.Read())
                {
                    MapInfo info = new MapInfo();
                    info.BackMusic = reader["BackMusic"] == null ? "" : reader["BackMusic"].ToString();
                    info.BackPic = reader["BackPic"] == null ? "" : reader["BackPic"].ToString();
                    info.BackroundHeight = (int)reader["BackroundHeight"];
                    info.BackroundWidht = (int)reader["BackroundWidht"];
                    info.DeadHeight = (int)reader["DeadHeight"];
                    info.DeadPic = reader["DeadPic"] == null ? "" : reader["DeadPic"].ToString();
                    info.DeadWidth = (int)reader["DeadWidth"];
                    info.Description = reader["Description"] == null ? "" : reader["Description"].ToString();
                    info.DragIndex = (int)reader["DragIndex"];
                    info.ForegroundHeight = (int)reader["ForegroundHeight"];
                    info.ForegroundWidth = (int)reader["ForegroundWidth"];
                    info.ForePic = reader["ForePic"] == null ? "" : reader["ForePic"].ToString();
                    info.ID = (int)reader["ID"];
                    info.Name = reader["Name"] == null ? "" : reader["Name"].ToString();
                    info.Pic = reader["Pic"] == null ? "" : reader["Pic"].ToString();
                    info.Remark = reader["Remark"] == null ? "" : reader["Remark"].ToString();
                    info.Weight = (int)reader["Weight"];
                    info.PosX = reader["PosX"] == null ? "" : reader["PosX"].ToString();
                    info.PosX1 = reader["PosX1"] == null ? "" : reader["PosX1"].ToString();
                    info.Type = (byte)((int)reader["Type"]);
                    infos.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetAllMap", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos.ToArray();
        }

     
 

        public ServerMapInfo[] GetAllServerMap()
        {
            List<ServerMapInfo> infos = new List<ServerMapInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Maps_Server_All");
                while (reader.Read())
                {
                    ServerMapInfo info = new ServerMapInfo();
                    info.ServerID = (int)reader["ServerID"];
                    info.OpenMap = reader["OpenMap"].ToString();
                    info.IsSpecial = (int)reader["IsSpecial"];
                    infos.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetAllMapWeek", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos.ToArray();
        }
 
    }
}
