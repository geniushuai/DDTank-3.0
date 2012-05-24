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
    public class LoadCountries : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            XElement list = new XElement("list");
            list.Add(new XElement("item", new XAttribute("id",0),
                                        new XAttribute("name", "中国")));

            context.Response.ContentType = "text/plain";
            context.Response.Write(list.ToString(false));
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
