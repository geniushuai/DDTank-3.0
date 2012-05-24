using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Text;
using RoadDatabase;
using System.Collections.Generic;

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SearchUserExtendWish : IHttpHandler
    {
        class UserWish
        {
            public SysUsers User { get; set; }
            public GameUserextendwishes Wish { get; set; }
            public string SceneId { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //if (!string.IsNullOrEmpty(context.Request["keyword"]))
            //{
            string keyword = context.Request["keyword"];
            int page = 1, totalpage = 0;
            if (!string.IsNullOrEmpty(context.Request["page"]))
            {
                page = Convert.ToInt32(context.Request["page"]);
            }

            int type = 0;
            if (!string.IsNullOrEmpty(context.Request["type"]))
            {
                type = Convert.ToInt32(context.Request["type"]);
            }
            var query = from w in DbCenter.QueryDb.GameUserextendwishes
                        join u in DbCenter.QueryDb.SysUsers on w.ExtendUserId equals u.ID
                        join s in DbCenter.QueryDb.GameScenes on w.SceneId equals s.ID
                        orderby w.Indexs descending
                        select new UserWish { User = u, SceneId = s.ID, Wish = w };
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    SysUsers user = DbCenter.QueryDb.SysUsers.Single(u => u.ID == new Guid(keyword));
                    query = query.Where(u => u.User.SchoolId == user.SchoolId);

                    break;
                case 2:
                    query = query.Where(u => u.User.Sex == true);
                    break;
                case 3:
                    query = query.Where(u => u.User.Sex == false);
                    break;
                case 4:
                    query = query.Where(u => u.User.PetName.Equals(keyword));
                    break;
                case 5:
                    query = query.Where(u => u.User.TrueName.Equals(keyword));
                    break;
                case 6:
                    query = query.Where(u => u.User.Email.Equals(keyword));
                    break;
            }
            if (!string.IsNullOrEmpty(context.Request["sceneid"]))
            {
                query = query.Where(w => w.SceneId == context.Request["sceneid"].ToString());
            }
            int totalcout = query.Count();
            if ((totalcout % 8) != 0)
            {
                totalpage = (totalcout / 8) + 1;
            }
            else
            {
                totalpage = totalcout / 8;
            }
            IList<UserWish> list = query.Take(page * 8).Except(query.Take((page - 1) * 8)).ToList();
            XElement node = new XElement("list");

            foreach (UserWish i in list)
            {
                node.Add(new XElement("item", new XAttribute("id", i.Wish.Id.ToString()),
                                          new XAttribute("index", i.Wish.Indexs.ToString()),
                                          new XAttribute("userId", i.Wish.ExtendUserId.ToString()),
                                           new XAttribute("content", i.Wish.Content),
                                            new XAttribute("nickName", i.User.PetName),
                                             new XAttribute("wishType", i.Wish.Wishtype.ToString()),
                                              new XAttribute("time", i.Wish.Date.ToShortDateString()),
                                               new XAttribute("stone", i.Wish.StoneId.ToString()),
                                                new XAttribute("viewNums", i.Wish.ViewNum.ToString()),
                                                  new XAttribute("isAnonymity", i.Wish.IsNim.ToString()),
                                                   new XAttribute("totalPages", totalpage.ToString())
                                          ));
            }

            //context.Response.Write(totalcout.ToString());
            context.Response.ContentType = "text/plain";
            context.Response.Write(node.ToString(false));
            //}

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
