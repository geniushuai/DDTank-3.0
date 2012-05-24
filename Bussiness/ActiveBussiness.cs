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
using Bussiness.Managers;


namespace Bussiness
{
    public class ActiveBussiness : BaseBussiness
    {
        public ActiveInfo[] GetAllActives()
        {
            List<ActiveInfo> infos = new List<ActiveInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Active_All");
                while (reader.Read())
                {
                    infos.Add(InitActiveInfo(reader));
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

        public ActiveInfo GetSingleActives(int activeID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
                para[0].Value = activeID;
                db.GetReader(ref reader, "SP_Active_Single", para);
                while (reader.Read())
                {
                    return InitActiveInfo(reader);
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

        public ActiveInfo InitActiveInfo(SqlDataReader reader)
        {
            ActiveInfo info = new ActiveInfo();
            info.ActiveID = (int)reader["ActiveID"];
            info.Description = reader["Description"] == null ? "" : reader["Description"].ToString();
            info.Content = reader["Content"] == null ? "" : reader["Content"].ToString();
            info.AwardContent = reader["AwardContent"] == null ? "" : reader["AwardContent"].ToString();
            info.HasKey = (int)reader["HasKey"];
            //info.EndDate = reader["EndDate"] == null ? reader["EndDate"] : (DateTime)reader["EndDate"];
            if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
            {
                info.EndDate = (DateTime)reader["EndDate"];
            }
            info.IsOnly = (bool)reader["IsOnly"];
            //info.StartDate = reader["StartDate"] == null ? reader["StartDate"] : (DateTime)reader["StartDate"];
            info.StartDate = (DateTime)reader["StartDate"];
            info.Title = reader["Title"].ToString();
            info.Type = (int)reader["Type"];
            info.ActionTimeContent = reader["ActionTimeContent"] == null ? "" : reader["ActionTimeContent"].ToString();

            return info;
        }

        public int PullDown(int activeID, string awardID, int userID, ref string msg)
        {
            int result = 1;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ActiveID", activeID);
                para[1] = new SqlParameter("@AwardID", awardID);
                para[2] = new SqlParameter("@UserID", userID);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                if (db.RunProcedure("SP_Active_PullDown", para))
                {
                    result = (int)para[3].Value;
                    switch (result)
                    {
                        case 0:
                            msg = "ActiveBussiness.Msg0";
                            break;
                        case 1:
                            msg = "ActiveBussiness.Msg1";
                            break;
                        case 2:
                            msg = "ActiveBussiness.Msg2";
                            break;
                        case 3:
                            msg = "ActiveBussiness.Msg3";
                            break;
                        case 4:
                            msg = "ActiveBussiness.Msg4";
                            break;
                        case 5:
                            msg = "ActiveBussiness.Msg5";
                            break;
                        case 6:
                            msg = "ActiveBussiness.Msg6";
                            break;
                        case 7:
                            msg = "ActiveBussiness.Msg7";
                            break;
                        case 8:
                            msg = "ActiveBussiness.Msg8";
                            break;
                        default:
                            msg = "ActiveBussiness.Msg9";
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }
    }
}
