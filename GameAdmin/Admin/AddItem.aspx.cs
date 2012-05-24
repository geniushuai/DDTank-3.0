using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bussiness.Managers;

namespace WebApplication1.Admin
{
    public partial class AddItem : System.Web.UI.Page
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
            var business = new Bussiness.PlayerBussiness();
            try
            {

                var userid = business.GetUserSingleByNickName(UserName_tbx.Text).ID;
                var agility = int.Parse(Agility_tbx.Text);
                var attack = int.Parse(Attack_tbx.Text);
                var defence = int.Parse(Defence_tbx.Text);
                var luck = int.Parse(Luck_tbx.Text);
              
                business.SendMailAndItem("Add Item", "AddItem", userid, int.Parse(Template_tbx.Text), 1,50, 0, 0, int.Parse(Level_tbx.Text), attack, defence, agility,luck, false);
                Error_lbl.Text = "OK";
            }
            catch
            {
                Error_lbl.Text = "User is not valid or TemplateID is not valid :";
            }
            Template_tbx.Text = string.Empty;
            Level_tbx.Text = string.Empty;

            return;
        }
    }
}