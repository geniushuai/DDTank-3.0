using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using log4net;
using System.Reflection;
using SqlDataProvider.Data;
using System.Configuration;
using Bussiness.CenterService;

namespace Bussiness.Interface
{
    public abstract class BaseInterface
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region interface

        public static string GetInterName
        {
            get
            {
                return ConfigurationSettings.AppSettings["InterName"].ToLower();
            }
        }

        public static string GetLoginKey
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginKey"];
            }
        }

        public static string GetChargeKey
        {
            get
            {
                return ConfigurationSettings.AppSettings["ChargeKey"];
            }
        }

        public static string LoginUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginUrl"];
            }
        }

        #endregion

        #region Active

        public virtual int ActiveGold
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["DefaultGold"]);
            }
        }

        public virtual int ActiveMoney
        {
            get
            {
                return int.Parse(ConfigurationSettings.AppSettings["DefaultMoney"]);
            }
        }

        #endregion

        #region 辅助方法

        public static string GetNameBySite(string user, string site)
        {

            if (!string.IsNullOrEmpty(site))
            {
                string key = ConfigurationSettings.AppSettings[string.Format("LoginKey_{0}", site)];
                if (!string.IsNullOrEmpty(key))
                {
                    user = string.Format("{0}_{1}", site, user);
                }
            }
            return user;
        }


        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static System.DateTime ConvertIntDateTime(double d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddSeconds(d);
            return time;
        }

        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return (int)intResult;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string md5(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
        }

        /// <summary>
        /// 页面请求
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string RequestContent(string Url)
        {
            //byte[] buf = new byte[256];
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //request.ContentType = "text/plain";
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //Stream resStream = response.GetResponseStream();
            //int count = resStream.Read(buf, 0, buf.Length);
            //string result = Encoding.UTF8.GetString(buf, 0, count);
            //resStream.Close();
            //return result;

            return RequestContent(Url, 2560);
        }

        public static string RequestContent(string Url, int byteLength)
        {
            byte[] buf = new byte[byteLength];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.ContentType = "text/plain";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int count = resStream.Read(buf, 0, buf.Length);
            string result = Encoding.UTF8.GetString(buf, 0, count);
            resStream.Close();
            return result;
        }

        /// <summary>
        /// 页面请求
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string RequestContent(string Url, string param, string code)
        {
            Encoding GBK = Encoding.GetEncoding(code);
            byte[] bs = GBK.GetBytes(param);
            //byte[] bs = Encoding.UTF8.GetBytes(param);

            string temp = GBK.GetString(bs);
            //bool res = false;
            //if(param==temp)
            //{
            //    res = true;
            //}

            byte[] buf = new byte[2560];
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(Url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded;charset=utf8";
            //req.ContentType = "application/x-www-form-urlencoded;charset=" + code;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理
                HttpWebResponse response = (HttpWebResponse)wr;
                Stream resStream = response.GetResponseStream();
                int count = resStream.Read(buf, 0, buf.Length);
                string result = Encoding.UTF8.GetString(buf, 0, count);

                return result;
            }

        }

        #endregion

        public static BaseInterface CreateInterface()
        {
            switch (GetInterName)
            {
                case "qunying":
                    return new QYInterface();
                case "sevenroad":
                    return new SRInterface();
                case "duowan":
                    return new DWInterface();
            }
            return null;
        }

        public virtual PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, string nickname)
        {
            try
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    bool isExist = true;
                    DateTime forbidDate = DateTime.Now;
                    PlayerInfo info = db.LoginGame(name, ref isFirst, ref isExist, ref isError, firstValidate, ref forbidDate, nickname);
                    if (info == null)
                    {
                        if (!db.ActivePlayer(ref info, name, password, true, ActiveGold, ActiveMoney, IP, site))
                        {
                            info = null;
                            // message = "Active is fail!";
                            message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Fail");
                        }
                        else
                        {
                            isActive = true;
                            using (CenterServiceClient client = new CenterServiceClient())
                            {
                                client.ActivePlayer(true);
                            }
                        }
                    }
                    else
                    {
                        if (isExist)
                        {
                            using (CenterServiceClient client = new CenterServiceClient())
                            {
                                client.CreatePlayer(info.ID, name, password, isFirst == 0);
                            }
                        }
                        else
                        {
                            message = LanguageMgr.GetTranslation("ManageBussiness.Forbid1", forbidDate.Year, forbidDate.Month, forbidDate.Day, forbidDate.Hour, forbidDate.Minute);
                            return null;
                        }
                    }
                    return info;
                }

            }
            catch (Exception ex)
            {
                log.Error("LoginAndUpdate", ex);
            }
            return null;
        }

        public virtual PlayerInfo LoginGame(string name, string pass, ref bool isFirst)
        {
            try
            {
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    int userID = 0;
                    if (client.ValidateLoginAndGetID(name, pass, ref userID, ref isFirst))
                    {
                        PlayerInfo player = new PlayerInfo();
                        player.ID = userID;
                        player.UserName = name;
                        return player;
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("LoginGame", ex);
            }

            return null;

        }

        public virtual string[] UnEncryptLogin(string content, ref int result, string site)
        {
            try
            {
                string key = string.Empty;
                if (!string.IsNullOrEmpty(site))
                {
                    key = ConfigurationSettings.AppSettings[string.Format("LoginKey_{0}", site)];
                }

                if (string.IsNullOrEmpty(key))
                {
                    key = GetLoginKey;
                }

                if (!string.IsNullOrEmpty(key))
                {
                    string[] str = content.Split('|');
                    if (str.Length > 3)
                    {
                        string v = md5(str[0] + str[1] + str[2] + key);
                        if (v == str[3].ToLower())
                            return str;
                        else
                        {
                            result = 5;
                        }
                    }
                    else
                    {
                        result = 2;
                    }
                }
                else
                {
                    result = 4;
                }


            }
            catch (Exception ex)
            {
                log.Error("UnEncryptLogin", ex);
            }
            return new string[0];
        }

        public virtual string[] UnEncryptCharge(string content, ref int result, string site)
        {
            try
            {
                string key = string.Empty;
                if (!string.IsNullOrEmpty(site))
                {
                    key = ConfigurationSettings.AppSettings[string.Format("ChargeKey_{0}", site)];
                }

                if (string.IsNullOrEmpty(key))
                {
                    key = GetChargeKey;
                }

                if (!string.IsNullOrEmpty(key))
                {
                    string[] str = content.Split('|');
                    string v = md5(str[0] + str[1] + str[2] + str[3] + str[4] + key);

                    if (str.Length > 5)
                    {
                        if (v == str[5].ToLower())
                            return str;
                        result = 7;
                    }
                    else
                    {
                        result = 8;
                    }
                }
                else
                {
                    result = 6;
                }

            }
            catch (Exception ex)
            {
                log.Error("UnEncryptCharge", ex);
            }

            return new string[0];
        }


        public virtual string[] UnEncryptSentReward(string content, ref int result, string key)
        {
            try
            {
                //         0       1     2       3       4   5    6    7
                //content=title|content|username|gold|money|param|time|v
                string[] str = content.Split('#');

                //检查content的参数合法
                if (str.Length == 8)
                {

                    string str_spanTime = System.Configuration.ConfigurationSettings.AppSettings["SentRewardTimeSpan"];
                    int int_spantime = Int32.Parse(string.IsNullOrEmpty(str_spanTime) ? "1" : str_spanTime);
                    //检查time是否超时
                    TimeSpan timeSpan = string.IsNullOrEmpty(str[6]) ? new TimeSpan(1, 1, 1) : DateTime.Now - BaseInterface.ConvertIntDateTime(Double.Parse(str[6]));
                    if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes < int_spantime)
                    {
                        //检查Key
                        if (string.IsNullOrEmpty(key))
                            return str;
                        else
                        {
                            string v = md5(str[2] + str[3] + str[4] + str[5] + str[6] + key);
                            if (v == str[7].ToLower())
                                return str;
                            else
                                result = 5;     //Key无效
                        }
                    }
                    else
                    {
                        result = 7;  //time超时
                    }

                }
                else
                {
                    result = 6;  //content参数不对
                }

            }
            catch (Exception ex)
            {
                log.Error("UnEncryptSentReward", ex);
            }

            return new string[0];
        }



        public virtual bool GetUserSex(string name)
        {
            return true;
        }

    }
}
