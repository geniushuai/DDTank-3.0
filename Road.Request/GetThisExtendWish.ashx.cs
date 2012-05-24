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
    public class GetThisExtendWish : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["index"]) && !string.IsNullOrEmpty(context.Request["sceneid"]))
            {
                int page = Convert.ToInt32(context.Request["index"]);
                var query = from w in DbCenter.QueryDb.GameUserextendwishes                       
                            select w;
                query = query.Where(wh => wh.SceneId == context.Request["sceneid"].ToString());
                IList<GameUserextendwishes> list = query.OrderByDescending(c=>c.Indexs).Skip(page - 1).Take(1).ToList();
                XElement item;
                foreach (GameUserextendwishes w in list)
                {
                    SysUsers user = DbCenter.QueryDb.SysUsers.Single(u => u.ID == w.ExtendUserId);
                    item = new XElement("item", new XAttribute("id", w.Id.ToString()),
                        new XAttribute("index", w.Indexs.ToString()),
                         new XAttribute("userId", w.ExtendUserId.ToString()),
                          new XAttribute("content", w.Content.ToString()),
                           new XAttribute("nickName", user.PetName),
                            new XAttribute("wishType", w.Wishtype.ToString()),
                              new XAttribute("time", w.Date.ToShortDateString()),
                               new XAttribute("stone", w.SceneId.ToString()),
                                new XAttribute("viewNums", w.ViewNum.ToString()),
                                 new XAttribute("isAnonymity", w.IsNim.ToString())

                        );
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(item.ToString());


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
