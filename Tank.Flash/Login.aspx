<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Tank.Flash.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<title>弹弹堂</title>
<style type="text/css" >
body{
	margin:0px auto;
	padding:0px;
	background-image:url(images/bg.jpg);
}
.top{
	margin-top:387px;
}
.login_input{
	padding-left:445px;
}
.login_input ul{
	margin:0px;
	padding:0px;
}
.login_input li{
	list-style-type:none;
	margin:0px;
	padding:0px;
	margin-bottom:7px;
}
.user_input{
	width:178px;
	height:26px;
	line-height:22px;
	padding-left:4px;
	font-size:16px;
	font-family:Verdana, Arial, Helvetica, sans-serif;
	font-weight:bold;
	color:#FFFFFF;
	background-color:#7B3C06;
	border-top-width: 1px;
	border-right-width: 1px;
	border-bottom-width: 1px;
	border-left-width: 1px;
	border-top-style: solid;
	border-right-style: solid;
	border-bottom-style: solid;
	border-left-style: solid;
	border-top-color: #CCCCCC;
	border-right-color: #F0F0F0;
	border-bottom-color: #F0F0F0;
	border-left-color: #CCCCCC;
}
.login_button{
	text-align:center;
}
.login_button a{
	background-image: url(images/login_button.gif);
	display: block;
	height: 36px;
	width: 102px;
}
</style>
</head>
<body scroll="no">
    <form id="form1" runat="server" method="post" >
<table width="100%" height="100%" border="0" align="center" cellpadding="0" cellspacing="0">
  <tr>
    <td height="608" valign="middle"><table width="1008" border="0" align="center" cellpadding="0" cellspacing="0">
      <tr>
        <td height="608" valign="top" background="images/main_bg.jpg">
        	<div class="top"></div>
            <form action="LoginGame.aspx" target="LoginGame.aspx" >
            <div class="login_input">
            	<ul>
                	<li><!--<input type="text" class="user_input">-->
                	<asp:TextBox ID="txtUserName" CssClass="user_input" runat="server"></asp:TextBox>
                	</li>
                    <li><!--<input type="password" class="user_input">-->
                    <asp:TextBox ID="txtPassword" CssClass="user_input" runat="server" Width="167px"></asp:TextBox>
                    </li>
                </ul>
            </div>
            <div class="login_button">
            	<a href="FlashGame/index.aspx" ></a>
                <input type="submit" />
            </div>
            </form>
        </td>
      </tr>
    </table></td>
  </tr>
</table>
</form>
</body>
</html>
