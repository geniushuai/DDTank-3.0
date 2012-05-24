using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace WebApplication1.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            var username = LoginUser.UserName;
            var password = LoginUser.Password;
            if (username == ConfigurationManager.AppSettings["adminUser"] && password == ConfigurationManager.AppSettings["adminPassword"])
            {
                FormsAuthentication.SetAuthCookie(username, LoginUser.RememberMeSet /* createPersistentCookie */);
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                LoginUser.FailureText = "username and password is not match";
              
            }
        }
    }
}
