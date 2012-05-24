using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bussiness.Managers;
using SqlDataProvider.Data;

namespace WebApplication1.Admin
{
    public partial class SendMail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Content_tbx.Text = string.Empty;
                Title_tbx.Text = string.Empty;
                UserName_tbx.Text = string.Empty;
            }
        }

        protected void SendMail_btn_Click(object sender, EventArgs e)
        {
   
            var business = new Bussiness.PlayerBussiness();
            try
            {

                var userid = business.GetUserSingleByNickName(UserName_tbx.Text).ID;

                //business.SendMailAndItemByUserName(Title_tbx.Text, Content_tbx.Text, UserName_tbx.Text,0,0,string.Empty);
                var mail=new MailInfo();
                mail.Title=Title_tbx.Text;
                mail.Content=Content_tbx.Text;
                mail.ReceiverID=userid;
                mail.Sender = "Administrators";
                mail.SenderID = 0;
                mail.Type = 1;
                business.SendMail(mail);
                Error_lbl.Text = "OK";
            }
            catch
            {
                Error_lbl.Text = "User  :";
            }
      
        }


 
    }
}