using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using RoadDatabase;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class AddUserExtendWish : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["extenduserid"]))
            {
                Guid sendid = new Guid(context.Request["extenduserid"]);
                var query = from w in DbCenter.QueryDb.GameUserextendwishes
                            where w.ExtendUserId == sendid && w.Date>= DateTime.Now.Date
                            select w;
                int count = query.Count();
                XElement item = new XElement("item");
                if (count > 0)
                {
                    item.Add(new XAttribute("Result", "false"));
                    item.Add(new XAttribute("Msg", "你今天已经许过愿了！"));
                }
                else
                {
                    bool IsSuc = false;


                    int lastindex = Convert.ToInt32(DbCenter.QueryDb.GameUserextendwishes.Select(w => w.Indexs).Max());
                    GameUserextendwishes wish = new GameUserextendwishes();
                    wish.Indexs = lastindex + 1;
                  
                    wish.Content = (!string.IsNullOrEmpty(context.Request["content"])) ? context.Request["content"] : "";
                    wish.Date = DateTime.Now;
                    wish.ExtendUserId = sendid;
                    wish.Id = Guid.NewGuid();
                    if (!string.IsNullOrEmpty(context.Request["isAnonymity"]))
                    {
                        wish.IsNim = Convert.ToBoolean(Convert.ToInt32(context.Request["isAnonymity"]));
                    }
                    if (!string.IsNullOrEmpty(context.Request["wishtype"]))
                    {
                        wish.Wishtype = Convert.ToInt32(context.Request["wishtype"]);
                    }

                    wish.RecevedUserId = sendid;
                    if (!string.IsNullOrEmpty(context.Request["currentSceneId"]))
                    {
                        wish.SceneId = context.Request["currentSceneId"];
                    }
                    if (!string.IsNullOrEmpty(context.Request["stoneid"]))
                    {
                        wish.StoneId = Convert.ToInt32(context.Request["stoneid"]);
                    }
                    DbCenter.QueryDb.GameUserextendwishes.InsertOnSubmit(wish);
                    DbCenter.QueryDb.SubmitChanges();

                    IsSuc = true;

                    if (IsSuc)
                    {
                        item.Add(new XAttribute("Result", "true"));
                        item.Add(new XAttribute("Msg", ""));
                    }
                    else
                    {
                        item.Add(new XAttribute("Result", "true"));
                        item.Add(new XAttribute("Msg", "你输入的用户不存在!"));

                    }
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write(item.ToString(false));
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
