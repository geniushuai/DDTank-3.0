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
using Bussiness.CenterService;

namespace Bussiness
{
    public class ManageBussiness : BaseBussiness
    {
        #region KitoffUser

        public int KitoffUserByUserName(string name,string msg)
        {
            int result = 1;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo player = db.GetUserSingleByUserName(name);
                if (player == null)
                    return 2;

                result = KitoffUser(player.ID, msg);
            }
            return result;
        }

        public int KitoffUserByNickName(string name, string msg)
        {
            int result = 1;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo player = db.GetUserSingleByNickName(name);
                if (player == null)
                    return 2;

                result = KitoffUser(player.ID, msg);
            }
            return result;
        }

        public int KitoffUser(int id, string msg)
        {
            try
            {
                using (CenterServiceClient temp = new CenterServiceClient())
                {
                    if (temp.KitoffUser(id, msg))
                        return 0;
                    else
                        return 3;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("KitoffUser", e);
                return 1;
            }
        }

        #endregion

        #region Notice

        public bool SystemNotice(string msg)
        {
            bool  result = false;
            try
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    using (CenterServiceClient temp = new CenterServiceClient())
                    {
                        if (temp.SystemNotice(msg))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SystemNotice", e);
            }
            return result;
        }

        #endregion

        #region  Forbid

        /// <summary>
        /// 禁止用户登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isExist"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist)
        {
            return ForbidPlayer(userName, nickName, userID, forbidDate, isExist, "");
        }

        private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist, string ForbidReason)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@NickName", nickName);
                para[2] = new SqlParameter("@UserID", userID);
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@ForbidDate", forbidDate);
                para[4] = new SqlParameter("@IsExist", isExist);
                para[5] = new SqlParameter("@ForbidReason", ForbidReason);
                db.RunProcedure("SP_Admin_ForbidUser", para);
                userID = (int)para[2].Value;
                if (userID > 0)
                {
                    result = true;
                    if (!isExist)
                        KitoffUser(userID, "You are kicking out by GM!!");
                    //KitoffUser(userID, LanguageMgr.GetTranslation("ManageBussiness.Forbid"));
                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        //
        public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist)
        {
            return ForbidPlayer(userName, "", 0, date, isExist);
        }

        public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist)
        {
            return ForbidPlayer("", nickName, 0, date, isExist);
        }

        public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist)
        {
            return ForbidPlayer("", "", userID, date, isExist);
        }

        //重载
        public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist, string ForbidReason)
        {
            return ForbidPlayer(userName, "", 0, date, isExist,ForbidReason);
        }

        public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist, string ForbidReason)
        {
            return ForbidPlayer("", nickName, 0, date, isExist, ForbidReason);
        }

        public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist, string ForbidReason)
        {
            return ForbidPlayer("", "", userID, date, isExist, ForbidReason);
        }

        #endregion

        #region reload

        public bool ReLoadServerList()
        {
            bool result = false;
            try
            {
                using (CenterServiceClient temp = new CenterServiceClient())
                {
                    if (temp.ReLoadServerList())
                    {
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ReLoadServerList", e);
            }
            return result;
        }

        public int GetConfigState(int type)
        {
            int result = 2;
            try
            {
                using (CenterServiceClient temp = new CenterServiceClient())
                {
                    return temp.GetConfigState(type);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetConfigState", e);
            }
            return result;
        }

        public bool UpdateConfigState(int type, bool state)
        {
            bool result = false;
            try
            {
                using (CenterServiceClient temp = new CenterServiceClient())
                {
                    return temp.UpdateConfigState(type,state);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdateConfigState", e);
            }
            return result;
        }

        public bool Reload(string type)
        {
            bool result = false;
            try
            {
                using (CenterServiceClient temp = new CenterServiceClient())
                {
                    return temp.Reload(type);
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Reload", e);
            }
            return result;
        }

        #endregion



    }
}
