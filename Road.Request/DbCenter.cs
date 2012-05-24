using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using RoadDatabase;
using System.Data.Linq;

namespace Road.Request
{
    public class DbCenter
    {
        private static readonly string KEY = "datacontext";

        public static Db7roaddatacontext QueryDb
        {
            get
            {
                Db7roaddatacontext query = null;
                if (HttpContext.Current.Items.Contains(KEY))
                {
                    query = (Db7roaddatacontext)HttpContext.Current.Items[KEY];
                }
                else
                {
                    query = new Db7roaddatacontext(ConfigurationManager.ConnectionStrings["road"].ConnectionString);
                    HttpContext.Current.Items.Add(KEY, query);
                }
                return query;
            }
        }

        public static void ClearDataContext()
        {
            if (HttpContext.Current.Items.Contains(KEY))
            {
                DataContext session = (DataContext)HttpContext.Current.Items[KEY];
                session.Dispose();
                HttpContext.Current.Items.Remove(KEY);
            }
        }
    }
}
