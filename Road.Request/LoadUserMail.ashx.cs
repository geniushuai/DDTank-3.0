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
    public class LoadUserMail : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string user = context.Request.QueryString["user"];
            string pass = context.Request.QueryString["pass"];
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                XElement node = new XElement("list");

                var mail = from e in DbCenter.QueryDb.UserMessages
                           join u in DbCenter.QueryDb.SysUsers on e.ReceiverID equals u.ID
                           join f in DbCenter.QueryDb.SysUsers on e.SenderID equals f.ID
                           where u.UserName == user && u.PassWord == pass && e.IsDelete == false
                           select new { Mail = e, From = f.Email };

                foreach (var m in mail)
                {
                    XElement temp = new XElement("item", new XAttribute("id", m.Mail.ID),
                                                new XAttribute("title", m.Mail.Title),
                                                new XAttribute("content", m.Mail.Content),
                                                new XAttribute("sender", m.From),
                                                new XAttribute("sendTime", m.Mail.SendTime),
                                                new XAttribute("isRead", m.Mail.IsRead));
                    XElement t1 = SeleteMailAttachment(m.Mail.Annex1);
                    if (t1 != null)
                    {
                        temp.Add(t1);
                    }

                    XElement t2 = SeleteMailAttachment(m.Mail.Annex2);
                    if (t2 != null)
                    {
                        temp.Add(t2);
                    }
                    node.Add(temp);
                }

                context.Response.Write(node.ToString(false));
            }
            else
            {
                context.Response.Write("参数错误");
            }

            context.Response.ContentType = "text/plain";
        }

        private XElement SeleteMailAttachment(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var item = from g in DbCenter.QueryDb.GameUsersGoods
                           join m in DbCenter.QueryDb.GameSysgoodsmodels on g.GoodsModelId equals m.ID
                           where g.Id == new Guid(id)
                           select new Goods { Good = g, Model = m };
                Goods goodes = item.FirstOrDefault();
                if (goodes != null)
                {
                    return FlashUtils.CreateGoods(goodes);
                }
 
            }
            return null;
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
