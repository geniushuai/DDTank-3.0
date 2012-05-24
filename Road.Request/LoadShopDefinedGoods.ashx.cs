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
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LoadShopDefinedGoods : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["shopcode"]) && !string.IsNullOrEmpty(context.Request["categoryid"]))
            {
                string code = context.Request["shopcode"];
                GameShops shop = DbCenter.QueryDb.GameShops.SingleOrDefault(s => s.Code == code);
                int categoryid=Convert.ToInt32(context.Request["categoryid"]);
                context.Response.Write("<list>");
                if (shop == null)
                {
                    context.Response.Write("</list>");
                    context.Response.End();
                }

                var query = from g in DbCenter.QueryDb.GameShopGoods
                            where g.ShopId == shop.ID
                            join m in DbCenter.QueryDb.GameSysgoodsmodels on g.GoodsModelId equals m.ID
                            where m.CategoryId == categoryid
                            select g;
                            
                           

                foreach (GameShopGoods sg in query)
                {
                    GameSysgoodsmodels model = DbCenter.QueryDb.GameSysgoodsmodels.SingleOrDefault(m => m.ID == sg.GoodsModelId);
                    if (model != null)
                    {
                   
                        context.Response.Write("<item id=\"" + sg.GoodsModelId + "\" score=\"" + model.NeedMark + "\" price=\"" + model.NeedVirtualMoney + "\" level=\"" + model.Grade + "\" dayToOverDue=\"" + model.EffectiveDays + "\"  func=\"" + model.Func + "\"    asset=\"" + model.Asset + "\" />");
                    }

                }
                context.Response.Write("</list>");
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
