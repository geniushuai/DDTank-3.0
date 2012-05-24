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

namespace Tank.Flash
{
    public partial class _Default : System.Web.UI.Page
    {
        private string _content = "";

        public string Content
        {
            get
            {
                return _content;
            }
        }
        public string Edition
        {
            get
            {
                return ConfigurationSettings.AppSettings["Edition"].ToLower();
            }
        }
        public string LoginOnUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginOnUrl"];
            }
        }

        public string SiteTitle
        {
            get
            {
                return ConfigurationSettings.AppSettings["SiteTitle"] == null ? "弹弹堂" : ConfigurationSettings.AppSettings["SiteTitle"];
            }
        }

        public long Rand
        {
            get
            {
                return DateTime.Now.Ticks;
            }
        }

        private string autoParam = "";
        public string AutoParam
        {
            get
            {
                return autoParam;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string user = HttpUtility.UrlDecode(Request["user"]);
                string key = HttpUtility.UrlDecode(Request["key"]);
                string site = Request["site"] == null ? "" : HttpUtility.UrlDecode(Request["site"]).ToLower();
                string sitename = Request["sitename"] == null ? "" : HttpUtility.UrlDecode(Request["sitename"]);
                //string fav_name = Request["fav_name"] == null ? "" : HttpUtility.UrlDecode(Request["fav_name"]);
                //string fav_url = Request["fav_url"] == null ? "" : HttpUtility.UrlDecode(Request["fav_url"]);
                //string exit_url = Request["exit_url"] == null ? "" : HttpUtility.UrlDecode(Request["exit_url"]);
                //string pay_url = Request["pay_url"] == null ? "" : HttpUtility.UrlDecode(Request["pay_url"]);

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(key))
                {
                    if (!string.IsNullOrEmpty(site))
                    {
                        string refSite = ConfigurationSettings.AppSettings["Site"] == null ? "" : ConfigurationSettings.AppSettings["Site"].ToString().ToLower();
                        if (refSite == "true")
                        {
                            user = string.Format("{0}_{1}", site, user);
                        }
                    }

                    //_content = Request.QueryString.ToString();
                    _content = "user=" + HttpUtility.UrlEncode(user) + "&key=" + HttpUtility.UrlEncode(key);
                    autoParam = "site=" + HttpUtility.UrlEncode(site) + "&sitename=" + HttpUtility.UrlEncode(sitename);
                    //autoParam = "fav_name=" + fav_name + "&fav_url=" + fav_url + "&exit_url=" + exit_url + "&pay_url=" + pay_url;
                }
                else
                {
                    Response.Redirect(LoginOnUrl,false);
                }

            }
            catch
            {
                Response.Redirect(LoginOnUrl,false);
            }

        }
    }
}
