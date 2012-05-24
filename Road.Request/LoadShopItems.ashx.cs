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
    public class LoadShopItems : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["code"]))
            {
                string shopId = context.Request.Params["code"];
                GameShops shop = DbCenter.QueryDb.GameShops.Single(s => s.Code == shopId);
                if (!string.IsNullOrEmpty(shopId))
                {
                    var list = from s in DbCenter.QueryDb.GameShops
                               join i in DbCenter.QueryDb.GameShopGoods on s.ID equals i.ShopId
                               join m in DbCenter.QueryDb.GameSysgoodsmodels on i.GoodsModelId equals m.ID
                               where i.ShopId == shop.ID
                               select m;

                    XElement node = new XElement("list");

                    foreach (var t in list)
                    {
                        node.Add(new XElement("item", new XAttribute("id", t.ID),
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
                                                    new XAttribute("mark", t.NeedMark)));


                    }

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
