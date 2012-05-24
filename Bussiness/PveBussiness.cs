using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class PveBussiness : BaseBussiness
    {
        public PveInfo[] GetAllPveInfos()
        {
            List<PveInfo> infos = new List<PveInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_PveInfos_All");
                while (reader.Read())
                {
                    PveInfo info = new PveInfo();
                    info.ID = (int)reader["Id"];
                    info.Name = reader["Name"] == null ? "" : reader["Name"].ToString();
                    info.Type = (int)reader["Type"];
                    info.LevelLimits = (int)reader["LevelLimits"];
                    info.SimpleTemplateIds = reader["SimpleTemplateIds"] == null ? "" : reader["SimpleTemplateIds"].ToString();
                    info.NormalTemplateIds = reader["NormalTemplateIds"] == null ? "" : reader["NormalTemplateIds"].ToString();
                    info.HardTemplateIds = reader["HardTemplateIds"] == null ? "" : reader["HardTemplateIds"].ToString();
                    info.TerrorTemplateIds = reader["TerrorTemplateIds"] == null ? "" : reader["TerrorTemplateIds"].ToString();
                    info.Pic = reader["Pic"] == null ? "" : reader["Pic"].ToString();
                    info.Description = reader["Description"] == null ? "" : reader["Description"].ToString();
                    info.SimpleGameScript = reader["SimpleGameScript"] as string;
                    info.NormalGameScript = reader["NormalGameScript"] as string;
                    info.HardGameScript = reader["HardGameScript"] as string;
                    info.TerrorGameScript = reader["TerrorGameScript"] as string;
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
    }
}
