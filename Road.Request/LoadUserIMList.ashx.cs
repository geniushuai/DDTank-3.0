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
    /// Load user im list
    /// </summary>
    public class LoadUserIMList : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            
            if (context.Request.Cookies["id"] != null)
            {
                Guid clientId = new Guid(context.Request.Cookies["id"].Value);

                XElement list = new XElement("list");

                XElement group;

                //分组中好友
                var groups = from g in DbCenter.QueryDb.UserFriendGroups
                             where g.UserId == clientId
                             select g;
                foreach (UserFriendGroups g in groups)
                {
                    group = new XElement("group", new XAttribute("id", g.Id), new XAttribute("name", g.GroupName));

                    var users = from u in DbCenter.QueryDb.SysUsers
                                join f in DbCenter.QueryDb.UserFriends on u.ID equals f.FriendID
                                where f.UserID == clientId && f.GroupId == g.Id
                                select u;
                    foreach (SysUsers u in users)
                    {
                        group.Add(CreatePlayerNode(u));
                    }
                    list.Add(group);
                }

                //默认好友
                group = new XElement("group", new XAttribute("id", Guid.Empty), new XAttribute("name", "默认"));
                var defaults = from u in DbCenter.QueryDb.SysUsers
                               join f in DbCenter.QueryDb.UserFriends on u.ID equals f.FriendID
                               where f.UserID == clientId && (f.GroupId == Guid.Empty || f.GroupId == null)
                               select u;
                foreach (SysUsers u in defaults)
                {
                    group.Add(CreatePlayerNode(u));
                }
                list.Add(group);

                //黑名单
                group = new XElement("group", new XAttribute("id", "-1"), new XAttribute("name", "黑名单"));
                var black = from u in DbCenter.QueryDb.SysUsers
                            join b in DbCenter.QueryDb.UserBlacklists on u.ID equals b.BlackerID
                            where b.UserID == clientId
                            select u;
                foreach (SysUsers u in black)
                {
                    group.Add(CreatePlayerNode(u));
                }
                list.Add(group);

                context.Response.ContentType = "text/plain";
                context.Response.Write(list.ToString());
            }
        }

        private XElement CreatePlayerNode(SysUsers u)
        {
            return new XElement("item", new XAttribute("id", u.IdentityID),
                                                             new XAttribute("guidId", u.ID),
                                                             new XAttribute("nickName", u.PetName),
                                                             new XAttribute("sex", u.Sex),
                                                             new XAttribute("style", u.Style),
                                                             new XAttribute("styleType", u.ClassType),
                                                             new XAttribute("email", u.UserName),
                                                             new XAttribute("state", u.State),
                                                             new XAttribute("level", u.Grade),
                                                             new XAttribute("score", u.Mark),
                                                             new XAttribute("friendVerify", u.FriendVerify),
                                                             new XAttribute("sign", u.UserSign),
                                                             new XAttribute("university", ""),
                                                             new XAttribute("memberType", (bool)u.IsApprove ? 0 : 1));
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
