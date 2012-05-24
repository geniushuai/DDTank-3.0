using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using RoadDatabase;
using System.Collections.Generic;

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GetGameSecne : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["schoolId"]))
            {
                Guid schoolid = new Guid(context.Request["schoolId"]);
                XElement list = new XElement("list");
                var query = from s in DbCenter.QueryDb.GameScenes select s;
                query = query.Where(s => s.SchoolId == schoolid);

                if (!string.IsNullOrEmpty(context.Request["type1"]))
                {
                    int type1 = Convert.ToInt32(context.Request["type1"]);
                    if (type1 != 0)
                    {
                        query = query.Where(s => s.Type1 == type1);
                    }
                }

                query = query.Where(s => s.Type == 1);

                IList<GameScenes> slist = query.ToList();
                foreach (GameScenes s in slist)
                {
                    XElement item = new XElement("item",
                        new XAttribute("id", s.ID.ToString()),
                        new XAttribute("sceneName", s.SceneName),
                        new XAttribute("userNum", s.UserNum.ToString()),
                        new XAttribute("maxUserNum", s.MaxUserNum.ToString()),
                        new XAttribute("type", s.Type.ToString()),
                        new XAttribute("type1", s.Type1.ToString())

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
