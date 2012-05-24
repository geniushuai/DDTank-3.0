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

namespace Road.Request
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BuyDefinedGoods : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["user"]) && !string.IsNullOrEmpty(context.Request["pass"]) && !string.IsNullOrEmpty(context.Request["shopid"]) && !string.IsNullOrEmpty(context.Request["fitments"]))
            {
                int NeedMarkTal = 0, MarkTal = 0, buynum = 0;
                decimal NeedVirtualMoneyTal = 0m, AccountTal = 0m;

                XElement list;
                XElement item;

                SysUsers user = DbCenter.QueryDb.SysUsers.SingleOrDefault(u => u.Email == context.Request["user"] && u.PassWord == context.Request["pass"]);
                //判断用户名与密码
                if (user == null)
                {
                    list = new XElement("list", new XAttribute("result", "false"),
                        new XAttribute("msg", "用户名与密码不正确")
                        );
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(list.ToString(false));
                    context.Response.End();
                }
                MarkTal = user.Mark;
                AccountTal = (decimal)user.Account;
                string fitments = context.Request["fitments"];

                string[] fitmentsArr = Regex.Split(fitments, ",", RegexOptions.IgnoreCase);


                foreach (string i in fitmentsArr)//计算所需要的积分与金币
                {
                    Guid id = new Guid(i.Split('|')[0]);
                    int count = Convert.ToInt32(i.Split('|')[1]);
                    GameSysgoodsmodels model = DbCenter.QueryDb.GameSysgoodsmodels.SingleOrDefault(m => m.ID == id);
                    if (model != null)
                    {
                        NeedMarkTal += model.NeedMark * count;
                        NeedVirtualMoneyTal += (decimal)model.NeedVirtualMoney * count;
                    }
                    buynum += count;
                }
                //判断金币与积分够
                if (MarkTal < NeedMarkTal || AccountTal < NeedVirtualMoneyTal)
                {
                    list = new XElement("list", new XAttribute("result", "false"),
                      new XAttribute("msg", "金币或者积分不够")
                      );
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(list.ToString(false));
                    context.Response.End();

                }
                var query = from g in DbCenter.QueryDb.GameUserdefinedgoods where g.UserId == user.ID && g.IsInObject == false select g;
                if ((query.Count() + buynum) > 1000)
                {
                    list = new XElement("list", new XAttribute("result", "false"),
                    new XAttribute("msg", "仓库已满")
                    );
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(list.ToString(false));
                    context.Response.End();
                }
                GameShops shop = DbCenter.QueryDb.GameShops.SingleOrDefault(s => s.Code == context.Request["shopid"]);
                if (shop == null)
                {
                    list = new XElement("list", new XAttribute("result", "false"),
                    new XAttribute("msg", "商店不存在")
                    );
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(list.ToString(false));
                    context.Response.End();
                }

                list = new XElement("list");
                foreach (string i in fitmentsArr)//计算所需要的积分与金币
                {


                    Guid id = new Guid(i.Split('|')[0]);

                    int count = Convert.ToInt32(i.Split('|')[1]);
                    GameSysgoodsmodels model = DbCenter.QueryDb.GameSysgoodsmodels.SingleOrDefault(m => m.ID == id);
                    if (model != null)
                    {
                        for (int x = 0; x < count; x++)
                        {
                            GameUserdefinedgoods g = new GameUserdefinedgoods();
                            g.ID = Guid.NewGuid();
                            g.AddTime = DateTime.Now;
                            g.BuyDate = DateTime.Now;
                            g.CategoryId = Convert.ToInt32(model.CategoryId);
                            g.GoodsModelId = model.ID;
                            g.InObjectId = Guid.Empty;
                            g.IsInObject = false;
                            g.UserId = user.ID;
                            g.PosX = 0;
                            g.PosY = 0;
                            g.Dir = 0;
                    
                            DbCenter.QueryDb.GameUserdefinedgoods.InsertOnSubmit(g);
                            item = new XElement("item", new XAttribute("id", g.ID),
                                new XAttribute("posX", g.PosX),
                                 new XAttribute("posY", g.PosY),
                                  new XAttribute("dir", g.Dir),
                                   new XAttribute("height",model.Height),
                                     new XAttribute("type", model.CategoryId),
                                    new XAttribute("asset",model.Asset),
                                     new XAttribute("func", model.Func)                                     
                                 );
                            list.Add(item);
                        }
                    }

                }
                user.Mark -= NeedMarkTal;
                user.Account -= NeedVirtualMoneyTal;
                DbCenter.QueryDb.SubmitChanges();
                list.Add(new XAttribute("result", "true"));
                list.Add(new XAttribute("msg", "true"));
                list.Add(new XAttribute("spareMark", user.Mark));
                list.Add(new XAttribute("spareMoney", user.Account));
                context.Response.ContentType = "text/plain";
                context.Response.Write(list.ToString(false));
                context.Response.End();


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
