using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Bussiness.CenterService;
using Bussiness.Managers;
using DAL;
using log4net;
using log4net.Util;
using SqlDataProvider.BaseClass;
using SqlDataProvider.Data;

namespace Bussiness
{
    public class ConsortiaBussiness : BaseBussiness
    {
        #region Consortia

        public bool AddConsortia(ConsortiaInfo info, ref string msg, ref ConsortiaDutyInfo dutyInfo)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[23];
                para[0] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@BuildDate", info.BuildDate);
                para[2] = new SqlParameter("@CelebCount", info.CelebCount);
                para[3] = new SqlParameter("@ChairmanID", info.ChairmanID);
                para[4] = new SqlParameter("@ChairmanName", info.ChairmanName == null ? "" : info.ChairmanName);
                para[5] = new SqlParameter("@ConsortiaName", info.ConsortiaName == null ? "" : info.ConsortiaName);
                para[6] = new SqlParameter("@CreatorID", info.CreatorID);
                para[7] = new SqlParameter("@CreatorName", info.CreatorName == null ? "" : info.CreatorName);
                para[8] = new SqlParameter("@Description", info.Description);
                para[9] = new SqlParameter("@Honor", info.Honor);
                para[10] = new SqlParameter("@IP", info.IP);
                para[11] = new SqlParameter("@IsExist", info.IsExist);
                para[12] = new SqlParameter("@Level", info.Level);
                para[13] = new SqlParameter("@MaxCount", info.MaxCount);
                para[14] = new SqlParameter("@Placard", info.Placard);
                para[15] = new SqlParameter("@Port", info.Port);
                para[16] = new SqlParameter("@Repute", info.Repute);
                para[17] = new SqlParameter("@Count", info.Count);
                para[18] = new SqlParameter("@Riches", info.Riches);
                para[19] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[19].Direction = ParameterDirection.ReturnValue;
                para[20] = new SqlParameter("@tempDutyLevel", System.Data.SqlDbType.Int);
                para[20].Direction = ParameterDirection.InputOutput;
                para[20].Value = dutyInfo.Level;
                para[21] = new SqlParameter("@tempDutyName", System.Data.SqlDbType.VarChar, 100);
                para[21].Direction = ParameterDirection.InputOutput;
                para[21].Value = "";
                para[22] = new SqlParameter("@tempRight", System.Data.SqlDbType.Int);
                para[22].Direction = ParameterDirection.InputOutput;
                para[22].Value = dutyInfo.Right;
                result = db.RunProcedure("SP_Consortia_Add", para);
                int returnValue = (int)para[19].Value;
                result = returnValue == 0;
                if (result)
                {
                    info.ConsortiaID = (int)para[0].Value;
                    dutyInfo.Level = (int)para[20].Value;
                    dutyInfo.DutyName = para[21].Value.ToString();
                    dutyInfo.Right = (int)para[22].Value;
                }
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddConsortia.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        //public bool UpdateConsortia(ConsortiaInfo info)
        //{
        //    bool result = false;
        //    try
        //    {
        //        SqlParameter[] para = new SqlParameter[21];
        //        para[0] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
        //        para[0].Direction = ParameterDirection.InputOutput;
        //        para[1] = new SqlParameter("@BuildDate", info.BuildDate);
        //        para[2] = new SqlParameter("@CelebCount", info.CelebCount);
        //        para[3] = new SqlParameter("@ChairmanID", info.ChairmanID);
        //        para[4] = new SqlParameter("@ChairmanName", info.ChairmanName == null ? "" : info.ChairmanName);
        //        para[5] = new SqlParameter("@ConsortiaName", info.ConsortiaName == null ? "" : info.ConsortiaName);
        //        para[6] = new SqlParameter("@CreatorID", info.CreatorID);
        //        para[7] = new SqlParameter("@CreatorName", info.CreatorName == null ? "" : info.CreatorName);
        //        para[8] = new SqlParameter("@Description", info.Description == null ? "" : info.Description);
        //        para[9] = new SqlParameter("@Honor", info.Honor);
        //        para[10] = new SqlParameter("@IP", info.IP == null ? "" : info.IP);
        //        para[11] = new SqlParameter("@IsExist", info.IsExist);
        //        para[12] = new SqlParameter("@Level", info.Level);
        //        para[13] = new SqlParameter("@MaxCount", info.MaxCount);
        //        para[14] = new SqlParameter("@Placard", info.Placard == null ? "" : info.Placard);
        //        para[15] = new SqlParameter("@Port", info.Port);
        //        para[16] = new SqlParameter("@Repute", info.Repute);
        //        para[17] = new SqlParameter("@Count", info.Count);
        //        para[18] = new SqlParameter("@Riches", info.Riches);
        //        para[19] = new SqlParameter("@DeductDate", info.DeductDate);
        //        para[20] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
        //        para[20].Direction = ParameterDirection.ReturnValue;
        //        result = db.RunProcedure("SP_Consortia_Update", para);
        //        info.ConsortiaID = (int)para[0].Value;
        //        int returnValue = (int)para[20].Value;
        //        result = returnValue == 0;
        //        info.IsDirty = false;
        //    }
        //    catch (Exception e)
        //    {
        //        if (log.IsErrorEnabled)
        //            log.Error("Init", e);
        //    }
        //    finally
        //    {
        //    }
        //    return result;
        //}

        public bool DeleteConsortia(int consortiaID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Consortia_Delete", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.DeleteConsortia.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.DeleteConsortia.Msg3";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ConsortiaInfo[] GetConsortiaPage(int page, int size, ref int total, int order, string name, int consortiaID, int level, int openApply)
        {
            List<ConsortiaInfo> infos = new List<ConsortiaInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere += " and ConsortiaName like '%" + name + "%' ";
                }
                if (consortiaID != -1)
                {
                    sWhere += " and ConsortiaID =" + consortiaID + " ";
                }
                if (level != -1)
                {
                    sWhere += " and Level =" + level + " ";
                }
                if (openApply != -1)
                {
                    sWhere += " and OpenApply =" + openApply + " ";
                }
                string sOrder = "ConsortiaName";
                switch (order)
                {
                    case 1:
                        sOrder = "ReputeSort";
                        break;
                    case 2:
                        sOrder = "ChairmanName";
                        break;
                    case 3:
                        sOrder = "Count desc";
                        break;
                    case 4:
                        sOrder = "Level desc";
                        break;
                    case 5:
                        sOrder = "Honor desc";
                        break;
                    case 10:
                        sOrder = "LastDayRiches desc";
                        break;
                    case 11:
                        sOrder = "AddDayRiches desc";
                        break;
                    case 12:
                        sOrder = "AddWeekRiches desc";
                        break;
                    case 13:
                        sOrder = "LastDayHonor desc";
                        break;
                    case 14:
                        sOrder = "AddDayHonor desc";
                        break;
                    case 15:
                        sOrder = "AddWeekHonor desc";
                        break;
                    case 16:
                        sOrder = "level desc,LastDayRiches desc";
                        break;
                }

                sOrder += ",ConsortiaID ";

                DataTable dt = GetPage("V_Consortia", sWhere, page, size, "*", sOrder, "ConsortiaID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaInfo info = new ConsortiaInfo();
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.BuildDate = (DateTime)dr["BuildDate"];
                    info.CelebCount = (int)dr["CelebCount"];
                    info.ChairmanID = (int)dr["ChairmanID"];
                    info.ChairmanName = dr["ChairmanName"].ToString();
                    info.ConsortiaName = dr["ConsortiaName"].ToString();
                    info.CreatorID = (int)dr["CreatorID"];
                    info.CreatorName = dr["CreatorName"].ToString();
                    info.Description = dr["Description"].ToString();
                    info.Honor = (int)dr["Honor"];
                    info.IsExist = (bool)dr["IsExist"];
                    info.Level = (int)dr["Level"];
                    info.MaxCount = (int)dr["MaxCount"];
                    info.Placard = dr["Placard"].ToString();
                    info.IP = dr["IP"].ToString();
                    info.Port = (int)dr["Port"];
                    info.Repute = (int)dr["Repute"];
                    info.Count = (int)dr["Count"];
                    info.Riches = (int)dr["Riches"];
                    info.DeductDate = (DateTime)dr["DeductDate"];
                    info.AddDayHonor = (int)dr["AddDayHonor"];
                    info.AddDayRiches = (int)dr["AddDayRiches"];
                    info.AddWeekHonor = (int)dr["AddWeekHonor"];
                    info.AddWeekRiches = (int)dr["AddWeekRiches"];
                    info.LastDayRiches = (int)dr["LastDayRiches"];
                    info.OpenApply = (bool)dr["OpenApply"];
                    info.StoreLevel = (int)dr["StoreLevel"];
                    info.SmithLevel = (int)dr["SmithLevel"];
                    info.ShopLevel = (int)dr["ShopLevel"];
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
            }

            return infos.ToArray();
        }

