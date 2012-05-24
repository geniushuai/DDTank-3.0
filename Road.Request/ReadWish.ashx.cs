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
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ReadWish : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["id"]))
            {
                Guid id = new Guid(context.Request["id"]);

                XElement list = new XElement("list");

                XElement wish;
                XElement item;
                GameUserextendwishes w = DbCenter.QueryDb.GameUserextendwishes.Single(c => c.Id == id);
                if (w != null)
                {
                    w.ViewNum++;
                    DbCenter.QueryDb.SubmitChanges();
                    SysUsers u = DbCenter.QueryDb.SysUsers.Single(us => us.ID == w.ExtendUserId);
                    wish = new XElement("wish", new XAttribute("id", w.Id.ToString()),
                        new XAttribute("index", w.Indexs.ToString()),
                                          new XAttribute("userId", w.ExtendUserId.ToString()),
                                           new XAttribute("content", w.Content),
                                             new XAttribute("nickName", u.PetName),
                                             new XAttribute("wishType", w.Wishtype.ToString()),
                                              new XAttribute("time", w.Date.ToShortDateString()),
                                               new XAttribute("stone", w.StoneId.ToString()),
                                                new XAttribute("viewNums", w.ViewNum.ToString()),
                                                  new XAttribute("isAnonymity", w.IsNim.ToString())
                        );

                    item = FlashUtils.CreateBaseMemberInfo(u, "");
                    list.Add(wish);
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
