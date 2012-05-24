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
    public class LoadUserDetailInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["id"]))
            {
                Guid id = new Guid(context.Request.Params["id"]);
                SysUsers u = DbCenter.QueryDb.SysUsers.Single(c => c.ID == id);
                if (u != null)
                {
                    var f1 = from f in DbCenter.QueryDb.UserFriends
                             where f.UserID == u.ID
                             group f by f.UserID into g
                             select g.Count();

                    var f2 = from f in DbCenter.QueryDb.UserFriends
                             where f.FriendID == u.ID
                             group f by f.FriendID into g
                             select g.Count();

                    BlogBlogers blog = DbCenter.QueryDb.BlogBlogers.SingleOrDefault(c => c.UserId == u.ID);

                    var bbs = from c in DbCenter.QueryDb.BbsUserinf
                              where c.UserID == u.ID
                              select c.BbsArticlesAnswers;

                    var study = from s in DbCenter.QueryDb.UserStudyrecord
                                where s.UserID == u.ID
                                select s;

                    int hotByBlog = blog == null ? 0 : blog.DiarViewCount + blog.PhotoViewCount;
                    int hotByBbs = bbs.SingleOrDefault() * 2 + u.LookedNumInGame;
                    int hotByFriends = f1.SingleOrDefault() + f2.SingleOrDefault() * 3;

                    UserStudyrecord re = DbCenter.QueryDb.UserStudyrecord.Single(c => c.UserID == u.ID);

                    SysUniversityprofessionals pro = DbCenter.QueryDb.SysUniversityprofessionals.SingleOrDefault(c => c.Id == re.Professional);
                    SchoolRoominghouses house = DbCenter.QueryDb.SchoolRoominghouses.SingleOrDefault(c => c.Id == re.UniversityRoomingHouses);

                    XElement list = new XElement("list");
                    XElement node = new XElement("player", new XElement("item", new XAttribute("id", u.IdentityID),
                                                                 new XAttribute("guidId", u.ID),
                                                                 new XAttribute("nickName", u.PetName),
                                                                 new XAttribute("realName", u.TrueName),
                                                                 new XAttribute("sex", u.Sex),
                                                                 new XAttribute("style", u.Style),
                                                                 new XAttribute("styleType", u.ClassType),
                                                                 new XAttribute("email", u.UserName),
                                                                 new XAttribute("state", u.State),
                                                                 new XAttribute("level", u.Grade),
                                                                 new XAttribute("score", u.Mark),
                                                                 new XAttribute("friendVerify", u.FriendVerify),
                                                                 new XAttribute("sign", u.UserSign == null ? "" : u.UserSign),
                                                                 new XAttribute("university", re.UniversityName),
                                                                 new XAttribute("universityId", u.SchoolId),
                                                                 new XAttribute("memberType", (bool)u.IsApprove ? 0 : 1),
                                                                 new XAttribute("country", u.Country == null ? "" : u.Country),
                                                                 new XAttribute("age", u.Age),
                                                                 new XAttribute("city", u.City == null ? "" : u.City),
                                                                 new XAttribute("province", u.Provice == null ? "" : u.Provice),
                                                                 new XAttribute("highSchool", re.SeniorName == null ? "" : re.SeniorName),
                                                                 new XAttribute("blood", u.BloodType == null ? "" : u.BloodType),
                                                                 new XAttribute("hotPoint", hotByBbs + hotByBlog + hotByFriends),
                                                                 new XAttribute("hotPointByBlog", hotByBlog),
                                                                 new XAttribute("hotPointByBbs", hotByBbs),
                                                                 new XAttribute("hotPointByFriend", hotByFriends),
                                                                 new XAttribute("introduce", u.Introduction == null ? "" : u.Introduction),
                                                                 new XAttribute("birthday", u.Birthday == null ? "" : u.Birthday.ToString()),
                                                                 new XAttribute("star", u.StarSigns == null ? "" : u.StarSigns),
                                                                 new XAttribute("animal", u.BirthPet == null ? "" : u.BirthPet),
                                                                 new XAttribute("univerYear", re.UniversityScope == null ? "" : re.UniversityScope.ToString()),
                                                                 new XAttribute("department", re.DepartMentName == null ? "" : re.DepartMentName),
                                                                 new XAttribute("profession", pro == null ? "" : pro.ProfessionalName),
                                                                 new XAttribute("dorm", house == null ? "" : house.HouseName),
                                                                 new XAttribute("figureURL", u.XxBigPicUrl == null ? "" : u.XxBigPicUrl),
                                                                 new XAttribute("highSchoolYear", re.SeniorName==null?"":re.SeniorName),
                                                                 new XAttribute("high_one", re.SeniorOneClass==null?"":re.SeniorOneClass),
                                                                 new XAttribute("high_two", re.SeniorSecondClass==null?"":re.SeniorSecondClass),
                                                                 new XAttribute("high_three", re.SeniorThreeClass==null?"":re.SeniorThreeClass),
                                                                 new XAttribute("junior", re.UniorSchoolName==null?"":re.UniorSchoolName),
                                                                 new XAttribute("juniorYear", re.UniorScope),
                                                                 new XAttribute("grade", re.PrimarySchool==null?"":re.PrimarySchool),
                                                                 new XAttribute("gradeYear", re.PrimaryScope==null?"":re.PrimaryScope.ToString()),
                                                                 new XAttribute("studentKind", re.UniversityKind==null?"":re.UniversityKind)
                                                                 ));



                    list.Add(node);

                    XElement diary = new XElement("diarylist");
                    XElement photo = new XElement("photolist");
                    if (blog != null)
                    {

                        var dl = from d in DbCenter.QueryDb.BlogerDiars
                                 where d.UserId == id
                                 orderby d.AddTime
                                 select d;
                        foreach (BlogerDiars b in dl.Take(4))
                        {
                            diary.Add(new XElement("item", new XAttribute("Id", b.ID),
                                                        new XAttribute("Title", b.Title),
                                                        new XAttribute("Url", string.Format("blog/TheDiar.aspx?diarid={0}", b.ID)),
                                                        new XAttribute("ViewCount", b.ViewCount),
                                                        new XAttribute("CommentCount", b.CommentCount)));
                        }



                        var pl = from f in DbCenter.QueryDb.AlbumPhotos
                                 where f.UserId == id
                                 orderby f.AddTime
                                 select f;
                        foreach (AlbumPhotos p in pl.Take(4))
                        {
                            photo.Add(new XElement("item", new XAttribute("Id", p.ID),
                                                        new XAttribute("PicPath", p.SmallXxPicPath.Replace("../", "")),
                                                        new XAttribute("Url", string.Format("blog/ThisPhoto.aspx?photoid={0}&bloger={1}", p.ID, blog.BlogName))));
                        }
                    }

                    list.Add(diary);
                    list.Add(photo);

                    context.Response.ContentType = "text/plain";
                    context.Response.Write(list.ToString(false));
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