        public bool UpdateConsortiaDescription(int consortiaID, int userID, string description, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Description", description);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaDescription_Update", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaDescription.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool UpdateConsortiaPlacard(int consortiaID, int userID, string placard, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Placard", placard);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaPlacard_Update", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaPlacard.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool UpdateConsortiaChairman(string nickName, int consortiaID, int userID, ref string msg, ref ConsortiaDutyInfo info, ref int tempUserID, ref string tempUserName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@NickName", nickName);
                para[1] = new SqlParameter("@ConsortiaID", consortiaID);
                para[2] = new SqlParameter("@UserID", userID);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                para[4] = new SqlParameter("@tempUserID", System.Data.SqlDbType.Int);
                para[4].Direction = ParameterDirection.InputOutput;
                para[4].Value = tempUserID;
                para[5] = new SqlParameter("@tempUserName", System.Data.SqlDbType.VarChar, 100);
                para[5].Direction = ParameterDirection.InputOutput;
                para[5].Value = tempUserName;
                para[6] = new SqlParameter("@tempDutyLevel", System.Data.SqlDbType.Int);
                para[6].Direction = ParameterDirection.InputOutput;
                para[6].Value = info.Level;
                para[7] = new SqlParameter("@tempDutyName", System.Data.SqlDbType.VarChar, 100);
                para[7].Direction = ParameterDirection.InputOutput;
                para[7].Value = "";
                para[8] = new SqlParameter("@tempRight", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.InputOutput;
                para[8].Value = info.Right;
                result = db.RunProcedure("SP_ConsortiaChangeChairman", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                if (result)
                {
                    tempUserID = (int)para[4].Value;
                    tempUserName = para[5].Value.ToString();
                    info.Level = (int)para[6].Value;
                    info.DutyName = para[7].Value.ToString();
                    info.Right = (int)para[8].Value;
                }
                switch (returnValue)
                {
                    case 1:
                        msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg3";
                        break;
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool UpGradeConsortia(int consortiaID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Consortia_UpGrade", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpGradeConsortia.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.UpGradeConsortia.Msg3";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.UpGradeConsortia.Msg4";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }


        public bool UpGradeShopConsortia(int consortiaID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Consortia_Shop_UpGrade", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg3";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg4";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;

        }

        public bool UpGradeStoreConsortia(int consortiaID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Consortia_Store_UpGrade", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg3";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg4";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpGradeSmithConsortia(int consortiaID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Consortia_Smith_UpGrade", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg3";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg4";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ConsortiaInfo[] GetConsortiaAll()
        {
            List<ConsortiaInfo> infos = new List<ConsortiaInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Consortia_All");
                while (reader.Read())
                {
                    ConsortiaInfo info = new ConsortiaInfo();
                    info.ConsortiaID = (int)reader["ConsortiaID"];
                    info.Honor = (int)reader["Honor"];
                    info.Level = (int)reader["Level"];
                    info.Riches = (int)reader["Riches"];
                    info.MaxCount = (int)reader["MaxCount"];
                    info.BuildDate = (DateTime)reader["BuildDate"];
                    info.IsExist = (bool)reader["IsExist"];
                    info.DeductDate = (DateTime)reader["DeductDate"];
                    info.StoreLevel = (int)reader["StoreLevel"];
                    info.SmithLevel = (int)reader["SmithLevel"];
                    info.ShopLevel = (int)reader["ShopLevel"];
                    info.ConsortiaName = reader["ConsortiaName"] == null ? "" : reader["ConsortiaName"].ToString();
                    info.IsDirty = false;
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

        public ConsortiaInfo GetConsortiaSingle(int id)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", id);
                db.GetReader(ref reader, "SP_Consortia_Single", para);
                while (reader.Read())
                {
                    ConsortiaInfo info = new ConsortiaInfo();
                    info.ConsortiaID = (int)reader["ConsortiaID"];
                    info.BuildDate = (DateTime)reader["BuildDate"];
                    info.CelebCount = (int)reader["CelebCount"];
                    info.ChairmanID = (int)reader["ChairmanID"];
                    info.ChairmanName = reader["ChairmanName"].ToString();
                    info.ConsortiaName = reader["ConsortiaName"].ToString();
                    info.CreatorID = (int)reader["CreatorID"];
                    info.CreatorName = reader["CreatorName"].ToString();
                    info.Description = reader["Description"].ToString();
                    info.Honor = (int)reader["Honor"];
                    info.IsExist = (bool)reader["IsExist"];
                    info.Level = (int)reader["Level"];
                    info.MaxCount = (int)reader["MaxCount"];
                    info.Placard = reader["Placard"].ToString();
                    info.IP = reader["IP"].ToString();
                    info.Port = (int)reader["Port"];
                    info.Repute = (int)reader["Repute"];
                    info.Count = (int)reader["Count"];
                    info.Riches = (int)reader["Riches"];
                    info.DeductDate = (DateTime)reader["DeductDate"];
                    info.StoreLevel = (int)reader["StoreLevel"];
                    info.SmithLevel = (int)reader["SmithLevel"];
                    info.ShopLevel = (int)reader["ShopLevel"];
                    

                    
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

        public bool ConsortiaFight(int consortiWin, int consortiaLose, int playerCount, out int riches, int state, int totalKillHealth, float richesRate)
        {
            bool result = false;
            riches = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@ConsortiaWin", consortiWin);
                para[1] = new SqlParameter("@ConsortiaLose", consortiaLose);
                para[2] = new SqlParameter("@PlayerCount", playerCount);
                para[3] = new SqlParameter("@Riches", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.InputOutput;
                para[3].Value = riches;
                para[4] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[4].Direction = ParameterDirection.ReturnValue;
                para[5] = new SqlParameter("@State", state);
                para[6] = new SqlParameter("@TotalKillHealth", totalKillHealth);
                para[7] = new SqlParameter("@RichesRate", richesRate);
                result = db.RunProcedure("SP_Consortia_Fight", para);
                riches = (int)para[3].Value;
                int returnValue = (int)para[4].Value;
                result = returnValue == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ConsortiaFight", e);
            }
            finally
            {
            }

            return result;
        }

        public bool ConsortiaRichAdd(int consortiID, ref int riches)
        {
            return ConsortiaRichAdd(consortiID, ref riches, 0, "");
        }

        public bool ConsortiaRichAdd(int consortiID, ref int riches, int type, string username)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@ConsortiaID", consortiID);
                para[1] = new SqlParameter("@Riches", System.Data.SqlDbType.Int);
                para[1].Direction = ParameterDirection.InputOutput;
                para[1].Value = riches;
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                para[3] = new SqlParameter("@Type", type);
                para[4] = new SqlParameter("@UserName", username);
                result = db.RunProcedure("SP_Consortia_Riches_Add", para);
                riches = (int)para[1].Value;
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ConsortiaRichAdd", e);
            }
            finally
            {
            }
            return result;
        }

        public bool ScanConsortia(ref string noticeID)
        {
            bool result = false;
            try
            {

                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@NoticeID", System.Data.SqlDbType.NVarChar, 4000);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[1].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_Consortia_Scan", para);
                int returnValue = (int)para[1].Value;
                result = returnValue == 0;
                if (result)
                {
                    noticeID = para[0].Value.ToString();
                    //noticeID = para[0].Value == null ? "" : para[0].Value.ToString();
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        #endregion

        #region ConsortiaApplyUsers

        public bool UpdateConsotiaApplyState(int consortiaID, int userID, bool state, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@State", state);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Consortia_Apply_State", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsotiaApplyState.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool AddConsortiaApplyUsers(ConsortiaApplyUserInfo info, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@ApplyDate", info.ApplyDate);
                para[2] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
                para[3] = new SqlParameter("@ConsortiaName", info.ConsortiaName == null ? "" : info.ConsortiaName);
                para[4] = new SqlParameter("@IsExist", info.IsExist);
                para[5] = new SqlParameter("@Remark", info.Remark == null ? "" : info.Remark);
                para[6] = new SqlParameter("@UserID", info.UserID);
                para[7] = new SqlParameter("@UserName", info.UserName == null ? "" : info.UserName);
                para[8] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaApplyUser_Add", para);
                info.ID = (int)para[0].Value;
                int returnValue = (int)para[8].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg2";
                        break;
                    case 6:
                        msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg6";
                        break;
                    case 7:
                        msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg7";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool DeleteConsortiaApplyUsers(int applyID, int userID, int consortiaID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", applyID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@ConsortiaID", consortiaID);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_ConsortiaApplyUser_Delete", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0 ? true : returnValue == 3 ? true : false);
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.DeleteConsortiaApplyUsers.Msg2";
                        break;
                    case 3:
                        //msg = "ConsortiaBussiness.DeleteConsortiaApplyUsers.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool PassConsortiaApplyUsers(int applyID, int userID, string userName, int consortiaID, ref string msg, ConsortiaUserInfo info, ref int consortiaRepute)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[24];
                para[0] = new SqlParameter("@ID", applyID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@UserName", userName);
                para[3] = new SqlParameter("@ConsortiaID", consortiaID);
                para[4] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[4].Direction = ParameterDirection.ReturnValue;
                para[5] = new SqlParameter("@tempID", System.Data.SqlDbType.Int);
                para[5].Direction = ParameterDirection.InputOutput;
                para[5].Value = info.UserID;
                para[6] = new SqlParameter("@tempName", System.Data.SqlDbType.NVarChar, 100);
                para[6].Direction = ParameterDirection.InputOutput;
                para[6].Value = "";
                para[7] = new SqlParameter("@tempDutyID", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.InputOutput;
                para[7].Value = info.DutyID;
                para[8] = new SqlParameter("@tempDutyName", System.Data.SqlDbType.NVarChar, 100);
                para[8].Direction = ParameterDirection.InputOutput;
                para[8].Value = "";
                para[9] = new SqlParameter("@tempOffer", System.Data.SqlDbType.Int);
                para[9].Direction = ParameterDirection.InputOutput;
                para[9].Value = info.Offer;
                para[10] = new SqlParameter("@tempRichesOffer", System.Data.SqlDbType.Int);
                para[10].Direction = ParameterDirection.InputOutput;
                para[10].Value = info.RichesOffer;
                para[11] = new SqlParameter("@tempRichesRob", System.Data.SqlDbType.Int);
                para[11].Direction = ParameterDirection.InputOutput;
                para[11].Value = info.RichesRob;
                para[12] = new SqlParameter("@tempLastDate", System.Data.SqlDbType.DateTime);
                para[12].Direction = ParameterDirection.InputOutput;
                para[12].Value = DateTime.Now;
                para[13] = new SqlParameter("@tempWin", System.Data.SqlDbType.Int);
                para[13].Direction = ParameterDirection.InputOutput;
                para[13].Value = info.Win;
                para[14] = new SqlParameter("@tempTotal", System.Data.SqlDbType.Int);
                para[14].Direction = ParameterDirection.InputOutput;
                para[14].Value = info.Total;
                para[15] = new SqlParameter("@tempEscape", System.Data.SqlDbType.Int);
                para[15].Direction = ParameterDirection.InputOutput;
                para[15].Value = info.Escape;
                para[16] = new SqlParameter("@tempGrade", System.Data.SqlDbType.Int);
                para[16].Direction = ParameterDirection.InputOutput;
                para[16].Value = info.Grade;
                para[17] = new SqlParameter("@tempLevel", System.Data.SqlDbType.Int);
                para[17].Direction = ParameterDirection.InputOutput;
                para[17].Value = info.Level;
                para[18] = new SqlParameter("@tempCUID", System.Data.SqlDbType.Int);
                para[18].Direction = ParameterDirection.InputOutput;
                para[18].Value = info.ID;
                para[19] = new SqlParameter("@tempState", System.Data.SqlDbType.Int);
                para[19].Direction = ParameterDirection.InputOutput;
                para[19].Value = info.State;
                para[20] = new SqlParameter("@tempSex", System.Data.SqlDbType.Bit);
                para[20].Direction = ParameterDirection.InputOutput;
                para[20].Value = info.Sex;
                para[21] = new SqlParameter("@tempDutyRight", System.Data.SqlDbType.Int);
                para[21].Direction = ParameterDirection.InputOutput;
                para[21].Value = info.Right;
                para[22] = new SqlParameter("@tempConsortiaRepute", System.Data.SqlDbType.Int);
                para[22].Direction = ParameterDirection.InputOutput;
                para[22].Value = consortiaRepute;
                para[23] = new SqlParameter("@tempLoginName", System.Data.SqlDbType.NVarChar, 200);
                para[23].Direction = ParameterDirection.InputOutput;
                para[23].Value = consortiaRepute;

                db.RunProcedure("SP_ConsortiaApplyUser_Pass", para);
                int returnValue = (int)para[4].Value;

                result = returnValue == 0;
                if (result)
                {
                    info.UserID = (int)para[5].Value;
                    info.UserName = para[6].Value.ToString();
                    info.DutyID = (int)para[7].Value;
                    info.DutyName = para[8].Value.ToString();
                    info.Offer = (int)para[9].Value;
                    info.RichesOffer = (int)para[10].Value;
                    info.RichesRob = (int)para[11].Value;
                    info.LastDate = (DateTime)para[12].Value;
                    info.Win = (int)para[13].Value;
                    info.Total = (int)para[14].Value;
                    info.Escape = (int)para[15].Value;
                    info.Grade = (int)para[16].Value;
                    info.Level = (int)para[17].Value;
                    info.ID = (int)para[18].Value;
                    info.State = (int)para[19].Value;
                    info.Sex = (bool)para[20].Value;
                    info.Right = (int)para[21].Value;
                    consortiaRepute = (int)para[22].Value;
                    info.LoginName = para[23].Value.ToString();

                }
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg3";
                        break;
                    case 6:
                        msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg6";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ConsortiaApplyUserInfo[] GetConsortiaApplyUserPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int userID)
        {
            List<ConsortiaApplyUserInfo> infos = new List<ConsortiaApplyUserInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (consortiaID != -1)
                {
                    sWhere += " and ConsortiaID =" + consortiaID + " ";
                }
                if (applyID != -1)
                {
                    sWhere += " and ID =" + applyID + " ";
                }
                if (userID != -1)
                {
                    sWhere += " and UserID ='" + userID + "' ";
                }
                string sOrder = "ID";
                switch (order)
                {
                    case 1:
                        sOrder = "UserName,ID";
                        break;
                    case 2:
                        sOrder = "ApplyDate,ID";
                        break;
                }

                DataTable dt = GetPage("V_Consortia_Apply_Users", sWhere, page, size, "*", sOrder, "ID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaApplyUserInfo info = new ConsortiaApplyUserInfo();
                    info.ID = (int)dr["ID"];
                    info.ApplyDate = (DateTime)dr["ApplyDate"];
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.ConsortiaName = dr["ConsortiaName"].ToString();
                    info.ID = (int)dr["ID"];
                    info.IsExist = (bool)dr["IsExist"];
                    info.Remark = dr["Remark"].ToString();
                    info.UserID = (int)dr["UserID"];
                    info.UserName = dr["UserName"].ToString();
                    info.UserLevel = (int)dr["Grade"];
                    info.Win = (int)dr["Win"];
                    info.Total = (int)dr["Total"];
                    info.Repute = (int)dr["Repute"];
                    info.FightPower = (int)dr["FightPower"];
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
            }

            return infos.ToArray();
        }

        #endregion

        #region ConsortiaInviteUsers

        public bool AddConsortiaInviteUsers(ConsortiaInviteUserInfo info, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[11];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
                para[2] = new SqlParameter("@ConsortiaName", info.ConsortiaName == null ? "" : info.ConsortiaName);
                para[3] = new SqlParameter("@InviteDate", info.InviteDate);
                para[4] = new SqlParameter("@InviteID", info.InviteID);
                para[5] = new SqlParameter("@InviteName", info.InviteName == null ? "" : info.InviteName);
                para[6] = new SqlParameter("@IsExist", info.IsExist);
                para[7] = new SqlParameter("@Remark", info.Remark == null ? "" : info.Remark);
                para[8] = new SqlParameter("@UserID", info.UserID);
                para[8].Direction = ParameterDirection.InputOutput;
                para[9] = new SqlParameter("@UserName", info.UserName == null ? "" : info.UserName);
                para[10] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[10].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaInviteUser_Add", para);
                info.ID = (int)para[0].Value;
                info.UserID = (int)para[8].Value;
                int returnValue = (int)para[10].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg2";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg4";
                        break;
                    case 5:
                        msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg5";
                        break;
                    case 6:
                        msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg6";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool DeleteConsortiaInviteUsers(int intiveID, int userID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ID", intiveID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_ConsortiaInviteUser_Delete", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool PassConsortiaInviteUsers(int inviteID, int userID, string userName, ref int consortiaID, ref string consortiaName, ref string msg, ConsortiaUserInfo info, ref int tempID, ref string tempName, ref int consortiaRepute)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[24];
                para[0] = new SqlParameter("@ID", inviteID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@UserName", userName);
                para[3] = new SqlParameter("@ConsortiaID", consortiaID);
                para[3].Direction = ParameterDirection.InputOutput;
                para[4] = new SqlParameter("@ConsortiaName", System.Data.SqlDbType.NVarChar, 100);
                para[4].Value = consortiaName;
                para[4].Direction = ParameterDirection.InputOutput;
                para[5] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[5].Direction = ParameterDirection.ReturnValue;
                para[6] = new SqlParameter("@tempName", System.Data.SqlDbType.NVarChar, 100);
                para[6].Direction = ParameterDirection.InputOutput;
                para[6].Value = tempName;
                para[7] = new SqlParameter("@tempDutyID", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.InputOutput;
                para[7].Value = info.DutyID;
                para[8] = new SqlParameter("@tempDutyName", System.Data.SqlDbType.NVarChar, 100);
                para[8].Direction = ParameterDirection.InputOutput;
                para[8].Value = "";
                para[9] = new SqlParameter("@tempOffer", System.Data.SqlDbType.Int);
                para[9].Direction = ParameterDirection.InputOutput;
                para[9].Value = info.Offer;
                para[10] = new SqlParameter("@tempRichesOffer", System.Data.SqlDbType.Int);
                para[10].Direction = ParameterDirection.InputOutput;
                para[10].Value = info.RichesOffer;
                para[11] = new SqlParameter("@tempRichesRob", System.Data.SqlDbType.Int);
                para[11].Direction = ParameterDirection.InputOutput;
                para[11].Value = info.RichesRob;
                para[12] = new SqlParameter("@tempLastDate", System.Data.SqlDbType.DateTime);
                para[12].Direction = ParameterDirection.InputOutput;
                para[12].Value = DateTime.Now;
                para[13] = new SqlParameter("@tempWin", System.Data.SqlDbType.Int);
                para[13].Direction = ParameterDirection.InputOutput;
                para[13].Value = info.Win;
                para[14] = new SqlParameter("@tempTotal", System.Data.SqlDbType.Int);
                para[14].Direction = ParameterDirection.InputOutput;
                para[14].Value = info.Total;
                para[15] = new SqlParameter("@tempEscape", System.Data.SqlDbType.Int);
                para[15].Direction = ParameterDirection.InputOutput;
                para[15].Value = info.Escape;
                para[16] = new SqlParameter("@tempID", System.Data.SqlDbType.Int);
                para[16].Direction = ParameterDirection.InputOutput;
                para[16].Value = tempID;
                para[17] = new SqlParameter("@tempGrade", System.Data.SqlDbType.Int);
                para[17].Direction = ParameterDirection.InputOutput;
                para[17].Value = info.Level;
                para[18] = new SqlParameter("@tempLevel", System.Data.SqlDbType.Int);
                para[18].Direction = ParameterDirection.InputOutput;
                para[18].Value = info.Level;
                para[19] = new SqlParameter("@tempCUID", System.Data.SqlDbType.Int);
                para[19].Direction = ParameterDirection.InputOutput;
                para[19].Value = info.ID;
                para[20] = new SqlParameter("@tempState", System.Data.SqlDbType.Int);
                para[20].Direction = ParameterDirection.InputOutput;
                para[20].Value = info.State;
                para[21] = new SqlParameter("@tempSex", System.Data.SqlDbType.Bit);
                para[21].Direction = ParameterDirection.InputOutput;
                para[21].Value = info.Sex;
                para[22] = new SqlParameter("@tempRight", System.Data.SqlDbType.Int);
                para[22].Direction = ParameterDirection.InputOutput;
                para[22].Value = info.Right;
                para[23] = new SqlParameter("@tempConsortiaRepute", System.Data.SqlDbType.Int);
                para[23].Direction = ParameterDirection.InputOutput;
                para[23].Value = consortiaRepute;

                db.RunProcedure("SP_ConsortiaInviteUser_Pass", para);
                int returnValue = (int)para[5].Value;

                result = returnValue == 0;
                if (result)
                {
                    consortiaID = (int)para[3].Value;
                    consortiaName = para[4].Value.ToString();

                    tempName = para[6].Value.ToString();
                    info.DutyID = (int)para[7].Value;
                    info.DutyName = para[8].Value.ToString();
                    info.Offer = (int)para[9].Value;
                    info.RichesOffer = (int)para[10].Value;
                    info.RichesRob = (int)para[11].Value;
                    info.LastDate = (DateTime)para[12].Value;
                    info.Win = (int)para[13].Value;
                    info.Total = (int)para[14].Value;
                    info.Escape = (int)para[15].Value;
                    tempID = (int)para[16].Value;
                    info.Grade = (int)para[17].Value;
                    info.Level = (int)para[18].Value;
                    info.ID = (int)para[19].Value;
                    info.State = (int)para[20].Value;
                    info.Sex = (bool)para[21].Value;
                    info.Right = (int)para[22].Value;
                    consortiaRepute = (int)para[23].Value;
                }

                switch (returnValue)
                {
                    case 3:
                        msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg3";
                        break;
                    case 6:
                        msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg6";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ConsortiaInviteUserInfo[] GetConsortiaInviteUserPage(int page, int size, ref int total, int order, int userID, int inviteID)
        {
            List<ConsortiaInviteUserInfo> infos = new List<ConsortiaInviteUserInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (userID != -1)
                {
                    sWhere += " and UserID =" + userID + " ";
                }
                if (inviteID != -1)
                {
                    sWhere += " and UserID =" + inviteID + " ";
                }
                string sOrder = "ConsortiaName";
                switch (order)
                {
                    case 1:
                        sOrder = "Repute";
                        break;
                    case 2:
                        sOrder = "ChairmanName";
                        break;
                    case 3:
                        sOrder = "Count";
                        break;
                    case 4:
                        sOrder = "CelebCount";
                        break;
                    case 5:
                        sOrder = "Honor";
                        break;
                }

                sOrder += ",ID ";

                DataTable dt = GetPage("V_Consortia_Invite", sWhere, page, size, "*", sOrder, "ID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaInviteUserInfo info = new ConsortiaInviteUserInfo();
                    info.ID = (int)dr["ID"];
                    info.CelebCount = (int)dr["CelebCount"];
                    info.ChairmanName = dr["ChairmanName"].ToString();
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.ConsortiaName = dr["ConsortiaName"].ToString();
                    info.Count = (int)dr["Count"];
                    info.Honor = (int)dr["Honor"];
                    info.InviteDate = (DateTime)dr["InviteDate"];
                    info.InviteID = (int)dr["InviteID"];
                    info.InviteName = dr["InviteName"].ToString();
                    info.IsExist = (bool)dr["IsExist"];
                    info.Remark = dr["Remark"].ToString();
                    info.Repute = (int)dr["Repute"];
                    info.UserID = (int)dr["UserID"];
                    info.UserName = dr["UserName"].ToString();
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
            }

            return infos.ToArray();
        }


        #endregion

        #region ConsortiaUser

        public bool DeleteConsortiaUser(int userID, int kickUserID, int consortiaID, ref string msg, ref string nickName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@UserID", userID);
                para[1] = new SqlParameter("@KickUserID", kickUserID);
                para[2] = new SqlParameter("@ConsortiaID", consortiaID);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                para[4] = new SqlParameter("@NickName", System.Data.SqlDbType.VarChar, 200);
                para[4].Direction = ParameterDirection.InputOutput;
                para[4].Value = nickName;
                db.RunProcedure("SP_ConsortiaUser_Delete", para);
                int returnValue = (int)para[3].Value;
                if (returnValue == 0)
                {
                    result = true;
                    nickName = para[4].Value.ToString();
                }
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg3";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg4";
                        break;
                    case 5:
                        msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg5";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpdateConsortiaIsBanChat(int banUserID, int consortiaID, int userID, bool isBanChat, ref int tempID, ref string tempName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@ID", banUserID);
                para[1] = new SqlParameter("@ConsortiaID", consortiaID);
                para[2] = new SqlParameter("@UserID", userID);
                para[3] = new SqlParameter("@IsBanChat", isBanChat);
                para[4] = new SqlParameter("@TempID", tempID);
                para[4].Direction = ParameterDirection.InputOutput;
                para[5] = new SqlParameter("@TempName", System.Data.SqlDbType.NVarChar, 100);
                para[5].Value = tempName;
                para[5].Direction = ParameterDirection.InputOutput;
                para[6] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[6].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaIsBanChat_Update", para);
                int returnValue = (int)para[6].Value;
                tempID = (int)para[4].Value;
                tempName = para[5].Value.ToString();
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg3";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool UpdateConsortiaUserRemark(int id, int consortiaID, int userID, string remark, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@ID", id);
                para[1] = new SqlParameter("@ConsortiaID", consortiaID);
                para[2] = new SqlParameter("@UserID", userID);
                para[3] = new SqlParameter("@Remark", remark);
                para[4] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[4].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaUserRemark_Update", para);
                int returnValue = (int)para[4].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaUserRemark.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool UpdateConsortiaUserGrade(int id, int consortiaID, int userID, bool upGrade, ref string msg, ref ConsortiaDutyInfo info, ref string tempUserName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@ID", id);
                para[1] = new SqlParameter("@ConsortiaID", consortiaID);
                para[2] = new SqlParameter("@UserID", userID);
                para[3] = new SqlParameter("@UpGrade", upGrade);
                para[4] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[4].Direction = ParameterDirection.ReturnValue;
                para[5] = new SqlParameter("@tempUserName", System.Data.SqlDbType.VarChar, 100);
                para[5].Direction = ParameterDirection.InputOutput;
                para[5].Value = tempUserName;
                para[6] = new SqlParameter("@tempDutyLevel", System.Data.SqlDbType.Int);
                para[6].Direction = ParameterDirection.InputOutput;
                para[6].Value = info.Level;
                para[7] = new SqlParameter("@tempDutyName", System.Data.SqlDbType.VarChar, 100);
                para[7].Direction = ParameterDirection.InputOutput;
                para[7].Value = "";
                para[8] = new SqlParameter("@tempRight", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.InputOutput;
                para[8].Value = info.Right;
                result = db.RunProcedure("SP_ConsortiaUserGrade_Update", para);
                int returnValue = (int)para[4].Value;
                result = returnValue == 0;
                if (result)
                {
                    tempUserName = para[5].Value.ToString();
                    info.Level = (int)para[6].Value;
                    info.DutyName = para[7].Value.ToString();
                    info.Right = (int)para[8].Value;
                }
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg2";
                        break;
                    case 3:
                        msg = upGrade ? "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg3" : "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg10";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg4";
                        break;
                    case 5:
                        msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg5";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }


        public ConsortiaUserInfo[] GetConsortiaUsersPage(int page, int size, ref int total, int order, int consortiaID, int userID, int state)
        {
            List<ConsortiaUserInfo> infos = new List<ConsortiaUserInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (consortiaID != -1)
                {
                    sWhere += " and ConsortiaID =" + consortiaID + " ";
                }
                if (userID != -1)
                {
                    sWhere += " and UserID =" + userID + " ";
                }
                if (state != -1)
                {
                    sWhere += " and state =" + state + " ";
                }
                string sOrder = "UserName";
                switch (order)
                {
                    case 1:
                        sOrder = "DutyID";
                        break;
                    case 2:
                        sOrder = "Grade";
                        break;
                    case 3:
                        sOrder = "Repute";
                        break;
                    case 4:
                        sOrder = "GP";
                        break;
                    case 5:
                        sOrder = "State";
                        break;
                    case 6:
                        sOrder = "Offer";
                        break;
                }

                sOrder += ",ID ";

                DataTable dt = GetPage("V_Consortia_Users", sWhere, page, size, "*", sOrder, "ID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaUserInfo info = new ConsortiaUserInfo();
                    info.ID = (int)dr["ID"];
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.DutyID = (int)dr["DutyID"];
                    info.DutyName = dr["DutyName"].ToString();
                    info.IsExist = (bool)dr["IsExist"];
                    info.RatifierID = (int)dr["RatifierID"];
                    info.RatifierName = dr["RatifierName"].ToString();
                    info.Remark = dr["Remark"].ToString();
                    info.UserID = (int)dr["UserID"];
                    info.UserName = dr["UserName"].ToString();
                    info.Grade = (int)dr["Grade"];
                    info.GP = (int)dr["GP"];
                    info.Repute = (int)dr["Repute"];
                    info.State = (int)dr["State"];
                    info.Right = (int)dr["Right"];
                    info.Offer = (int)dr["Offer"];
                    info.Colors = dr["Colors"].ToString();
                    info.Style = dr["Style"].ToString();
                    info.Hide = (int)dr["Hide"];
                    info.Skin = dr["Skin"] == null ? "" : info.Skin;
                    info.Level = (int)dr["Level"];
                    info.LastDate = (DateTime)dr["LastDate"];
                    info.Sex = (bool)dr["Sex"];
                    info.IsBanChat = (bool)dr["IsBanChat"];
                    info.Win = (int)dr["Win"];
                    info.Total = (int)dr["Total"];
                    info.Escape = (int)dr["Escape"];
                    info.RichesOffer = (int)dr["RichesOffer"];
                    info.RichesRob = (int)dr["RichesRob"];
                    //info.LoginName = dr["LoginName"] == null ? "" : info.LoginName;
                    info.LoginName = dr["LoginName"] == null ? "" : dr["LoginName"].ToString();
                    info.Nimbus = (int)dr["Nimbus"];
                    info.FightPower = (int)dr["FightPower"];
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
            }

            return infos.ToArray();
        }

        public ConsortiaUserInfo GetConsortiaUsersByUserID(int userID)
        {
            int total = 0;
            ConsortiaUserInfo[] infos = GetConsortiaUsersPage(1, 1, ref total, -1, -1, userID, -1);
            if (infos.Length == 1)
                return infos[0];
            return null;
        }

        #endregion

        #region ConsortiaDuty

        public bool AddConsortiaDuty(ConsortiaDutyInfo info, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@DutyID", info.DutyID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
                para[2] = new SqlParameter("@DutyName", info.DutyName);
                para[3] = new SqlParameter("@Level", info.Level);
                para[4] = new SqlParameter("@UserID", userID);
                para[5] = new SqlParameter("@Right", info.Right);
                para[6] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[6].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaDuty_Add", para);
                info.DutyID = (int)para[0].Value;
                int returnValue = (int)para[6].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddConsortiaDuty.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.AddConsortiaDuty.Msg3";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool DeleteConsortiaDuty(int dutyID, int userID, int consortiaID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserID", userID);
                para[1] = new SqlParameter("@ConsortiaID", consortiaID);
                para[2] = new SqlParameter("@DutyID", dutyID);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_ConsortiaDuty_Delete", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg3";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpdateConsortiaDuty(ConsortiaDutyInfo info, int userID, int updateType, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@DutyID", info.DutyID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
                para[2] = new SqlParameter("@DutyName", System.Data.SqlDbType.NVarChar, 100);
                para[2].Direction = ParameterDirection.InputOutput;
                para[2].Value = info.DutyName;
                para[3] = new SqlParameter("@Right", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.InputOutput;
                para[3].Value = info.Right;
                para[4] = new SqlParameter("@Level", System.Data.SqlDbType.Int);
                para[4].Direction = ParameterDirection.InputOutput;
                para[4].Value = info.Level;
                para[5] = new SqlParameter("@UserID", userID);
                para[6] = new SqlParameter("@UpdateType", updateType);
                para[7] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaDuty_Update", para);

                int returnValue = (int)para[7].Value;
                result = returnValue == 0;
                if (result)
                {
                    info.DutyID = (int)para[0].Value;
                    info.DutyName = para[2].Value == null ? "" : para[2].Value.ToString();
                    info.Right = (int)para[3].Value;
                    info.Level = (int)para[4].Value;
                }
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg2";
                        break;
                    case 3:
                    case 4:
                        msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg3";
                        break;
                    case 5:
                        msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg5";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public ConsortiaDutyInfo[] GetConsortiaDutyPage(int page, int size, ref int total, int order, int consortiaID, int dutyID)
        {
            List<ConsortiaDutyInfo> infos = new List<ConsortiaDutyInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (consortiaID != -1)
                {
                    sWhere += " and ConsortiaID =" + consortiaID + " ";
                }
                if (dutyID != -1)
                {
                    sWhere += " and DutyID =" + dutyID + " ";
                }
                string sOrder = "Level";
                switch (order)
                {
                    case 1:
                        sOrder = "DutyName";
                        break;
                }
                sOrder += ",DutyID ";

                DataTable dt = GetPage("Consortia_Duty", sWhere, page, size, "*", sOrder, "DutyID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                    info.DutyID = (int)dr["DutyID"];
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.DutyID = (int)dr["DutyID"];
                    info.DutyName = dr["DutyName"].ToString();
                    info.IsExist = (bool)dr["IsExist"];
                    info.Right = (int)dr["Right"];
                    info.Level = (int)dr["Level"];
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
            }

            return infos.ToArray();
        }

        #endregion

        #region ConsortiaApplyAlly

        public int[] GetConsortiaByAllyByState(int consortiaID, int state)
        {
            List<int> infos = new List<int>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@State", state);
                db.GetReader(ref reader, "SP_Consortia_AllyByState", para);
                while (reader.Read())
                {
                    infos.Add((int)reader["Consortia2ID"]);
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

        public bool AddConsortiaApplyAlly(ConsortiaApplyAllyInfo info, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
                para[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
                para[3] = new SqlParameter("@Date", info.Date);
                para[4] = new SqlParameter("@Remark", info.Remark);
                para[5] = new SqlParameter("@IsExist", info.IsExist);
                para[6] = new SqlParameter("@UserID", userID);
                para[7] = new SqlParameter("@State", info.State);
                para[8] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_ConsortiaApplyAlly_Add", para);
                info.ID = (int)para[0].Value;
                int returnValue = (int)para[8].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg3";
                        break;
                    case 4:
                        msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg4";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public bool DeleteConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", applyID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@ConsortiaID", consortiaID);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_ConsortiaApplyAlly_Delete", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.DeleteConsortiaApplyAlly.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool PassConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref int tempID, ref int state, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@ID", applyID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@ConsortiaID", consortiaID);
                para[3] = new SqlParameter("@tempID", tempID);
                para[3].Direction = ParameterDirection.InputOutput;
                para[4] = new SqlParameter("@State", state);
                para[4].Direction = ParameterDirection.InputOutput;
                para[5] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[5].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_ConsortiaApplyAlly_Pass", para);
                int returnValue = (int)para[5].Value;
                //result = returnValue == 0;
                if (returnValue == 0)
                {
                    result = true;
                    tempID = (int)para[3].Value;
                    state = (int)para[4].Value;
                }
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg3";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ConsortiaApplyAllyInfo[] GetConsortiaApplyAllyPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int state)
        {
            List<ConsortiaApplyAllyInfo> infos = new List<ConsortiaApplyAllyInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (consortiaID != -1)
                {
                    sWhere += " and Consortia2ID =" + consortiaID + " ";
                }
                if (applyID != -1)
                {
                    sWhere += " and ID =" + applyID + " ";
                }
                if (state != -1)
                {
                    sWhere += " and State =" + state + " ";
                }
                string sOrder = "ConsortiaName";
                switch (order)
                {
                    case 1:
                        sOrder = "Repute";
                        break;
                    case 2:
                        sOrder = "ChairmanName";
                        break;
                    case 3:
                        sOrder = "Count";
                        break;
                    case 4:
                        sOrder = "Level";
                        break;
                    case 5:
                        sOrder = "Honor";
                        break;
                }
                sOrder += ",ID ";

                DataTable dt = GetPage("V_Consortia_Apply_Ally", sWhere, page, size, "*", sOrder, "ID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaApplyAllyInfo info = new ConsortiaApplyAllyInfo();
                    info.ID = (int)dr["ID"];
                    info.CelebCount = (int)dr["CelebCount"];
                    info.ChairmanName = dr["ChairmanName"].ToString();
                    info.Consortia1ID = (int)dr["Consortia1ID"];
                    info.Consortia2ID = (int)dr["Consortia2ID"];
                    info.ConsortiaName = dr["ConsortiaName"].ToString();
                    info.Count = (int)dr["Count"];
                    info.Date = (DateTime)dr["Date"];
                    info.Honor = (int)dr["Honor"];
                    info.IsExist = (bool)dr["IsExist"];
                    info.Remark = dr["Remark"].ToString();
                    info.Repute = (int)dr["Repute"];
                    info.State = (int)dr["State"];
                    info.Level = (int)dr["Level"];
                    info.Description = dr["Description"] == null ? "" : dr["Description"].ToString();
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
            }

            return infos.ToArray();
        }

        #endregion

        #region ConsortiaAlly


        public Dictionary<int, int> GetConsortiaByAlly(int consortiaID)
        {
            Dictionary<int, int> consortiaIDs = new Dictionary<int, int>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                db.GetReader(ref reader, "SP_Consortia_Ally_Neutral", para);

                while (reader.Read())
                {
                    if ((int)reader["Consortia1ID"] != consortiaID)
                    {
                        consortiaIDs.Add((int)reader["Consortia1ID"], (int)reader["State"]);
                    }
                    else
                    {
                        consortiaIDs.Add((int)reader["Consortia2ID"], (int)reader["State"]);
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetConsortiaByAlly", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return consortiaIDs;
        }


        public bool AddConsortiaAlly(ConsortiaAllyInfo info, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
                para[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
                para[3] = new SqlParameter("@State", info.State);
                para[4] = new SqlParameter("@Date", info.Date);
                para[5] = new SqlParameter("@ValidDate", info.ValidDate);
                para[6] = new SqlParameter("@IsExist", info.IsExist);
                para[7] = new SqlParameter("@UserID", userID);
                para[8] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_ConsortiaAlly_Add", para);
                int returnValue = (int)para[8].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddConsortiaAlly.Msg2";
                        break;
                    case 3:
                        msg = "ConsortiaBussiness.AddConsortiaAlly.Msg3";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ConsortiaAllyInfo[] GetConsortiaAllyPage(int page, int size, ref int total, int order, int consortiaID, int state, string name)
        {
            List<ConsortiaAllyInfo> infos = new List<ConsortiaAllyInfo>();

            string sWhere = " IsExist=1 and ConsortiaID<>" + consortiaID;
            Dictionary<int, int> consortiaIDs = GetConsortiaByAlly(consortiaID);
            try
            {
                if (state != -1)
                {
                    string ids = string.Empty;
                    foreach (int id in consortiaIDs.Keys)
                    {
                        ids += id + ",";
                    }
                    ids += 0;

                    if (state == 0)
                    {
                        sWhere += " and ConsortiaID not in (" + ids + ") ";
                    }
                    else
                    {
                        sWhere += " and ConsortiaID in (" + ids + ") ";
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    sWhere += " and ConsortiaName like '%" + name + "%' ";
                }

                DataTable dt = GetPage("Consortia", sWhere, page, size, "*", "ConsortiaID", "ConsortiaID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaAllyInfo info = new ConsortiaAllyInfo();
                    //info.ID = (int)dr["ID"];
                    info.Consortia1ID = (int)dr["ConsortiaID"];
                    info.ConsortiaName1 = dr["ConsortiaName"] == null ? "" : dr["ConsortiaName"].ToString();
                    info.ConsortiaName2 = "";
                    info.Count1 = (int)dr["Count"];
                    info.Repute1 = (int)dr["Repute"];
                    info.ChairmanName1 = dr["ChairmanName"] == null ? "" : dr["ChairmanName"].ToString();
                    info.ChairmanName2 = "";
                    info.Level1 = (int)dr["Level"];
                    info.Honor1 = (int)dr["Honor"];
                    info.Description1 = dr["Description"] == null ? "" : dr["Description"].ToString();
                    info.Description2 = "";
                    info.Riches1 = (int)dr["Riches"];
                    info.Date = DateTime.Now;
                    info.IsExist = true;
                    if (consortiaIDs.ContainsKey(info.Consortia1ID))
                    {
                        info.State = consortiaIDs[info.Consortia1ID];
                    }
                    info.ValidDate = 0;
                    infos.Add(info);
                }

                //try
                //{
                //    string sWhere = " IsExist=1 ";
                //    if (consortiaID != -1)
                //    {
                //        sWhere += " and ((Consortia1ID =" + consortiaID + " or Consortia2ID = " + consortiaID + " ";

                //        sWhere += " and ((Consortia1ID =" + consortiaID + " ";

                //        if (!string.IsNullOrEmpty(name))
                //        {
                //            sWhere += " and ConsortiaName2 like '%" + name + "%'";
                //        }

                //        sWhere += ")";

                //        sWhere += "  or (Consortia2ID = " + consortiaID + " ";

                //        if (!string.IsNullOrEmpty(name))
                //        {
                //            sWhere += " and ConsortiaName1 like '%" + name + "%'";
                //        }
                //        sWhere += "))";

                //    }
                //    if (state != -1)
                //    {
                //        sWhere += " and State =" + state + " ";
                //    }
                //    if (string.IsNullOrEmpty(name))
                //    {
                //        sWhere += " and (ConsortiaName1 like '%" + name + "%' or ConsortiaName2 like '%" + name + "%') ";
                //    }
                //    string sOrder = "ConsortiaName1,ConsortiaName2";
                //    switch (order)
                //    {
                //        case 1:
                //            sOrder = "Repute1,Repute2";
                //            break;
                //        case 2:
                //            sOrder = "ChairmanName1,ChairmanName2";
                //            break;
                //        case 3:
                //            sOrder = "Count1,Count2";
                //            break;
                //        case 4:
                //            sOrder = "Level1,Level2";
                //            break;
                //        case 5:
                //            sOrder = "Honor1,Honor2";
                //            break;

                //    }
                //    sOrder += ",ID ";

                //    DataTable dt = GetPage("V_Consortia_Ally", sWhere, page, size, "*", sOrder, "ID", ref total);
                //foreach (DataRow dr in dt.Rows)
                //{
                //    ConsortiaAllyInfo info = new ConsortiaAllyInfo();
                //   // info.ID = (int)dr["ID"];
                //    info.Consortia1ID = (int)dr["Consortia1ID"];
                //    if (info.Consortia1ID != consortiaID)
                //    {
                //        info.Consortia2ID = (int)dr["Consortia2ID"];
                //        info.ConsortiaName1 = dr["ConsortiaName1"] == null ? "" : dr["ConsortiaName1"].ToString();
                //        info.ConsortiaName2 = dr["ConsortiaName2"] == null ? "" : dr["ConsortiaName2"].ToString();
                //        info.Count1 = (int)dr["Count1"];
                //        info.Count2 = (int)dr["Count2"];
                //        info.Repute1 = (int)dr["Repute1"];
                //        info.Repute2 = (int)dr["Repute2"];
                //        info.ChairmanName1 = dr["ChairmanName1"] == null ? "" : dr["ChairmanName1"].ToString();
                //        info.ChairmanName2 = dr["ChairmanName2"] == null ? "" : dr["ChairmanName2"].ToString();
                //        info.Level1 = (int)dr["Level1"];
                //        info.Level2 = (int)dr["Level2"];
                //        info.Honor1 = (int)dr["Honor1"];
                //        info.Honor2 = (int)dr["Honor2"];
                //        info.Description1 = dr["Description1"] == null ? "" : dr["Description1"].ToString();
                //        info.Description2 = dr["Description2"] == null ? "" : dr["Description2"].ToString();
                //        info.Riches1 = (int)dr["Riches1"];
                //        info.Riches2 = (int)dr["Riches2"];
                //    }
                //    else
                //    {
                //        info.Consortia1ID = (int)dr["Consortia2ID"];
                //        info.Consortia2ID = (int)dr["Consortia1ID"];
                //        info.ConsortiaName1 = dr["ConsortiaName2"] == null ? "" : dr["ConsortiaName2"].ToString();
                //        info.ConsortiaName2 = dr["ConsortiaName1"] == null ? "" : dr["ConsortiaName1"].ToString();
                //        info.Count1 = (int)dr["Count2"];
                //        info.Count2 = (int)dr["Count1"];
                //        info.Repute1 = (int)dr["Repute2"];
                //        info.Repute2 = (int)dr["Repute1"];
                //        info.ChairmanName1 = dr["ChairmanName2"] == null ? "" : dr["ChairmanName2"].ToString();
                //        info.ChairmanName2 = dr["ChairmanName1"] == null ? "" : dr["ChairmanName1"].ToString();
                //        info.Level1 = (int)dr["Level2"];
                //        info.Level2 = (int)dr["Level1"];
                //        info.Honor1 = (int)dr["Honor2"];
                //        info.Honor2 = (int)dr["Honor1"];
                //        info.Description1 = dr["Description2"] == null ? "" : dr["Description2"].ToString();
                //        info.Description2 = dr["Description1"] == null ? "" : dr["Description1"].ToString();
                //        info.Riches1 = (int)dr["Riches2"];
                //        info.Riches2 = (int)dr["Riches1"];
                //    }
                //    info.Date = (DateTime)dr["Date"];
                //    info.IsExist = (bool)dr["IsExist"];
                //    info.State = (int)dr["State"];
                //    info.ValidDate = (int)dr["ValidDate"];
                //    infos.Add(info);
                //}

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetConsortiaAllyPage", e);
            }
            finally
            {
            }

            return infos.ToArray();
        }

        public ConsortiaAllyInfo[] GetConsortiaAllyAll()
        {
            List<ConsortiaAllyInfo> infos = new List<ConsortiaAllyInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_ConsortiaAlly_All");
                while (reader.Read())
                {
                    ConsortiaAllyInfo info = new ConsortiaAllyInfo();
                    info.Consortia1ID = (int)reader["Consortia1ID"];
                    info.Consortia2ID = (int)reader["Consortia2ID"];
                    info.Date = (DateTime)reader["Date"];
                    info.ID = (int)reader["ID"];
                    info.State = (int)reader["State"];
                    info.ValidDate = (int)reader["ValidDate"];
                    info.IsExist = (bool)reader["IsExist"];
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

        #endregion

        #region ConsortiaEvent

        public ConsortiaEventInfo[] GetConsortiaEventPage(int page, int size, ref int total, int order, int consortiaID)
        {
            List<ConsortiaEventInfo> infos = new List<ConsortiaEventInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (consortiaID != -1)
                {
                    sWhere += " and ConsortiaID =" + consortiaID + " ";
                }

                //string sOrder = "ID";
                //switch (order)
                //{
                //    case 1:
                //        sOrder = "Remark";
                //        break;
                //    case 2:
                //        sOrder = "Date";
                //        break;
                //}
                string sOrder = "Date desc,ID ";

                DataTable dt = GetPage("Consortia_Event", sWhere, page, size, "*", sOrder, "ID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaEventInfo info = new ConsortiaEventInfo();
                    info.ID = (int)dr["ID"];
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.Date = (DateTime)dr["Date"];
                    info.IsExist = (bool)dr["IsExist"];
                    info.Remark = dr["Remark"].ToString();
                    info.Type = (int)dr["Type"];
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
            }

            return infos.ToArray();
        }

        #endregion

        #region ConsortiaLevel

        public ConsortiaLevelInfo[] GetAllConsortiaLevel()
        {
            List<ConsortiaLevelInfo> infos = new List<ConsortiaLevelInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Consortia_Level_All");
                while (reader.Read())
                {
                    ConsortiaLevelInfo info = new ConsortiaLevelInfo();
                    info.Count = (int)reader["Count"];
                    info.Deduct = (int)reader["Deduct"];
                    info.Level = (int)reader["Level"];
                    info.NeedGold = (int)reader["NeedGold"];
                    info.NeedItem = (int)reader["NeedItem"];
                    info.Reward = (int)reader["Reward"];
                    info.Riches = (int)reader["Riches"];
                    info.ShopRiches = (int)reader["ShopRiches"];
                    info.SmithRiches = (int)reader["SmithRiches"];
                    info.StoreRiches = (int)reader["StoreRiches"];
                    infos.Add(info);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetAllConsortiaLevel", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return infos.ToArray();
        }

        #endregion

        #region ConsortiaEuqipControl

        //Type 1表示商城，2表示铁匠铺
        public bool AddAndUpdateConsortiaEuqipControl(ConsortiaEquipControlInfo info, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
                para[1] = new SqlParameter("@Level", info.Level);
                para[2] = new SqlParameter("@Type", info.Type);
                para[3] = new SqlParameter("@Riches", info.Riches);
                para[4] = new SqlParameter("@UserID", userID);
                para[5] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[5].Direction = ParameterDirection.ReturnValue;

                result = db.RunProcedure("SP_Consortia_Equip_Control_Add", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;

                switch (returnValue)
                {
                    case 2:
                        msg = "ConsortiaBussiness.AddAndUpdateConsortiaEuqipControl.Msg2";
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }
            return result;
        }

        public ConsortiaEquipControlInfo GetConsortiaEuqipRiches(int consortiaID, int Level, int type)
        {
            ConsortiaEquipControlInfo info = null;
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ConsortiaID", consortiaID);
                para[1] = new SqlParameter("@Level", Level);
                para[2] = new SqlParameter("@Type", type);

                db.GetReader(ref reader, "SP_Consortia_Equip_Control_Single", para);

                while (reader.Read())
                {
                    info = new ConsortiaEquipControlInfo();
                    info.ConsortiaID = (int)reader["ConsortiaID"];
                    info.Level = (int)reader["Level"];
                    info.Riches = (int)reader["Riches"];
                    info.Type = (int)reader["Type"];
                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetAllConsortiaLevel", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            if (info == null)
            {
                info = new ConsortiaEquipControlInfo();
                info.ConsortiaID = consortiaID;
                info.Level = Level;
                info.Riches = 100;
                info.Type = type;
            }
            return info;

        }

        public ConsortiaEquipControlInfo[] GetConsortiaEquipControlPage(int page, int size, ref int total, int order, int consortiaID, int level, int type)
        {
            List<ConsortiaEquipControlInfo> infos = new List<ConsortiaEquipControlInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (consortiaID != -1)
                {
                    sWhere += " and ConsortiaID =" + consortiaID + " ";
                }
                if (level != -1)
                {
                    sWhere += " and Level =" + level + " ";
                }
                if (type != -1)
                {
                    sWhere += " and Type =" + type + " ";
                }

                string sOrder = "ConsortiaID ";

                DataTable dt = GetPage("Consortia_Equip_Control", sWhere, page, size, "*", sOrder, "ConsortiaID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    ConsortiaEquipControlInfo info = new ConsortiaEquipControlInfo();
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.Level = (int)dr["Level"];
                    info.Riches = (int)dr["Riches"];
                    info.Type = (int)dr["Type"];

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
            }

            return infos.ToArray();
        }

        #endregion


    }
}
