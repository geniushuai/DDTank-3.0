using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using RoadDatabase;
using Road.Flash;

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class LoadUserItems : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {                                                                                                                                                                                                                                                                                                          
            string user = context.Request.QueryString["user"];
            string pass = context.Request.QueryString["pass"];

            SysUsers player = DbCenter.QueryDb.SysUsers.SingleOrDefault(c => c.UserName == user && c.PassWord == pass);
            if (player != null)
            {
                var list = from g in DbCenter.QueryDb.GameUsersGoods
                           join m in DbCenter.QueryDb.GameSysgoodsmodels on g.GoodsModelId equals m.ID
                           where g.UserId == player.ID && g.State == 0
                           select new Goods { Good = g, Model = m };

                XElement node = new XElement("list");
                foreach (var t in list)
                {
                    node.Add(FlashUtils.CreateGoods(t));
                }

                context.Response.ContentType = "text/plain";
                context.Response.Write(node.ToString(false));
            }
            else
            {
                context.Response.Write(string.Format("用户不存在:{0}",user));
            }
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
