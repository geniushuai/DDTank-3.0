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
using System.Collections;

namespace Bussiness
{
    public class PlayerBussiness : BaseBussiness
    {

        #region PlayerInfo

        public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
        {
            bool result = false;
            try
            {
                player = new PlayerInfo();
                player.Agility = 0;
                player.Attack = 0;
                player.Colors = ",,,,,,";
                player.Skin = "";
                player.ConsortiaID = 0;
                player.Defence = 0;
                player.Gold = gold;
                player.GP = 1;
                player.Grade = 1;
                player.ID = 0;
                player.Luck = 0;
                player.Money = money;
                player.NickName = "";
                player.Sex = sex;
                player.State = 0;
                player.Style = ",,,,,,";
                player.Hide = 1111111111;
                //isFirst = true;

                SqlParameter[] para = new SqlParameter[21];
                para[0] = new SqlParameter("@UserID", System.Data.SqlDbType.Int);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Attack", player.Attack);
                para[2] = new SqlParameter("@Colors", player.Colors == null ? "" : player.Colors);
                para[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                para[4] = new SqlParameter("@Defence", player.Defence);
                para[5] = new SqlParameter("@Gold", player.Gold);
                para[6] = new SqlParameter("@GP", player.GP);
                para[7] = new SqlParameter("@Grade", player.Grade);
                para[8] = new SqlParameter("@Luck", player.Luck);
                para[9] = new SqlParameter("@Money", player.Money);
                para[10] = new SqlParameter("@Style", player.Style == null ? "" : player.Style);
                para[11] = new SqlParameter("@Agility", player.Agility);
                para[12] = new SqlParameter("@State", player.State);
                para[13] = new SqlParameter("@UserName", userName);
                para[14] = new SqlParameter("@PassWord", passWord);
                para[15] = new SqlParameter("@Sex", sex);
                para[16] = new SqlParameter("@Hide", player.Hide);
                para[17] = new SqlParameter("@ActiveIP", IP);
                para[18] = new SqlParameter("@Skin", player.Skin == null ? "" : player.Skin);
                para[19] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[19].Direction = ParameterDirection.ReturnValue;
                para[20] = new SqlParameter("@Site", site);
                result = db.RunProcedure("SP_Users_Active", para);
                player.ID = (int)para[0].Value;
                result = (int)para[19].Value == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, int sex, ref string msg, int validDate)
        {
            bool result = false;
            try
            {
                string[] bStyles = bStyle.Split(',');
                string[] gStyles = gStyle.Split(',');
                //
                SqlParameter[] para = new SqlParameter[18];

                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@PassWord", passWord);
                para[2] = new SqlParameter("@NickName", nickName);
                para[3] = new SqlParameter("@BArmID", bStyles[0]);
                para[4] = new SqlParameter("@BHairID", bStyles[1]);
                para[5] = new SqlParameter("@BFaceID", bStyles[2]);
                para[6] = new SqlParameter("@BClothID", bStyles[3]);
                para[7] = new SqlParameter("@GArmID", gStyles[0]);
                para[8] = new SqlParameter("@GHairID", gStyles[1]);
                para[9] = new SqlParameter("@GFaceID", gStyles[2]);
                para[10] = new SqlParameter("@GClothID", gStyles[3]);
                para[11] = new SqlParameter("@ArmColor", armColor);
                para[12] = new SqlParameter("@HairColor", hairColor);
                para[13] = new SqlParameter("@FaceColor", faceColor);
                para[14] = new SqlParameter("@ClothColor", clothColor);
                para[15] = new SqlParameter("@Sex", sex);
                para[16] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[16].Direction = ParameterDirection.ReturnValue;
                para[17] = new SqlParameter("@StyleDate", validDate);

                result = db.RunProcedure("SP_Users_RegisterNotValidate", para);
                int returnValue = (int)para[16].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    //case 2:
                    //    msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2");
                    //    break;
                    //case 3:
                    //    msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3");
                    //    break;

                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2");
                        break;
                    case 3:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3");
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

        public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@NickName", nickName);
                para[2] = new SqlParameter("@NewNickName", newNickName);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;

                result = db.RunProcedure("SP_Users_RenameNick", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4");
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("RenameNick", e);
            }
            return result;
        }

        public bool DisableUser(string userName, bool isExit)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@IsExist", isExit);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_Disable_User", para);
                if ((int)para[2].Value == 0) result = true;
 
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DisableUser", e);
            }
            return result;
        }
        public bool RenameConsortiaName(string userName, string nickName, string consortiaName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@NickName", nickName);
                para[2] = new SqlParameter("@ConsortiaName", consortiaName);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;

                result = db.RunProcedure("SP_Users_RenameConsortiaName", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.SP_Users_RenameConsortiaName.Msg4");
                        break;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("RenameNick", e);
            }
            return result;
        }

