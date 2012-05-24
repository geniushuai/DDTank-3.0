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
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LoadUserDefinedGoods : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["user"]) && !string.IsNullOrEmpty(context.Request["pass"]))
            {
                SysUsers user = DbCenter.QueryDb.SysUsers.SingleOrDefault(u => u.Email == context.Request["user"] && u.PassWord == context.Request["pass"]);
                if (user == null)
                {
                    return;

                }
                XElement list = new XElement("list");


                XElement item;


                var glist = from g in DbCenter.QueryDb.GameUserdefinedgoods where g.UserId == user.ID && g.State == 0 select g;

                foreach (GameUserdefinedgoods g in glist)
                {
                    GameSysgoodsmodels model = DbCenter.QueryDb.GameSysgoodsmodels.SingleOrDefault(m => m.ID == g.GoodsModelId);

                    if (model != null)
                    {

                        item = new XElement("item", new XAttribute("id", g.ID.ToString()),
                       new XAttribute("posX", g.PosX.ToString()),
                         new XAttribute("posY", g.PosY.ToString()),
                           new XAttribute("dir", g.Dir.ToString()),
                             new XAttribute("height", model.Height),
                               new XAttribute("roomId", g.InObjectId),
                                 new XAttribute("asset", model.Asset),
                                   new XAttribute("func", model.Func),
                                    new XAttribute("type", model.CategoryId),
                            new XAttribute("riseNum",g.RiseNum)
                       );
                        list.Add(item);
                    }
                }

                context.Response.ContentType = "text/plain";
                context.Response.Write(list.ToString(false));

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
