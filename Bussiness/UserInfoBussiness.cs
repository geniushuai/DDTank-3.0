using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class UserInfoBussiness : BaseBussiness
    {
        /// <summary>
        /// 通过Uid查询本地DB
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="userName"></param>
        /// <param name="portrait"></param>
        public bool GetFromDbByUid(string uid, ref string userName, ref string portrait)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@Uid", uid);

                db.GetReader(ref reader, "SP_User_Info_QueryByUid", para);
                while (reader.Read())
                {
                    userName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
                    portrait = reader["Portrait"] == null ? "" : reader["Portrait"].ToString();
                }
                if ((!string.IsNullOrEmpty(userName)) && (!string.IsNullOrEmpty(portrait)))
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
        /// 在本地DB中添加UserInfo
        /// </summary>SP_User_Info_Insert
        /// <param name="uid"></param>
        /// <param name="userName"></param>
        /// <param name="portrait"></param>
        /// <returns></returns>
        public bool AddUserInfo(string uid, string userName, string portrait)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@Uid", uid);
                para[1] = new SqlParameter("@UserName", userName);
                para[2] = new SqlParameter("@Portrait", portrait);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_User_Info_Insert", para);
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
