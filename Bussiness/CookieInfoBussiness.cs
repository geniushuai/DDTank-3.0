using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class CookieInfoBussiness : BaseBussiness
    {
        /// <summary>
        /// 根据bdSigUser从数据库获取CookieInfo
        /// </summary>
        /// <param name="bdSigUser"></param>
        /// <param name="bdSigPortrait"></param>
        /// <param name="bdSigSessionKey"></param>
        /// <returns></returns>
        public bool GetFromDbByUser(string bdSigUser, ref string bdSigPortrait, ref string bdSigSessionKey)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@BdSigUser", bdSigUser);

                db.GetReader(ref reader, "SP_Cookie_Info_QueryByUser", para);
                while (reader.Read())
                {
                    bdSigPortrait = reader["BdSigPortrait"] == null ? "" : reader["BdSigPortrait"].ToString();
                    bdSigSessionKey = reader["BdSigSessionKey"] == null ? "" : reader["BdSigSessionKey"].ToString();
                }
                if ((!string.IsNullOrEmpty(bdSigPortrait)) && (!string.IsNullOrEmpty(bdSigSessionKey)))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
                return false;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
        }

        /// <summary>
        /// 添加CookieInfo到数据库中。如果数据库中已经存在该用户，则根据bgSigUser更新其他字段的值
        /// </summary>
        /// <param name="bdSigUser"></param>
        /// <param name="bdSigPortrait"></param>
        /// <param name="bdSigSessionKey"></param>
        /// <returns></returns>
        public bool AddCookieInfo(string bdSigUser, string bdSigPortrait, string bdSigSessionKey)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@BdSigUser", bdSigUser);
                para[1] = new SqlParameter("@BdSigPortrait", bdSigPortrait);
                para[2] = new SqlParameter("@BdSigSessionKey", bdSigSessionKey);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Cookie_Info_Insert", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
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
