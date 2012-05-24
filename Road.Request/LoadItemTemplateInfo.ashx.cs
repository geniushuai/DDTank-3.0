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
    public class LoadItemTemplateInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request.Params["id"];
            if (!string.IsNullOrEmpty(id))
            {
                Guid mid = new Guid(id);

                GameSysgoodsmodels t = DbCenter.QueryDb.GameSysgoodsmodels.SingleOrDefault( c=> c.ID == mid);
                if (t != null)
                {
                    XElement node = new XElement("item", new XAttribute("id", t.ID),
                                                    new XAttribute("name", t.ModelName),
                                                    new XAttribute("needLevel", t.Grade),
                                                    new XAttribute("styleIndex", t.StyleIndex),
                                                    new XAttribute("styleClass", t.StyleClass),
                                                    new XAttribute("url", t.Pic),
                                                    new XAttribute("money", t.NeedVirtualMoney),
                                                    new XAttribute("category", t.CategoryId),
                                                    new XAttribute("sex", t.Sex),
                                                    new XAttribute("limitedDate", t.EffectiveDays),
                                                    new XAttribute("sellDate", t.AddTime),
                                                    new XAttribute("buyedCount", t.SentOutNum),
                                                    new XAttribute("isCommend", t.IsCommend),
                                                    new XAttribute("isGift", t.IsGift),
                                                    new XAttribute("mark", t.NeedMark));
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(node.ToString(false));
                }
                
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
