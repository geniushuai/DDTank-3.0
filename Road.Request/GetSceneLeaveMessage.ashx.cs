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
using System.Collections.Generic;

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GetSceneLeaveMessage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["currentSceneId"]))
            {
                string sceneid = context.Request["currentSceneId"];
                var query = from m in DbCenter.QueryDb.GameUsersceneleavemessage select m;
                XElement list = new XElement("list");
                query = query.Where(l => l.ScendId == sceneid).OrderByDescending(c => c.AddTime);
                IList<GameUsersceneleavemessage> mlist = query.Take(200).ToList();
                foreach (GameUsersceneleavemessage ms in mlist)
                {
                    XElement item = new XElement("item",
                        new XAttribute("id", ms.Id.ToString()),
                            new XAttribute("content", ms.Conent),
                                new XAttribute("userid", ms.UserId.ToString()),
                                    new XAttribute("time", ms.AddTime.ToString())

                        );
                    list.Add(item);
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
