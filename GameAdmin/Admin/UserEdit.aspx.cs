using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1.Admin
{
    public partial class UserEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                accountPanel.Visible = false;
                error_lbl.Text = string.Empty;
            }
        }

        protected void Edit_Click(object sender, EventArgs e)
        {
            ObjectDataSource1.Select();
            ObjectDataSource1.DataBind();
            accountPanel.Visible = true;
            SearchPanel.Visible = false;

        }

        protected void ObjectDataSource1_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["userName"] = UserName_tbx.Text;
            //var business = new Bussiness.PlayerBussiness();
            //var player = business.GetUserSingleByNickName(UserName_tbx.Text);
            //if (player == null)
            //{
            //     error_lbl.Text="Username is not exits or not active ,Change to Acive it";
            //}
        }

        protected void ObjectDataSource1_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            var business = new Bussiness.PlayerBussiness();
            var player = business.GetUserSingleByNickName(UserName_tbx.Text);
            if (player != null)
            {
                player.GiftToken = Int32.Parse(((TextBox)FormView1.FindControl("GiftTokenTextBox")).Text);
                player.Gold = Int32.Parse(((TextBox)FormView1.FindControl("GoldTextBox")).Text);
                player.Money = Int32.Parse(((TextBox)FormView1.FindControl("MoneyTextBox")).Text);
                business.UpdatePlayer(player);
            }
            e.Cancel = true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            accountPanel.Visible = false;
            SearchPanel.Visible = true;
        }

        protected void Active_btn_Click(object sender, EventArgs e)
        {
            var business = new Bussiness.PlayerBussiness();
            var player = business.DisableUser(UserName_tbx.Text, true);
            Button1_Click(sender, e);
        }

        protected void DeActive_btn_Click(object sender, EventArgs e)
        {
            var business = new Bussiness.PlayerBussiness();
            var player = business.DisableUser(UserName_tbx.Text, false);
            Button1_Click(sender, e);
           
        }

    }
}