        public bool UpdatePassWord(int userID, string password)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@UserID", userID);
                para[1] = new SqlParameter("@Password", password);
                result = db.RunProcedure("SP_Users_UpdatePassword", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@UserID", userID);
                para[1] = new SqlParameter("@PasswordQuestion1", PasswordQuestion1);
                para[2] = new SqlParameter("@PasswordAnswer1", PasswordAnswer1);
                para[3] = new SqlParameter("@PasswordQuestion2", PasswordQuestion2);
                para[4] = new SqlParameter("@PasswordAnswer2", PasswordAnswer2);
                para[5] = new SqlParameter("@FailedPasswordAttemptCount", Count);

                result = db.RunProcedure("SP_Sys_Users_Password_Add", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
        {
            DateTime Today;
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", userID);

                db.GetReader(ref reader, "SP_Users_PasswordInfo", para);
                while (reader.Read())
                {
                    PasswordQuestion1 = reader["PasswordQuestion1"] == null ? "" : reader["PasswordQuestion1"].ToString();
                    PasswordAnswer1 = reader["PasswordAnswer1"] == null ? "" : reader["PasswordAnswer1"].ToString();
                    PasswordQuestion2 = reader["PasswordQuestion2"] == null ? "" : reader["PasswordQuestion2"].ToString();
                    PasswordAnswer2 = reader["PasswordAnswer2"] == null ? "" : reader["PasswordAnswer2"].ToString();
                    Today = (DateTime)reader["LastFindDate"];
                    if (Today == DateTime.Today)
                        Count = (int)reader["FailedPasswordAttemptCount"];
                    else
                        Count = 5;
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
        }

        public bool UpdatePasswordTwo(int userID, string passwordTwo)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@UserID", userID);
                para[1] = new SqlParameter("@PasswordTwo", passwordTwo);
                result = db.RunProcedure("SP_Users_UpdatePasswordTwo", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }



        public PlayerInfo[] GetUserLoginList(string userName)
        {
            List<PlayerInfo> infos = new List<PlayerInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserName", SqlDbType.VarChar, 200);
                para[0].Value = userName;
                db.GetReader(ref reader, "SP_Users_LoginList", para);
                while (reader.Read())
                {
                    infos.Add(InitPlayerInfo(reader));
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

        /// <summary>
        /// 用户登陆,如果锁定ID返回-2
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isFirst"></param>
        /// <param name="isExist"></param>
        /// <returns></returns>
        public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname)
        {
            SqlDataReader reader = null;
            try
            {
                //SqlParameter[] para = new SqlParameter[1];
                //para[0] = new SqlParameter("@UserName", username);
                //db.GetReader(ref reader, "SP_Users_SingleByUserName", para);

                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserName", username);
                para[1] = new SqlParameter("@Password", "");
                para[2] = new SqlParameter("@FirstValidate", firstValidate);
                para[3] = new SqlParameter("@Nickname", nickname);
                db.GetReader(ref reader, "SP_Users_LoginWeb", para);
                while (reader.Read())
                {
                    isFirst = (int)reader["IsFirst"];
                    isExist = (bool)reader["IsExist"];
                    forbidDate = (DateTime)reader["ForbidDate"];
                    //先加后查
                    if (isFirst > 1)
                        isFirst--;
                    return InitPlayerInfo(reader);
                }
            }
            catch (Exception e)
            {
                isError = true;
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

        public PlayerInfo LoginGame(string username, string password)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@UserName", username);
                para[1] = new SqlParameter("@Password", password);
                db.GetReader(ref reader, "SP_Users_Login", para);
                while (reader.Read())
                {
                    return InitPlayerInfo(reader);
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

        public bool UpdatePlayer(PlayerInfo player)
        {
            bool result = false;
            try
            {
                if (player.Grade < 1)
                    return result;

                SqlParameter[] para = new SqlParameter[35];
                para[0] = new SqlParameter("@UserID", player.ID);
                para[1] = new SqlParameter("@Attack", player.Attack);
                para[2] = new SqlParameter("@Colors", player.Colors == null ? "" : player.Colors);
                para[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                para[4] = new SqlParameter("@Defence", player.Defence);
                para[5] = new SqlParameter("@Gold", player.Gold);
                para[6] = new SqlParameter("@GP", player.GP);
                para[7] = new SqlParameter("@Grade", player.Grade);
                para[8] = new SqlParameter("@Luck", player.Luck);
                para[9] = new SqlParameter("@Money", player.Money);
                para[10] = new SqlParameter("@Style", player.Style == null ? "" : player.Style);
                para[11] = new SqlParameter("@Agility", player.Agility);
                para[12] = new SqlParameter("@State", player.State);
                para[13] = new SqlParameter("@Hide", player.Hide);
                para[14] = new SqlParameter("@ExpendDate", player.ExpendDate == null ? "" : player.ExpendDate.ToString());
                para[15] = new SqlParameter("@Win", player.Win);
                para[16] = new SqlParameter("@Total", player.Total);
                para[17] = new SqlParameter("@Escape", player.Escape);
                para[18] = new SqlParameter("@Skin", player.Skin == null ? "" : player.Skin);
                para[19] = new SqlParameter("@Offer", player.Offer);
                para[20] = new SqlParameter("@AntiAddiction", player.AntiAddiction);
                para[20].Direction = ParameterDirection.InputOutput;
                para[21] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[21].Direction = ParameterDirection.ReturnValue;
                para[22] = new SqlParameter("@RichesOffer", player.RichesOffer);
                para[23] = new SqlParameter("@RichesRob", player.RichesRob);
                para[24] = new SqlParameter("@CheckCount", player.CheckCount);
                para[24].Direction = ParameterDirection.InputOutput;
                para[25] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
                para[26] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
                para[27] = new SqlParameter("@Nimbus", player.Nimbus);
                para[28] = new SqlParameter("@LastAward", player.LastAward);
                para[29] = new SqlParameter("@GiftToken", player.GiftToken);
                para[30] = new SqlParameter("@QuestSite", player.QuestSite);
                para[31] = new SqlParameter("@PvePermission", player.PvePermission);
                para[32] = new SqlParameter("@FightPower", player.FightPower);
                para[33] = new SqlParameter("@AnswerSite", player.AnswerSite);
                
                para[34] = new SqlParameter("@LastAuncherAward", player.LastAward);
                //para[34].Direction = ParameterDirection.Output;
                db.RunProcedure("SP_Users_Update", para);
	           
                result = (int)para[21].Value == 0;
                //TrieuLSL
                //result = true;
                if (result)
                {
                    player.AntiAddiction = (int)para[20].Value;
                    player.CheckCount = (int)para[24].Value;
                }
                player.IsDirty = false;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpdatePlayerMarry(PlayerInfo player)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@UserID", player.ID);
                para[1] = new SqlParameter("@IsMarried", player.IsMarried);
                para[2] = new SqlParameter("@SpouseID", player.SpouseID);
                para[3] = new SqlParameter("@SpouseName", player.SpouseName);
                para[4] = new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom);
                para[5] = new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID);
                para[6] = new SqlParameter("@IsGotRing", player.IsGotRing);

                result = db.RunProcedure("SP_Users_Marry", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdatePlayerMarry", e);
            }
            return result;
        }

        /// <summary>
        /// 更新用户最后获取物品时间
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool UpdatePlayerLastAward(int id)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", id);
                result = db.RunProcedure("SP_Users_LastAward", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdatePlayerAward", e);
            }
            return result;
        }

        public PlayerInfo GetUserSingleByUserID(int UserID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                db.GetReader(ref reader, "SP_Users_SingleByUserID", para);
                while (reader.Read())
                {
                    return InitPlayerInfo(reader);
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

        public PlayerInfo GetUserSingleByUserName(string userName)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserName", SqlDbType.VarChar, 200);
                para[0].Value = userName;
                db.GetReader(ref reader, "SP_Users_SingleByUserName", para);
                while (reader.Read())
                {
                    return InitPlayerInfo(reader);
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

        public PlayerInfo GetUserSingleByNickName(string nickName)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@NickName", SqlDbType.VarChar, 200);
                para[0].Value = nickName;
                db.GetReader(ref reader, "SP_Users_SingleByNickName", para);
                while (reader.Read())
                {
                    return InitPlayerInfo(reader);
                }
            }
            catch
            {
                throw (new Exception());
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return null;
        }

        public PlayerInfo InitPlayerInfo(SqlDataReader reader)
        {
            PlayerInfo player = new PlayerInfo();
            player.Password = (string)reader["Password"];
            player.IsConsortia = (bool)reader["IsConsortia"];
            player.Agility = (int)reader["Agility"];
            player.Attack = (int)reader["Attack"];
            player.Colors = reader["Colors"] == null ? "" : reader["Colors"].ToString();
            player.ConsortiaID = (int)reader["ConsortiaID"];
            player.Defence = (int)reader["Defence"];
            player.Gold = (int)reader["Gold"];
            player.GP = (int)reader["GP"];
            player.Grade = (int)reader["Grade"];
            player.ID = (int)reader["UserID"];
            player.Luck = (int)reader["Luck"];
            player.Money = (int)reader["Money"];
            player.NickName = reader["NickName"] == null ? "" : reader["NickName"].ToString();
            player.Sex = (bool)reader["Sex"];
            player.State = (int)reader["State"];
            player.Style = reader["Style"] == null ? "" : reader["Style"].ToString();
            player.Hide = (int)reader["Hide"];
            player.Repute = (int)reader["Repute"];
            player.UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
            player.ConsortiaName = reader["ConsortiaName"] == null ? "" : reader["ConsortiaName"].ToString();
            player.Offer = (int)reader["Offer"];
            player.Win = (int)reader["Win"];
            player.Total = (int)reader["Total"];
            player.Escape = (int)reader["Escape"];
            player.Skin = reader["Skin"] == null ? "" : reader["Skin"].ToString();
            player.IsBanChat = (bool)reader["IsBanChat"];
            player.ReputeOffer = (int)reader["ReputeOffer"];
            player.ConsortiaRepute = (int)reader["ConsortiaRepute"];
            player.ConsortiaLevel = (int)reader["ConsortiaLevel"];
            player.StoreLevel = (int)reader["StoreLevel"];
            player.ShopLevel = (int)reader["ShopLevel"];
            player.SmithLevel = (int)reader["SmithLevel"];
            player.ConsortiaHonor = (int)reader["ConsortiaHonor"];
            player.RichesOffer = (int)reader["RichesOffer"];
            player.RichesRob = (int)reader["RichesRob"];
            player.AntiAddiction = (int)reader["AntiAddiction"];
            player.DutyLevel = (int)reader["DutyLevel"];
            player.DutyName = reader["DutyName"] == null ? "" : reader["DutyName"].ToString();
            player.Right = (int)reader["Right"];
            player.ChairmanName = reader["ChairmanName"] == null ? "" : reader["ChairmanName"].ToString();
            player.AddDayGP = (int)reader["AddDayGP"];
            player.AddDayOffer = (int)reader["AddDayOffer"];
            player.AddWeekGP = (int)reader["AddWeekGP"];
            player.AddWeekOffer = (int)reader["AddWeekOffer"];
            player.ConsortiaRiches = (int)reader["ConsortiaRiches"];
            player.CheckCount = (int)reader["CheckCount"];
            player.IsMarried = (bool)reader["IsMarried"];
            player.SpouseID = (int)reader["SpouseID"];
            player.SpouseName = reader["SpouseName"] == null ? "" : reader["SpouseName"].ToString();
            player.MarryInfoID = (int)reader["MarryInfoID"];
            player.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
            player.DayLoginCount = (int)reader["DayLoginCount"];
            player.PasswordTwo = reader["PasswordTwo"] == null ? "" : reader["PasswordTwo"].ToString();
            player.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
            player.IsGotRing = (bool)reader["IsGotRing"];
            player.Rename = (bool)reader["Rename"];
            player.ConsortiaRename = (bool)reader["ConsortiaRename"];
            player.IsDirty = false;
            player.IsFirst = (int)reader["IsFirst"];
            player.Nimbus = (int)reader["Nimbus"];
            player.LastAward = (DateTime)reader["LastAward"];
            player.GiftToken = (int)reader["GiftToken"];
            player.QuestSite = reader["QuestSite"] == null ? new byte[200] : (byte[])reader["QuestSite"];
            player.PvePermission = reader["PvePermission"] == null ? "" : reader["PvePermission"].ToString();
            player.FightPower = (int)reader["FightPower"];
            player.PasswordQuest1 = reader["PasswordQuestion1"] == null ? "" : reader["PasswordQuestion1"].ToString();
            player.PasswordQuest2 = reader["PasswordQuestion2"] == null ? "" : reader["PasswordQuestion2"].ToString();
            player.FailedPasswordAttemptCount = (DateTime)reader["LastFindDate"] == null ? 5 : (int)reader["FailedPasswordAttemptCount"];
            player.AnswerSite = (int)reader["AnswerSite"];
            return player;
        }

        public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
        {
            List<PlayerInfo> infos = new List<PlayerInfo>();
            try
            {
                string sWhere = " IsExist=1 and IsFirst<> 0 ";
                if (userID != -1)
                {
                    sWhere += " and UserID =" + userID + " ";
                }
                string sOrder = "GP desc";
                //string sOrder = "LastDayGP desc";
                switch (order)
                {
                    case 1:
                        sOrder = "Offer desc";
                        //sOrder = "LastDayOffer desc";
                        break;
                    case 2:
                        sOrder = "AddDayGP desc";
                        break;
                    case 3:
                        sOrder = "AddWeekGP desc";
                        break;
                    case 4:
                        sOrder = "AddDayOffer desc";
                        break;
                    case 5:
                        sOrder = "AddWeekOffer desc";
                        break;
                    case 6:
                        sOrder = "FightPower desc";
                        break;
                }

                sOrder += ",UserID";

                DataTable dt = GetPage("V_Sys_Users_Detail", sWhere, page, size, "*", sOrder, "UserID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    PlayerInfo info = new PlayerInfo();
                    info.Agility = (int)dr["Agility"];
                    info.Attack = (int)dr["Attack"];
                    info.Colors = dr["Colors"] == null ? "" : dr["Colors"].ToString();
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.Defence = (int)dr["Defence"];
                    info.Gold = (int)dr["Gold"];
                    info.GP = (int)dr["GP"];
                    info.Grade = (int)dr["Grade"];
                    info.ID = (int)dr["UserID"];
                    info.Luck = (int)dr["Luck"];
                    info.Money = (int)dr["Money"];
                    info.NickName = dr["NickName"] == null ? "" : dr["NickName"].ToString();
                    info.Sex = (bool)dr["Sex"];
                    info.State = (int)dr["State"];
                    info.Style = dr["Style"] == null ? "" : dr["Style"].ToString();
                    info.Hide = (int)dr["Hide"];
                    info.Repute = (int)dr["Repute"];
                    info.UserName = dr["UserName"] == null ? "" : dr["UserName"].ToString();
                    info.ConsortiaName = dr["ConsortiaName"] == null ? "" : dr["ConsortiaName"].ToString();
                    info.Offer = (int)dr["Offer"];
                    info.Skin = dr["Skin"] == null ? "" : dr["Skin"].ToString();
                    info.IsBanChat = (bool)dr["IsBanChat"];
                    info.ReputeOffer = (int)dr["ReputeOffer"];
                    info.ConsortiaRepute = (int)dr["ConsortiaRepute"];
                    info.ConsortiaLevel = (int)dr["ConsortiaLevel"];
                    info.StoreLevel = (int)dr["StoreLevel"];
                    info.ShopLevel = (int)dr["ShopLevel"];
                    info.SmithLevel = (int)dr["SmithLevel"];
                    info.ConsortiaHonor = (int)dr["ConsortiaHonor"];
                    info.RichesOffer = (int)dr["RichesOffer"];
                    info.RichesRob = (int)dr["RichesRob"];
                    info.DutyLevel = (int)dr["DutyLevel"];
                    info.DutyName = dr["DutyName"] == null ? "" : dr["DutyName"].ToString();
                    info.Right = (int)dr["Right"];
                    info.ChairmanName = dr["ChairmanName"] == null ? "" : dr["ChairmanName"].ToString();
                    info.Win = (int)dr["Win"];
                    info.Total = (int)dr["Total"];
                    info.Escape = (int)dr["Escape"];
                    info.AddDayGP = (int)dr["AddDayGP"];
                    info.AddDayOffer = (int)dr["AddDayOffer"];
                    info.AddWeekGP = (int)dr["AddWeekGP"];
                    info.AddWeekOffer = (int)dr["AddWeekOffer"];
                    info.ConsortiaRiches = (int)dr["ConsortiaRiches"];
                    info.CheckCount = (int)dr["CheckCount"];
                    info.Nimbus = (int)dr["Nimbus"];
                    info.GiftToken = (int)dr["GiftToken"];
                    info.QuestSite = dr["QuestSite"] == null ? new byte[200] : (byte[])dr["QuestSite"];
                    info.PvePermission = dr["PvePermission"] == null ? "" : dr["PvePermission"].ToString();
                    info.FightPower = (int)dr["FightPower"];
                    infos.Add(info);
                }
                resultValue = true;

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

        #region GoodsInfo

        public ItemInfo[] GetUserItem(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                db.GetReader(ref reader, "SP_Users_Items_All", para);

                while (reader.Read())
                {
                    items.Add(InitItem(reader));
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
            return items.ToArray();

        }

        public ItemInfo[] GetUserBagByType(int UserID, int bagType)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                para[1] = new SqlParameter("@BagType", bagType);
                db.GetReader(ref reader, "SP_Users_BagByType", para);

                while (reader.Read())
                {
                    items.Add(InitItem(reader));
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
            return items.ToArray();

        }

        //public ItemInfo[] GetUserEuqip(int UserID)
        //{
        //    return GetUserEuqip(UserID,true);
        //}

        public List<ItemInfo> GetUserEuqip(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                db.GetReader(ref reader, "SP_Users_Items_Equip", para);

                while (reader.Read())
                {
                    items.Add(InitItem(reader));
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
            return items;
        }

        public ItemInfo GetUserItemSingle(int itemID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", SqlDbType.Int, 4);
                para[0].Value = itemID;
                db.GetReader(ref reader, "SP_Users_Items_Single", para);
                while (reader.Read())
                {
                    return InitItem(reader);
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

        public ItemInfo InitItem(SqlDataReader reader)
        {
            ItemInfo item = new ItemInfo(ItemMgr.FindItemTemplate((int)reader["TemplateID"]));
            item.AgilityCompose = (int)reader["AgilityCompose"];
            item.AttackCompose = (int)reader["AttackCompose"];
            item.Color = reader["Color"].ToString();
            item.Count = (int)reader["Count"];
            item.DefendCompose = (int)reader["DefendCompose"];
            item.ItemID = (int)reader["ItemID"];
            item.LuckCompose = (int)reader["LuckCompose"];
            item.Place = (int)reader["Place"];
            item.StrengthenLevel = (int)reader["StrengthenLevel"];
            item.TemplateID = (int)reader["TemplateID"];
            item.UserID = (int)reader["UserID"];
            item.ValidDate = (int)reader["ValidDate"];
            item.IsDirty = false;
            item.IsExist = (bool)reader["IsExist"];
            item.IsBinds = (bool)reader["IsBinds"];
            item.IsUsed = (bool)reader["IsUsed"];
            item.BeginDate = (DateTime)reader["BeginDate"];
            item.IsJudge = (bool)reader["IsJudge"];
            item.BagType = (int)reader["BagType"];
            item.Skin = reader["Skin"].ToString();
            item.RemoveDate = (DateTime)reader["RemoveDate"];
            item.RemoveType = (int)reader["RemoveType"];
            item.Hole1 = (int)reader["Hole1"];
            item.Hole2 = (int)reader["Hole2"];
            item.Hole3 = (int)reader["Hole3"];
            item.Hole4 = (int)reader["Hole4"];
            item.Hole5 = (int)reader["Hole5"];
            item.Hole6 = (int)reader["Hole6"];



            item.IsDirty = false;
            return item;
        }

        public bool AddGoods(ItemInfo item)
        {
            bool result = false;
            try
            {
                if (item.TemplateID == 11301)
                {

                }
                SqlParameter[] para = new SqlParameter[26];
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", item.Color == null ? "" : item.Color);
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                para[17] = new SqlParameter("@Skin", item.Skin == null ? "" : item.Skin);
                para[18] = new SqlParameter("@IsUsed", item.IsUsed);
                para[19] = new SqlParameter("@RemoveType", item.RemoveType);
                para[20] = new SqlParameter("@Hole1", item.Hole1);
                para[21] = new SqlParameter("@Hole2", item.Hole2);
                para[22] = new SqlParameter("@Hole3", item.Hole3);
                para[23] = new SqlParameter("@Hole4", item.Hole4);
                para[24] = new SqlParameter("@Hole5", item.Hole5);
                para[25] = new SqlParameter("@Hole6", item.Hole6);
       
                result = db.RunProcedure("SP_Users_Items_Add", para);
                item.ItemID = (int)para[0].Value;
                item.IsDirty = false;
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

        public bool UpdateGoods(ItemInfo item)
        {
            bool result = false;
            try
            {
                if (item.TemplateID == 11301)
                {

                }
                SqlParameter[] para = new SqlParameter[27];
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", item.Color == null ? "" : item.Color);//物品颜色
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                para[17] = new SqlParameter("@Skin", item.Skin);
                para[18] = new SqlParameter("@IsUsed", item.IsUsed);
                para[19] = new SqlParameter("@RemoveDate", item.RemoveDate);
                para[20] = new SqlParameter("@RemoveType", item.RemoveType);
                para[21] = new SqlParameter("@Hole1", item.Hole1);
                para[22] = new SqlParameter("@Hole2", item.Hole2);
                para[23] = new SqlParameter("@Hole3", item.Hole3);
                para[24] = new SqlParameter("@Hole4", item.Hole4);
                para[25] = new SqlParameter("@Hole5", item.Hole5);
                para[26] = new SqlParameter("@Hole6", item.Hole6);


                result = db.RunProcedure("SP_Users_Items_Update", para);
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool DeleteGoods(int itemID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", itemID);
                result = db.RunProcedure("SP_Users_Items_Delete", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        //public BestEquipInfo[] 


        public BestEquipInfo[] GetCelebByDayBestEquip()
        {
            List<BestEquipInfo> infos = new List<BestEquipInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_Users_BestEquip");

                while (reader.Read())
                {
                    BestEquipInfo info = new BestEquipInfo();
                    info.Date = (DateTime)reader["RemoveDate"];
                    info.GP = (int)reader["GP"];
                    info.Grade = (int)reader["Grade"];
                    info.ItemName = reader["Name"] == null ? "" : reader["Name"].ToString();
                    info.NickName = reader["NickName"] == null ? "" : reader["NickName"].ToString();
                    info.Sex = (bool)reader["Sex"];
                    info.Strengthenlevel = (int)reader["Strengthenlevel"];
                    info.UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
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

        #region MailInfo

        public MailInfo InitMail(SqlDataReader reader)
        {
            MailInfo info = new MailInfo();
            info.Annex1 = reader["Annex1"].ToString();
            info.Annex2 = reader["Annex2"].ToString();
            info.Content = reader["Content"].ToString();
            info.Gold = (int)reader["Gold"];
            info.ID = (int)reader["ID"];
            info.IsExist = (bool)reader["IsExist"];
            info.Money = (int)reader["Money"];
            info.Receiver = reader["Receiver"].ToString();
            info.ReceiverID = (int)reader["ReceiverID"];
            info.Sender = reader["Sender"].ToString();
            info.SenderID = (int)reader["SenderID"];
            info.Title = reader["Title"].ToString();
            info.Type = (int)reader["Type"];
            info.ValidDate = (int)reader["ValidDate"];
            info.IsRead = (bool)reader["IsRead"];
            info.SendTime = (DateTime)reader["SendTime"];
            info.Annex1Name = reader["Annex1Name"] == null ? "" : reader["Annex1Name"].ToString();
            info.Annex2Name = reader["Annex2Name"] == null ? "" : reader["Annex2Name"].ToString();
            info.Annex3 = reader["Annex3"].ToString();
            info.Annex4 = reader["Annex4"].ToString();
            info.Annex5 = reader["Annex5"].ToString();
            info.Annex3Name = reader["Annex3Name"] == null ? "" : reader["Annex3Name"].ToString();
            info.Annex4Name = reader["Annex4Name"] == null ? "" : reader["Annex4Name"].ToString();
            info.Annex5Name = reader["Annex5Name"] == null ? "" : reader["Annex5Name"].ToString();
            info.AnnexRemark = reader["AnnexRemark"] == null ? "" : reader["AnnexRemark"].ToString();
            return info;
        }

        public MailInfo[] GetMailByUserID(int userID)
        {
            List<MailInfo> items = new List<MailInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = userID;
                db.GetReader(ref reader, "SP_Mail_ByUserID", para);

                while (reader.Read())
                {
                    items.Add(InitMail(reader));
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
            return items.ToArray();
        }

        public MailInfo[] GetMailBySenderID(int userID)
        {
            List<MailInfo> items = new List<MailInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = userID;
                db.GetReader(ref reader, "SP_Mail_BySenderID", para);

                while (reader.Read())
                {
                    items.Add(InitMail(reader));
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
            return items.ToArray();
        }

        public MailInfo GetMailSingle(int UserID, int mailID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ID", mailID);
                para[1] = new SqlParameter("@UserID", UserID);
                db.GetReader(ref reader, "SP_Mail_Single", para);
                while (reader.Read())
                {
                    return InitMail(reader);
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

        public bool SendMail(MailInfo mail)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[29];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Annex1", mail.Annex1 == null ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", mail.Annex2 == null ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", mail.Content == null ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", true);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", mail.Receiver == null ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", mail.Sender == null ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", mail.Title == null ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", false);
                para[16] = new SqlParameter("@SendTime", DateTime.Now);
                para[17] = new SqlParameter("@Type", mail.Type);
                para[18] = new SqlParameter("@Annex1Name", mail.Annex1Name == null ? "" : mail.Annex1Name);
                para[19] = new SqlParameter("@Annex2Name", mail.Annex2Name == null ? "" : mail.Annex2Name);
                para[20] = new SqlParameter("@Annex3", mail.Annex3 == null ? "" : mail.Annex3);
                para[21] = new SqlParameter("@Annex4", mail.Annex4 == null ? "" : mail.Annex4);
                para[22] = new SqlParameter("@Annex5", mail.Annex5 == null ? "" : mail.Annex5);
                para[23] = new SqlParameter("@Annex3Name", mail.Annex3Name == null ? "" : mail.Annex3Name);
                para[24] = new SqlParameter("@Annex4Name", mail.Annex4Name == null ? "" : mail.Annex4Name);
                para[25] = new SqlParameter("@Annex5Name", mail.Annex5Name == null ? "" : mail.Annex5Name);
                para[26] = new SqlParameter("@ValidDate", mail.ValidDate);
                para[27] = new SqlParameter("@AnnexRemark", mail.AnnexRemark == null ? "" : mail.AnnexRemark);
                para[28] = new SqlParameter("GiftToken", mail.GiftToken);
        //        @ID = @ID OUTPUT,
                result = db.RunProcedure("SP_Mail_Send", para);
                mail.ID = (int)para[0].Value;
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    client.MailNotice(mail.ReceiverID);
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

        public bool DeleteMail(int UserID, int mailID, out int senderID)
        {
            bool result = false;
            senderID = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", mailID);
                para[1] = new SqlParameter("@UserID", UserID);
                para[2] = new SqlParameter("@SenderID", System.Data.SqlDbType.Int);
                para[2].Value = senderID;
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                result = db.RunProcedure("SP_Mail_Delete", para);
                int returnValue = (int)para[3].Value;
                if (returnValue == 0)
                {
                    result = true;
                    senderID = (int)para[2].Value;
                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool UpdateMail(MailInfo mail, int oldMoney)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[30];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[1] = new SqlParameter("@Annex1", mail.Annex1 == null ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", mail.Annex2 == null ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", mail.Content == null ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", mail.IsExist);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", mail.Receiver == null ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", mail.Sender == null ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", mail.Title == null ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", mail.IsRead);
                para[16] = new SqlParameter("@SendTime", mail.SendTime);
                para[17] = new SqlParameter("@Type", mail.Type);
                para[18] = new SqlParameter("@OldMoney", oldMoney);
                para[19] = new SqlParameter("@ValidDate", mail.ValidDate);
                para[20] = new SqlParameter("@Annex1Name", mail.Annex1Name);
                para[21] = new SqlParameter("@Annex2Name", mail.Annex2Name);
                para[22] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[22].Direction = ParameterDirection.ReturnValue;
                para[23] = new SqlParameter("@Annex3", mail.Annex3 == null ? "" : mail.Annex3);
                para[24] = new SqlParameter("@Annex4", mail.Annex4 == null ? "" : mail.Annex4);
                para[25] = new SqlParameter("@Annex5", mail.Annex5 == null ? "" : mail.Annex5);
                para[26] = new SqlParameter("@Annex3Name", mail.Annex3Name == null ? "" : mail.Annex3Name);
                para[27] = new SqlParameter("@Annex4Name", mail.Annex4Name == null ? "" : mail.Annex4Name);
                para[28] = new SqlParameter("@Annex5Name", mail.Annex5Name == null ? "" : mail.Annex5Name);
                para[29] = new SqlParameter("GiftToken", mail.GiftToken);
                db.RunProcedure("SP_Mail_Update", para);

                int returnValue = (int)para[22].Value;
                result = returnValue == 0;
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

        public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@userid", userid);
                para[1] = new SqlParameter("@mailID", mailID);
                para[2] = new SqlParameter("@senderID", System.Data.SqlDbType.Int);
                para[2].Value = senderID;
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Mail_PaymentCancel", para);
                int returnValue = (int)para[3].Value;
                result = returnValue == 0;
                if (result)
                {
                    senderID = (int)para[2].Value;
                }
                //switch (returnValue)
                //{
                //    case 0:
                //        msg = "退回邮件成功!";
                //        break;
                //    case 2:
                //        msg = "邮件不存在!";
                //        break;
                //    default:
                //        msg = "退回邮件失败!";
                //        break;
                //}
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool ScanMail(ref string noticeUserID)
        {
            bool result = false;
            try
            {

                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@NoticeUserID", System.Data.SqlDbType.NVarChar, 4000);
                para[0].Direction = ParameterDirection.Output;
                db.RunProcedure("SP_Mail_Scan", para);
                noticeUserID = para[0].Value.ToString();
                result = true;
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

        public bool SendMailAndItem(MailInfo mail, ItemInfo item, ref int returnValue)
        {
            bool result = false;
            try
            {

                SqlParameter[] para = new SqlParameter[34];
                //物品
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", item.Color == null ? "" : item.Color);
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                //信箱
                para[17] = new SqlParameter("@ID", mail.ID);
                para[17].Direction = ParameterDirection.Output;
                para[18] = new SqlParameter("@Annex1", mail.Annex1 == null ? "" : mail.Annex1);
                para[19] = new SqlParameter("@Annex2", mail.Annex2 == null ? "" : mail.Annex2);
                para[20] = new SqlParameter("@Content", mail.Content == null ? "" : mail.Content);
                para[21] = new SqlParameter("@Gold", mail.Gold);
                para[22] = new SqlParameter("@Money", mail.Money);
                para[23] = new SqlParameter("@Receiver", mail.Receiver == null ? "" : mail.Receiver);
                para[24] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[25] = new SqlParameter("@Sender", mail.Sender == null ? "" : mail.Sender);
                para[26] = new SqlParameter("@SenderID", mail.SenderID);
                para[27] = new SqlParameter("@Title", mail.Title == null ? "" : mail.Title);
                para[28] = new SqlParameter("@IfDelS", false);
                para[29] = new SqlParameter("@IsDelete", false);
                para[30] = new SqlParameter("@IsDelR", false);
                para[31] = new SqlParameter("@IsRead", false);
                para[32] = new SqlParameter("@SendTime", DateTime.Now);
                para[33] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[33].Direction = ParameterDirection.ReturnValue;

                result = db.RunProcedure("SP_Admin_SendUserItem", para);
                returnValue = (int)para[33].Value;
                result = returnValue == 0;

                if (result)
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        client.MailNotice(mail.ReceiverID);
                    }
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

        public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[18];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Annex1", mail.Annex1 == null ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", mail.Annex2 == null ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", mail.Content == null ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", true);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", mail.Receiver == null ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", mail.Sender == null ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", mail.Title == null ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", false);
                para[16] = new SqlParameter("@SendTime", DateTime.Now);
                para[17] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[17].Direction = ParameterDirection.ReturnValue;

                result = db.RunProcedure("SP_Admin_SendUserMoney", para);
                returnValue = (int)para[17].Value;
                result = returnValue == 0;

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

        public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            MailInfo message = new MailInfo();
            message.Annex1 = "";
            message.Content = title;
            message.Gold = gold;
            message.Money = money;
            message.Receiver = "";
            message.ReceiverID = UserID;
            message.Sender = "Administrators";//LanguageMgr.GetTranslation("PlayerBussiness.SendMailAndItem.Sender");
            message.SenderID = 0;
            message.Title = content;

            ItemInfo userGoods = new ItemInfo(null);
            userGoods.AgilityCompose = AgilityCompose;
            userGoods.AttackCompose = AttackCompose;
            userGoods.BeginDate = DateTime.Now;
            userGoods.Color = "";
            userGoods.DefendCompose = DefendCompose;
            userGoods.IsDirty = false;
            userGoods.IsExist = true;
            userGoods.IsJudge = true;
            userGoods.LuckCompose = LuckCompose;
            userGoods.StrengthenLevel = StrengthenLevel;
            userGoods.TemplateID = templateID;
            userGoods.ValidDate = validDate;
            userGoods.Count = count;
            userGoods.IsBinds = isBinds;

            int returnValue = 1;
            SendMailAndItem(message, userGoods, ref returnValue);
            return returnValue;
        }

        public int SendMailAndItemByUserName(string title, string content, string userName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo player = GetUserSingleByUserName(userName);
            if (player != null)
            {
                return SendMailAndItem(title, content, player.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            return 2;
        }

        public int SendMailAndItemByNickName(string title, string content, string NickName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo player = GetUserSingleByNickName(NickName);
            if (player != null)
            {
                return SendMailAndItem(title, content, player.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            return 2;
        }

        public int SendMailAndItem(string title, string content, int userID, int gold, int money, string param)
        {
            bool result = false;
            int returnValue = 1;
            try
            {

                //SP_Admin_SendAllItem 'kenken','kenken',17,100,200,'11020,4,0,0,0,0,0,0,1|7014,2,9,400,400,400,400,400,0'
                SqlParameter[] para = new SqlParameter[8];
                //物品
                para[0] = new SqlParameter("@Title", title);
                para[1] = new SqlParameter("@Content", content);
                para[2] = new SqlParameter("@UserID", userID);
                para[3] = new SqlParameter("@Gold", gold);
                para[4] = new SqlParameter("@Money", money);
                para[5] = new SqlParameter("@GiftToken", 0);
                para[6] = new SqlParameter("@Param", param);
                para[7] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;

                result = db.RunProcedure("SP_Admin_SendAllItem", para);
                returnValue = (int)para[7].Value;
                result = returnValue == 0;

                if (result)
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        client.MailNotice(userID);
                    }
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
            return returnValue;
        }

        //templateID,count,validDate,StrengthenLevel,AttackCompose,DefendCompose,AgilityCompose,LuckCompose,isBinds
        //int templateID, int count, int validDate, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds
        public int SendMailAndItemByUserName(string title, string content, string userName, int gold, int money, string param)
        {
            PlayerInfo player = GetUserSingleByUserName(userName);
            if (player != null)
            {
                return SendMailAndItem(title, content, player.ID, gold, money, param);
            }
            return 2;
        }

        public int SendMailAndItemByNickName(string title, string content, string nickName, int gold, int money, string param)
        {
            PlayerInfo player = GetUserSingleByNickName(nickName);
            if (player != null)
            {
                return SendMailAndItem(title, content, player.ID, gold, money, param);
            }
            return 2;
        }


        #endregion

        #region FriendsInfo

        public Dictionary<int, int> GetFriendsIDAll(int UserID)
        {
            Dictionary<int, int> info = new Dictionary<int, int>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                db.GetReader(ref reader, "SP_Users_Friends_All", para);
                while (reader.Read())
                {
                    //info.Add((int)reader["FriendID"]);
                    if (!info.ContainsKey((int)reader["FriendID"]))
                    {
                        info.Add((int)reader["FriendID"], (int)reader["Relation"]);
                    }
                    else
                    {
                        info[(int)reader["FriendID"]] = (int)reader["Relation"];
                    }
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
            return info;

        }

        public bool AddFriends(FriendInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@ID", info.ID);
                para[1] = new SqlParameter("@AddDate", DateTime.Now);
                para[2] = new SqlParameter("@FriendID", info.FriendID);
                para[3] = new SqlParameter("@IsExist", true);
                para[4] = new SqlParameter("@Remark", info.Remark == null ? "" : info.Remark);
                para[5] = new SqlParameter("@UserID", info.UserID);
                para[6] = new SqlParameter("@Relation", info.Relation);
                result = db.RunProcedure("SP_Users_Friends_Add", para);
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

        public bool DeleteFriends(int UserID, int FriendID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ID", FriendID);
                para[1] = new SqlParameter("@UserID", UserID);
                result = db.RunProcedure("SP_Users_Friends_Delete", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public FriendInfo[] GetFriendsAll(int UserID)
        {
            List<FriendInfo> infos = new List<FriendInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                db.GetReader(ref reader, "SP_Users_Friends", para);
                while (reader.Read())
                {
                    FriendInfo info = new FriendInfo();
                    info.AddDate = (DateTime)reader["AddDate"];
                    info.Colors = reader["Colors"] == null ? "" : reader["Colors"].ToString();
                    info.FriendID = (int)reader["FriendID"];
                    info.Grade = (int)reader["Grade"];
                    info.Hide = (int)reader["Hide"];
                    info.ID = (int)reader["ID"];
                    info.IsExist = (bool)reader["IsExist"];
                    info.NickName = reader["NickName"] == null ? "" : reader["NickName"].ToString();
                    info.Remark = reader["Remark"] == null ? "" : reader["Remark"].ToString();
                    info.Sex = ((bool)reader["Sex"]) ? 1 : 0;
                    info.State = (int)reader["State"];
                    info.Style = reader["Style"] == null ? "" : reader["Style"].ToString();
                    info.UserID = (int)reader["UserID"];
                    info.ConsortiaName = reader["ConsortiaName"] == null ? "" : reader["ConsortiaName"].ToString();
                    info.Offer = (int)reader["Offer"];
                    info.Win = (int)reader["Win"];
                    info.Total = (int)reader["Total"];
                    info.Escape = (int)reader["Escape"];
                    info.Relation = (int)reader["Relation"];
                    info.Repute = (int)reader["Repute"];
                    info.UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
                    info.DutyName = reader["DutyName"] == null ? "" : reader["DutyName"].ToString();
                    info.Nimbus = (int)reader["Nimbus"];
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

        /// <summary>
        /// 获取用户的好友帐号
        /// </summary>
        /// <param name="UserName">传入用户名</param>
        /// <returns>返回好友帐号</returns>
        public ArrayList GetFriendsGood(string UserName)
        {
            ArrayList friends = new ArrayList();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserName", SqlDbType.VarChar);
                para[0].Value = UserName;
                db.GetReader(ref reader, "SP_Users_Friends_Good", para);
                while (reader.Read())
                {
                    friends.Add(reader["UserName"] == null ? "" : reader["UserName"].ToString());
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return friends;
        }

        /// <summary>
        /// 批量获取用户好友信息
        /// </summary>
        /// <param name="condictArray">好友用户参数，以逗号切割</param>
        /// <returns>返回用户信息</returns>
        public FriendInfo[] GetFriendsBbs(string condictArray)
        {
            List<FriendInfo> infos = new List<FriendInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000);
                para[0].Value = condictArray;
                db.GetReader(ref reader, "SP_Users_FriendsBbs", para);
                while (reader.Read())
                {
                    FriendInfo info = new FriendInfo();
                    info.NickName = reader["NickName"] == null ? "" : reader["NickName"].ToString();
                    info.UserID = (int)reader["UserID"];
                    info.UserName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
                    info.IsExist = (int)reader["UserID"] > 0 ? true : false;
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

        #region QuestInfo
        /// <summary>
        /// 从数据库中获取当前玩家的系统中存在的任务
        /// </summary>
        /// <param name="userID">用户信息</param>
        /// <returns></returns>
        public QuestDataInfo[] GetUserQuest(int userID)
        {
            List<QuestDataInfo> infos = new List<QuestDataInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = userID;
                db.GetReader(ref reader, "SP_QuestData_All", para);
                while (reader.Read())
                {
                    QuestDataInfo info = new QuestDataInfo();
                    info.CompletedDate = (DateTime)reader["CompletedDate"];
                    info.IsComplete = (bool)reader["IsComplete"];
                    info.Condition1 = (int)reader["Condition1"];
                    info.Condition2 = (int)reader["Condition2"];
                    info.Condition3 = (int)reader["Condition3"];
                    info.Condition4 = (int)reader["Condition4"];
                    info.QuestID = (int)reader["QuestID"];
                    info.UserID = (int)reader["UserId"];
                    info.IsExist = (bool)reader["IsExist"];
                    info.RandDobule = (int)reader["RandDobule"];
                    info.RepeatFinish = (int)reader["RepeatFinish"];
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
        /// <summary>
        /// 更新或插入一条用户任务信息
        /// </summary>
        /// <param name="info">任务信息</param>
        /// <returns>返回结果</returns>
        public bool UpdateDbQuestDataInfo(QuestDataInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[11];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@QuestID", info.QuestID);
                para[2] = new SqlParameter("@CompletedDate", info.CompletedDate);
                para[3] = new SqlParameter("@IsComplete", info.IsComplete);
                para[4] = new SqlParameter("@Condition1", info.Condition1);
                para[5] = new SqlParameter("@Condition2", info.Condition2);
                para[6] = new SqlParameter("@Condition3", info.Condition3);
                para[7] = new SqlParameter("@Condition4", info.Condition4);
                para[8] = new SqlParameter("@IsExist", info.IsExist);
                para[9] = new SqlParameter("@RepeatFinish", info.RepeatFinish);
                para[10] = new SqlParameter("@RandDobule", info.RandDobule);
                result = db.RunProcedure("SP_QuestData_Add", para);  /*备注需要改回SP_QuestData_Add*/
                info.IsDirty = false;  //置为拉圾数据
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

        #region

        public BufferInfo[] GetUserBuffer(int userID)
        {
            List<BufferInfo> infos = new List<BufferInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = userID;
                db.GetReader(ref reader, "SP_User_Buff_All", para);
                while (reader.Read())
                {
                    BufferInfo info = new BufferInfo();
                    info.BeginDate = (DateTime)reader["BeginDate"];
                    info.Data = reader["Data"] == null ? "" : reader["Data"].ToString();
                    info.Type = (int)reader["Type"];
                    info.UserID = (int)reader["UserID"];
                    info.ValidDate = (int)reader["ValidDate"];
                    info.Value = (int)reader["Value"];
                    info.IsExist = (bool)reader["IsExist"];
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

        public bool SaveBuffer(BufferInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@Type", info.Type);
                para[2] = new SqlParameter("@BeginDate", info.BeginDate);
                para[3] = new SqlParameter("@Data", info.Data == null ? "" : info.Data);
                para[4] = new SqlParameter("@IsExist", info.IsExist);
                para[5] = new SqlParameter("@ValidDate", info.ValidDate);
                para[6] = new SqlParameter("@Value", info.Value);
                result = db.RunProcedure("SP_User_Buff_Add", para);
                info.IsDirty = false;
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

        #region Charge

        public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, string nickName)
        {
            bool result = false;
            userID = 0;
            try
            {
                //DateTime date = DateTime.Now;
                SqlParameter[] para = new SqlParameter[10];
                para[0] = new SqlParameter("@ChargeID", chargeID);
                para[1] = new SqlParameter("@UserName", userName);
                para[2] = new SqlParameter("@Money", money);
                para[3] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                para[4] = new SqlParameter("@PayWay", payWay);
                para[5] = new SqlParameter("@NeedMoney", needMoney);
                para[6] = new SqlParameter("@UserID", userID);
                para[6].Direction = ParameterDirection.InputOutput;
                para[7] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;
                para[8] = new SqlParameter("@IP", IP);
                para[9] = new SqlParameter("@NickName", nickName);
                result = db.RunProcedure("SP_Charge_Money_Add", para);
                userID = (int)para[6].Value;
                isResult = (int)para[7].Value;
                result = isResult == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool ChargeToUser(string userName, ref int money, string nickName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@money", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Output;
                para[2] = new SqlParameter("@NickName", nickName);
                result = db.RunProcedure("SP_Charge_To_User", para);
                money = (int)para[1].Value;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
        {
            List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                para[1] = new SqlParameter("@Second", SaveRecordSecond);

                db.GetReader(ref reader, "SP_Charge_Record", para);
                while (reader.Read())
                {
                    ChargeRecordInfo info = new ChargeRecordInfo();
                    info.BoyTotalPay = (int)reader["BoyTotalPay"];
                    info.GirlTotalPay = (int)reader["GirlTotalPay"];
                    info.PayWay = reader["PayWay"] == null ? "" : reader["PayWay"].ToString();
                    info.TotalBoy = (int)reader["TotalBoy"];
                    info.TotalGirl = (int)reader["TotalGirl"];
                    list.Add(info);
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
            return list.ToArray();
        }

        #endregion

        #region AuctionInfo

        public AuctionInfo GetAuctionSingle(int auctionID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@AuctionID", auctionID);
                db.GetReader(ref reader, "SP_Auction_Single", para);
                while (reader.Read())
                {
                    return InitAuctionInfo(reader);
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

        public bool AddAuction(AuctionInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[17];
                para[0] = new SqlParameter("@AuctionID", info.AuctionID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
                para[2] = new SqlParameter("@AuctioneerName", info.AuctioneerName == null ? "" : info.AuctioneerName);
                para[3] = new SqlParameter("@BeginDate", info.BeginDate);
                para[4] = new SqlParameter("@BuyerID", info.BuyerID);
                para[5] = new SqlParameter("@BuyerName", info.BuyerName == null ? "" : info.BuyerName);
                para[6] = new SqlParameter("@IsExist", info.IsExist);
                para[7] = new SqlParameter("@ItemID", info.ItemID);
                para[8] = new SqlParameter("@Mouthful", info.Mouthful);
                para[9] = new SqlParameter("@PayType", info.PayType);
                para[10] = new SqlParameter("@Price", info.Price);
                para[11] = new SqlParameter("@Rise", info.Rise);
                para[12] = new SqlParameter("@ValidDate", info.ValidDate);
                para[13] = new SqlParameter("@TemplateID", info.TemplateID);
                para[14] = new SqlParameter("Name", info.Name);
                para[15] = new SqlParameter("Category", info.Category);
                para[16] = new SqlParameter("Random", info.Random);
                result = db.RunProcedure("SP_Auction_Add", para);
                info.AuctionID = (int)para[0].Value;
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

        public bool UpdateAuction(AuctionInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[16];
                para[0] = new SqlParameter("@AuctionID", info.AuctionID);
                para[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
                para[2] = new SqlParameter("@AuctioneerName", info.AuctioneerName == null ? "" : info.AuctioneerName);
                para[3] = new SqlParameter("@BeginDate", info.BeginDate);
                para[4] = new SqlParameter("@BuyerID", info.BuyerID);
                para[5] = new SqlParameter("@BuyerName", info.BuyerName == null ? "" : info.BuyerName);
                para[6] = new SqlParameter("@IsExist", info.IsExist);
                para[7] = new SqlParameter("@ItemID", info.ItemID);
                para[8] = new SqlParameter("@Mouthful", info.Mouthful);
                para[9] = new SqlParameter("@PayType", info.PayType);
                para[10] = new SqlParameter("@Price", info.Price);
                para[11] = new SqlParameter("@Rise", info.Rise);
                para[12] = new SqlParameter("@ValidDate", info.ValidDate);
                para[13] = new SqlParameter("Name", info.Name);
                para[14] = new SqlParameter("Category", info.Category);
                para[15] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[15].Direction = ParameterDirection.ReturnValue;

                db.RunProcedure("SP_Auction_Update", para);
                int returnValue = (int)para[15].Value;
                result = returnValue == 0;
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

        public bool DeleteAuction(int auctionID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@AuctionID", auctionID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Auction_Delete", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                switch (returnValue)
                {
                    case 0:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1");
                        break;
                    case 1:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2");
                        break;
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3");
                        break;
                    default:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4");
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

        //拼抽
        public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
        {
            List<AuctionInfo> infos = new List<AuctionInfo>();
            //SqlDataReader reader = null;
            try
            {
                //int View_flag = 1;//标识别:1代表type在1-24的视图,0代表type在其余的视图.
                string sWhere = " IsExist=1 ";
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere += " and Name like '%" + name + "%' ";
                }
                if (type != -1)
                {
                    //sWhere += " and Category =" + type + " ";
                    //                -美容
                    //  翅膀
                    //  眼睛
                    //  脸饰
                    //  头发
                    //-装备
                    //  套装
                    //  衣服
                    //  帽子
                    //  眼镜
                    //-饰品
                    //  项链
                    //  戒指
                    //  手镯
                    //-道具
                    //  特殊
                    //  战斗道具
                    //  宝石及公式

                    switch (type)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                            sWhere += " and Category =" + type + " ";
                            break;
                        case 21:
                            sWhere += " and Category in(1,2,5,8,9) ";   //服装
                            break;
                        case 22:
                            sWhere += " and Category in(13,15,6,4,3) "; //美容
                            break;
                        case 23:
                            sWhere += " and Category in(16,11,10) ";    //道具
                            break;
                        case 24:
                            sWhere += " and Category in(8,9) ";         //首饰
                            break;
                        case 25:
                            sWhere += " and Category in (7,17) ";       //武器、副武器
                            break;
                        case 26:
                            sWhere += " and TemplateId>=311000 and TemplateId<=313999"; //所有宝珠
                            break;
                        case 27:
                            sWhere += " and TemplateId>=311000 and TemplateId<=311999 ";//三角宝珠
                            break;
                        case 28:
                            sWhere += " and TemplateId>=312000 and TemplateId<=312999 ";//方形宝珠
                            break;
                        case 29:
                            sWhere += " and TemplateId>=313000 and TempLateId<=313999"; //圆形宝珠
                            break;
                        case 1100:
                            //View_flag = 0;
                            sWhere += " and TemplateID in (11019,11021,11022,11023) "; //所有强化石
                            break;
                        case 1101:
                            //View_flag = 0;
                            sWhere += " and TemplateID='11019' ";     //强化石1
                            break;
                        case 1102:
                            //View_flag = 0;
                            sWhere += " and TemplateID='11021' ";     //强化石2
                            break;
                        case 1103:
                            //View_flag = 0;
                            sWhere += " and TemplateID='11022' ";     //强化石3
                            break;
                        case 1104:
                            //View_flag = 0;
                            sWhere += " and TemplateID='11023' ";     //强化石4
                            break;
                        case 1105:
                            //View_flag = 0;
                            sWhere += " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";   //所有合成石
                            break;
                        case 1106:
                            //View_flag = 0;
                            sWhere += " and TemplateID in (11001,11002,11003,11004) ";   //朱雀石
                            break;
                        case 1107:
                            //View_flag = 0;
                            sWhere += " and TemplateID in (11005,11006,11007,11008) ";  //玄武石
                            break;
                        case 1108:
                            //View_flag = 0;
                            sWhere += " and TemplateID in (11009,11010,11011,11012) ";  //青龙石
                            break;
                        case 1109:
                            //View_flag = 0;
                            sWhere += " and TemplateID in (11013,11014,11015,11016) ";  //白虎石
                            break;



                    }
                }
                if (pay != -1)
                {
                    sWhere += " and PayType =" + pay + " ";
                }
                if (userID != -1)
                {
                    sWhere += " and AuctioneerID =" + userID + " ";
                }
                if (buyID != -1)
                {
                    //sWhere += " and BuyerID =" + buyID + " ";
                    //if (string.IsNullOrEmpty(AuctionIDs))
                    //{
                    //    AuctionIDs = null;
                    //}
                    sWhere += " and (BuyerID =" + buyID + " or AuctionID in (" + AuctionIDs + ")) ";
                }
                //默认排序按照排序的优先级进行：物品类型>物品名称>当前价格（以一口价为准）>剩余时间>出售人
                string sOrder = "Category,Name,Price,dd,AuctioneerID";
                switch (order)
                {
                    case 0:
                        sOrder = "Name";
                        break;
                    case 1:
                        //sOrder = "Repute1";
                        break;
                    case 2:
                        //sOrder = "Count1";
                        sOrder = "dd";
                        break;
                    case 3:
                        sOrder = "AuctioneerName";
                        break;
                    case 4:
                        sOrder = "Price";
                        break;
                    case 5:
                        sOrder = "BuyerName";
                        break;
                }

                sOrder += sort ? " desc" : "";
                sOrder += ",AuctionID ";

                SqlParameter[] para = new SqlParameter[8];

                para[0] = new SqlParameter("@QueryStr", "V_Auction_Scan");//表名，视图名，查询语句           
                para[1] = new SqlParameter("@QueryWhere", sWhere);//查询条件
                para[2] = new SqlParameter("@PageSize", size);//每页的行数
                para[3] = new SqlParameter("@PageCurrent", page);//要显示页面
                para[4] = new SqlParameter("@FdShow", "*");//要显示的字段列表,必须排除表识字段
                para[5] = new SqlParameter("@FdOrder", sOrder);//排序字段列表
                para[6] = new SqlParameter("@FdKey", "AuctionID");//强制指定主键
                para[7] = new SqlParameter("@TotalRow", total);//总记录行数
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = db.GetDataTable("Auction", "SP_CustomPage", para);
                total = (int)para[7].Value;
                foreach (DataRow dr in dt.Rows)
                {
                    AuctionInfo info = new AuctionInfo();
                    info.AuctioneerID = (int)dr["AuctioneerID"];
                    info.AuctioneerName = dr["AuctioneerName"].ToString();
                    info.AuctionID = (int)dr["AuctionID"];
                    info.BeginDate = (DateTime)dr["BeginDate"];
                    info.BuyerID = (int)dr["BuyerID"];
                    info.BuyerName = dr["BuyerName"].ToString();
                    info.Category = (int)dr["Category"];
                    info.IsExist = (bool)dr["IsExist"];
                    info.ItemID = (int)dr["ItemID"];
                    info.Name = dr["Name"].ToString();
                    info.Mouthful = (int)dr["Mouthful"];
                    info.PayType = (int)dr["PayType"];
                    info.Price = (int)dr["Price"];
                    info.Rise = (int)dr["Rise"];
                    info.ValidDate = (int)dr["ValidDate"];

                    infos.Add(info);
                }
                //db.GetReader(ref reader, "SP_CustomPage", para);
                //total = (int)para[7].Value;
                //while (reader.Read())
                //{
                //    infos.Add(InitAuctionInfo(reader));
                //}
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                //if (reader != null && !reader.IsClosed)
                //    reader.Close();
            }

            return infos.ToArray();
        }

        public AuctionInfo InitAuctionInfo(SqlDataReader reader)
        {
            AuctionInfo info = new AuctionInfo();
            info.AuctioneerID = (int)reader["AuctioneerID"];
            info.AuctioneerName = reader["AuctioneerName"] == null ? "" : reader["AuctioneerName"].ToString();
            info.AuctionID = (int)reader["AuctionID"];
            info.BeginDate = (DateTime)reader["BeginDate"];
            info.BuyerID = (int)reader["BuyerID"];
            info.BuyerName = reader["BuyerName"] == null ? "" : reader["BuyerName"].ToString();
            info.IsExist = (bool)reader["IsExist"];
            info.ItemID = (int)reader["ItemID"];
            info.Mouthful = (int)reader["Mouthful"];
            info.PayType = (int)reader["PayType"];
            info.Price = (int)reader["Price"];
            info.Rise = (int)reader["Rise"];
            info.ValidDate = (int)reader["ValidDate"];
            info.Name = reader["Name"].ToString();
            info.Category = (int)reader["Category"];
            return info;
        }

        public bool ScanAuction(ref string noticeUserID)
        {
            bool result = false;
            try
            {

                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@NoticeUserID", System.Data.SqlDbType.NVarChar, 4000);
                para[0].Direction = ParameterDirection.Output;
                db.RunProcedure("SP_Auction_Scan", para);
                noticeUserID = para[0].Value.ToString();
                result = true;
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

        #region MarryInfo
        public bool AddMarryInfo(MarryInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", info.UserID);
                para[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
                para[3] = new SqlParameter("@Introduction", info.Introduction);
                para[4] = new SqlParameter("@RegistTime", info.RegistTime);
                result = db.RunProcedure("SP_MarryInfo_Add", para);
                info.ID = (int)para[0].Value;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("AddMarryInfo", e);
            }
            return result;
        }

        public bool DeleteMarryInfo(int ID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ID", ID);
                para[1] = new SqlParameter("@UserID", userID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_MarryInfo_Delete", para);
                int returnValue = (int)para[2].Value;
                result = returnValue == 0;
                if (returnValue == 0)
                {
                    msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed");
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DeleteAuction", e);
            }
            return result;
        }


        public MarryInfo GetMarryInfoSingle(int ID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", ID);
                db.GetReader(ref reader, "SP_MarryInfo_Single", para);
                while (reader.Read())
                {
                    MarryInfo info = new MarryInfo();
                    info.ID = (int)reader["ID"];
                    info.UserID = (int)reader["UserID"];
                    info.IsPublishEquip = (bool)reader["IsPublishEquip"];
                    info.Introduction = reader["Introduction"].ToString();
                    info.RegistTime = (DateTime)reader["RegistTime"];
                    //info.Sex = (bool)reader["Sex"];
                    //info.State = (bool)reader["State"];
                    //info.IsMarried = (bool)reader["IsMarried"];
                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetMarryInfoSingle", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return null;
        }

        public bool UpdateMarryInfo(MarryInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@ID", info.ID);
                para[1] = new SqlParameter("@UserID", info.UserID);
                para[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
                para[3] = new SqlParameter("@Introduction", info.Introduction);
                para[4] = new SqlParameter("@RegistTime", info.RegistTime);
                para[5] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[5].Direction = ParameterDirection.ReturnValue;

                db.RunProcedure("SP_MarryInfo_Update", para);
                int returnValue = (int)para[5].Value;
                result = returnValue == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
        {
            List<MarryInfo> infos = new List<MarryInfo>();
            //SqlDataReader reader = null;
            try
            {
                string sWhere = "";
                if (sex)
                {
                    sWhere = " IsExist=1 and Sex=1 and UserExist=1";
                }
                else
                {
                    sWhere = " IsExist=1 and Sex=0 and UserExist=1";
                }

                if (!string.IsNullOrEmpty(name))
                {
                    sWhere += " and NickName like '%" + name + "%' ";
                }

                string sOrder = "State desc,IsMarried";
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@QueryStr", "V_Sys_Marry_Info");
                para[1] = new SqlParameter("@QueryWhere", sWhere);
                para[2] = new SqlParameter("@PageSize", size);
                para[3] = new SqlParameter("@PageCurrent", page);
                para[4] = new SqlParameter("@FdShow", "*");
                para[5] = new SqlParameter("@FdOrder", sOrder);
                para[6] = new SqlParameter("@FdKey", "ID");
                para[7] = new SqlParameter("@TotalRow", total);
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", para);
                total = (int)para[7].Value;
                foreach (DataRow dr in dt.Rows)
                {
                    MarryInfo info = new MarryInfo();
                    info.ID = (int)dr["ID"];
                    info.UserID = (int)dr["UserID"];
                    info.IsPublishEquip = (bool)dr["IsPublishEquip"];
                    info.Introduction = dr["Introduction"].ToString();
                    info.NickName = dr["NickName"].ToString();
                    info.IsConsortia = (bool)dr["IsConsortia"];
                    info.ConsortiaID = (int)dr["ConsortiaID"];
                    info.Sex = (bool)dr["Sex"];
                    info.Win = (int)dr["Win"];
                    info.Total = (int)dr["Total"];
                    info.Escape = (int)dr["Escape"];
                    info.GP = (int)dr["GP"];
                    info.Honor = dr["Honor"].ToString();
                    info.Style = dr["Style"].ToString();
                    info.Colors = dr["Colors"].ToString();
                    info.Hide = (int)dr["Hide"];
                    info.Grade = (int)dr["Grade"];
                    info.State = (int)dr["State"];
                    info.Repute = (int)dr["Repute"];
                    info.Skin = dr["Skin"].ToString();
                    info.Offer = (int)dr["Offer"];
                    info.IsMarried = (bool)dr["IsMarried"];
                    info.ConsortiaName = dr["ConsortiaName"].ToString();
                    info.DutyName = dr["DutyName"].ToString();
                    info.Nimbus = (int)dr["Nimbus"];
                    info.FightPower = (int)dr["FightPower"];
                    infos.Add(info);
                }
                //db.GetReader(ref reader, "SP_CustomPage", para);
                //total = (int)para[7].Value;
                //while (reader.Read())
                //{
                //    infos.Add(InitAuctionInfo(reader));
                //}
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                //if (reader != null && !reader.IsClosed)
                //    reader.Close();
            }

            return infos.ToArray();
        }

        public bool InsertPlayerMarryApply(MarryApplyInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@ApplyUserID", info.ApplyUserID);
                para[2] = new SqlParameter("@ApplyUserName", info.ApplyUserName);
                para[3] = new SqlParameter("@ApplyType", info.ApplyType);
                para[4] = new SqlParameter("@ApplyResult", info.ApplyResult);
                para[5] = new SqlParameter("@LoveProclamation", info.LoveProclamation);
                para[6] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[6].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Insert_Marry_Apply", para);
                result = (int)para[6].Value == 0;

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("InsertPlayerMarryApply", e);
            }

            return result;
        }

        public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserID", UserID);
                para[1] = new SqlParameter("@LoveProclamation", loveProclamation);
                para[2] = new SqlParameter("@isExist", isExist);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Update_Marry_Apply", para);
                result = (int)para[3].Value == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdatePlayerMarryApply", e);
            }

            return result;
        }

        public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
        {
            SqlDataReader reader = null;
            List<MarryApplyInfo> infos = new List<MarryApplyInfo>();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", UserID);

                db.GetReader(ref reader, "SP_Get_Marry_Apply", para);
                while (reader.Read())
                {
                    MarryApplyInfo info = new MarryApplyInfo();
                    info.UserID = (int)reader["UserID"];
                    info.ApplyUserID = (int)reader["ApplyUserID"];
                    info.ApplyUserName = reader["ApplyUserName"].ToString();
                    info.ApplyType = (int)reader["ApplyType"];
                    info.ApplyResult = (bool)reader["ApplyResult"];
                    info.LoveProclamation = reader["LoveProclamation"].ToString();
                    info.ID = (int)reader["Id"];
                    infos.Add(info);
                }

                return infos.ToArray();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetPlayerMarryApply", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return null;
        }


        public bool InsertMarryRoomInfo(MarryRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[20];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@Name", info.Name);
                para[2] = new SqlParameter("@PlayerID", info.PlayerID);
                para[3] = new SqlParameter("@PlayerName", info.PlayerName);
                para[4] = new SqlParameter("@GroomID", info.GroomID);
                para[5] = new SqlParameter("@GroomName", info.GroomName);
                para[6] = new SqlParameter("@BrideID", info.BrideID);
                para[7] = new SqlParameter("@BrideName", info.BrideName);
                para[8] = new SqlParameter("@Pwd", info.Pwd);
                para[9] = new SqlParameter("@AvailTime", info.AvailTime);
                para[10] = new SqlParameter("@MaxCount", info.MaxCount);
                para[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
                para[12] = new SqlParameter("@MapIndex", info.MapIndex);
                para[13] = new SqlParameter("@BeginTime", info.BeginTime);
                para[14] = new SqlParameter("@BreakTime", info.BreakTime);
                para[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
                para[16] = new SqlParameter("@ServerID", info.ServerID);
                para[17] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
                para[18] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
                para[19] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[19].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Insert_Marry_Room_Info", para);

                result = (int)para[19].Value == 0;
                if (result)
                {
                    info.ID = (int)para[0].Value;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("InsertMarryRoomInfo", e);
            }
            return result;
        }

        public bool UpdateMarryRoomInfo(MarryRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@ID", info.ID);
                para[1] = new SqlParameter("@AvailTime", info.AvailTime);
                para[2] = new SqlParameter("@BreakTime", info.BreakTime);
                para[3] = new SqlParameter("@roomIntroduction", info.RoomIntroduction);
                para[4] = new SqlParameter("@isHymeneal", info.IsHymeneal);
                para[5] = new SqlParameter("@Name", info.Name);
                para[6] = new SqlParameter("@Pwd", info.Pwd);
                para[7] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
                para[8] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Update_Marry_Room_Info", para);
                result = (int)para[8].Value == 0;

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdateMarryRoomInfo", e);
            }

            return result;
        }

        public bool DisposeMarryRoomInfo(int ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ID", ID);
                para[1] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[1].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Dispose_Marry_Room_Info", para);
                result = (int)para[1].Value == 0;

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DisposeMarryRoomInfo", e);
            }

            return result;
        }

        public MarryRoomInfo[] GetMarryRoomInfo()
        {
            SqlDataReader reader = null;
            List<MarryRoomInfo> infos = new List<MarryRoomInfo>();
            try
            {
                db.GetReader(ref reader, "SP_Get_Marry_Room_Info");
                while (reader.Read())
                {
                    MarryRoomInfo info = new MarryRoomInfo();
                    info.ID = (int)reader["ID"];
                    info.Name = reader["Name"].ToString();
                    info.PlayerID = (int)reader["PlayerID"];
                    info.PlayerName = reader["PlayerName"].ToString();
                    info.GroomID = (int)reader["GroomID"];
                    info.GroomName = reader["GroomName"].ToString();
                    info.BrideID = (int)reader["BrideID"];
                    info.BrideName = reader["BrideName"].ToString();
                    info.Pwd = reader["Pwd"].ToString();
                    info.AvailTime = (int)reader["AvailTime"];
                    info.MaxCount = (int)reader["MaxCount"];
                    info.GuestInvite = (bool)reader["GuestInvite"];
                    info.MapIndex = (int)reader["MapIndex"];
                    info.BeginTime = (DateTime)reader["BeginTime"];
                    info.BreakTime = (DateTime)reader["BreakTime"];
                    info.RoomIntroduction = reader["RoomIntroduction"].ToString();
                    info.ServerID = (int)reader["ServerID"];
                    info.IsHymeneal = (bool)reader["IsHymeneal"];
                    info.IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"];
                    infos.Add(info);
                }

                return infos.ToArray();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetMarryRoomInfo", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return null;
        }

        public MarryRoomInfo GetMarryRoomInfoSingle(int id)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", id);

                db.GetReader(ref reader, "SP_Get_Marry_Room_Info_Single", para);
                while (reader.Read())
                {
                    MarryRoomInfo info = new MarryRoomInfo();
                    info.ID = (int)reader["ID"];
                    info.Name = reader["Name"].ToString();
                    info.PlayerID = (int)reader["PlayerID"];
                    info.PlayerName = reader["PlayerName"].ToString();
                    info.GroomID = (int)reader["GroomID"];
                    info.GroomName = reader["GroomName"].ToString();
                    info.BrideID = (int)reader["BrideID"];
                    info.BrideName = reader["BrideName"].ToString();
                    info.Pwd = reader["Pwd"].ToString();
                    info.AvailTime = (int)reader["AvailTime"];
                    info.MaxCount = (int)reader["MaxCount"];
                    info.GuestInvite = (bool)reader["GuestInvite"];
                    info.MapIndex = (int)reader["MapIndex"];
                    info.BeginTime = (DateTime)reader["BeginTime"];
                    info.BreakTime = (DateTime)reader["BreakTime"];
                    info.RoomIntroduction = reader["RoomIntroduction"].ToString();
                    info.ServerID = (int)reader["ServerID"];
                    info.IsHymeneal = (bool)reader["IsHymeneal"];
                    info.IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"];
                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetMarryRoomInfo", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return null;
        }

        public bool UpdateBreakTimeWhereServerStop()
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[0].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", para);
                result = (int)para[0].Value == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdateBreakTimeWhereServerStop", e);

            }

            return result;
        }

        public MarryProp GetMarryProp(int id)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", id);

                db.GetReader(ref reader, "SP_Select_Marry_Prop", para);
                while (reader.Read())
                {
                    MarryProp info = new MarryProp();
                    info.IsMarried = (bool)reader["IsMarried"];
                    info.SpouseID = (int)reader["SpouseID"];
                    info.SpouseName = reader["SpouseName"].ToString();
                    info.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
                    info.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
                    info.IsGotRing = (bool)reader["IsGotRing"];
                    return info;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("GetMarryProp", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return null;
        }

        public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@ApplyUserID", info.ApplyUserID);
                para[2] = new SqlParameter("@ApplyUserName", info.ApplyUserName);
                para[3] = new SqlParameter("@ApplyType", info.ApplyType);
                para[4] = new SqlParameter("@ApplyResult", info.ApplyResult);
                para[5] = new SqlParameter("@LoveProclamation", info.LoveProclamation);
                para[6] = new SqlParameter("@AnswerId", answerId);
                para[7] = new SqlParameter("@ouototal", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.Output;
                para[8] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;


                db.RunProcedure("SP_Insert_Marry_Notice", para);
                id = (int)para[7].Value;
                result = (int)para[8].Value == 0;


            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("SavePlayerMarryNotice", e);
            }

            return result;
        }

        public bool UpdatePlayerGotRingProp(int groomID, int brideID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@GroomID", groomID);
                para[1] = new SqlParameter("@BrideID", brideID);
                para[2] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[2].Direction = ParameterDirection.ReturnValue;

                db.RunProcedure("SP_Update_GotRing_Prop", para);
                result = (int)para[2].Value == 0;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdatePlayerGotRingProp", e);
            }
            return result;
        }


        #endregion

        public bool Test(string DutyName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@DutyName", DutyName);
                result = db.RunProcedure("SP_Test1", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool TankAll()
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[0];
                result = db.RunProcedure("SP_Tank_All", para);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public bool RegisterUser(string UserName,string NickName,string Password,bool Sex,int Money,int GiftToken,int Gold )
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@UserName", UserName);
                para[1] = new SqlParameter("@Password", Password);
                para[2] = new SqlParameter("@NickName", NickName);
                para[3] = new SqlParameter("@Sex", Sex);
                para[4] = new SqlParameter("@Money", Money);
                para[5] = new SqlParameter("@GiftToken", GiftToken);
                para[6] = new SqlParameter("@Gold", Gold);

                para[7] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;
               db.RunProcedure("SP_Account_Register", para);
                if ((int)para[7].Value == 0) result = true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init Register", e);
            }
            return result;
        }
        public bool CheckEmailIsValid(string Email)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@Email", Email);
                para[1] = new SqlParameter("@count", 0);
                para[1].Direction = ParameterDirection.Output;
                db.RunProcedure("CheckEmailIsValid", para);
                if (Int32.Parse(para[1].Value.ToString()) == 0) result = true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init CheckEmailIsValid", e);
            }
            return result;
        }

        public bool RegisterUserInfo(UserInfo userinfo)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];

                para[0] = new SqlParameter("@UserID", userinfo.UserID);
                para[1] = new SqlParameter("@UserEmail", userinfo.UserEmail);
                para[2] = new SqlParameter("@UserPhone", userinfo.UserPhone == null ? string.Empty : userinfo.UserPhone);
                para[3] = new SqlParameter("@UserOther1", userinfo.UserOther1 == null ? string.Empty : userinfo.UserOther1);
                para[4] = new SqlParameter("@UserOther2", userinfo.UserOther2 == null ? string.Empty : userinfo.UserOther2);
                para[5] = new SqlParameter("@UserOther3", userinfo.UserOther3 == null ? string.Empty : userinfo.UserOther3);
         

                result = db.RunProcedure("[SP_User_Info_Add]", para);
               
               return result;
               
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }
        public UserInfo GetUserInfo(int UserId)
        {
           
            SqlDataReader reader = null;
            UserInfo user=new UserInfo(){UserID=UserId};
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", UserId);

                db.GetReader(ref reader, "SP_Get_User_Info", para);
                
                while (reader.Read())
                {
                    user.UserID = Int32.Parse(reader["UserID"].ToString());
                    user.UserEmail = reader["UserEmail"] == null ? "" : reader["UserEmail"].ToString();
                    user.UserPhone = reader["UserPhone"] == null ? "" : reader["UserPhone"].ToString();
                    user.UserOther1 = reader["UserOther1"] == null ? "" : reader["UserOther1"].ToString();
                    user.UserOther2 = reader["UserOther2"] == null ? "" : reader["UserOther2"].ToString();
                    user.UserOther3 = reader["UserOther3"] == null ? "" : reader["UserOther3"].ToString();
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
            return user;
        }
    }
}
