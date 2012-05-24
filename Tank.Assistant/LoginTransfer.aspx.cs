using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Bussiness.Interface;

namespace Tank.Assistant
{
    public partial class LoginTransfer : System.Web.UI.Page
    {
        //http://game_server/login.php?userid=1234&username=abc&time=12341234134&flag=md5字串
        //http://web.4399.com/api/game/passport.php?userid=1234&username=abc&time=12341234134&flag=md5字串 

        public static string GetLoginIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginIP"];
            }
        }

        public static bool ValidLoginIP(string ip)
        {
            string ips = GetLoginIP;
            if (string.IsNullOrEmpty(ips) || ips.Split('|').Contains(ip))
                return true;
            return false;
        } 

        public static string FlashUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["FlashUrl"];
            }
        }

        public string ValidateUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["ValidateUrl"];
            }
        }

        public string LoginOnUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginOnUrl"];
            }
        }

        public string ServerID
        {
            get
            {
                return ConfigurationSettings.AppSettings["ServerID"];
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string result = "";
            try
            {
                if (ValidLoginIP(Context.Request.UserHostAddress))
                {
                    string content = Request.Url.Query.ToString();
                    string userid = HttpUtility.UrlDecode(Request["userid"]);
                    string username = HttpUtility.UrlDecode(Request["username"]);
                    string time = HttpUtility.UrlDecode(Request["time"]);
                    string flag = HttpUtility.UrlDecode(Request["flag"]);

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(time) && !string.IsNullOrEmpty(flag))
                    {
                        //flag=md5(userid + username + time + 密钥 ) 
                        string newFlag = BaseInterface.md5(userid + username + time + BaseInterface.GetLoginKey);
                        DateTime date = BaseInterface.ConvertIntDateTime(int.Parse(time));
                        if (flag == newFlag && DateTime.Now.AddMinutes(5).CompareTo(date) > 0 && DateTime.Now.AddMinutes(-5).CompareTo(date) < 0)
                        {
                            if (!string.IsNullOrEmpty(ValidateUrl))
                            {
                                string validateUrl = ValidateUrl + content + "&serverid" + ServerID;
                                result = BaseInterface.RequestContent(ValidateUrl);
                            }
                            else
                            {
                                result = "0";
                            }

                            if (result == "0")
                            {
                                string password = Guid.NewGuid().ToString();
                                //int time = BaseInterface.ConvertDateTimeInt(DateTime.Now);
                                string v = BaseInterface.md5(username + password + time.ToString() + BaseInterface.GetLoginKey);
                                string Url = BaseInterface.LoginUrl + "?content=" + HttpUtility.UrlEncode(username + "|" + password + "|" + time.ToString() + "|" + v);
                                result = BaseInterface.RequestContent(Url);
                                if (result == "0")
                                {
                                    string flashUrl = FlashUrl + "?user=" + HttpUtility.UrlEncode(username) + "&key=" + HttpUtility.UrlEncode(password);
                                    Response.Redirect(flashUrl, false);
                                    return;
                                }
                                result = "";
                            }
                        }
                    }
                }
                else
                {
                    //Response.Redirect(LoginOnUrl + "?" + , false);
                    Response.Write(Context.Request.UserHostAddress);
                    return;
                }

                Response.Redirect(LoginOnUrl, false);
            }
            catch
            {
                Response.Redirect(LoginOnUrl, false);
            }
        }

    }
}
