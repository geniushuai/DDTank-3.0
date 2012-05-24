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
    public class LoadUniversitiesByProvince : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request.Params["id"];
            if (!string.IsNullOrEmpty(id))
            {
                int cid = int.Parse(id);
                var query = from s in DbCenter.QueryDb.SysSchools
                            join c in DbCenter.QueryDb.Places on s.City equals c.PlaceName
                            join p in DbCenter.QueryDb.Places on c.Pid equals p.Id
                            where p.Id == cid && s.Category == 0 && s.IsOpen == true
                            select s;


                XElement list = new XElement("list");
                foreach (var q in query)
                {
                    list.Add(new XElement("item", new XAttribute("id", q.ID),
                                                new XAttribute("name", q.SchoolName)));
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
