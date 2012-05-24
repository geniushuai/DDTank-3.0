using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using RoadDatabase;
using System.Text;
using System.Collections.Specialized;
using log4net;
using System.Reflection;
using System.Collections.Generic;

namespace Road.Request
{

    /// <summary>
    /// 查找好友
    /// </summary>
    public class SearchFriends : IHttpHandler
    {
        class TempPlayer
        {
            public SysUsers User { get; set; }
            public string SchoolName { get; set; }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            NameValueCollection para = context.Request.Params;
            byte type = byte.Parse(para["type"]);
            byte page = byte.Parse(para["page"]);
            byte count = byte.Parse(para["count"]);

            Db7roaddatacontext db = DbCenter.QueryDb;
            var query = from u in db.SysUsers
                        join s in db.SysSchools on u.SchoolId equals s.ID
                        select new TempPlayer { User = u, SchoolName = s.SchoolName };
            bool online = false;
            switch (type)
            {
                //普通查询
                case 0:
                    online = bool.Parse(para["online"]);
                    if (online)
                    {
                        query = query.Where(c => c.User.State > 0);
                    }
                    else
                    {


                        string temp = para["account"] as string;
                        if (!string.IsNullOrEmpty(temp))
                        {
                            query = query.Where(c => c.User.UserName.Contains(para["account"] as string));
                        }

                        temp = para["nickname"] as string;
                        if (!string.IsNullOrEmpty(temp))
                        {
                            query = query.Where(c => c.User.PetName.Contains(para["nickname"] as string));
                        }

                        temp = para["realname"] as string;
                        if (!string.IsNullOrEmpty(temp))
                        {
                            query = query.Where(c => c.User.TrueName.Contains(para["realname"] as string));
                        }

                    }
                    break;
                case 1:
                    online = bool.Parse(para["online"]);
                    if (online)
                    {
                        query = query.Where(c => c.User.State > 0);
                    }

                    if (!string.IsNullOrEmpty(para["province"]))
                    {
                        query = query.Where(c => c.User.Provice.Contains(para["province"] as string));
                    }

                    if (!string.IsNullOrEmpty(para["city"]))
                    {
                        query = query.Where(c => c.User.City.Contains(para["city"] as string));
                    }

                    string temp1 = para["age1"] as string;
                    int age1 = string.IsNullOrEmpty(temp1) ? int.MinValue : int.Parse(temp1);
                    temp1 = para["age2"] as string;
                    int age2 = string.IsNullOrEmpty(temp1) ? int.MaxValue : int.Parse(temp1);
                    if (age1 != int.MinValue || age2 != int.MaxValue)
                    {
                        query = query.Where(c => c.User.Age >= age1 && c.User.Age <= age2);
                    }

                    temp1 = para["sex"] as string;
                    if (!string.IsNullOrEmpty(temp1))
                    {
                        if (temp1.StartsWith("t"))
                        {
                            query = query.Where(c => c.User.Sex == true);
                        }
                        else
                        {
                            query = query.Where(c => c.User.Sex == false);
                        }
                    }
                    break;

                case 2:

                    if (!string.IsNullOrEmpty(para["province"]))
                    {
                        query = query.Where(c => c.User.Provice.Contains(para["province"] as string));
                    }

                    if (!string.IsNullOrEmpty(para["city"]))
                    {
                        query = query.Where(c => c.User.City.Contains(para["city"] as string));
                    }

                    if (!string.IsNullOrEmpty(para["university"]))
                    {
                        query = query.Where(c => c.SchoolName.Contains(para["univercity"] as string));
                    }

                    if (!string.IsNullOrEmpty(para["schoolyear"]))
                    {
                        int scrope = int.Parse(para["schoolyear"]);
                        query = from tu in query
                                join y in db.UserStudyrecord on tu.User.ID equals y.UserID
                                where tu.User.SchoolId == y.University && y.UniversityScope == scrope
                                select tu;
                    }

                    if (!string.IsNullOrEmpty(para["sex"]))
                    {
                        if (para["sex"].StartsWith("t"))
                        {
                            query = query.Where(c => c.User.Sex == true);
                        }
                        else
                        {
                            query = query.Where(c => c.User.Sex == false);
                        }
                    }

                    break;
                default:
                    return;
            }

            int tCount = query.Count();

            IList<TempPlayer> ulist = query.Take(count * page).ToList();

            XElement node = new XElement("list", new XAttribute("total", Math.Ceiling((double)tCount / count)),
                                                new XAttribute("current", page));

            for (int i = (page - 1) * count; i < ulist.Count; i++)
            {
                node.Add(Flash.FlashUtils.CreateBaseMemberInfo(ulist[i].User, ulist[i].SchoolName));
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(node.ToString(false));
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
