using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using RoadDatabase;

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class Login : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string user = context.Request.Params["user"];
            string pass = context.Request.Params["pass"];
            XElement result = new XElement("result");
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                SysUsers p = DbCenter.QueryDb.SysUsers.First(c => c.UserName == user && c.PassWord == pass);
                if (user != null)
                {
                    context.Response.Cookies.Add(new HttpCookie("id", p.ID.ToString()));
                    result.Add(new XAttribute("value", true));
                }
                else
                {
                    result.Add(new XAttribute("value", false));
                    result.Add(new XAttribute("message", "用户名或者密码错误!"));
                }
            }
            else 
            {
                result.Add(new XAttribute("value", false));
                result.Add(new XAttribute("message", "参数个数不正确!"));
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(result.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
