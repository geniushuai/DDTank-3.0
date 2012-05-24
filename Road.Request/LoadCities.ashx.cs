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
    public class LoadCities : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request["id"];
            if (!string.IsNullOrEmpty(id))
            {
                int pid = int.Parse(id);
                var query = from q in DbCenter.QueryDb.Places
                            where q.Pid == pid
                            select q;

                XElement node = new XElement("list");
                foreach (var p in query)
                {
                    node.Add(new XElement("item", new XAttribute("id", p.Id),
                                                new XAttribute("name", p.PlaceName)));
                }

                context.Response.ContentType = "text/plain";
                context.Response.Write(node.ToString(false));
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
