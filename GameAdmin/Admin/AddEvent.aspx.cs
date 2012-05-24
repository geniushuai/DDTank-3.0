using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bussiness.Managers;

namespace WebApplication1.Admin
{
    public partial class AddEvent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Error_lbl.Text = "";
            if (!IsPostBack)
            {
                Template_tbx.Text = string.Empty;
                Level_tbx.Text = string.Empty;
            }
        }


        protected void Edit_Click(object sender, EventArgs e)
        {
            var item = ItemMgr.FindItemTemplate(int.Parse(Template_tbx.Text));
       
         
            Template_tbx.Text = string.Empty;
            Level_tbx.Text = string.Empty;

            return;
        }
    }
}