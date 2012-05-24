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
    public class LoadMajors : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request.Params["id"];
            if (!string.IsNullOrEmpty(id))
            {
                Guid did = new Guid(id);
                var query = from u in DbCenter.QueryDb.SysUniversityprofessionals
                            where u.DepartmentId == did
                            select u;

                XElement list = new XElement("list");
                foreach (var q in query)
                {
                    list.Add(new XElement("item", new XAttribute("id", q.Id),
                                                new XAttribute("name", q.ProfessionalName)));
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
