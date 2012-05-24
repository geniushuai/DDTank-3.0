<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Tank.Flash._Default" %>

<html>
<head id="Head1" runat="server">
    <META   HTTP-EQUIV="Pragma"   CONTENT="no-cache">    
    <META   HTTP-EQUIV="Cache-Control"   CONTENT="no-cache">    
    <META   HTTP-EQUIV="Expires"   CONTENT="0">    
    <title><%=SiteTitle%></title>
    <script type="text/javascript" src="scripts/dandantang.js"></script>
    <script type="text/javascript" src="scripts/rightClick.js"></script>
    <script type="text/javascript">
<!--
	var allowLeave = 1;
	var www="";
	var name="弹弹堂";

	function setFavorite(path,titlename,allowvalue)
	{ 
             www=path;
             name=titlename;
	     allowLeave=allowvalue;
	}
	window.onbeforeunload = function()
    	{
           if(allowLeave==1)
           {
        	
           }
           if(allowLeave==2)
           {
        	window.event.returnValue = '当前操作将退出弹弹堂游戏，是否继续？';
           }
           if(allowLeave==3)
           {
        	window.external.addFavorite(www,name);
           }
    }
    function killErrors() //杀掉所有的出错信息
    { 
	    return true; 
    }
    window.onerror = killErrors; 
// -->
    </script>     
    <style>
      body
        {
            margin: 0px auto;
            padding: 0px;
            background-image: url(images/bg_all.jpg);
	     background-repeat: no-repeat;
        background-position: center center;
        }
    </style>
</head>
<body scroll="no" onload="checkScreen()">
    <table width="100%" height="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td valign="middle">
                    <table border="0" align="center" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="center">
                                <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0"
                                    name="Main" width="1000" height="600" align="middle" id="Main">
                                    <param name="allowScriptAccess" value="always" />
                                    <param name="movie" value='Loading.swf?<%=Content %>&v=<%=Edition %>&rand=<%=Rand %>' />                      
                                    <param name="quality" value="high" />
                                    <param name="menu" value="false">
                                    <param name="bgcolor" value="#000000" />
                                    <param name="FlashVars" value="<%=AutoParam %>" /> 
                                    <embed src='Loading.swf?<%=Content %>&v=<%=Edition %>&rand=<%=Rand %>' width="1000" height="600"
                                        align="middle" quality="high" name="Main" allowscriptaccess="sameDomain" type="application/x-shockwave-flash"
                                        pluginspage="http://www.macromedia.com/go/getflashplayer" />
                                </object>
                            </td>
                        </tr>
                    </table>
            </td>
        </tr>
    </table>
</body>
</html>
